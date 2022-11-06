using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Action = Nekres.RotationTrainer.Core.Player.Models.Action;
namespace Nekres.RotationTrainer.Core.Player.Controls {
    internal class ActionHint : ActionHintBase<Action> {

        private int         _remReqActivations;
        private Stopwatch   _timer;

        public bool Completed { get; private set; }

        private ActionHint(Action action) : base(action) {
            _timer = new Stopwatch();
            _timer.Start();
            // Single-Usage Ability: If no duration and no repetitions ensure at least one required activation.
            _remReqActivations = action.Repetitions > 0 ? action.Repetitions : Convert.ToInt32(action.Duration <= 0);
        }

        protected override void OnActivated() {
            _remReqActivations--;
            base.OnActivated();
        }

        protected override void DisposeControl() {
            _timer?.Stop();
            base.DisposeControl();
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {

            base.Paint(spriteBatch, bounds);

            if (_timer == null || this.Completed) {
                return;
            }

            this.Completed = _timer.Elapsed.TotalMilliseconds >= _action.Duration && _remReqActivations <= 0;

            if (_action.Duration > 0 && _timer.Elapsed.TotalMilliseconds < _action.Duration) {
                var duration = $"{(_action.Duration - _timer.Elapsed.TotalMilliseconds) / 1000:0.00}".Replace(',', '.');
                var durWidth = (int)_smallFont.MeasureString(duration).Width;
                spriteBatch.DrawStringOnCtrl(this, duration, _smallFont, new Rectangle(this.Bounds.X + (this.Bounds.Width - durWidth) / 2, this.Bounds.Y, durWidth, this.Bounds.Height), Color.White, false, true, 1, HorizontalAlignment.Center, VerticalAlignment.Bottom);
            }

            if (_action.Repetitions > 0 && _remReqActivations > 0) {
                var repetition = _remReqActivations.ToString();
                var repWidth   = (int)_smallFont.MeasureString(repetition).Width;
                spriteBatch.DrawStringOnCtrl(this, repetition, _smallFont, new Rectangle(this.Bounds.X + (this.Bounds.Width - repWidth) / 2, this.Bounds.Y, repWidth, this.Bounds.Height), Color.White, false, true, 1, HorizontalAlignment.Center, VerticalAlignment.Top);
            }
        }

        public static ActionHint ShowHint(Action action) {
            return new ActionHint(action);
        }

    }
}
