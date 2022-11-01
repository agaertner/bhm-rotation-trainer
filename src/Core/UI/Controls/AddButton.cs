using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace Nekres.RotationTrainer.Core.UI.Controls
{
    internal class AddButton : Image
    {
        private static Texture2D _icon;
        private static Texture2D _iconHover;
        private static Texture2D _iconClick;

        private bool _hovering;
        public AddButton(ContentsManager content)
        {
            _icon      ??= content.GetTexture("155902.png");
            _iconHover ??= content.GetTexture("155904.png");
            _iconClick ??= content.GetTexture("155903.png");
            this.Texture = _icon;
        }

        protected override void OnClick(MouseEventArgs e) {
            GameService.Content.PlaySoundEffectByName("button-click");
            base.OnClick(e);
        }

        protected override void OnMouseEntered(MouseEventArgs e) {
            _hovering = true;
            this.Texture = _iconHover;
            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e) {
            _hovering = false;
            this.Texture = _icon;
            base.OnMouseLeft(e);
        }

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e) {
            this.Texture = _iconClick;
            base.OnLeftMouseButtonPressed(e);
        }

        protected override void OnRightMouseButtonPressed(MouseEventArgs e) {
            this.Texture = _iconClick;
            base.OnRightMouseButtonPressed(e);
        }

        protected override void OnLeftMouseButtonReleased(MouseEventArgs e) {
            this.Texture = _hovering ? _iconHover : _icon;
            base.OnLeftMouseButtonReleased(e);
        }

        protected override void OnRightMouseButtonReleased(MouseEventArgs e) {
            this.Texture = _hovering ? _iconHover : _icon;
            base.OnRightMouseButtonReleased(e);
        }
    }
}
