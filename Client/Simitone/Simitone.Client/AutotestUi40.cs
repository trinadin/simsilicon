using FSO.Client;
using FSO.Client.UI.Controls;
using FSO.Client.UI.Framework;
using FSO.Common.Rendering.Framework.IO;
using FSO.Common.Rendering.Framework.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simitone.Client.UI.Panels;
using Simitone.Client.UI.Screens;
using System;
using System.IO;
using System.Linq;

namespace Simitone.Client
{
    // UI-40 'ui40fx' (opt-in): the Interface FX window laws on PRODUCTION
    // surfaces — the cWinCPanel::SetPanel mode-window fades through
    // UIMainPanel.SetMode/ApplyMode (333ms linear ramps, r243 state machine)
    // and the dialog SetTransparent(1,0x80) fixed 128/255 translucency
    // through the production UIMobileAlert mount. Decode:
    // coordination/evidence/UI-40/interfacefx-law.md. Cases numbered 27+
    // per the tasking; never touches other probes' surfaces.
    internal static class AutotestUi40
    {
        internal static bool Check(TS1GameScreen game, out string diagnostics)
        {
            var failures = new System.Collections.Generic.List<string>();
            int passed = 0;
            Action<bool, string> require = (ok, name) => { if (ok) passed++; else failures.Add(name); };
            var settings = GlobalSettings.Default;
            bool fx = settings.TS1InterfaceFX;
            var panel = game.Frontend?.MainPanel;
            var savedMode = panel?.Mode ?? UIMainPanelMode.LIVE;
            long clock = 50000;
            var state = new UpdateState { WindowFocused = true, Time = new GameTime() };
            var gd = GameFacade.GraphicsDevice;
            var targets = gd.GetRenderTargets(); var viewport = gd.Viewport;
            var blend = gd.BlendState; var depth = gd.DepthStencilState; var raster = gd.RasterizerState;
            var sampler = gd.SamplerStates[0]; var texture = gd.Textures[0]; var scissor = gd.ScissorRectangle;
            try
            {
                require(panel != null, "case27-main-panel-mounted");
                if (panel != null)
                {
                    panel.ModeFadeClock = () => clock;
                    void tick(int elapsed) { clock += elapsed; panel.Update(state); }
                    void settle() { clock += 1000; panel.Update(state); }
                    // Normalize to a settled LIVE before pinning ramps.
                    if (panel.Mode != UIMainPanelMode.LIVE) panel.SetMode(UIMainPanelMode.LIVE);
                    settings.TS1InterfaceFX = true;
                    settle();
                    require(panel.Mode == UIMainPanelMode.LIVE && panel.PeopleChrome != null
                        && panel.PeopleChrome.Visible && panel.PeopleFadeOpacity == 255,
                        "case27-live-people-shown-idle");

                    // --- SetPanel fade-in/fade-out ramps (333ms linear law) ---
                    require(panel.ArchChrome != null && panel.BuyChrome != null && panel.CameraChrome != null,
                        "case27-mode-chromes-mounted");
                    if (panel.ArchChrome != null)
                    {
                        panel.SetMode(UIMainPanelMode.BUILD);
                        // Fresh show: logical visibility immediate, alpha 0.
                        require(panel.ArchChrome.Visible && panel.ArchFadeOpacity == 0,
                            "case28-arch-opening-logical-visible-alpha0");
                        // Fresh close: logically visible the whole ramp, alpha from 255.
                        require(panel.PeopleChrome.Visible && panel.PeopleFadeOpacity == 255,
                            "case28-people-closing-logical-visible");
                        tick(166);
                        require(panel.ArchFadeOpacity == 127, "case28-arch-midpoint127");
                        // The CLOSING side is linear 255->0, so its midpoint
                        // rounds to 128 (255*167/333+0.5 = 128.38), NOT 127 —
                        // the open side is 0->255 and rounds to 127. Asymmetric
                        // per the truncate(+0.5) law; the probe originally
                        // pinned both at 127 (off-by-one on the close).
                        require(panel.PeopleFadeOpacity == 128, "case28-people-midpoint128");
                        tick(157);   // 323ms
                        require(panel.ArchFadeOpacity == 247, "case28-arch-323ms-247");
                        require(panel.PeopleChrome.Visible, "case28-people-visible-through-closing");
                        tick(1);     // 324ms
                        require(panel.ArchFadeOpacity == 248, "case28-arch-324ms-248");
                        tick(9);     // 333ms
                        require(panel.ArchChrome.Visible && panel.ArchFadeOpacity == 255, "case28-arch-completes333");
                        require(!panel.PeopleChrome.Visible && panel.PeopleFadeOpacity == 0,
                            "case28-people-hidden-at333");
                        // The live plate rider shares the people fade (TSPaint law).
                        if (panel.TallSubpanel != null)
                        {
                            panel.SetMode(UIMainPanelMode.LIVE);
                            require(panel.TallSubpanel.Visible && Math.Abs(panel.TallSubpanel.Opacity - panel.PeopleFadeOpacity / 255f) < 0.01f,
                                "case32-rider-alpha-tracks-people");
                            tick(166);
                            require(panel.TallSubpanel.Visible && Math.Abs(panel.TallSubpanel.Opacity - 127f / 255f) < 0.01f,
                                "case32-rider-midpoint");
                            tick(167);
                            panel.SetMode(UIMainPanelMode.BUILD);   // rider rides the close
                            tick(332);
                            require(panel.TallSubpanel.Visible, "case32-rider-visible-through-close");
                            tick(1);
                            require(!panel.TallSubpanel.Visible, "case32-rider-hidden-at-close-completion");
                        }
                    }

                    // --- Reversal laws (same numbers the PIP r243 gate pins) ---
                    settle();       // BUILD, people hidden
                    panel.SetMode(UIMainPanelMode.LIVE);   // people opening
                    tick(100);
                    require(panel.PeopleFadeOpacity == 77, "case29-opening100ms-77");
                    panel.SetMode(UIMainPanelMode.BUILD);  // open->close preserves value
                    require(panel.PeopleFadeOpacity == 77, "case29-open-close-preserves-value");
                    tick(50);
                    require(panel.PeopleFadeOpacity == 38, "case29-reversed-close-half");
                    panel.SetMode(UIMainPanelMode.LIVE);   // close->open complements
                    require(panel.PeopleFadeOpacity == 217, "case29-close-open-native-complement");
                    tick(282);
                    require(panel.PeopleChrome.Visible && panel.PeopleFadeOpacity == 255,
                        "case29-reversed-open-rounding");

                    // --- OPTIONS panel fade-in on mount (cWinOptions window) ---
                    settle();
                    panel.SetMode(UIMainPanelMode.OPTIONS);
                    require(panel.SubPanel != null && panel.SubPanel.Visible && panel.OptionsFadeOpacity == 0,
                        "case30-options-mount-fade-start");
                    tick(166);
                    require(panel.OptionsFadeOpacity == 127, "case30-options-midpoint127");
                    tick(167);
                    require(panel.OptionsFadeOpacity == 255 && panel.SubPanel.Visible,
                        "case30-options-completes");
                    require(panel.SubPanel.Opacity > 0.999f, "case30-options-tree-opacity-restored");

                    // --- Interface FX OFF: the plain ShowWindow/HideWindow snaps ---
                    settings.TS1InterfaceFX = false;
                    settle();
                    panel.SetMode(UIMainPanelMode.BUY);
                    require(panel.BuyChrome != null && panel.BuyChrome.Visible && panel.BuyFadeOpacity == 255,
                        "case31-fxoff-buy-snap-shown");
                    require(!panel.PeopleChrome.Visible && panel.PeopleFadeOpacity == 0,
                        "case31-fxoff-people-snap-hidden");
                    if (panel.TallSubpanel != null)
                        require(!panel.TallSubpanel.Visible, "case31-fxoff-rider-snap-hidden");
                    panel.SetMode(UIMainPanelMode.LIVE);
                    settle();
                    settings.TS1InterfaceFX = true;
                    // Same-direction request while FX was off must not restart a
                    // dormant ramp (native option-toggle law); people was shown
                    // directly, alpha stays full.
                    require(panel.PeopleChrome.Visible && panel.PeopleFadeOpacity == 255,
                        "case31-fx-restored-no-phantom-ramp");
                    settle();
                }

                // --- Dialog translucency: SetTransparent(1,0x80) = 128/255 ---
                var options = new UIAlertOptions
                {
                    Title = "UI-40 probe",
                    Message = "Translucency law probe",
                    Buttons = new[] { new UIAlertButton(UIAlertButtonType.OK, null) }
                };
                settings.TS1InterfaceFX = true;
                var alert = new UIMobileAlert(options);
                require(alert.OriginalChrome, "case33-alert-original-chrome");
                require(Math.Abs(alert.OriginalOpacityMultiplier - 128f / 255f) < 0.0001f,
                    "case33-fx-on-multiplier-128");
                alert.InterpolatedAnimation = 1f;
                require(Math.Abs(alert.Opacity - 128f / 255f) < 0.0001f, "case33-fx-on-opacity-128");
                // Tutorial-kind dialogs keep their own presentation (excluded).
                var tutorial = new UIMobileAlert(options, false, 1.5);
                require(Math.Abs(tutorial.OriginalOpacityMultiplier - 1f) < 0.0001f,
                    "case33-tutorial-kind-excluded");
                settings.TS1InterfaceFX = false;
                var opaque = new UIMobileAlert(options);
                require(Math.Abs(opaque.OriginalOpacityMultiplier - 1f) < 0.0001f, "case33-fx-off-opaque");
                opaque.InterpolatedAnimation = 1f;
                require(Math.Abs(opaque.Opacity - 1f) < 0.0001f, "case33-fx-off-opacity-1");
                settings.TS1InterfaceFX = true;

                // Whole-window composite at 128/255 through the production draw
                // path: the RGB(0,0,82) fill over a fixed underlay must land at
                // the 50% blend, strictly between the opaque dialog and the
                // underlay, and the composite buffer must exist.
                var root = new UIContainer { ScaleX = UIScreen.Current.ScaleX, ScaleY = UIScreen.Current.ScaleY };
                root.Add(alert);
                int width = (int)Math.Ceiling(game.ScreenWidth * root.ScaleX);
                int height = (int)Math.Ceiling(game.ScreenHeight * root.ScaleY);
                var output = Path.Combine(FSO.Common.FSOEnvironment.UserDir, "ui-audit", "ui40");
                Directory.CreateDirectory(output);
                var underlay = new Color(45, 65, 45);
                Color[] half = null, reference = null;
                using (var target = new RenderTarget2D(gd, width, height, false, SurfaceFormat.Color, DepthFormat.None))
                using (var batch = new UISpriteBatch(gd, 0))
                {
                    Color[] capture()
                    {
                        Prime(root); Prime(alert);
                        batch.UIBegin(BlendState.AlphaBlend, SpriteSortMode.Immediate);
                        alert.PreDraw(batch);   // composes the private buffer at <1 multiplier
                        batch.End();
                        gd.SetRenderTarget(target); gd.Clear(underlay);
                        batch.UIBegin(BlendState.AlphaBlend, SpriteSortMode.Immediate);
                        alert.Draw(batch);
                        batch.End();
                        gd.SetRenderTargets(targets);
                        var pixels = new Color[width * height];
                        target.GetData(pixels);
                        return pixels;
                    }
                    half = capture();
                    using (var stream = File.Create(Path.Combine(output, "dialog-128.png")))
                        target.SaveAsPng(stream, width, height);
                    alert.OriginalOpacityMultiplier = 1f; alert.Opacity = 1f;
                    reference = capture();
                    using (var stream = File.Create(Path.Combine(output, "dialog-opaque.png")))
                        target.SaveAsPng(stream, width, height);
                }
                require(alert.TutorialBufferCountForProbe > 0, "case33-composite-buffer-live");
                var center = half[height / 2 * width + width / 2];
                var centerRef = reference[height / 2 * width + width / 2];
                var expected = Color.Lerp(underlay, centerRef, 128f / 255f);
                require(center != centerRef && Math.Abs(center.B - expected.B) <= 6 && Math.Abs(center.G - expected.G) <= 6,
                    "case33-fill-blends-at-128");
                root.Remove(alert);
            }
            catch (Exception ex) { failures.Add(ex.ToString()); }
            finally
            {
                settings.TS1InterfaceFX = fx;
                if (panel != null)
                {
                    panel.ModeFadeClock = () => Environment.TickCount64;
                    if (panel.Mode != savedMode) { try { panel.SetMode(savedMode); } catch { } }
                    try { panel.Update(new UpdateState { WindowFocused = true, Time = new GameTime() }); } catch { }
                }
                gd.SetRenderTargets(targets); gd.Viewport = viewport; gd.ScissorRectangle = scissor;
                gd.BlendState = blend; gd.DepthStencilState = depth; gd.RasterizerState = raster;
                gd.SamplerStates[0] = sampler; gd.Textures[0] = texture;
            }
            diagnostics = failures.Count == 0
                ? passed + " ok: SetPanel 333ms ramps (midpoints 127/247/248, completion, logical visibility, rider), reversal laws 77/38/217, options fade-in, FX-off snaps, dialog 128/255 composite"
                : string.Join("; ", failures);
            return failures.Count == 0;
        }

        private static void Prime(UIElement element)
        {
            element.CalculateMatrix(); _ = element.BlendColor;
            if (element is UIContainer container) foreach (var child in container.GetChildren()) Prime(child);
        }
    }
}
