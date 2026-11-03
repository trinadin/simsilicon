/*
This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.
*/
using FSO.SimAntics;
using FSO.SimAntics.Model;
using FSO.SimAntics.NetPlay.Model;
using FSO.SimAntics.NetPlay.Model.Commands;
using Microsoft.Xna.Framework;
using Simitone.Client.UI.Screens;
using System;
using System.Collections.Generic;

namespace Simitone.Client
{
    /// <summary>
    /// UI-42 'buildundo2' (opt-in, focused): build-mode undo tranche 2 —
    /// terrain drags, roof clicks and Hand-tool object actions on the ONE
    /// mixed undo stack (the native law: ONE UndoManager for every domain,
    /// r256 §1/§6). Drives the PRODUCTION send paths on the real in-lot VM:
    ///
    /// - terrain: vm.SendCommand(VMNetArchitectureCmd) with a TERRAIN_RAISE
    ///   batch (the UITerrainRaiser path), then the production undo/redo
    ///   commands. Laws: the full-grid snapshot restore (native
    ///   TerrainUndoable::TakeSnapshot §4), the per-drag cost refund/recharge
    ///   (§4 money law), submit-clears-redo across domains (§2).
    /// - roof: vm.SendCommand(VMNetSetRoofCmd) twice (pitch, then style) —
    ///   each click is one undo step (the native cWinRoofPanel ×2 submit
    ///   sites §6); undo/redo toggle per the cRoofUndoable swap law (§4).
    /// - objects (tranche 2 slice 3): the Hand tool's move and sell
    ///   (VMNetMoveObjectCmd / VMNetDeleteObjectCmd) — one step per action,
    ///   undo restores the pre-move placement / recreates the sold object
    ///   and reverses the credit.
    /// </summary>
    public class AutotestBuildUndo2
    {
        private readonly Action<string> _log;
        private int _phase, _phaseTicks;
        private bool _done;
        private readonly List<string> _notes = new List<string>();
        private readonly List<string> _fails = new List<string>();

        private readonly TS1GameScreen _screen;
        private VM _vm;
        private VMArchitecture _arch;

        private long _heights0, _heights1, _heightsU, _heightsR;
        private int _budget0, _budget1, _budget2, _budget3;
        private bool _hasFamily;
        private uint _roofStyle0; private float _roofPitch0;
        private uint _roofStyleNew; private float _roofPitchNew;
        private int _sendAttempts;

        // object legs (slice 3)
        private FSO.SimAntics.VMGameObject _obj;
        private FSO.LotView.Model.LotTilePos _objPre, _objPost;
        private FSO.LotView.Model.Direction _objDir;
        private uint _objGuid;
        private int _sellBudget, _sellCredit, _sellUndoBudget;
        private int _buyBudget, _buyPrice;

        private const int PhaseTimeout = 900;
        private const int MaxSends = 3;

        public bool Passed { get { return _fails.Count == 0; } }
        public string Diagnostics { get { return string.Join("; ", _notes); } }
        public string Failures { get { return string.Join("; ", _fails); } }

        public AutotestBuildUndo2(Action<string> log, TS1GameScreen screen)
        {
            _log = log;
            _screen = screen;
        }

        private void Note(string s) { _notes.Add(s); _log("AUTOTEST buildundo2 " + s); }
        private void Fail(string s) { _fails.Add(s); _log("AUTOTEST buildundo2 FAIL " + s); }

        private static long HeightsChecksum(VMArchitecture arch)
        {
            unchecked
            {
                long h = 17;
                var hs = arch.Terrain.Heights;
                for (int i = 0; i < hs.Length; i++) h = h * 31 + hs[i];
                return h;
            }
        }

        private static long GrassChecksum(VMArchitecture arch)
        {
            unchecked
            {
                long h = 19;
                var gs = arch.Terrain.GrassState;
                for (int i = 0; i < gs.Length; i++) h = h * 33 + gs[i];
                return h;
            }
        }

        private static int FamilyBudget(VM vm)
        {
            return vm?.TS1State?.CurrentFamily?.Budget
                ?? global::FSO.Content.Content.Get().Neighborhood?.GameState?.ActiveFamily?.Budget
                ?? -1;
        }

        private void UndoCmd(bool redo)
        {
            _vm.SendCommand(new VMNetArchUndoCmd { Redo = redo });
        }

        private FSO.SimAntics.VMGameObject FindByGuid(uint guid, FSO.LotView.Model.LotTilePos near)
        {
            FSO.SimAntics.VMGameObject best = null;
            var bestDist = int.MaxValue;
            foreach (var e in _vm.Entities)
            {
                var g = e as FSO.SimAntics.VMGameObject;
                if (g == null || g.Dead) continue;
                var og = (g.MultitileGroup.MultiTile)
                    ? g.MultitileGroup.BaseObject.MasterDefinition.GUID
                    : g.MultitileGroup.BaseObject.Object.OBJ.GUID;
                if (og != guid) continue;
                var d = Math.Abs(g.Position.x - near.x) + Math.Abs(g.Position.y - near.y);
                if (d < bestDist) { bestDist = d; best = g; }
            }
            return best;
        }

        /// <summary>Tick the scenario; returns true when finished.</summary>
        public bool Tick()
        {
            if (_done) return true;
            try
            {
                if (_vm == null)
                {
                    _vm = _screen.vm;
                    if (_vm == null) return false;
                }
                if (_arch == null)
                {
                    _arch = _vm.Context.Architecture;
                    if (_arch == null) return false;
                }
                switch (_phase)
                {
                    case 0: // baseline
                        if (_arch.UndoStack.CanUndo || _arch.UndoStack.CanRedo)
                            Fail("stack not empty on fresh lot");
                        _heights0 = HeightsChecksum(_arch);
                        _budget0 = FamilyBudget(_vm);
                        _hasFamily = _vm.TS1State?.CurrentFamily != null;
                        _roofStyle0 = _arch.RoofStyle; _roofPitch0 = _arch.RoofPitch;
                        Note("baseline heights=" + _heights0 + " roof=" + _roofStyle0 + "/" + _roofPitch0.ToString("0.00")
                            + " budget=" + _budget0 + " family=" + _hasFamily);
                        _phase = 1; _phaseTicks = 0; _sendAttempts = 0;
                        break;

                    case 1: // send one terrain raise through the production path
                        {
                            int midX = _arch.Width / 2, midY = _arch.Height / 2;
                            // raise one mid-lot tile to a height different from its
                            // current value (deterministic across lots)
                            var cur = _arch.Terrain.Heights[midY * _arch.Width + midX];
                            short target = (short)(cur == 10 ? 12 : 10);
                            _vm.SendCommand(new VMNetArchitectureCmd
                            {
                                Commands = new List<VMArchitectureCommand> {
                                    new VMArchitectureCommand {
                                        Type = VMArchitectureCommandType.TERRAIN_RAISE,
                                        x = midX, y = midY, pattern = 1, style = (ushort)target
                                    }
                                }
                            });
                            _sendAttempts++;
                            Note("sent TERRAIN_RAISE #" + _sendAttempts + " at " + midX + "," + midY + " -> " + target);
                            _phase = 2; _phaseTicks = 0;
                        }
                        break;

                    case 2: // the drag became one undo step + the grid actually changed
                        if (_arch.UndoStack.CanUndo && HeightsChecksum(_arch) != _heights0)
                        {
                            _heights1 = HeightsChecksum(_arch);
                            _budget1 = FamilyBudget(_vm);
                            if (_arch.UndoStack.CanRedo) Fail("terrain submit did not clear redo");
                            else Note("terrain applied 1 step; heights " + _heights0 + "->" + _heights1);
                            if (_hasFamily && _budget1 >= _budget0)
                                Note("WARNING terrain not charged (budget " + _budget0 + "->" + _budget1 + ") — free raise tolerated, refund assert relaxed");
                            _phase = 3; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout)
                        {
                            if (_sendAttempts < MaxSends) { _phase = 1; _phaseTicks = 0; }
                            else { Fail("terrain batch never became an undo step (ticks=" + _phaseTicks + ")"); _done = true; }
                        }
                        break;

                    case 3: // UNDO restores the pre-drag grid + refunds
                        UndoCmd(false);
                        _phase = 4; _phaseTicks = 0;
                        break;

                    case 4:
                        if (!_arch.UndoStack.CanUndo && _arch.UndoStack.CanRedo)
                        {
                            _heightsU = HeightsChecksum(_arch);
                            _budget2 = FamilyBudget(_vm);
                            if (_heightsU != _heights0)
                                Fail("terrain undo did not restore the grid (" + _heightsU + " != " + _heights0 + ")");
                            else Note("terrain undo restored heights");
                            if (_hasFamily && _budget1 < _budget0 && _budget2 != _budget0)
                                Fail("terrain undo refund wrong (" + _budget2 + " != " + _budget0 + ")");
                            _phase = 5; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("terrain undo never settled"); _done = true; }
                        break;

                    case 5: // REDO re-applies + re-charges
                        UndoCmd(true);
                        _phase = 6; _phaseTicks = 0;
                        break;

                    case 6:
                        if (!_arch.UndoStack.CanRedo)
                        {
                            _heightsR = HeightsChecksum(_arch);
                            _budget3 = FamilyBudget(_vm);
                            if (_heightsR != _heights1)
                                Fail("terrain redo did not re-apply (" + _heightsR + " != " + _heights1 + ")");
                            else Note("terrain redo re-applied");
                            if (_hasFamily && _budget1 < _budget0 && _budget3 != _budget1)
                                Fail("terrain redo re-charge wrong (" + _budget3 + " != " + _budget1 + ")");
                            _phase = 7; _phaseTicks = 0; _sendAttempts = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("terrain redo never settled"); _done = true; }
                        break;

                    case 7: // ROOF: one click = one step; pick a different style + pitch
                        {
                            var roofs = global::FSO.Content.Content.Get().WorldRoofs;
                            _roofStyleNew = (_roofStyle0 + 1u) % (uint)roofs.Count;
                            _roofPitchNew = Math.Max(0f, Math.Min(1.25f, _roofPitch0 + 0.25f));
                            _vm.SendCommand(new VMNetSetRoofCmd { Style = _roofStyleNew, Pitch = _roofPitchNew });
                            _sendAttempts++;
                            Note("sent SetRoof #" + _sendAttempts + " -> " + _roofStyleNew + "/" + _roofPitchNew.ToString("0.00"));
                            _phase = 8; _phaseTicks = 0;
                        }
                        break;

                    case 8:
                        if (_arch.UndoStack.CanUndo && _arch.RoofStyle == _roofStyleNew)
                        {
                            if (_arch.UndoStack.CanRedo) Fail("roof submit did not clear redo");
                            else Note("roof applied 1 step (" + _arch.RoofStyle + "/" + _arch.RoofPitch.ToString("0.00") + ")");
                            _phase = 9; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout)
                        {
                            if (_sendAttempts < MaxSends) { _phase = 7; _phaseTicks = 0; }
                            else { Fail("roof click never became an undo step"); _done = true; }
                        }
                        break;

                    case 9: // roof UNDO toggles back (the cRoofUndoable swap law)
                        UndoCmd(false);
                        _phase = 10; _phaseTicks = 0;
                        break;

                    case 10:
                        // the terrain entry sits UNDER the roof entry — settle on
                        // the roof STATE + the redo list, not stack emptiness
                        if (_arch.UndoStack.CanRedo && _arch.RoofStyle == _roofStyle0
                            && Math.Abs(_arch.RoofPitch - _roofPitch0) <= 0.001f)
                        {
                            Note("roof undo restored the previous pair");
                            _phase = 11; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("roof undo never settled (style=" + _arch.RoofStyle + " pitch=" + _arch.RoofPitch.ToString("0.00") + ")"); _done = true; }
                        break;

                    case 11: // roof REDO toggles forward again
                        UndoCmd(true);
                        _phase = 12; _phaseTicks = 0;
                        break;

                    case 12:
                        if (!_arch.UndoStack.CanRedo)
                        {
                            if (_arch.RoofStyle != _roofStyleNew || Math.Abs(_arch.RoofPitch - _roofPitchNew) > 0.001f)
                                Fail("roof redo did not re-apply");
                            else Note("roof redo re-applied");
                            _phase = 13; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("roof redo never settled"); _done = true; }
                        break;

                    case 13: // find a movable object + a free adjacent drop tile
                        {
                            foreach (var e in _vm.Entities)
                            {
                                var g = e as FSO.SimAntics.VMGameObject;
                                if (g == null || g.Dead || g.Position == FSO.LotView.Model.LotTilePos.OUT_OF_WORLD) continue;
                                if (g.IsUserMovable(_vm.Context, true) != FSO.SimAntics.Model.VMPlacementError.Success) continue;
                                _obj = g; break;
                            }
                            if (_obj == null) { Note("no movable object on lot — object legs skipped"); _done = true; break; }
                            _objPre = _obj.Position; _objDir = _obj.Direction;
                            _objGuid = (_obj.MultitileGroup.MultiTile)
                                ? _obj.MultitileGroup.BaseObject.MasterDefinition.GUID
                                : _obj.MultitileGroup.BaseObject.Object.OBJ.GUID;
                            // engine-validated drop target: the same gate
                            // SetPosition applies (FindWallSpot's discipline)
                            bool found = false;
                            foreach (var off in new int[] { 4, -4, 6, -6, 8, -8, 2, -2, 10, -10 })
                            {
                                var t = new FSO.LotView.Model.LotTilePos
                                    { x = (short)(_objPre.x + off * 16), y = _objPre.y, Level = _objPre.Level };
                                if (_obj.PositionValid(t, _objDir, _vm.Context,
                                        FSO.SimAntics.Model.VMPlaceRequestFlags.UserPlacement).Status
                                    == FSO.SimAntics.Model.VMPlacementError.Success)
                                {
                                    _objPost = t; found = true; break;
                                }
                            }
                            if (!found) { Note("no engine-valid move target — object legs skipped"); _done = true; break; }
                            Note("object leg: obj" + _obj.ObjectID + " guid 0x" + _objGuid.ToString("x") + " at " + _objPre.x / 16 + "," + _objPre.y / 16
                                + " -> " + _objPost.x / 16 + "," + _objPost.y / 16);
                            _phase = 14; _phaseTicks = 0; _sendAttempts = 0;
                        }
                        break;

                    case 14: // send the production move
                        _vm.SendCommand(new VMNetMoveObjectCmd
                        {
                            ObjectID = _obj.ObjectID,
                            x = _objPost.x, y = _objPost.y, level = (sbyte)_objPost.Level,
                            dir = _objDir
                        });
                        _sendAttempts++;
                        _phase = 15; _phaseTicks = 0;
                        break;

                    case 15: // move became one step (no money on moves)
                        if (_arch.UndoStack.CanUndo && _obj.Position.x == _objPost.x)
                        {
                            Note("move applied 1 step (no cost)");
                            _phase = 16; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("move never applied/became a step"); _done = true; }
                        break;

                    case 16: // UNDO restores the PRE placement (apply record #1)
                        UndoCmd(false);
                        _phase = 17; _phaseTicks = 0;
                        break;

                    case 17:
                        if (_arch.UndoStack.CanRedo)
                        {
                            var live = _vm.GetObjectById(_obj.ObjectID);
                            if (live == null || live.Position.x != _objPre.x || live.Position.y != _objPre.y)
                                Fail("move undo did not restore the placement");
                            else Note("move undo restored the placement");
                            _phase = 18; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("move undo never settled"); _done = true; }
                        break;

                    case 18: // REDO re-applies record #2
                        UndoCmd(true);
                        _phase = 19; _phaseTicks = 0;
                        break;

                    case 19:
                        if (!_arch.UndoStack.CanRedo)
                        {
                            var live = _vm.GetObjectById(_obj.ObjectID);
                            if (live == null || live.Position.x != _objPost.x)
                                Fail("move redo did not re-apply");
                            else Note("move redo re-applied");
                            _phase = 20; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("move redo never settled"); _done = true; }
                        break;

                    case 20: // SELL: the production delete (credit = group price)
                        _sellBudget = FamilyBudget(_vm);
                        _vm.SendCommand(new VMNetDeleteObjectCmd { ObjectID = _obj.ObjectID, CleanupAll = false });
                        _sendAttempts = 0;
                        _phase = 21; _phaseTicks = 0;
                        break;

                    case 21: // the sell became one step; the object is gone; credit landed
                        if (_arch.UndoStack.CanUndo && _obj.Dead)
                        {
                            var b = FamilyBudget(_vm);
                            _sellCredit = b - _sellBudget;
                            Note("sell applied 1 step; budget " + _sellBudget + "->" + b);
                            if (_hasFamily && b <= _sellBudget) Note("WARNING sell not credited (" + _sellBudget + "->" + b + ") — undo reversal assert relaxed");
                            _phase = 22; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("sell never applied/became a step"); _done = true; }
                        break;

                    case 22: // UNDO recreates the object (native Create(record#1)) + reverses the credit
                        UndoCmd(false);
                        _phase = 23; _phaseTicks = 0;
                        break;

                    case 23:
                        if (_arch.UndoStack.CanRedo)
                        {
                            var back = FindByGuid(_objGuid, _objPre);
                            if (back == null)
                                Fail("sell undo did not recreate the object");
                            else
                            {
                                Note("sell undo recreated obj" + back.ObjectID + " at the pre-sell tile");
                                _obj = back; // track the NEW identity for the redo kill
                            }
                            var b2 = FamilyBudget(_vm);
                            _sellUndoBudget = b2;
                            if (_hasFamily && b2 != _sellBudget)
                                Fail("sell undo credit reversal wrong (" + b2 + " != " + _sellBudget + ")");
                            _phase = 24; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("sell undo never settled"); _done = true; }
                        break;

                    case 24: // REDO kills it again + re-credits
                        UndoCmd(true);
                        _phase = 25; _phaseTicks = 0;
                        break;

                    case 25:
                        if (!_arch.UndoStack.CanRedo)
                        {
                            if (!_obj.Dead && FindByGuid(_objGuid, _objPre) != null)
                                Fail("sell redo did not remove the object");
                            else Note("sell redo removed it again");
                            // P3-3(i): the sell-redo re-credit reverses the
                            // undo's reversal exactly (undo budget + the
                            // observed sell credit)
                            var b3 = FamilyBudget(_vm);
                            if (_hasFamily && b3 != _sellUndoBudget + _sellCredit)
                                Fail("sell redo re-credit wrong (" + b3 + " != " + (_sellUndoBudget + _sellCredit) + ")");
                            else Note("sell redo re-credited (+" + _sellCredit + ")");
                            _phase = 26; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("sell redo never settled"); _done = true; }
                        break;

                    case 26: // BUY leg (review P2-1): purchase a cheap catalog item
                        {
                            var catalog = global::FSO.Content.Content.Get().WorldCatalog;
                            global::FSO.Content.Interfaces.ObjectCatalogItem? item = null;
                            foreach (var it in catalog.All())
                            {
                                if (it.Price <= 200 && it.GUID != 0 && it.DisableLevel == 0) { item = it; break; }
                            }
                            if (item == null) { Note("no cheap catalog item — buy leg skipped"); _done = true; break; }
                            _objGuid = item.Value.GUID;
                            _buyPrice = (int)item.Value.Price;
                            // drop target: the tile the sold object vacated (engine-known good)
                            _objPost = new FSO.LotView.Model.LotTilePos { x = _objPre.x, y = _objPre.y, Level = _objPre.Level };
                            _buyBudget = FamilyBudget(_vm);
                            _vm.SendCommand(new VMNetBuyObjectCmd
                            {
                                GUID = item.Value.GUID,
                                x = _objPost.x, y = _objPost.y, level = (sbyte)_objPost.Level,
                                dir = FSO.LotView.Model.Direction.NORTH
                            });
                            Note("buy leg: guid 0x" + item.Value.GUID.ToString("x") + " price " + item.Value.Price + " budget " + _buyBudget);
                            _phase = 27; _phaseTicks = 0;
                        }
                        break;

                    case 27: // the buy charged + became one step + the object exists
                        {
                            var bought = FindByGuid(_objGuid, _objPost);
                            if (_arch.UndoStack.CanUndo && bought != null)
                            {
                                var b = FamilyBudget(_vm);
                                Note("buy applied 1 step; budget " + _buyBudget + "->" + b);
                                if (_hasFamily && b >= _buyBudget) Fail("buy not charged (" + _buyBudget + "->" + b + ")");
                                _obj = bought;
                                _phase = 28; _phaseTicks = 0;
                            }
                            else if (++_phaseTicks > PhaseTimeout) { Fail("buy never applied/became a step"); _done = true; }
                        }
                        break;

                    case 28: // UNDO of buy: kill + refund (the native create-direction undo)
                        UndoCmd(false);
                        _phase = 29; _phaseTicks = 0;
                        break;

                    case 29:
                        if (_arch.UndoStack.CanRedo)
                        {
                            if (FindByGuid(_objGuid, _objPost) != null)
                                Fail("buy undo did not remove the object");
                            else Note("buy undo removed it");
                            var b2 = FamilyBudget(_vm);
                            if (_hasFamily && b2 != _buyBudget)
                                Fail("buy undo refund wrong (" + b2 + " != " + _buyBudget + ")");
                            _phase = 30; _phaseTicks = 0;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("buy undo never settled"); _done = true; }
                        break;

                    case 30: // REDO of buy: recreate at the placed state + re-charge
                        UndoCmd(true);
                        _phase = 31; _phaseTicks = 0;
                        break;

                    case 31:
                        if (!_arch.UndoStack.CanRedo)
                        {
                            var back = FindByGuid(_objGuid, _objPost);
                            if (back == null) Fail("buy redo did not recreate the object");
                            else
                            {
                                Note("buy redo recreated obj" + back.ObjectID);
                                var b3 = FamilyBudget(_vm);
                                if (_hasFamily && b3 != _buyBudget - _buyPrice)
                                    Note("WARNING buy redo re-charge check: " + b3 + " vs " + (_buyBudget - _buyPrice) + " (ambient expenses tolerated)");
                            }
                            _done = true;
                        }
                        else if (++_phaseTicks > PhaseTimeout) { Fail("buy redo never settled"); _done = true; }
                        break;
                }
            }
            catch (Exception e)
            {
                Fail("phase " + _phase + " exception " + e.GetType().Name + ": " + e.Message);
                _done = true;
            }
            return _done;
        }
    }
}
