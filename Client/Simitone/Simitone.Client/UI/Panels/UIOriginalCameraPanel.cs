using FSO.Client;
using FSO.Client.UI.Framework;
using FSO.Common.Rendering.Framework.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simitone.Client.UI.Model;
using Simitone.Client.UI.Panels.LiveSubpanels;
using System;

namespace Simitone.Client.UI.Panels
{
    /// <summary>
    /// The original cWinCamPanel camera-mode toolbar. The panel occupies the
    /// same 804x100 band as the other cWinCPanel children; PanelBack is owned
    /// by UIMainPanel, so this class mounts only the eight child buttons and
    /// the two painted separator strips.
    ///
    /// Geometry comes directly from UIText.iff STR# 140 coordinate entries
    /// and cWinCamPanel::TSPaint (0x2667a0): size controls at x=82, quality at
    /// x=154, Custom at (11,11), Album at (226,35), and the 2x89 toothpicks at
    /// (133,6)/(206,6). Every button follows SetImage(sheet, 4, 1).
    /// </summary>
    public class UIOriginalCameraPanel : UIContainer
    {
        public UIOriginalSheetButton CamFrameSmall;
        public UIOriginalSheetButton CamFrameMed;
        public UIOriginalSheetButton CamFrameLarge;
        public UIOriginalSheetButton CamFrameCust;
        public UIOriginalSheetButton CamViewAlbum;
        public UIOriginalSheetButton CamQualLow;
        public UIOriginalSheetButton CamQualMed;
        public UIOriginalSheetButton CamQualHigh;

        public UIOriginalSheetButton[] SizeButtons;
        public UIOriginalSheetButton[] QualityButtons;

        public Texture2D ToothpickLeft;
        public Texture2D ToothpickRight;

        /// CPState defaults both fields to Medium (enum value 1).
        public int SnapshotSize { get; private set; } = 1;
        public int SnapshotQuality { get; private set; } = 1;

        public event Action<int> OnSnapshotSizeChanged;
        public event Action<int> OnSnapshotQualityChanged;
        public event Action OnViewAlbum;

        // ---- UI-39b: the native Quick Tips hover-dwell layer (camera panel) ----
        // Decode receipt: coordination/evidence/UI-39/quicktips-decode.md. The
        // camera panel's quick-tip payloads are STR# 140 [16]/[17] 'Snapshot
        // Sizes' + [18]/[19] 'Quality Settings' with the two group BMPs
        // kCamPopupDetail (0x455) / kCamPopupQualDetail (0x456), loaded by
        // cWinCamPanel::Init @0x266c2c/@0x266c38. The eight button one-liners
        // (the existing Tooltip fields) are NOT popup triples — the quick-tip
        // POPUP is the SIZE group for the four frame buttons and the QUALITY
        // group for the three quality buttons. The presenter is the same
        // cWinLivePopup (UIOptionAboutPopup); the dwell law is the same
        // cTSWinBtn arm/pump (0x50b702 / 0x50cdc0).
        public class QuickTipGroup
        {
            public string Kind;                          // "size" | "quality"
            public string Title; public string Body;
            public int ResourceID; public Texture2D Art; public string Member;
            public UIOriginalSheetButton[] Buttons;
            public UIOriginalOptionsPanel.OptRow Row;    // popup content triple
        }

        public QuickTipGroup SnapshotSizeTip;
        public QuickTipGroup QualitySettingTip;
        public readonly UIContainer QuickTipOverlay = new UIContainer();

        public const int QuickTipDwellMs = 250;           // native ON dwell (0xfa)
        public QuickTipGroup QuickTipHoveredGroup;        // native +0x164 arm state
        public QuickTipGroup QuickTipOpenedGroup;         // popup opened by a dwell fire
        public UIOptionAboutPopup CurrentPopup;
        public Func<int> QuickTipClock = () => Environment.TickCount;  // timeGetTime stand-in
        private int _qtStart;
        private bool _qtFired;

        public bool QuickTipsEnabled
        {
            get { return GlobalSettings.Default.TS1QuickTips; }
        }

        public UIOriginalCameraPanel()
        {
            Size = new Vector2(804, 100);

            CamFrameSmall = CameraButton("cpanel\\Buttons\\CamFrameSmall.bmp", 82, 70, 1);
            CamFrameMed = CameraButton("cpanel\\Buttons\\CamFrameMed.bmp", 82, 36, 3);
            CamFrameLarge = CameraButton("cpanel\\Buttons\\CamFrameLarge.bmp", 82, 4, 5);
            CamFrameCust = CameraButton("cpanel\\Buttons\\CamFrameCust.bmp", 11, 11, 7);
            CamViewAlbum = CameraButton("cpanel\\Buttons\\CamViewAlbum.bmp", 226, 35, 9);
            CamQualLow = CameraButton("cpanel\\Buttons\\CamQualLow.bmp", 154, 70, 11);
            CamQualMed = CameraButton("cpanel\\Buttons\\CamQualMed.bmp", 154, 36, 13);
            CamQualHigh = CameraButton("cpanel\\Buttons\\CamQualHigh.bmp", 154, 4, 15);

            SizeButtons = new[] { CamFrameSmall, CamFrameMed, CamFrameLarge, CamFrameCust };
            QualityButtons = new[] { CamQualLow, CamQualMed, CamQualHigh };

            for (int i = 0; i < SizeButtons.Length; i++)
            {
                var index = i;
                SizeButtons[i].OnButtonClick += (b) => SelectSnapshotSize(index);
            }
            for (int i = 0; i < QualityButtons.Length; i++)
            {
                var index = i;
                QualityButtons[i].OnButtonClick += (b) => SelectSnapshotQuality(index);
            }
            CamViewAlbum.OnButtonClick += (b) => OnViewAlbum?.Invoke();

            ToothpickLeft = Resolve("cpanel\\Backgrounds\\CameraToothpkLeft.bmp");
            ToothpickRight = Resolve("cpanel\\Backgrounds\\CameraToothpkRight.bmp");

            // UI-39b: the two group tip triples (STR# 140 [16]/[17], [18]/[19])
            // + their group BMPs; the overlay is added last so popups draw above
            // the buttons (the camera panel is a plain UIContainer — no caching
            // to clip at negative local Y).
            SnapshotSizeTip = MakeTipGroup("size", 16, 17, 1109, SizeButtons);   // kCamPopupDetail
            QualitySettingTip = MakeTipGroup("quality", 18, 19, 1110, QualityButtons); // kCamPopupQualDetail
            Add(QuickTipOverlay);

            RefreshSelection();
        }

        private UIOriginalSheetButton CameraButton(string member, int x, int y, int tooltipIndex)
        {
            var button = new UIOriginalSheetButton(member)
            {
                Position = new Vector2(x, y),
                Tooltip = GameFacade.Strings.GetString("140", tooltipIndex.ToString()),
            };
            Add(button);
            return button;
        }

        private static Texture2D Resolve(string member)
        {
            try { return UIOriginal.EnsureResolved(member)?.Get(GameFacade.GraphicsDevice); }
            catch { return null; }
        }

        private QuickTipGroup MakeTipGroup(string kind, int titleIdx, int bodyIdx, int resourceID, UIOriginalSheetButton[] buttons)
        {
            var strings = GameFacade.Strings;
            var title = strings.GetString("140", titleIdx.ToString());
            var body = strings.GetString("140", bodyIdx.ToString());
            var member = UIOriginal.ResourceNameForIDForProbe(resourceID);
            var row = new UIOriginalOptionsPanel.OptRow
            {
                Caption = title,
                AboutTitle = title,
                AboutBody = body,
                PopupMember = member ?? ("cpanel\\PopupCam" + kind + ".bmp"),
            };
            return new QuickTipGroup
            {
                Kind = kind,
                Title = title,
                Body = body,
                ResourceID = resourceID,
                Member = member,
                Art = UIOriginal.EnsureResolvedByID(resourceID)?.Get(GameFacade.GraphicsDevice),
                Buttons = buttons,
                Row = row,
            };
        }

        public override void Update(UpdateState state)
        {
            // UI-39b: the dwell pump runs every frame (arming must work with no
            // popup open). The hit test only arms the camera panel's own buttons;
            // the popup lives above the panel at negative local Y, so the
            // outside-the-panel check below dismisses it (native TSOnMouseExit).
            QuickTipProcess(QuickTipHitTest(state), QuickTipClock());
            base.Update(state);
            if (CurrentPopup == null) return;
            var mouse = GetMousePosition(state.MouseState);
            if (mouse.X < 0 || mouse.Y < 0 || mouse.X >= Size.X || mouse.Y >= Size.Y)
                ClosePopup();
        }

        /// Production hit test: the group whose button is under the mouse (the
        /// native buttons arm on hover; all four frame buttons share the SIZE
        /// group triple, all three quality buttons the QUALITY group triple).
        public QuickTipGroup QuickTipHitTest(UpdateState state)
        {
            if (HoverSet(SizeButtons, state)) return SnapshotSizeTip;
            if (HoverSet(QualityButtons, state)) return QualitySettingTip;
            return null;
        }

        private static bool HoverSet(UIOriginalSheetButton[] arr, UpdateState state)
        {
            if (arr == null) return false;
            foreach (var b in arr)
                if (HoverOver(b, state)) return true;
            return false;
        }

        private static bool HoverOver(UIElement e, UpdateState state)
        {
            if (e == null || !e.WillDraw()) return false;
            var p = e.GetMousePosition(state.MouseState);
            var s = e.Size;
            return p.X >= 0 && p.Y >= 0 && p.X < s.X && p.Y < s.Y;
        }

        /// The dwell pump. Production Update calls this with the live hit test
        /// and clock; the UI-39 gate calls the SAME method with a synthetic
        /// hover group and clock (disclosed probe seam — no synthetic mouse).
        public void QuickTipProcess(QuickTipGroup hovered, int now)
        {
            if (!ReferenceEquals(hovered, QuickTipHoveredGroup))
            {
                // 0x14 on leaving the armed group: its quick-tip popup closes.
                if (QuickTipOpenedGroup != null && CurrentPopup != null
                    && ReferenceEquals(CurrentPopup.Row, QuickTipOpenedGroup.Row))
                    ClosePopup();
                QuickTipOpenedGroup = null;
                QuickTipHoveredGroup = hovered;
                _qtStart = now;
                _qtFired = false;
                return;
            }
            if (QuickTipHoveredGroup == null || _qtFired) return;
            if (!QuickTipsEnabled) return;   // native OFF: buttons disarmed
            if (now - _qtStart >= QuickTipDwellMs)
            {
                _qtFired = true;             // native clears the arm after firing
                OpenPopup(QuickTipHoveredGroup);   // 0x13: same triple, same window
                QuickTipOpenedGroup = CurrentPopup != null ? QuickTipHoveredGroup : null;
            }
        }

        private void OpenPopup(QuickTipGroup g)
        {
            if (g == null) return;
            if (CurrentPopup != null && ReferenceEquals(CurrentPopup.Row, g.Row))
            {
                ClosePopup();
                return;
            }
            ClosePopup();
            var popup = new UIOptionAboutPopup(null, g.Row, g.Art);
            // cWinLivePopup::RebuildBuffer anchors to the parent's right/top
            // edges; the camera band is 804x100 like the options child.
            popup.Position = new Vector2(Size.X - popup.Size.X, -popup.Size.Y);
            CurrentPopup = popup;
            QuickTipOverlay.Add(popup);
        }

        public void ClosePopup()
        {
            if (CurrentPopup != null)
            {
                QuickTipOverlay.Remove(CurrentPopup);
                CurrentPopup = null;
            }
        }

        public void SelectSnapshotSize(int size)
        {
            if (size < 0 || size >= SizeButtons.Length) return;
            SnapshotSize = size;
            RefreshSelection();
            OnSnapshotSizeChanged?.Invoke(size);
        }

        public void SelectSnapshotQuality(int quality)
        {
            if (quality < 0 || quality >= QualityButtons.Length) return;
            SnapshotQuality = quality;
            RefreshSelection();
            OnSnapshotQualityChanged?.Invoke(quality);
        }

        public void RefreshSelection()
        {
            if (SizeButtons != null)
                for (int i = 0; i < SizeButtons.Length; i++)
                    SizeButtons[i].State = (byte)(i == SnapshotSize ? 1 : 0);
            if (QualityButtons != null)
                for (int i = 0; i < QualityButtons.Length; i++)
                    QualityButtons[i].State = (byte)(i == SnapshotQuality ? 1 : 0);
            if (CamViewAlbum != null) CamViewAlbum.State = 0;
        }

        public override void Draw(UISpriteBatch batch)
        {
            if (!Visible) return;

            // cWinCamPanel::TSPaint draws the seams into the parent buffer
            // before cTSWin composes the child buttons.
            if (ToothpickLeft != null)
                DrawLocalTexture(batch, ToothpickLeft, null, new Vector2(133, 6), Vector2.One);
            if (ToothpickRight != null)
                DrawLocalTexture(batch, ToothpickRight, null, new Vector2(206, 6), Vector2.One);

            base.Draw(batch);
        }
    }
}
