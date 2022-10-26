using System;
using System.Collections.Generic;
using System.Diagnostics;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Nekres.RotationTrainer.Player.Models;

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

        private Rectangle   _bounds;
        private string      _text;

        private int         _arrowHeight;
        private Glide.Tween _arrowTween;

        private int       _repetitions;
        private Stopwatch _timer;

        private Ability _ability;

        public bool Completed { get; private set; }

        private AbilityHint(Ability ability) {
            _ability = ability;
            this.Parent = GameService.Graphics.SpriteScreen;
            this.Size   = GameService.Graphics.SpriteScreen.Size;

            this.Completed = ability.Action == GuildWarsAction.None;

            _arrowTween  = GameService.Animation.Tweener.Tween(this, new {_arrowHeight = _arrowHeight + 10}, 0.7f).Repeat();

            _abilityBounds.TryGetValue(ability.Action, out _bounds);
            _abilityText.TryGetValue(ability.Action, out _text);

            _timer = new Stopwatch();
            _timer.Start();
            _repetitions = ability.Repetitions > 0 ? ability.Repetitions : 1;
            ability.Activated += OnActivated;
        }

        private void OnActivated(object o, EventArgs e) {
            _repetitions--;
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.None;
        }

        protected override void DisposeControl() {
            _ability.Activated -= OnActivated;
            _timer?.Stop();
            _arrowTween?.CancelAndComplete();
            base.DisposeControl();
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (_timer == null || this.Completed) {
                return;
            }

            var frameDest = new Rectangle(this.Width / 2 + _bounds.X - _bounds.Width / 2, this.Height - _bounds.Y   - _bounds.Height, _bounds.Width, _bounds.Height);
            var arrowDest = new Rectangle(frameDest.X,                                              frameDest.Y - frameDest.Height - _arrowHeight,        frameDest.Width,    frameDest.Height);
            
            if (string.IsNullOrEmpty(_text)) {
                spriteBatch.DrawOnCtrl(this, _frame, frameDest, Color.Red);
            } else {
                var textWidth = (int)_font.MeasureString(_text).Width;
                spriteBatch.DrawStringOnCtrl(this, _text, _font, new Rectangle(arrowDest.X + (arrowDest.Width - textWidth) / 2, arrowDest.Y - arrowDest.Height, textWidth, arrowDest.Height), Color.White, false, true);
            }

            spriteBatch.DrawOnCtrl(this, _arrow, arrowDest, Color.Red);

            this.Completed = _timer.Elapsed.TotalMilliseconds >= _ability.Duration && _repetitions <= 0;

            if (_ability.Duration > 0)
            {
                var duration = $"{(_ability.Duration - _timer.Elapsed.TotalMilliseconds) / 1000:0.00}".Replace(',', '.');
                var durWidth = (int)_font.MeasureString(duration).Width;
                spriteBatch.DrawStringOnCtrl(this, duration, _font, new Rectangle(frameDest.X + (frameDest.Width - durWidth) / 2, frameDest.Y, durWidth, frameDest.Height), Color.White, false, true);
            }

            if (_ability.Repetitions > 0) {
                var repetition = _repetitions.ToString();
                var repWidth   = (int)_font.MeasureString(repetition).Width;
                spriteBatch.DrawStringOnCtrl(this, repetition, _font, new Rectangle(frameDest.X + (frameDest.Width - repWidth) / 2, frameDest.Y, repWidth, frameDest.Height), Color.White, false, true);
            }
        }

        public static AbilityHint ShowHint(Ability ability) {
            return new AbilityHint(ability);
        }

    }
}
