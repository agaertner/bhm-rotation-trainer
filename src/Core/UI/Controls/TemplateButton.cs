﻿using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using HorizontalAlignment = Blish_HUD.Controls.HorizontalAlignment;
using MouseEventArgs = Blish_HUD.Input.MouseEventArgs;

namespace Nekres.RotationTrainer.Controls {
    internal class TemplateButton : DetailsButton
    {
        public event EventHandler<MouseEventArgs> EditClick;
        public event EventHandler<EventArgs>      PlayClick;

        private const  int       BUTTON_WIDTH         = 345;
        private const  int       BUTTON_HEIGHT        = 100;
        private const  int       USER_WIDTH           = 75;
        private const  int       BOTTOMSECTION_HEIGHT = 35;

        private static Texture2D _backgroundSprite     = ContentService.Textures.Pixel;
        private static Texture2D _clipboardSprite      = RotationTrainerModule.Instance.ContentsManager.GetTexture("clipboard.png");
        private static Texture2D _dividerSprite        = GameService.Content.GetTexture("157218");
        private static Texture2D _glowClipboardSprite  = RotationTrainerModule.Instance.ContentsManager.GetTexture("glow_clipboard.png");
        private static Texture2D _glowPlaySprite       = RotationTrainerModule.Instance.ContentsManager.GetTexture("glow_play.png");
        private static Texture2D _glowUtilitySprite    = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");
        private static Texture2D _iconBoxSprite        = GameService.Content.GetTexture("controls/detailsbutton/605003");
        private static Texture2D _playSprite           = RotationTrainerModule.Instance.ContentsManager.GetTexture("play.png");
        private static Texture2D _utilitySprite        = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");

        private static Texture2D _editMacroTex         = RotationTrainerModule.Instance.ContentsManager.GetTexture("155941.png");
        private static Texture2D _editMacroTexHover    = RotationTrainerModule.Instance.ContentsManager.GetTexture("155940.png");
        private static Texture2D _editMacroTexActive   = RotationTrainerModule.Instance.ContentsManager.GetTexture("155942.png");
        private static Texture2D _editMacroTexDisabled = RotationTrainerModule.Instance.ContentsManager.GetTexture("155939.png");

        private        bool      _mouseOverPlay;
        private        Rectangle _playBounds;

        private bool _mouseOverTemplate;
        private Rectangle _templateBounds;

        private bool _mouseOverUtility1;
        private Rectangle _utility1Bounds;

        private bool _mouseOverUtility2;
        private Rectangle _utility2Bounds;

        private bool _mouseOverUtility3;
        private Rectangle _utility3Bounds;

        private Rectangle _editButtonBounds;
        private bool      _mouseOverEditButton;

        private bool      _active;
        public bool Active {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        public TemplateModel TemplateModel { get; }

        public string        BottomText    { get; set; }

        private int[] _utilRemappings;

        internal TemplateButton(TemplateModel templateModel)
        {
            if (templateModel == null) {
                return;
            }

            this.TemplateModel = templateModel;
            _utilRemappings    = templateModel.UtilityOrder.ToArray();
            Size               = new Point(BUTTON_WIDTH, BUTTON_HEIGHT);
        }



        protected override async void OnClick(MouseEventArgs e)
        {
            if (_mouseOverEditButton && !this.Active) {
                this.EditClick?.Invoke(this, e);
                GameService.Content.PlaySoundEffectByName("button-click");
            } 
            else if (_mouseOverTemplate) {
                await ClipboardUtil.WindowsClipboardService.SetTextAsync(this.TemplateModel.ToString());
                ScreenNotification.ShowNotification("Copied Template!");
                GameService.Content.PlaySoundEffectByName("button-click");
            }
            else if (_mouseOverPlay)
            {
                this.PlayClick?.Invoke(this, EventArgs.Empty);
                GameService.Content.PlaySoundEffectByName("button-click");
            } 
            else if (_mouseOverUtility1 || _mouseOverUtility2 || _mouseOverUtility3)
            {
                this.UpdateUtilityKeys();
                GameService.Content.PlaySoundEffectByName("button-click");
            }
            base.OnClick(e);
        }

        protected override void OnMouseMoved(MouseEventArgs e)
        {
            var relPos = RelativeMousePosition;
            _mouseOverPlay       = _playBounds.Contains(relPos);
            _mouseOverTemplate   = _templateBounds.Contains(relPos);
            _mouseOverUtility3   = _utility3Bounds.Contains(relPos);
            _mouseOverUtility2   = _utility2Bounds.Contains(relPos);
            _mouseOverUtility1   = _utility1Bounds.Contains(relPos);
            _mouseOverEditButton = _editButtonBounds.Contains(relPos);

            if (_mouseOverPlay) {
                this.BasicTooltipText = "Practice!";
            } else if (_mouseOverTemplate) {
                this.BasicTooltipText = "Copy Template";
            } else if (_mouseOverUtility1) {
                this.BasicTooltipText = "Assign Utility Key 1";
            } else if (_mouseOverUtility2) {
                this.BasicTooltipText = "Assign Utility Key 2";
            } else if (_mouseOverUtility3) {
                this.BasicTooltipText = "Assign Utility Key 3";
            } else {
                this.BasicTooltipText = this.Title;
            }

            base.OnMouseMoved(e);
        }

        private void UpdateUtilityKeys()
        {
            int index = _mouseOverUtility1 ? 0 : _mouseOverUtility2 ? 1 : 2;
            int swap  = _utilRemappings[index] == 2 ? 0 : _utilRemappings[index] + 1;

            if (Array.Exists(_utilRemappings, e => e == swap))
            {
                _utilRemappings[Array.FindIndex(_utilRemappings, e => e == swap)] = _utilRemappings[index];
            }

            _utilRemappings[index] = swap;

            this.TemplateModel.UtilityOrder = new ObservableCollection<int>(_utilRemappings); // Save changes.
        }

        protected override CaptureType CapturesInput()
        {
            return CaptureType.Mouse | CaptureType.Filter;
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            var iconSize = IconSize == DetailsIconSize.Large
                ? BUTTON_HEIGHT
                : BUTTON_HEIGHT - BOTTOMSECTION_HEIGHT;

            // Draw background
            spriteBatch.DrawOnCtrl(this, _backgroundSprite, bounds, Color.Black * 0.25f);

            // Draw bottom section (overlap to make background darker here)
            spriteBatch.DrawOnCtrl(this, _backgroundSprite,
                new Rectangle(0, bounds.Height - BOTTOMSECTION_HEIGHT, bounds.Width - BOTTOMSECTION_HEIGHT,
                    BOTTOMSECTION_HEIGHT), Color.Black * 0.1f);

            // Draw icons
            #region Icons
            _playBounds = new Rectangle(BUTTON_WIDTH - 36, bounds.Height - BOTTOMSECTION_HEIGHT + 1, 32, 32);
            spriteBatch.DrawOnCtrl(this, _mouseOverPlay ? _glowPlaySprite : _playSprite, _playBounds, Color.White);

            _templateBounds = new Rectangle(BUTTON_WIDTH - 73, bounds.Height - BOTTOMSECTION_HEIGHT + 1, 32, 32);
            spriteBatch.DrawOnCtrl(this, _mouseOverTemplate ? _glowClipboardSprite : _clipboardSprite, _templateBounds, Color.White);

            _utility3Bounds = new Rectangle(BUTTON_WIDTH - 109, bounds.Height - BOTTOMSECTION_HEIGHT + 1, 32, 32);
            spriteBatch.DrawOnCtrl(this, _mouseOverUtility3 ? _glowUtilitySprite : _utilitySprite, _utility3Bounds, Color.White);

            spriteBatch.DrawStringOnCtrl(this, (_utilRemappings[2]+1).ToString(), Content.DefaultFont14, _utility3Bounds, Color.White, 
                false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Bottom);

            _utility2Bounds = new Rectangle(BUTTON_WIDTH - 145, bounds.Height - BOTTOMSECTION_HEIGHT + 1, 32, 32);
            spriteBatch.DrawOnCtrl(this, _mouseOverUtility2 ? _glowUtilitySprite : _utilitySprite, _utility2Bounds, Color.White);

            spriteBatch.DrawStringOnCtrl(this, (_utilRemappings[1]+1).ToString(), Content.DefaultFont14, _utility2Bounds, Color.White,
                false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Bottom);

            _utility1Bounds = new Rectangle(BUTTON_WIDTH - 181, bounds.Height - BOTTOMSECTION_HEIGHT + 1, 32, 32);
            spriteBatch.DrawOnCtrl(this, _mouseOverUtility1 ? _glowUtilitySprite : _utilitySprite, _utility1Bounds, Color.White);

            spriteBatch.DrawStringOnCtrl(this, (_utilRemappings[0]+1).ToString(), Content.DefaultFont14, _utility1Bounds, Color.White,
                false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Bottom);

            #endregion

            // Draw bottom section seperator
            spriteBatch.DrawOnCtrl(this, _dividerSprite, new Rectangle(0, bounds.Height - 40, bounds.Width, 8), Color.White);

            // Draw edit button
            _editButtonBounds = new Rectangle(BUTTON_WIDTH - 66, (this.Height - BOTTOMSECTION_HEIGHT - 64) / 2, 64, 64);
            var editIcon = this.Active ? _editMacroTexActive : _mouseOverEditButton ? _editMacroTexHover : this.Enabled ? _editMacroTex : _editMacroTexDisabled;
            spriteBatch.DrawOnCtrl(this, editIcon, _editButtonBounds, Color.White);

            // Draw icon
            if (this.Icon != null)
            {
                spriteBatch.DrawOnCtrl(this, this.Icon, new Rectangle((bounds.Height - BOTTOMSECTION_HEIGHT) / 2 - 32, (bounds.Height - 35) / 2 - 32, 64, 64), Color.White);
                // Draw icon box
                spriteBatch.DrawOnCtrl(this, _iconBoxSprite, new Rectangle(0, 0, iconSize, iconSize), Color.White);
            }

            // Wrap text
            var text = Text;
            var wrappedText = DrawUtil.WrapText(Content.DefaultFont14, text, BUTTON_WIDTH - 40 - iconSize - 20);
            spriteBatch.DrawStringOnCtrl(this, wrappedText, Content.DefaultFont14,
                new Rectangle(89, 0, 216, Height - BOTTOMSECTION_HEIGHT), Color.White, false, true, 2);

            // Draw the profession;
            spriteBatch.DrawStringOnCtrl(this, BottomText, Content.DefaultFont14,
                new Rectangle(5, bounds.Height - BOTTOMSECTION_HEIGHT, USER_WIDTH, 35), Color.White, false, false, 0);
        }
    }
}