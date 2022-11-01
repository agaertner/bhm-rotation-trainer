using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Core.UI.Models;
using System;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class EditView : View<EditPrersenter> {

        private const int  MARGIN_BOTTOM = 10;

        private       bool _deleted;

        public EditView(TemplateModel model) {
            this.WithPresenter(new EditPrersenter(this, model));
        }

        protected override void Build(Container buildPanel) {
            var editTitle = new TextBox {
                Parent              = buildPanel,
                Size                = new Point(buildPanel.ContentRegion.Width, 42),
                Location            = new Point(0,                              0),
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular),
                HorizontalAlignment = HorizontalAlignment.Center,
                Text                = this.Presenter.Model.Title
            };
            editTitle.InputFocusChanged += EditTitle_InputFocusChanged;

            var templateButton = new StoreButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent   = buildPanel,
                Location = new Point(0, editTitle.Bottom + MARGIN_BOTTOM),
                Size     = new Point(32, 32)
            };

            string professionText = string.Empty;

            if (this.Presenter.Model.TryGetBuildChatLink(out var buildChatLink)) {
                professionText = $"{buildChatLink.Profession} Build";
            }

            var professionLabel = new Label {
                Parent              = buildPanel,
                Size                = new Point(buildPanel.ContentRegion.Width, templateButton.Height),
                Location            = new Point(templateButton.Right + 5, templateButton.Top),
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular),
                HorizontalAlignment = HorizontalAlignment.Left,
                StrokeText = true,
                ShowShadow = true,
                Text                = professionText
            };

            templateButton.Click += (_, _) => {
                BuildChatLinkInputPrompt.ShowPrompt((confirmed, link) => {
                    if (!confirmed) {
                        return;
                    }
                    this.Presenter.Model.BuildTemplate = link.ToString();
                    professionLabel.Text               = $"{link.Profession} Build";
                }, "Enter a Build Chat Link:", this.Presenter.Model.BuildTemplate);
            };

            var rotationContainer = new ViewContainer {
                Parent = buildPanel,
                Width = buildPanel.ContentRegion.Width / 2 + 190,
                Height = buildPanel.ContentRegion.Height,
                Top = professionLabel.Bottom + MARGIN_BOTTOM,
                Left = 0
            };
            rotationContainer.Show(new RotationView(this.Presenter.Model.Rotation));

            var configContainer = new ViewContainer {
                Parent = buildPanel,
                Width  = buildPanel.ContentRegion.Width / 2 - 195,
                Height = buildPanel.ContentRegion.Height,
                Left   = rotationContainer.Right + 5,
                Top    = rotationContainer.Top,
                ShowTint = true,
                ShowBorder = true
            };

            var configView = new ConfigView(this.Presenter.Model);
            configView.DeleteClick += OnDeleteClick;
            configContainer.Show(configView);
        }

        private void OnDeleteClick(object o, EventArgs e) {
            _deleted = true;
            this.Presenter.Delete();
            ((DeleteButton)o).Parent.Parent.Parent.Hide();
        }

        private void EditTitle_InputFocusChanged(object o, EventArgs e) {
            var ctrl = (TextBox)o;
            if (ctrl.Focused) {
                return;
            }
            this.Presenter.Model.Title          = ctrl.Text;
            ((StandardWindow)ctrl.Parent).Title = $"Edit Template - {ctrl.Text}";
        }

        protected override void Unload() {
            if (!_deleted) {
                RotationTrainerModule.Instance.DataService.Upsert(this.Presenter.Model);
            }
            base.Unload();
        }
    }
}
