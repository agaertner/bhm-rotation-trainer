using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using Nekres.RotationTrainer.Core.UI.Models;
using Nekres.RotationTrainer.Player.Models;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class RotationView : View {

        private const int MARGIN_BOTTOM = 10;

        private TemplateModel.BuildRotation _model;
        private CancellationTokenSource     _cancelToken;
        public RotationView(TemplateModel.BuildRotation model) {
            _model = model;
        }

        protected override async void Build(Container buildPanel) {
            var openerLabel = new Label {
                Parent     = buildPanel,
                Text       = "Opener",
                StrokeText = true,
                Width      = buildPanel.ContentRegion.Width,
                Height     = 32,
                Font       = GameService.Content.DefaultFont18,
                Left       = 0,
                Top        = 0
            };

            var openerPanel = new FlowPanel {
                Parent = buildPanel,
                Size = new Point(buildPanel.ContentRegion.Width, 200),
                Location = new Point(0, openerLabel.Bottom + MARGIN_BOTTOM),
                ControlPadding = new Vector2(5, 5),
                ShowBorder = false,
                CanScroll = true,
                ShowTint = false
            };

            var loopLabel = new Label {
                Parent     = buildPanel,
                Text       = "Loop",
                StrokeText = true,
                Width      = buildPanel.ContentRegion.Width,
                Height     = 32,
                Font       = GameService.Content.DefaultFont18,
                Left       = 0,
                Top        = openerPanel.Bottom + 5
            };

            var loopPanel = new FlowPanel {
                Parent = buildPanel,
                Size = new Point(buildPanel.ContentRegion.Width, 200),
                Location = new Point(0, loopLabel.Bottom + MARGIN_BOTTOM),
                ControlPadding = new Vector2(5, 5),
                ShowBorder = false,
                CanScroll = true,
                ShowTint = false
            };

            _cancelToken = new CancellationTokenSource();
            await Task.Run(() => {
                this.AddAbilities(_model.Opener, openerPanel, OnRotationOpenerChanged);
                this.AddAbilities(_model.Loop,   loopPanel,   OnRotationLoopChanged);
            }, _cancelToken.Token);

            base.Build(buildPanel);
        }

        protected override void Unload() {
            _cancelToken?.Cancel();
            _cancelToken?.Dispose();
            base.Unload();
        }

        private void OnRotationOpenerChanged(object o, EventArgs e) {
            var rot = ((Rotation)o).ToString();
            if (!Rotation.TryParse(rot, out _)) {
                ScreenNotification.ShowNotification("Something went wrong.", ScreenNotification.NotificationType.Error);
                RotationTrainerModule.Logger.Debug($"Unable to deserialize rotation: {rot}");
                return;
            }
            _model.Opener = rot;
        }

        private void OnRotationLoopChanged(object o, EventArgs e) {
            var rot = ((Rotation)o).ToString();
            if (!Rotation.TryParse(rot, out _)) {
                ScreenNotification.ShowNotification("Something went wrong.", ScreenNotification.NotificationType.Error);
                RotationTrainerModule.Logger.Debug($"Unable to deserialize rotation: {rot}");
                return;
            }
            _model.Loop = rot;
        }

        private void AddAbilities(string rotation, FlowPanel panel, EventHandler<EventArgs> callback) {
            if (!Rotation.TryParse(rotation, out var opener)) {
                return;
            }
            
            int number = 1;
            foreach (var ability in opener) {
                if (_cancelToken.IsCancellationRequested) {
                    return;
                }

                var abilityDetails = new ViewContainer {
                    Parent   = panel,
                    Width    = panel.Width - 13,
                    Height   = 35,
                    ShowTint = true
                };

                var abilityDetailsView = new AbilityDetailsView(ability, number++);
                abilityDetailsView.Remove += (o, e) => {
                    var index = e.Value - 1;
                    panel.Children.RemoveAt(index);
                    panel.Invalidate();
                    for (int i = index; !_cancelToken.IsCancellationRequested && i < panel.Count(); i++) {
                        var ctrl = (AbilityDetailsView)((ViewContainer)panel.Children[i]).CurrentView;
                        ctrl.Number = i + 1;
                    }
                };
                abilityDetails.Show(abilityDetailsView);
            }
            opener.Changed += callback;
        }
    }
}
