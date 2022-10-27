using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using Nekres.RotationTrainer.Player.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nekres.RotationTrainer.Core.UI.Controls {
    internal class AbilityHint : Control {
        private static Dictionary<GuildWarsAction, Rectangle> _abilityBounds = new()
        {
            { GuildWarsAction.SwapWeapons, new Rectangle(-383,      38,  43, 43) },
            { GuildWarsAction.WeaponSkill1, new Rectangle(-328,     24,  58, 58) },
            { GuildWarsAction.WeaponSkill2, new Rectangle(-267,     24,  58, 58) },
            { GuildWarsAction.WeaponSkill3, new Rectangle(-206,     24,  58, 58) },
            { GuildWarsAction.WeaponSkill4, new Rectangle(-145,     24,  58, 58) },
            { GuildWarsAction.WeaponSkill5, new Rectangle(-84,      24,  58, 58) },
            { GuildWarsAction.HealingSkill, new Rectangle(87,       24,  58, 58) },
            { GuildWarsAction.UtilitySkill1, new Rectangle(148,     24,  58, 58) },
            { GuildWarsAction.UtilitySkill2, new Rectangle(209,     24,  58, 58) },
            { GuildWarsAction.UtilitySkill3, new Rectangle(270,     24,  58, 58) },
            { GuildWarsAction.EliteSkill, new Rectangle(332,        24,  58, 58) },
            { GuildWarsAction.ProfessionSkill1, new Rectangle(-350, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill2, new Rectangle(-330, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill3, new Rectangle(-310, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill4, new Rectangle(-290, 150, 48, 48) },
            { GuildWarsAction.ProfessionSkill5, new Rectangle(-270, 150, 48, 48) },
            { GuildWarsAction.SpecialAction, new Rectangle(-85,     157, 54, 54) },
            { GuildWarsAction.Dodge, new Rectangle(0,               100, 54, 54) },
            { GuildWarsAction.Interact, new Rectangle(664,          420, 54, 54) }
        };
        private static Dictionary<GuildWarsAction, string> _abilityText = new() {
            {GuildWarsAction.ProfessionSkill1, "F1"},
            {GuildWarsAction.ProfessionSkill2, "F2"},
            {GuildWarsAction.ProfessionSkill3, "F3"},
            {GuildWarsAction.ProfessionSkill4, "F4"},
            {GuildWarsAction.ProfessionSkill5, "F5"},
            {GuildWarsAction.Dodge,            "Dodge" },
            {GuildWarsAction.Interact,         "Interact" },
        };
        private static Texture2D  _frame = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");
        private static Texture2D  _arrow = RotationTrainerModule.Instance.ContentsManager.GetTexture("991944.png");
        private static BitmapFont _font  = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size36, ContentService.FontStyle.Regular);
        private static BitmapFont _smallFont  = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size22, ContentService.FontStyle.Regular);

        private        Rectangle  _bounds;
        private        string     _text;

        private int         _arrowHeight;
        private Glide.Tween _arrowTween;

        private int       _remReqActivations;
        private Stopwatch _timer;

        private Ability _ability;

        private KeyBinding _keyBinding;

        public bool Completed { get; private set; }

        private AbilityHint(Ability ability) {
            _ability = ability;
            this.Parent = GameService.Graphics.SpriteScreen;
            this.Size   = GameService.Graphics.SpriteScreen.Size;

            _arrowTween  = GameService.Animation.Tweener.Tween(this, new {_arrowHeight = _arrowHeight + 10}, 0.7f).Repeat();

            _abilityBounds.TryGetValue(ability.Action, out _bounds);
            _abilityText.TryGetValue(ability.Action, out _text);

            _timer = new Stopwatch();
            _timer.Start();
            // Single-Usage Ability: If no duration and no repetitions ensure at least one required activation.
            _remReqActivations = ability.Repetitions > 0 ? ability.Repetitions : Convert.ToInt32(ability.Duration <= 0);

            // Find key binding.
            if (RotationTrainerModule.Instance.ActionBindings.TryGetValue(_ability.Action, out SettingEntry<KeyBinding> keyBindingSetting))
            {
                _keyBinding = keyBindingSetting.Value;

                if (_keyBinding.PrimaryKey == Keys.None && _keyBinding.ModifierKeys == ModifierKeys.None) {
                    ScreenNotification.ShowNotification($"You need to assign a key to {_ability.Action.ToFriendlyString()}.");
                }
                
                _keyBinding.Activated += OnActivated;
            }

            // Complete prematurely if the setup implies an impossible configuration.
            this.Completed = ability.Action == GuildWarsAction.None || _remReqActivations > 0 && _keyBinding == null;
        }

        private void OnActivated(object o, EventArgs e) {
            _remReqActivations--;
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.None;
        }

        protected override void DisposeControl() {
            _keyBinding.Activated -= OnActivated;
            _timer?.Stop();
            _arrowTween?.CancelAndComplete();
            base.DisposeControl();
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (_timer == null || this.Completed) {
                return;
            }

            this.Completed = _timer.Elapsed.TotalMilliseconds >= _ability.Duration && _remReqActivations <= 0;

            var frameDest = new Rectangle(this.Width / 2 + _bounds.X - _bounds.Width / 2, this.Height - _bounds.Y   - _bounds.Height, _bounds.Width, _bounds.Height);
            var arrowDest = new Rectangle(frameDest.X,                                              frameDest.Y - frameDest.Height - _arrowHeight,        frameDest.Width,    frameDest.Height);
            
            if (string.IsNullOrEmpty(_text)) {
                spriteBatch.DrawOnCtrl(this, _frame, frameDest, Color.Red);
            } else {
                var textWidth = (int)_font.MeasureString(_text).Width;
                spriteBatch.DrawStringOnCtrl(this, _text, _font, new Rectangle(arrowDest.X + (arrowDest.Width - textWidth) / 2, arrowDest.Y - arrowDest.Height, textWidth, arrowDest.Height), Color.White, false, true);
            }

            spriteBatch.DrawOnCtrl(this, _arrow, arrowDest, Color.Red);

            if (_ability.Duration > 0 && _timer.Elapsed.TotalMilliseconds < _ability.Duration) {
                var duration = $"{(_ability.Duration - _timer.Elapsed.TotalMilliseconds) / 1000:0.00}".Replace(',', '.');
                var durWidth = (int)_smallFont.MeasureString(duration).Width;
                spriteBatch.DrawStringOnCtrl(this, duration, _smallFont, new Rectangle(frameDest.X + (frameDest.Width - durWidth) / 2, frameDest.Y, durWidth, frameDest.Height), Color.White, false, true, 1, HorizontalAlignment.Center, VerticalAlignment.Bottom);
            }

            if (_ability.Repetitions > 0 && _remReqActivations > 0) {
                var repetition = _remReqActivations.ToString();
                var repWidth   = (int)_smallFont.MeasureString(repetition).Width;
                spriteBatch.DrawStringOnCtrl(this, repetition, _smallFont, new Rectangle(frameDest.X + (frameDest.Width - repWidth) / 2, frameDest.Y, repWidth, frameDest.Height), Color.White, false, true, 1, HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }

        public static AbilityHint ShowHint(Ability ability) {
            return new AbilityHint(ability);
        }

    }
}
