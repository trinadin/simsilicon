using FSO.Client.UI.Framework;
using System;
using System.Collections.Generic;

namespace Simitone.Client.UI.Controls
{
    /// <summary>
    /// UI-40 — the native Interface FX (BoboVision) window fade, shared by the
    /// original-chrome surfaces beyond the PIP (evidence/UI-40/interfacefx-law.md).
    ///
    /// The law is cTSWindow::AsyncShowHideWin 0x502E10 driving a RampGenerator
    /// (+0xB4) evaluated by GetOpacity 0x502770 — the exact state machine the
    /// R240/243 PIP round ported (r243-fade/contract.md; the PIP keeps its own
    /// private copy with its timer-ownership extras). Call sites in
    /// cWinCPanel::SetPanel 0x26EEE0 pass duration 0, which substitutes the
    /// 333 ms default; the curve is LINEAR (no easing anywhere in the engine —
    /// ORIG-02/rampgenerator-queue-animation-law.md). With the option off the
    /// callers take the plain ShowWindow/HideWindow snaps instead.
    ///
    /// Divergences kept identical to the PIP port on purpose:
    /// - a monotonic managed millisecond clock (native timeGetTime/TimeBase
    ///   calibration is a scheduling detail, not a law);
    /// - Update/PreDraw advance the ramp before input/draw (disclosed
    ///   adaptation; native finishes expired phases inside paint).
    /// </summary>
    internal sealed class UIOriginalWindowFade
    {
        private int Direction;          // window +0xAC: 0 idle, +1 opening, -1 closing
        private long FadeEnd;           // ramp endTick
        private float FadeTo, FadeSlope; // RampGenerator endValue / signedRate
        private UIElement Root;         // the window whose tree opacity this drives
        private readonly List<UIElement> Riders = new List<UIElement>();
        private bool TreeOpaque = true; // opacity restore bookkeeping
        internal Func<long> Clock = () => Environment.TickCount64;

        /// <summary>Extra trees that share the root's fade opacity and
        /// visibility (native example: the cWinCPanel live plate composites at
        /// GetOpacity(people) — TSPaint 0x26F6D8 — so the live-only backdrop
        /// rides the people window's fade).</summary>
        internal void AttachRider(UIElement rider)
        {
            if (rider != null && !Riders.Contains(rider)) Riders.Add(rider);
        }

        /// <summary>GetOpacity law for a window without the fixed-alpha flags:
        /// hidden → 0, idle → 255, active phase → byte(truncate(255·value+0.5)).</summary>
        internal int Opacity => Root == null ? 255
            : !Root.Visible ? 0
            : Direction == 0 ? 255
            : (int)MathF.FusedMultiplyAdd(255, FadeValue(Clock()), .5f);

        internal bool Idle => Direction == 0;

        /// <summary>Force the ramp to idle with the tree at full opacity — used
        /// when the caller snaps a window directly (Interface FX off): any
        /// active ramp from a prior FX-on window is discarded so it cannot
        /// apply stale opacity/visibility later.</summary>
        internal void Cancel()
        {
            Direction = 0;
            // Force the guard in ApplyTreeOpacity to actually run: if the tree
            // was mid-ramp (TreeOpaque=false) the prior pre-set to true would
            // early-return and leave the stale opacity behind. Clear it first,
            // then apply full opacity (which re-arms TreeOpaque=true).
            if (Root != null)
            {
                TreeOpaque = false;
                ApplyTreeOpacity(255);
            }
            else TreeOpaque = true;
        }

        // Native RampGenerator GetVal 14e930: target once past endTick, else the
        // single-precision fused subtraction from the endpoint.
        private float FadeValue(long now) => now >= FadeEnd ? FadeTo
            : MathF.FusedMultiplyAdd(-(float)(FadeEnd - now), FadeSlope, FadeTo);

        /// <summary>AsyncShowHideWin(show) — TS1InterfaceFX gated. The option is
        /// re-read per request exactly like the native Fade sites; a direct
        /// show/hide does not cancel an active ramp (dormant-ramp law).</summary>
        internal void Fade(UIElement root, bool show)
        {
            if (root == null) return;
            Root = root;
            if (!FSO.Client.GlobalSettings.Default.TS1InterfaceFX)
            {
                if (show) { Cancel(); ShowTrees(); } else HideTrees();
                return;
            }
            long now = Clock();
            Advance(now);
            if (show ? Root.Visible && Direction >= 0 : !Root.Visible || Direction < 0) return;
            float from = show ? 0 : 1;
            int duration = 333;
            if (Direction != 0)
            {
                float value = FadeValue(now);
                // AsyncShowHideWin complements the current value when reversing
                // a close (502fb4..50301c); open->close preserves it.
                from = Direction < 0 ? 1 - value : value;
                duration = (int)(.5f + 333f * from);
            }
            Direction = show ? 1 : -1;
            FadeTo = show ? 1 : 0;
            FadeSlope = duration == 0 ? 0 : (FadeTo - from) / duration;
            FadeEnd = now + duration;
            if (show) ShowTrees();
            // Native opacity is evaluated at paint time (GetOpacity), so a
            // fresh transition is visible at its START value on the very
            // frame it begins — apply the tree opacity immediately rather
            // than waiting for the next Update.
            ApplyTreeOpacity(Opacity);
        }

        private void ShowTrees()
        {
            Root.Visible = true;
            foreach (var rider in Riders) rider.Visible = true;
        }

        private void HideTrees()
        {
            if (Root != null) Root.Visible = false;
            foreach (var rider in Riders) rider.Visible = false;
            ApplyTreeOpacity(255);
        }

        /// <summary>Per-frame advance: finishes expired phases (closing hides
        /// the trees at completion — native HideWindow at the blit's finish)
        /// and applies the ramp opacity to the whole element tree.</summary>
        internal void Advance()
        {
            if (Root == null) return;
            Advance(Clock());
        }

        private void Advance(long now)
        {
            if (Direction == 0) return;
            if (now < FadeEnd)
            {
                ApplyTreeOpacity(Opacity);
                return;
            }
            if (Direction < 0) HideTrees();
            Direction = 0;
            ApplyTreeOpacity(255);
        }

        // Whole-window alpha: BlendColor is per-element, so a fade has to walk
        // the tree (the native blits one composed private buffer instead; the
        // port's element trees compose the same coverage).
        private void ApplyTreeOpacity(int alpha)
        {
            bool opaque = alpha >= 255;
            if (opaque && TreeOpaque) return;
            Apply(Root, alpha);
            foreach (var rider in Riders) Apply(rider, alpha);
            TreeOpaque = opaque;
        }

        private static void Apply(UIElement element, int alpha)
        {
            element.Opacity = alpha / 255f;
            element.InvalidateOpacity();
            if (element is UIContainer container)
                foreach (var child in container.GetChildren()) Apply(child, alpha);
        }
    }
}
