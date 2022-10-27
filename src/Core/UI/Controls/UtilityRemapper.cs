using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Blish_HUD;

namespace Nekres.RotationTrainer.Core.UI.Controls {
    internal class UtilityRemapper : Control {

        public event EventHandler<ValueEventArgs<int[]>> Reordered;

        private Texture2D _utilitySprite = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");

        private bool      _mouseOverUtility1;
        private Rectangle _utility1Bounds;
        private bool      _mouseOverUtility2;
        private Rectangle _utility2Bounds;
        private bool      _mouseOverUtility3;
        private Rectangle _utility3Bounds;

        private int MARGIN = 3;

        private int[] _utilityOrder;

        public UtilityRemapper(int[] utilityOrder) {
            _utilityOrder = utilityOrder ?? new []{0,1,2};
            this.Size     = new Point(_utilitySprite.Width * 3 + MARGIN * 2, _utilitySprite.Height);
        }

        protected override void DisposeControl() {
            _utilitySprite?.Dispose();
            base.DisposeControl();
        }

        protected override void OnClick(MouseEventArgs e) {
            if (_mouseOverUtility1 || _mouseOverUtility2 || _mouseOverUtility3) {
                this.UpdateUtilityKeys();
                GameService.Content.PlaySoundEffectByName("button-click");
            }
            base.OnClick(e);
        }

        protected override void OnMouseMoved(MouseEventArgs e) {
            var relPos = RelativeMousePosition;
            _mouseOverUtility3 = _utility3Bounds.Contains(relPos);
            _mouseOverUtility2 = _utility2Bounds.Contains(relPos);
            _mouseOverUtility1 = _utility1Bounds.Contains(relPos);

            if (_mouseOverUtility1) {
                this.BasicTooltipText = "Reorder Utility Key 1";
            } else if (_mouseOverUtility2) {
                this.BasicTooltipText = "Reorder Utility Key 2";
            } else if (_mouseOverUtility3) {
                this.BasicTooltipText = "Reorder Utility Key 3";
            } else {
                this.BasicTooltipText = string.Empty;
            }

            base.OnMouseMoved(e);
        }

        private void UpdateUtilityKeys() {
            int index = _mouseOverUtility1 ? 0 : _mouseOverUtility2 ? 1 : 2;
            int swap  = _utilityOrder[index] == 2 ? 0 : _utilityOrder[index] + 1;

            if (Array.Exists(_utilityOrder, e => e == swap)) {
                _utilityOrder[Array.FindIndex(_utilityOrder, e => e == swap)] = _utilityOrder[index];
            }

            _utilityOrder[index] = swap;

            Reordered?.Invoke(this, new ValueEventArgs<int[]>(_utilityOrder));
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            _utility1Bounds = new Rectangle(0, 0, _utilitySprite.Width, _utilitySprite.Height);
            spriteBatch.DrawOnCtrl(this, _utilitySprite, _utility1Bounds, Color.White);
            spriteBatch.DrawStringOnCtrl(this, (_utilityOrder[0] + 1).ToString(), Content.DefaultFont14, _utility1Bounds, Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Bottom);

            _utility2Bounds = new Rectangle(_utility1Bounds.Right + MARGIN, 0, _utilitySprite.Width, _utilitySprite.Height);
            spriteBatch.DrawOnCtrl(this, _utilitySprite, _utility2Bounds, Color.White);
            spriteBatch.DrawStringOnCtrl(this, (_utilityOrder[1] + 1).ToString(), Content.DefaultFont14, _utility2Bounds, Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Bottom);

            _utility3Bounds = new Rectangle(_utility2Bounds.Right + MARGIN, 0, _utilitySprite.Width, _utilitySprite.Height);
            spriteBatch.DrawOnCtrl(this, _utilitySprite, _utility3Bounds, Color.White);
            spriteBatch.DrawStringOnCtrl(this, (_utilityOrder[2] + 1).ToString(), Content.DefaultFont14, _utility3Bounds, Color.White, false, true, 2, HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }

    }
}
