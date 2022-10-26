using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Core.UI.Models;

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

            var editTemplate = new TextBox {
                Parent              = buildPanel,
                Size                = new Point(buildPanel.ContentRegion.Width, 42),
                Location            = new Point(0,                              editTitle.Bottom + MARGIN_BOTTOM),
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size24, ContentService.FontStyle.Regular),
                HorizontalAlignment = HorizontalAlignment.Center,
                Text                = this.Presenter.Model.Template
            };
            editTemplate.InputFocusChanged += EditTemplate_InputFocusChanged;

            var rotationInput = new MultilineTextBox {
                Parent         = buildPanel,
                Size           = new Point(buildPanel.ContentRegion.Width, buildPanel.ContentRegion.Height / 2 - 60),
                Location       = new Point(0,                              editTemplate.Bottom + MARGIN_BOTTOM),
                Text = this.Presenter.Model.Rotation
            };
            rotationInput.InputFocusChanged += EditText_InputFocusChanged;

            // Delete button
            var delBtn = new DeleteButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent           = buildPanel,
                Size             = new Point(42,                                  42),
                Location         = new Point(buildPanel.ContentRegion.Width - 42, rotationInput.Bottom + MARGIN_BOTTOM),
                BasicTooltipText = "Delete"
            };
            delBtn.Click += DeleteButton_Click;
        }
        private void EditTitle_InputFocusChanged(object o, EventArgs e) {
            var ctrl = (TextBox)o;
            if (ctrl.Focused) {
                return;
            }
            this.Presenter.Model.Title          = ctrl.Text;
            ((StandardWindow)ctrl.Parent).Title = $"Edit Template - {ctrl.Text}";
        }

        private void EditTemplate_InputFocusChanged(object o, EventArgs e) {
            var ctrl = (TextBox)o;
            if (ctrl.Focused) {
                return;
            }
            this.Presenter.Model.Template = ctrl.Text;
        }

        private void EditText_InputFocusChanged(object o, EventArgs e) {
            var ctrl = (MultilineTextBox)o;
            if (ctrl.Focused) {
                return;
            }
            this.Presenter.Model.Rotation = ctrl.Text;
        }

        private void DeleteButton_Click(object o, MouseEventArgs e) {
            _deleted = true;
            this.Presenter.Delete();
            ((DeleteButton)o).Parent.Hide();
        }

        protected override void Unload() {
            if (!_deleted) {
                RotationTrainerModule.Instance.DataService.Upsert(this.Presenter.Model);
            }
            base.Unload();
        }
    }
}
