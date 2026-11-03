using FSO.LotView.Model;
using FSO.SimAntics.Entities;
using FSO.SimAntics.Model;
using System;
using System.Collections.Generic;

namespace FSO.SimAntics
{
    /// <summary>
    /// UI-22 tranche 1 — build-mode architecture undo/redo for TS1 lots.
    /// Implements the native law decoded in tools/iff-dump/r256-undo-decode/
    /// undo-machinery-law.md at VM-batch granularity:
    ///
    /// - One UndoManager equivalent per VMArchitecture (native: one at
    ///   cGameTools+8 for the whole game; the port's architecture state is per
    ///   lot, which also gives the native lifecycle for free — the stack is
    ///   runtime-only, dies on lot switch/reload and SURVIVES save because
    ///   VMArchitectureMarshal never carries it).
    /// - SubmitUndoable law (law §2): every applied real-mode batch pushes one
    ///   entry (one drag = one step, the native 12-caller census §6) and CLEARS
    ///   the redo list. Native depth is unbounded; this port caps at 100
    ///   entries (DEVIATION, design §5.1 — full-story snapshots weigh more
    ///   than the native 60-byte delta records).
    /// - Undo/redo law (§3): undo pops the newest entry, restores it, pushes it
    ///   onto the redo list; redo pops the newest redo entry, replays it and
    ///   pushes it back. A failed undo drops the remaining undo list; a failed
    ///   redo drops the remaining redo list.
    /// - Money law (§4 floors): undo refunds the batch's signed cost, redo
    ///   re-charges it, through the same GlobalLink.PerformTransaction path
    ///   the live architecture queue uses (uid1 pays uid2; null budget family
    ///   = maxis-infinite-money no-op, exactly like the purchase path).
    /// - Lifecycle law (§5): leaving build mode clears both stacks (the
    ///   UIMainPanel.SetMode hook mirrors the native CPState::SetMode
    ///   old-mode==2 guard); no save path touches the stack.
    ///
    /// Deliberate deviations (implementation-design.md §5): depth cap 100;
    /// redo re-runs the recorded batch (the restore makes state bit-identical
    /// to pre-batch) instead of a stored snapshot swap; refused batches never
    /// reach the stack because the port refuses them pre-transaction in
    /// VMTS1GlobalLinkStub.Tick (native flush-on-commit-fail is unreachable).
    /// </summary>
    /// <summary>
    /// UI-42 tranche 2 — one build-mode undo step. The native UndoManager
    /// (r256 §1-§6) holds ONE mixed stack: wall/floor drags, terrain drags,
    /// roof clicks and Hand-tool object actions all submit Undoable records
    /// to the same lists. The port models the domain payloads as virtual
    /// Restore/Replay on the entry: the architecture batch (this class) is
    /// one kind; VMObjectUndoEntry and VMRoofUndoEntry (below) are the
    /// tranche-2 kinds.
    /// </summary>
    public class VMArchitectureUndoEntry
    {
        /// <summary>The applied batch (per drag) — replayed verbatim on redo.</summary>
        public List<VMArchitectureCommand> Forward;

        /// <summary>Signed cost RunCommands returned for the batch (purchase > 0, sellback &lt; 0).</summary>
        public int Cost;

        /// <summary>Actor the batch was attributed to (transaction counterpart).</summary>
        public uint ActorUID;

        /// <summary>Pre-batch story snapshots, keyed by level (1-based).</summary>
        public Dictionary<int, VMArchitectureStorySnapshot> Saved;

        /// <summary>
        /// UI-42 tranche 2: the terrain drag's pre-batch grid — the native
        /// TerrainUndoable::TakeSnapshot law (r256 §4: FULL-grid snapshot of
        /// the 4-corner heights + the second per-tile array; the port adds
        /// Sloped, which is corner-derived but only recomputed on load, so a
        /// restore that skipped it would leave stale slope state). Null for
        /// pure architecture batches.
        /// </summary>
        public VMArchitectureTerrainSnapshot Terrain;

        /// <summary>
        /// Native UndoSelf (law §3/§4): restore the recorded pre-change
        /// state. The base implementation restores the wall/floor story
        /// snapshots and the terrain grid, then rebuilds derived state.
        /// </summary>
        public virtual void Restore(VMArchitecture arch)
        {
            if (Saved != null)
            {
                foreach (var kv in Saved)
                {
                    var walls = arch.Walls[kv.Key - 1];
                    var floors = arch.Floors[kv.Key - 1];
                    Array.Copy(kv.Value.Walls, walls, walls.Length);
                    Array.Copy(kv.Value.Floors, floors, floors.Length);
                }
            }
            if (Terrain != null)
            {
                var t = arch.Terrain;
                Array.Copy(Terrain.Heights, t.Heights, t.Heights.Length);
                Array.Copy(Terrain.Grass, t.GrassState, t.GrassState.Length);
                Array.Copy(Terrain.Sloped, t.Sloped, t.Sloped.Length);
                arch.SignalTerrainRedraw(); // native DirtyTileNeighborhood → the port's regen path
            }
        }

        /// <summary>
        /// Native RedoSelf (law §3): re-apply the change. The base
        /// implementation replays the recorded batch — bit-identical for
        /// terrain too (set-semantics flood-fill from the restored grid;
        /// documented tranche-1 deviation from the native snapshot swap).
        /// </summary>
        public virtual void Replay(VMArchitecture arch)
        {
            arch.RunCommands(Forward, false);
        }
    }

    /// <summary>UI-42: pre-batch terrain grid (native TakeSnapshot payload).</summary>
    public class VMArchitectureTerrainSnapshot
    {
        public short[] Heights;
        public byte[] Grass;
        public bool[] Sloped;
    }

    /// <summary>
    /// UI-42 tranche 2 — the native ObjectUndoable law (decoded
    /// coordination/evidence/UI-42/native-object-terrain-undo-law.md):
    /// TWO records, not a swap — record #1 (PRE) captured by the ctor via
    /// AssignFromObject (placement, level, container, the ObjSelector) and
    /// record #2 (POST) captured at CommitSelf; undo applies #1, redo
    /// applies #2, nothing reads live state at undo time. Action enum
    /// (dispatched at UndoSelf/RedoSelf): 0 = create, 1 = move (2 = a
    /// second move flavor the binary never constructs), 3 = delete.
    /// RedoSelf: create→CreateObject(#2), move→apply #2, delete→Kill(#1);
    /// UndoSelf mirrors: create→Kill, move→apply #1, delete→Create(#1) via
    /// MakeNewOutOfWorldObject from the stored master selector. MONEY: only
    /// the create direction carries the signed delta (buy +price; sell
    /// +depreciated value); moves are free (Cost 0). One Hand-tool action =
    /// one step on the ONE mixed stack.
    /// </summary>
    public class VMObjectUndoEntry : VMArchitectureUndoEntry
    {
        public const byte ActionCreate = 0;
        public const byte ActionMove = 1;
        public const byte ActionDelete = 3;

        public byte Action;

        // record #1 (PRE) — the ctor's AssignFromObject capture
        public ushort ObjectId;
        public uint MasterGUID;
        public LotView.Model.LotTilePos PrePos;
        public FSO.LotView.Model.Direction PreDir;

        // record #2 (POST) — CommitSelf's capture (the placed state for
        // moves/creates; unused for delete whose redo is Kill of the
        // recreated instance)
        public LotView.Model.LotTilePos PostPos;
        public FSO.LotView.Model.Direction PostDir;

        public override void Restore(VMArchitecture arch)
        {
            var vm = arch.Context.VM;
            if (vm == null) return;
            if (Action == ActionMove) Apply(vm, ObjectId, PrePos, PreDir);
            else if (Action == ActionCreate) Kill(vm, ObjectId);
            else if (Action == ActionDelete) ObjectId = Create(vm, MasterGUID, PrePos, PreDir);
        }

        public override void Replay(VMArchitecture arch)
        {
            var vm = arch.Context.VM;
            if (vm == null) return;
            if (Action == ActionMove) Apply(vm, ObjectId, PostPos, PostDir);
            else if (Action == ActionCreate) ObjectId = Create(vm, MasterGUID, PostPos, PostDir);
            else if (Action == ActionDelete) Kill(vm, ObjectId);
        }

        /// <summary>native InvokeOntoObject(record): re-apply the placement
        /// through the same production path the move command uses.</summary>
        private static void Apply(VM vm, ushort id, LotView.Model.LotTilePos pos, FSO.LotView.Model.Direction dir)
        {
            var obj = vm.GetObjectById((short)id);
            if (obj == null) return;
            obj.SetPosition(pos, dir, vm.Context, Model.VMPlaceRequestFlags.UserPlacement);
            obj.MultitileGroup.ExecuteEntryPoint(11, vm.Context); // User Placement
        }

        /// <summary>native KillObject: the production delete flow minus the
        /// money (the stack's money law owns the transaction).</summary>
        private static void Kill(VM vm, ushort id)
        {
            var obj = vm.GetObjectById((short)id);
            if (obj == null) return;
            obj.ExecuteEntryPoint(12, vm.Context, true);
            obj.Delete(false, vm.Context);
        }

        /// <summary>native ObjectState::CreateObject / MakeNewOutOfWorldObject:
        /// recreate from the stored master selector at the recorded
        /// placement. Returns the NEW base id (the instance identity changes
        /// across kill/create — the native record holds state, not identity,
        /// so the port entry refreshes its id after every recreate).</summary>
        private static ushort Create(VM vm, uint guid, LotView.Model.LotTilePos pos, FSO.LotView.Model.Direction dir)
        {
            var group = vm.Context.CreateObjectInstance(guid, pos, dir);
            group.ExecuteEntryPoint(11, vm.Context); // User Placement
            return (ushort)group.BaseObject.ObjectID;
        }
    }
    /// <summary>
    /// UI-42 tranche 2 — the native cRoofUndoable law (r256 §4 + the UI-42
    /// decode's fourth confirmation): stores the
    /// PREVIOUS roof pattern + pitch; UndoSelf SWAPS stored ↔ current (style
    /// then pitch) and RedoSelf is a 44-byte tail-call to UndoSelf — roof
    /// undo/redo is a PURE TOGGLE. The port models the toggle as a Swap used
    /// by both directions. CommitSelf is the base default (return 0): roof
    /// changes carry no money law. Submitted per click from the roof panel
    /// (the native cWinRoofPanel ×2 submit sites = the port's
    /// VMNetSetRoofCmd).
    /// </summary>
    public class VMRoofUndoEntry : VMArchitectureUndoEntry
    {
        /// <summary>The pre-change values (what undo restores); swapped on every apply.</summary>
        public uint Style;
        public float Pitch;

        public override void Restore(VMArchitecture arch) { Swap(arch); }
        public override void Replay(VMArchitecture arch) { Swap(arch); }

        private void Swap(VMArchitecture arch)
        {
            var s = arch.RoofStyle;
            var p = arch.RoofPitch;
            arch.SetRoof(Pitch, Style); // applies the stored pair (world refresh inside)
            Style = s; Pitch = p;       // native swap: displaced current becomes stored
        }
    }

    /// <summary>
    /// Pre-batch copy of one story's wall/floor arrays. WallsAt is NOT stored:
    /// it is fully derived from the Walls arrays (VMArchitecture.RebuildWallsAt
    /// is the authoritative rebuild), so restoring Walls regenerates it.
    /// </summary>
    public class VMArchitectureStorySnapshot
    {
        public WallTile[] Walls;
        public FloorTile[] Floors;
    }

    public class VMArchitectureUndoStack
    {
        /// <summary>DEVIATION from the native unbounded stack (law §2.3 / design §5.1).</summary>
        public const int MaxDepth = 100;

        private readonly List<VMArchitectureUndoEntry> UndoList = new List<VMArchitectureUndoEntry>();
        private readonly List<VMArchitectureUndoEntry> RedoList = new List<VMArchitectureUndoEntry>();

        /// <summary>
        /// Set while the stack itself drives RunCommands (redo replay) so the
        /// replay is not captured as a new user step (would clear the redo
        /// list and loop). Set across undo restores too, for symmetry.
        /// </summary>
        public bool CaptureSuspended;

        public bool CanUndo
        {
            get { return UndoList.Count > 0; }
        }

        public bool CanRedo
        {
            get { return RedoList.Count > 0; }
        }

        /// <summary>Native FlushCommandQueues equivalent (law §5): both stacks drop.</summary>
        public void Clear()
        {
            UndoList.Clear();
            RedoList.Clear();
        }

        /// <summary>
        /// Native SubmitUndoable (law §2): push the applied batch as one undo
        /// step and CLEAR the redo list — any newly submitted command kills
        /// redo. Called from VMArchitecture.RunCommands only for real-mode,
        /// non-transient, wall/floor-only TS1 batches that changed the lot.
        /// </summary>
        public void Submit(VMArchitectureUndoEntry entry)
        {
            UndoList.Add(entry);
            if (UndoList.Count > MaxDepth) UndoList.RemoveAt(0);
            RedoList.Clear();
        }

        /// <summary>
        /// Begin a capture for the touched stories of a batch. Returns null
        /// when capture must not happen (transient preview, visual (non-real)
        /// mode, non-TS1 lot, terrain/grass batch — tranche 2, or a replay in
        /// progress).
        /// </summary>
        public VMArchitectureUndoEntry BeginCapture(VMArchitecture arch, List<VMArchitectureCommand> commands, bool transient)
        {
            if (transient || !arch.RealMode || CaptureSuspended) return null;
            var vm = arch.Context.VM;
            if (vm == null || !vm.TS1) return null;

            bool hasTerrain = false;
            var levels = new List<int>();
            foreach (var cmd in commands)
            {
                if (cmd.Type == VMArchitectureCommandType.TERRAIN_RAISE ||
                    cmd.Type == VMArchitectureCommandType.TERRAIN_FLATTEN ||
                    cmd.Type == VMArchitectureCommandType.GRASS_DOT)
                {
                    hasTerrain = true; // UI-42 tranche 2: terrain drags now capture (native TerrainUndoable)
                }
                else if (!levels.Contains(cmd.level < 1 ? 1 : (cmd.level > arch.Stories ? arch.Stories : cmd.level)))
                {
                    // story snapshots only for architecture commands (pure
                    // terrain batches never touch walls/floors)
                    var level = cmd.level;
                    if (level < 1) level = 1;
                    if (level > arch.Stories) level = (sbyte)arch.Stories;
                    levels.Add(level);
                }
            }

            var entry = new VMArchitectureUndoEntry
            {
                Forward = commands,
                ActorUID = (commands.Count > 0) ? commands[0].CallerUID : 0,
                Saved = new Dictionary<int, VMArchitectureStorySnapshot>()
            };
            foreach (var level in levels)
            {
                var walls = arch.Walls[level - 1];
                var floors = arch.Floors[level - 1];
                var snapWalls = new WallTile[walls.Length];
                var snapFloors = new FloorTile[floors.Length];
                Array.Copy(walls, snapWalls, walls.Length);
                Array.Copy(floors, snapFloors, floors.Length);
                entry.Saved[level] = new VMArchitectureStorySnapshot { Walls = snapWalls, Floors = snapFloors };
            }
            if (hasTerrain)
            {
                // native TerrainUndoable::TakeSnapshot (r256 §4): full grid,
                // taken once per drag (one drag = one undo step, law §6)
                var t = arch.Terrain;
                entry.Terrain = new VMArchitectureTerrainSnapshot
                {
                    Heights = (short[])t.Heights.Clone(),
                    Grass = (byte[])t.GrassState.Clone(),
                    Sloped = (bool[])t.Sloped.Clone()
                };
            }
            return entry;
        }

        /// <summary>
        /// Native UndoLastCommand (law §3): restore the newest entry, move it
        /// to the redo list. Refunds the batch's signed cost first (money law
        /// §4); a refused takeback drops the remaining undo list, mirroring
        /// the native ReleaseUndoList failure path.
        /// </summary>
        public bool Undo(VMArchitecture arch)
        {
            if (UndoList.Count == 0) return false;
            var entry = UndoList[UndoList.Count - 1];
            UndoList.RemoveAt(UndoList.Count - 1);

            if (!ApplyMoney(arch, entry, true))
            {
                UndoList.Clear(); // native: a failed undo breaks the chain
                return false;
            }

            CaptureSuspended = true;
            try
            {
                entry.Restore(arch);
                arch.RebuildWallsAt();     // native UndoOne + list rebuild
                arch.SignalAllDirty();     // keep the dirty flags the Tick path expects
                arch.RegenRoomMap();       // native DirtyTileNeighborhood/room regen equivalent
                arch.SignalRedraw();
            }
            finally
            {
                CaptureSuspended = false;
            }

            RedoList.Add(entry);
            return true;
        }

        /// <summary>
        /// Native RedoLastCommand (law §3): replay the newest redo entry and
        /// push it back onto the undo list. Re-charges the batch's signed cost
        /// (Purchase law §4); a refused re-charge drops the remaining redo
        /// list, mirroring the native ReleaseRedoList failure path.
        /// </summary>
        public bool Redo(VMArchitecture arch)
        {
            if (RedoList.Count == 0) return false;
            var entry = RedoList[RedoList.Count - 1];
            RedoList.RemoveAt(RedoList.Count - 1);

            if (!ApplyMoney(arch, entry, false))
            {
                RedoList.Clear(); // native: a failed redo drops the rest of the redo list
                return false;
            }

            CaptureSuspended = true;
            try
            {
                // state is bit-identical to pre-batch after undo, so the batch
                // replays cleanly (design §2; native forward-iteration law §4).
                entry.Replay(arch);
            }
            finally
            {
                CaptureSuspended = false;
            }

            UndoList.Add(entry);
            return true;
        }

        /// <summary>
        /// Money law (§4 floors: per-change Purchase/Refund hooks): undo
        /// applies -Cost (refund a purchase / take back a sellback), redo
        /// re-applies +Cost (re-charge a purchase / replay a sellback).
        /// Routed through the synchronous bool PerformTransaction overload:
        /// the link's own family resolution (VMTS1GlobalLinkStub.
        /// FamilyForTransaction) carries the NBR-04 ActiveFamily fallback for
        /// community/downtown lots, its CanTransactBudgetForFamily guard
        /// refuses before mutating, and the refusal verdict now propagates to
        /// the Undo/Redo chain-drop law.
        /// No budget family (maxis infinite money) is a no-op, exactly like
        /// purchases.
        /// </summary>
        private static bool ApplyMoney(VMArchitecture arch, VMArchitectureUndoEntry entry, bool undo)
        {
            var vm = arch.Context.VM;
            if (vm == null || !vm.TS1) return true;
            int amount = Math.Abs(entry.Cost);
            if (amount == 0) return true;

            bool toPlayer = undo ? (entry.Cost > 0) : (entry.Cost < 0);
            uint uid1 = toPlayer ? uint.MaxValue : entry.ActorUID;
            uint uid2 = toPlayer ? entry.ActorUID : uint.MaxValue;

            // UI-22 P2 (indep-review 20260921): the previous body ran a
            // hand-rolled affordability pre-check that read only
            // TS1State.CurrentFamily (missing the NBR-04 ActiveFamily fallback,
            // so community-lot undo-of-sellback / redo-of-purchase skipped the
            // guard) and then called the void PerformTransaction overload,
            // discarding the stub's refusal verdict — a refused undo/redo
            // silently replayed the world without charging or refunding.
            // The bool overload carries both the fallback and the verdict.
            // null callback = purely synchronous transaction (the family budget
            // readouts read FAMI.Budget directly every frame).
            var link = vm.GlobalLink;
            if (link == null) return true;
            return link.PerformTransaction(vm, false, uid1, uid2, amount);
        }
    }
}
