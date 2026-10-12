using System;
using System.Linq;

namespace Simitone.Client.UI.Model
{
    /// <summary>
    /// R136/R137: the ORIGINAL route-history bookkeeping behind the Layout
    /// rating (House::AddLayoutTick 0x8c4f0 / ClearRouteHistory 0x8c430 /
    /// the pair rotation in House::EnterLiveMode's case 220 / the
    /// GetHouseStats tail 0x8c2ec-0x8c3d8 — machine-verified in
    /// tools/iff-dump/r136/, the call site decoded in r137/).
    ///
    /// ENGINE MODEL — RE-DERIVED AND VERIFIED (EXP-12 T1 decode, coordinated
    /// by the 2026-09-25 independent review
    /// coordination/evidence/COORD/indep-review-exp12-20260925-*; the R137
    /// field glosses below were REFUTED by that verification and corrected):
    ///   * House keeps twelve counters at +48..+92 = six (total, flagged)
    ///     generations; AddLayoutTick(flag) adds one sample per call:
    ///     `if (flag) H92++; H88++` — 0x10c2d8 is its ONLY call site.
    ///   * THE CALL SITE (cXPerson::Simulate, guard chain 0x10c254-0x10c2d8):
    ///     the tick fires when
    ///       personTick % 10 == 0
    ///       && s16[this+1484] == 0     (FAMILY-MEMBER gate: set by
    ///                                   Neighborhood::UpdateInstanceVisitorTypes
    ///                                   = Family::TestMember ? 0 : 1 —
    ///                                   VISITORS ARE NEVER SAMPLED)
    ///       && s16[this+142] == 0      (the cXObject dynamic flag)
    ///       && GetMotive(11) >= 0      (AWAKE: IsSleeping 0x108180 is
    ///                                   exactly GetMotive(11) < 0)
    ///     and the FLAG = (GetCurrentRoute() != NULL) — the Sim currently
    ///     HOLDS A ROUTE (inlined: route-stack count at +2724 != 0). So the
    ///     total channel counts sampled family-member Sims and the flagged
    ///     channel counts those holding a route; LayoutScore decays as Sims
    ///     spend more of their sampled time trekking.
    ///   * ClearRouteHistory 0x8c430 (image 0x100835a0; AUD-18-F
    ///     instruction-verified) seeds the five SNAPSHOT pairs
    ///     (House+0x30..+0x54, ten counters) from the global [TOC-0x7314]
    ///     (N0 = int(FCNS('fill value for layout history', 200.0f)) —
    ///     ORIG-02 layout-seed-n0.md) and ZEROES the live accumulator pair
    ///     +0x58/+0x5c (stw r4=0 at 0x10083620/24 — layout-seed-n0.md's
    ///     "all twelve reseeded" detail is corrected by AUD-18-F; the
    ///     fresh-house outcome is unchanged: SA = SB = 5*200 -> ratio -1).
    ///     Called from the house-rebuild/stat-snapshot paths
    ///     0xb05f4/0xb1684/0xb1c20 (renovation contexts).
    ///   * THE LAYOUT LAW (GetHouseStats tail, image 0x10083300;
    ///     AUD-18-F decompile): layoutScore =
    ///     (int)(100.0 * clamp((SA - 2*SB)/max(SA,1), 0.0, 1.0)) with
    ///     SA = H48+H56+H64+H72+H80+H88 and SB = H52+...+H92, both
    ///     int->double conversions PROPERLY XORed (r136's "RAW low word"
    ///     detail was wrong — ORIG-02 layout-seed-n0.md correction,
    ///     re-verified in the AUD-18-F decompile); single-precision
    ///     ratio, double 100.0 scale, fctiwz truncation. The engine's
    ///     max(sum,1) division guard pins the divisor as the TOTAL.
    ///
    /// PORT (disclosed where engine-internal):
    ///   * FLAG: the port's GetCurrentRoute() equivalent is a VMRoutingFrame
    ///     on the avatar's thread stack (the engine PushNewRoutingFrame per
    ///     route; it pops when the route completes) — NOT the R137
    ///     Velocity!=0 movement proxy (the route-hold state outlives
    ///     per-frame movement).
    ///   * GATES: family membership = GetPersonData(TS1FamilyNumber) ==
    ///     VM.TS1State.CurrentFamily.ChunkID (the engine's own test,
    ///     VMNetSimJoinCmd); awake = GetMotiveData(VMMotive.SleepState)
    ///     >= 0 (the port's motive slot 11 is literally SleepState). A
    ///     family-less lot samples nobody — engine-faithful (no TestMember
    ///     pass natively either).
    ///   * the +142 dynamic flag has NO port mapping — the Dead/GhostImage
    ///     liveness exclusion stands in for it (disclosed).
    ///   * cadence: the port samples once per VM clock tick rather than
    ///     every 10th per-person tick; the score normalizes by the total,
    ///     so the ratio is cadence-invariant (disclosed; unchanged from
    ///     R137).
    ///   * MULTI-GENERATION MODEL (closes the r136/orig-02 r136 disclosure):
    ///     the six-generation layout (five snapshot pairs + live accumulator)
    ///     with the EnterLiveMode rotation is implemented — the seed N0 = 200
    ///     (ORIG-02, recovered) reseeds the snapshot pairs and the accumulator
    ///     zeroes, and RotateLive() ages generations on LIVE entry, so the
    ///     post-renovation transient matches the engine instead of decaying by
    ///     the disclosed single-generation stand-in. LOS:
    ///     LayoutScore = (int)(100.0 * clamp((SA - 2*SB)/max(SA,1), 0, 1)).
    ///   * ClearRouteHistory = the port's WallsChanged (renovation) signal.
    /// </summary>
    public static class OriginalRouteHistory
    {
        // Six (total, flagged) generations (r136 §1, AUD-18-F corrected): the
        // five SNAPSHOT pairs (House+48..+84) retain seeded/aged history and the
        // live ACCUMULATOR pair (+88/+92) grows in-session. The engine rotates on
        // EnterLiveMode case 220 (0x8c8d4): the accumulator pair is pushed into the
        // newest history slot, the others age, and the accumulator zeroes.
        // The port keeps TotalSamples/FlaggedSamples as the ACCUMULATOR accessors
        // (the eng03flag live counters + the uihouse synthetic feeds read them).
        public static readonly long[] SnapshotTotals = new long[5];
        public static readonly long[] SnapshotFlagged = new long[5];
        private static long TotalAcc, FlaggedAcc;
        public static long TotalSamples { get => TotalAcc; set => TotalAcc = value; }
        public static long FlaggedSamples { get => FlaggedAcc; set => FlaggedAcc = value; }
        // AUD-17 E-2: the old `static bool Wired` latched on the FIRST VM —
        // every lot change disposes that VM (TS1GameScreen.CleanupLastWorld),
        // so from lot 2 onward WallsChanged was never wired and the static
        // counters blended every previous lot's trek history. Key the latch
        // on the VM instance; rewiring re-seeds (native ClearRouteHistory
        // runs on the house-rebuild/stat-snapshot paths of every entry).
        private static WeakReference WiredVMRef;
        private static long LastClockTicks = -1;

        /// <summary>
        /// The port's GetCurrentRoute() law: the avatar currently HOLDS a
        /// route iff a VMRoutingFrame sits on its thread stack (the native
        /// route-stack count at +2724).
        /// </summary>
        public static bool HoldsRoute(FSO.SimAntics.VMAvatar av)
        {
            var stack = av?.Thread?.Stack;
            if (stack == null) return false;
            foreach (var frame in stack)
                if (frame is FSO.SimAntics.Engine.VMRoutingFrame) return true;
            return false;
        }

        /// <summary>
        /// The verified +1484 gate: only members of the current family are
        /// sampled (native Family::TestMember; the engine's own membership
        /// test is TS1FamilyNumber == CurrentFamily.ChunkID).
        /// </summary>
        public static bool IsSampledFamilyMember(FSO.SimAntics.VM vm, FSO.SimAntics.VMAvatar av)
        {
            var fam = vm?.TS1State?.CurrentFamily;
            if (fam == null) return false; // family-less lot: nobody samples
            try
            {
                return av.GetPersonData(FSO.SimAntics.Model.VMPersonDataVariable.TS1FamilyNumber)
                    == fam.ChunkID;
            }
            catch { return false; }
        }

        /// <summary>
        /// One sampling pass over the family: each live, awake family-member
        /// Sim is one sample (AddLayoutTick's population channel); the flag
        /// marks Sims currently holding a route. Samples grow the live
        /// accumulator pair (the engine's H88/H92).
        /// </summary>
        public static void Sample(FSO.SimAntics.VM vm)
        {
            if (vm == null || vm.Context == null) return;
            Wire(vm);
            // engine-faithful pause handling: Simulate only runs when the VM
            // ticks — no clock advance, no samples
            long ticks;
            try { ticks = vm.Context.Clock.Ticks; }
            catch { return; }
            if (ticks == LastClockTicks) return;
            LastClockTicks = ticks;

            foreach (var e in vm.Entities)
            {
                var av = e as FSO.SimAntics.VMAvatar;
                if (av == null || av.Dead || av.GhostImage) continue;
                // the verified +1484 gate: family members only
                if (!IsSampledFamilyMember(vm, av)) continue;
                // the verified awake gate: Motive[11] (SleepState) >= 0
                try { if (av.GetMotiveData(FSO.SimAntics.Model.VMMotive.SleepState) < 0) continue; }
                catch { continue; }
                TotalAcc++;
                if (HoldsRoute(av)) FlaggedAcc++;
            }
        }

        /// <summary>
        /// The decoded Layout law over all six generations (GetHouseStats tail
        /// AUD-18-F): (int)(100.0 * clamp((SA - 2*SB)/max(SA,1), 0, 1)) with
        /// SA/SB the int->double sums (properly XORed in the engine); the f32
        /// ratio, f64 100.0 scale and fctiwz truncation are reproduced by the
        /// cast. CODE CLOSES the r136 disclosure: the port no longer uses the
        /// single-generation total-normalized approximation.
        /// </summary>
        public static int LayoutScore
        {
            get
            {
                long sa = TotalAcc, sb = FlaggedAcc;
                for (int i = 0; i < SnapshotTotals.Length; i++)
                {
                    sa += SnapshotTotals[i];
                    sb += SnapshotFlagged[i];
                }
                if (sa < 1) sa = 1; // the engine's max(sum,1) division guard
                float ratio = (float)(sa - 2 * sb) / sa;
                if (ratio < 0f) ratio = 0f;
                if (ratio > 1f) ratio = 1f;
                return (int)(100.0 * ratio);
            }
        }

        /// <summary>ClearRouteHistory — the renovation/rebuild reset. Native
        /// (image 0x100835a0; AUD-18-F instruction-verified) reseeds the TEN
        /// snapshot counters (five total + five flagged) to N0 = 200 and ZEROES
        /// the accumulator pair (stw r4=0 at 0x10083620/24). The fresh-house
        /// outcome is SA = SB = 5*200 -> ratio -1 -> score 0.</summary>
        public static void Clear()
        {
            for (int i = 0; i < SnapshotTotals.Length; i++)
            {
                SnapshotTotals[i] = 200;
                SnapshotFlagged[i] = 200;
            }
            TotalAcc = 0;
            FlaggedAcc = 0;
        }

        /// <summary>EnterLiveMode case 220 rotation (0x8c8d4): the accumulator
        /// pair is pushed into the newest history slot, the four older slots
        /// age (the oldest is dropped), and the accumulator zeroes. This is the
        /// native rolling-window semantics the r136 note banked as runtime-only;
        /// calling it on LIVE mode entry makes the post-renovation transient
        /// match the engine instead of the single-generation stand-in.</summary>
        public static void RotateLive()
        {
            for (int i = 0; i < SnapshotTotals.Length - 1; i++)
            {
                SnapshotTotals[i] = SnapshotTotals[i + 1];
                SnapshotFlagged[i] = SnapshotFlagged[i + 1];
            }
            SnapshotTotals[SnapshotTotals.Length - 1] = TotalAcc;
            SnapshotFlagged[SnapshotFlagged.Length - 1] = FlaggedAcc;
            TotalAcc = 0;
            FlaggedAcc = 0;
        }

        private static void Wire(FSO.SimAntics.VM vm)
        {
            if (WiredVMRef != null && ReferenceEquals(WiredVMRef.Target, vm)) return;
            var old = WiredVMRef?.Target as FSO.SimAntics.VM;
            if (old != null)
            {
                try { old.Context.Architecture.WallsChanged -= OnWallsChanged; } catch { }
            }
            WiredVMRef = new WeakReference(vm);
            Clear();
            LastClockTicks = -1;
            try
            {
                vm.Context.Architecture.WallsChanged += OnWallsChanged;
            }
            catch { }
        }

        private static void OnWallsChanged(FSO.SimAntics.VMArchitecture caller)
        {
            // the room map is rebuilt (ids renumbered) and the engine clears
            // the route history on the house-rebuild paths
            Clear();
        }
    }
}
