using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace Nekres.RotationTrainer.Core.UI.Controls {
    internal class StoreButton : Image {

        private static AsyncTexture2D _storeIcon;
        private static AsyncTexture2D _storeIconHover;

        public StoreButton(ContentsManager content) {
            _storeIcon                ??= content.GetTexture("2208348.png");
            _storeIconHover           ??= content.GetTexture("2208351.png");
            this.Texture              =   _storeIcon;
            _storeIcon.TextureSwapped +=  OnTextureLoaded;
        }

        private void OnTextureLoaded(object o, ValueChangedEventArgs<Texture2D> e) {
            _storeIcon.TextureSwapped -= OnTextureLoaded;
            this.Texture              =  _storeIcon;
        }

        protected override void OnMouseEntered(MouseEventArgs e) {
            this.Texture = _storeIconHover;
            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e) {
            this.Texture = _storeIcon;
            base.OnMouseLeft(e);
        }
    }
}
