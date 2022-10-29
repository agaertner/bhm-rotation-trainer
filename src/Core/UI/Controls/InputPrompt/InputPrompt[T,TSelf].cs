using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Nekres.RotationTrainer.Core.UI.Controls {
    internal abstract class InputPrompt<T, TSelf> : Container where TSelf : InputPrompt<T, TSelf> 
    {
        private static Texture2D _bgTexture;
        private BitmapFont _font;

        private static TSelf _singleton;

        private Rectangle _confirmButtonBounds;
        private Rectangle _cancelButtonBounds;
        private Rectangle _inputTextBoxBounds;

        private StandardButton _confirmButton;
        private StandardButton _cancelButton;
        private TextBox _inputTextBox;

        private readonly Action<bool, T> _callback;
        private readonly string _text;
        private readonly string _confirmButtonText;
        private readonly string _cancelButtonButtonText;

        private readonly string _defaultValue;

        /// <summary>
        /// Verifies that the <see cref="input"/> <see langword="string"/> can be converted to the target-type <see cref="T"/>.
        /// </summary>
        /// <remarks>
        /// Used to ensure that user-defined text in <see cref="_defaultValue"/> and <see cref="_inputTextBox"/> meets expectations.
        /// </remarks>
        /// <param name="input">String to validate.</param>
        /// <param name="result">Object of type <see cref="T"/> if <see langword="true"/>; otherwise <see langword="default"/></param>
        /// <returns><see langword="True"/> if parsing was successful; otherrwise <see langword="false"/>.</returns>
        protected abstract bool TryParse(string input, out T result);

        protected InputPrompt(Action<bool, T> callback, string text, string defaultValue, string confirmButtonText, string cancelButtonText)
        {
            _callback               =  callback;
            _text                   =  text;
            _defaultValue           =  defaultValue;
            _confirmButtonText      =  confirmButtonText;
            _cancelButtonButtonText =  cancelButtonText;

            _bgTexture ??= GameService.Content.GetTexture("tooltip");
            _font      = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular);

            this.Parent   = Graphics.SpriteScreen;
            this.Location = Point.Zero;
            this.Size     = Graphics.SpriteScreen.Size;
            
            base.ZIndex = 999;
            base.Show();

            GameService.Input.Keyboard.KeyPressed += OnKeyPressed;
        }

        public static void ShowPrompt(Action<bool, T> callback, string text, string defaultValue = "", string confirmButtonText = "Confirm", string cancelButtonText = "Cancel")
        {
            // Avoid stacking: Do not spawn another prompt ontop of an active one.
            if (_singleton != null) {
                return;
            }

            _singleton = Activator.CreateInstance(typeof(TSelf), callback, text, defaultValue, confirmButtonText, cancelButtonText) as TSelf;
        }

        private void CreateButtons()
        {
            if (_confirmButton == null)
            {
                _confirmButton = new StandardButton
                {
                    Parent   = this,
                    Text     = _confirmButtonText,
                    Size     = _confirmButtonBounds.Size,
                    Location = _confirmButtonBounds.Location,
                    Enabled  = this.TryParse(_defaultValue, out _)
                };
                _confirmButton.Click += (_, _) => this.Confirm();
            }

            if (_cancelButton == null)
            {
                _cancelButton = new StandardButton
                {
                    Parent = this,
                    Text = _cancelButtonButtonText,
                    Size = _cancelButtonBounds.Size,
                    Location = _cancelButtonBounds.Location
                };
                _cancelButton.Click += (_, _) => this.Cancel();
            }
            
        }

        private void Confirm()
        {
            GameService.Content.PlaySoundEffectByName("button-click");
            _callback(true, this.TryParse(_inputTextBox.Text, out var result) ? result : default);
            this.Dispose();
        }

        private void Cancel()
        {
            GameService.Content.PlaySoundEffectByName("button-click");
            _callback(false, default(T));
            this.Dispose();
        }

        private void OnKeyPressed(object o, KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Enter when _confirmButton.Enabled:
                    this.Confirm();
                    break;
                case Keys.Escape:
                    this.Cancel();
                    break;
                default: return;
            }
        }

        private void CreateTextInput()
        {
            if (_inputTextBox != null) {
                return;
            }
            string defaultText = this.TryParse(_defaultValue, out _) ? _defaultValue : string.Empty;
            _inputTextBox = new TextBox
            {
                Parent      = this,
                Size        = _inputTextBoxBounds.Size,
                Location    = _inputTextBoxBounds.Location,
                Font        = _font,
                Focused     = true,
                Text        = defaultText,
                CursorIndex = defaultText.Length
            };
            _inputTextBox.TextChanged += (o, _) =>
            {
                _confirmButton.Enabled = this.TryParse(((TextBox)o).Text, out var _);
            };
        }

        protected override void DisposeControl() {
            GameService.Input.Keyboard.KeyPressed -= OnKeyPressed;
            _singleton =  null;
            base.DisposeControl();
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            base.PaintBeforeChildren(spriteBatch, bounds);

            var textSize = _font.MeasureString(_text);

            // Darken background outside container
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, Color.Black * 0.8f);

            // Calculate background bounds
            var bgTextureSize = new Point((int)textSize.Width + 12, (int)textSize.Height + 125);
            var bgTexturePos = new Point((bounds.Width - bgTextureSize.X) / 2, (bounds.Height - bgTextureSize.Y) / 2);
            var bgBounds = new Rectangle(bgTexturePos, bgTextureSize);

            // Draw border
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bgBounds.X - 1, bgBounds.Y - 1, bgBounds.Width + 1, 1), Color.Black);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bgBounds.X - 1, bgBounds.Y - 1, 1, bgBounds.Height + 1), Color.Black);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bgBounds.X + bgBounds.Width, bgBounds.Y, 1, bgBounds.Height + 1), Color.Black);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(bgBounds.X, bgBounds.Y + bgBounds.Height, bgBounds.Width, 1), Color.Black);

            // Draw Background
            spriteBatch.DrawOnCtrl(this, _bgTexture, bgBounds, Color.White);

            // Draw text
            spriteBatch.DrawStringOnCtrl(this, _text, _font, new Rectangle(bgBounds.X + 6, bgBounds.Y + 5, bgBounds.Width - 11, bgBounds.Height), Color.White, true, HorizontalAlignment.Left, VerticalAlignment.Top);

            // Set button bounds
            _confirmButtonBounds = new Rectangle(bgBounds.Left + 5, bgBounds.Bottom - 50, 100, 45);
            _cancelButtonBounds = new Rectangle(_confirmButtonBounds.Right + 10, _confirmButtonBounds.Y, 100, 45);
            _inputTextBoxBounds = new Rectangle(_confirmButtonBounds.X, _confirmButtonBounds.Y - 55, bgBounds.Width - 10, 45);

            this.CreateTextInput();
            this.CreateButtons();
        }
    }
}
