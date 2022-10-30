using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace Nekres.RotationTrainer.Core.UI.Controls
{
    internal class RemoveButton : Image
    {
        private static Texture2D _deleteIcon;
        private static Texture2D _deleteIconHover;
        private static Texture2D _deleteIconDisabled;
        public RemoveButton(ContentsManager content)
        {
            _deleteIcon      ??= content.GetTexture("2175782.png");
            _deleteIconHover ??= content.GetTexture("2175784.png");
            _deleteIconDisabled ??= content.GetTexture("2175783.png");
            this.Texture     =   _deleteIcon;
        }

        protected override void OnMouseEntered(MouseEventArgs e)
        {
            this.Texture = _deleteIconHover;
            base.OnMouseEntered(e);
        }

        protected override void OnMouseLeft(MouseEventArgs e)
        {
            this.Texture = _deleteIcon;
            base.OnMouseLeft(e);
        }
    }
}
