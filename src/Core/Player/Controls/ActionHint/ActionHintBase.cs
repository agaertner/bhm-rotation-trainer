using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using Nekres.RotationTrainer.Core.Player.Models;
using System;
using System.Collections.Generic;

namespace Nekres.RotationTrainer.Core.Player.Controls {
    internal abstract class ActionHintBase<T> : Control where T : ActionBase {

        private static Dictionary<GuildWarsAction, Rectangle> _abilityBounds = new()
{
            { GuildWarsAction.SwapWeapons, new Rectangle(-383, 38, 43, 43) },
            { GuildWarsAction.WeaponSkill1, new Rectangle(-328, 24, 58, 58) },
            { GuildWarsAction.WeaponSkill2, new Rectangle(-267, 24, 58, 58) },
            { GuildWarsAction.WeaponSkill3, new Rectangle(-206, 24, 58, 58) },
            { GuildWarsAction.WeaponSkill4, new Rectangle(-145, 24, 58, 58) },
            { GuildWarsAction.WeaponSkill5, new Rectangle(-84, 24, 58, 58) },
            { GuildWarsAction.HealingSkill, new Rectangle(87, 24, 58, 58) },
            { GuildWarsAction.UtilitySkill1, new Rectangle(148, 24, 58, 58) },
            { GuildWarsAction.UtilitySkill2, new Rectangle(209, 24, 58, 58) },
            { GuildWarsAction.UtilitySkill3, new Rectangle(270, 24, 58, 58) },
            { GuildWarsAction.EliteSkill, new Rectangle(332, 24, 58, 58) },
            { GuildWarsAction.ProfessionSkill1, new Rectangle(-350, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill2, new Rectangle(-330, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill3, new Rectangle(-310, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill4, new Rectangle(-290, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill5, new Rectangle(-270, 150, 48, 48) },
            { GuildWarsAction.SpecialAction, new Rectangle(-85, 157, 54, 54) },
            { GuildWarsAction.Dodge, new Rectangle(0, 100, 54, 54) },
            { GuildWarsAction.Interact, new Rectangle(664, 420, 54, 54) }
        };
        private static Dictionary<GuildWarsAction, string> _abilityText = new() {
            { GuildWarsAction.ProfessionSkill1, "F1" },
            { GuildWarsAction.ProfessionSkill2, "F2" },
            { GuildWarsAction.ProfessionSkill3, "F3" },
            { GuildWarsAction.ProfessionSkill4, "F4" },
            { GuildWarsAction.ProfessionSkill5, "F5" },
            { GuildWarsAction.Dodge, "Dodge" },
            { GuildWarsAction.Interact, "Interact" },
        };
        private static   Texture2D  _frame     = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");
        private static   Texture2D  _arrow     = RotationTrainerModule.Instance.ContentsManager.GetTexture("991944.png");
        protected static BitmapFont _font      = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size36, ContentService.FontStyle.Regular);
        protected static BitmapFont _smallFont = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size22, ContentService.FontStyle.Regular);

        private   Rectangle   _bounds;
        private   string      _text;
        private   int         _arrowHeight;
        private   Glide.Tween _arrowTween;
        private   KeyBinding  _keyBinding;

        protected T           _action { get; private set; }
        protected Rectangle   Bounds  { get; private set; }

        public bool Completed { get; protected set; }

        protected ActionHintBase(T action) {
            _action     = action;
            this.Parent = GameService.Graphics.SpriteScreen;
            this.Size   = GameService.Graphics.SpriteScreen.Size;

            _arrowTween = GameService.Animation.Tweener.Tween(this, new { _arrowHeight = _arrowHeight + 10 }, 0.7f).Repeat();

            _abilityBounds.TryGetValue(action.Action, out _bounds);
            _abilityText.TryGetValue(action.Action, out _text);

            // Find key binding.
            if (RotationTrainerModule.Instance.ActionBindings.TryGetValue(_action.Action, out SettingEntry<KeyBinding> keyBindingSetting)) {
                _keyBinding = keyBindingSetting.Value;

                if (_keyBinding.PrimaryKey == Keys.None && _keyBinding.ModifierKeys == ModifierKeys.None) {
                    ScreenNotification.ShowNotification($"You need to assign a key to {_action.Action.ToFriendlyString()}.");
                }

                _keyBinding.Activated += OnActivated;
            }

            // Complete prematurely if the setup implies an impossible configuration.
            this.Completed = action.Action == GuildWarsAction.None;
        }

        private void OnActivated(object o, EventArgs e) {
            OnActivated();
        }

        protected virtual void OnActivated() {
            /* NOOP */
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Filter;
        }

        protected override void DisposeControl() {
            if (_keyBinding != null) {
                _keyBinding.Activated -= OnActivated;
            }
            _arrowTween?.CancelAndComplete();
            base.DisposeControl();
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (this.Completed) {
                return;
            }

            this.Bounds = new Rectangle(this.Width / 2 + _bounds.X - _bounds.Width / 2, this.Height - _bounds.Y - _bounds.Height, _bounds.Width, _bounds.Height);

            var arrowDest = new Rectangle(this.Bounds.X, this.Bounds.Y - this.Bounds.Height - _arrowHeight, this.Bounds.Width, this.Bounds.Height);

            int textOffsetY = 0;

            if (string.IsNullOrEmpty(_text)) {
                spriteBatch.DrawOnCtrl(this, _frame, this.Bounds, Color.Red);
            } else {
                var textSize = _font.MeasureString(_text);
                textOffsetY = (int)textSize.Height + _font.LineHeight;
                spriteBatch.DrawStringOnCtrl(this, _text, _font, new Rectangle(arrowDest.X + (arrowDest.Width - (int)textSize.Width) / 2, arrowDest.Y - arrowDest.Height, (int)textSize.Width, arrowDest.Height), Color.White, false, true, 1, HorizontalAlignment.Center);
            }

            if (!string.IsNullOrEmpty(_action.Message)) {
                var textWidth = (int)_font.MeasureString(_text).Width;
                spriteBatch.DrawStringOnCtrl(this, _action.Message, _font, new Rectangle(arrowDest.X + (arrowDest.Width - textWidth) / 2, arrowDest.Y - arrowDest.Height - textOffsetY, textWidth, arrowDest.Height), Color.White, false, true, 1, HorizontalAlignment.Center);
            }

            spriteBatch.DrawOnCtrl(this, _arrow, arrowDest, Color.Red);
        }
    }
}
