using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace Nekres.RotationTrainer.Core.UI.Controls {
    internal class StoreButton : Image {

        private static AsyncTexture2D _icon;
        private static AsyncTexture2D _iconHover;
        private static AsyncTexture2D _iconClick;

        private bool _hovering;

        public StoreButton(ContentsManager content) {
            _icon                ??= content.GetTexture("2208348.png");
            _iconHover           ??= content.GetTexture("2208351.png");
            _iconClick           ??= content.GetTexture("2208349.png");
            this.Texture         =   _icon;
            _icon.TextureSwapped +=  OnTextureLoaded;
        }

        private void OnTextureLoaded(object o, ValueChangedEventArgs<Texture2D> e) {
            _icon.TextureSwapped -= OnTextureLoaded;
            this.Texture              =  _icon;
        }

        protected override void OnMouseEntered(MouseEventArgs e) {
            _hovering    = true;
            this.Texture = _iconHover;
            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e) {
            _hovering    = false;
            this.Texture = _icon;
            base.OnMouseLeft(e);
        }

        protected override void OnClick(MouseEventArgs e) {
            GameService.Content.PlaySoundEffectByName("button-click");
            base.OnClick(e);
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
