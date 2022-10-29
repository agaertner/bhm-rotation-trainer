using System;
using System.Collections.ObjectModel;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Gw2Sharp.ChatLinks;
using Gw2Sharp.Models;
using Microsoft.Xna.Framework;
using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Core.UI.Models;
using Nekres.RotationTrainer.Player.Models;

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
                Text                = professionText
            };

            templateButton.Click += (_, _) => {
                BuildChatLinkInputPrompt.ShowPrompt((confirmed, link) => {
                    if (!confirmed) {
                        return;
                    }
                    this.Presenter.Model.BuildTemplate = $"{link.Profession} Build"; ;
                }, "Enter a Build Chat Link:", this.Presenter.Model.BuildTemplate);
            };

            var rotationPanel = new FlowPanel {
                Parent         = buildPanel,
                Size           = new Point(buildPanel.ContentRegion.Width, buildPanel.ContentRegion.Height - 150),
                Location       = new Point(0,                              professionLabel.Bottom          + MARGIN_BOTTOM),
                ControlPadding = new Vector2(5, 5),
                ShowBorder     = false,
                CanScroll      = true,
                ShowTint       = false
            };

            if (Rotation.TryParse(this.Presenter.Model.Rotation, out var rotation)) {

                int i = 1;
                foreach (var ability in rotation) {
                    var abilityDetails = new ViewContainer {
                        Parent = rotationPanel,
                        Width = rotationPanel.Width - 13,
                        Height = 35,
                        ShowTint = true
                    };

                    abilityDetails.Show(new AbilityDetailsView(ability, i++));
                }
                rotation.Changed += OnRotationChanged;
            }

            var utilRemapper = new UtilityRemapper(this.Presenter.Model.UtilityOrder.ToArray()) {
                Parent   = buildPanel,
                Location = new Point(0, rotationPanel.Bottom + MARGIN_BOTTOM)
            };
            utilRemapper.Reordered += OnUtilitiesReordered;

            // Delete button
            var delBtn = new DeleteButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent           = buildPanel,
                Size             = new Point(42, 42),
                Location         = new Point(buildPanel.ContentRegion.Width - 42, rotationPanel.Bottom + MARGIN_BOTTOM),
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

        private void DeleteButton_Click(object o, MouseEventArgs e) {
            _deleted = true;
            this.Presenter.Delete();
            ((DeleteButton)o).Parent.Hide();
        }

        private void OnUtilitiesReordered(object o, ValueEventArgs<int[]> e) {
            this.Presenter.Model.UtilityOrder = e.Value;
        }

        private void OnRotationChanged(object o, EventArgs e) {
            var rot = ((Rotation)o).ToString();
            if (!Rotation.TryParse(rot, out _)) {
                ScreenNotification.ShowNotification("Something went wrong.", ScreenNotification.NotificationType.Error);
                RotationTrainerModule.Logger.Debug($"Unable to deserialize rotation: {rot}");
                return;
            }
            this.Presenter.Model.Rotation = rot;
        }

        protected override void Unload() {
            if (!_deleted) {
                RotationTrainerModule.Instance.DataService.Upsert(this.Presenter.Model);
            }
            base.Unload();
        }
    }
}
