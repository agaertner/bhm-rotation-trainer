using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using Nekres.RotationTrainer.Core.Player.Models;
using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Core.UI.Models;
using Nekres.RotationTrainer.Player.Models;
using Action = Nekres.RotationTrainer.Core.Player.Models.Action;

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
                Width      = 80,
                Height     = 32,
                Font       = GameService.Content.DefaultFont18,
                Left       = 0,
                Top        = 0
            };

            var addToOpenerButton = new AddButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent = buildPanel,
                Width = 32,
                Height = 32,
                Bottom = openerLabel.Bottom,
                Left = openerLabel.Right,
                BasicTooltipText = "Add a new action."
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

            addToOpenerButton.Click += (_, _) => {
                this.AddAbility(_model.Opener, openerPanel, _model.Opener.Count());
            };

            var loopLabel = new Label {
                Parent     = buildPanel,
                Text       = "Loop",
                StrokeText = true,
                Width      = 80,
                Height     = 32,
                Font       = GameService.Content.DefaultFont18,
                Left       = 0,
                Top        = openerPanel.Bottom + 5
            };

            var addToLoopButton = new AddButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent           = buildPanel,
                Width            = 32,
                Height           = 32,
                Bottom           = loopLabel.Bottom,
                Left             = loopLabel.Right,
                BasicTooltipText = "Add a new action."
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

            addToLoopButton.Click += (_, _) => {
                this.AddAbility(_model.Loop, loopPanel, _model.Loop.Count());
            };

            _cancelToken = new CancellationTokenSource();
            await Task.Run(() => {
                this.AddAbilities(_model.Opener, openerPanel);
                this.AddAbilities(_model.Loop,   loopPanel);
            }, _cancelToken.Token);

            base.Build(buildPanel);
        }

        protected override void Unload() {
            _cancelToken?.Cancel();
            _cancelToken?.Dispose();
            base.Unload();
        }

        private void AddAbilities(Rotation rotation, FlowPanel panel) {
            for (int i = 0; !_cancelToken.IsCancellationRequested && i < rotation.Count(); i++) {
                this.CreateAbilityView(rotation, rotation[i], panel, i);
            }
        }

        private void CreateAbilityView(Rotation rotation, Action action, FlowPanel panel, int index) {
            var abilityDetails = new ViewContainer {
                Parent = panel,
                Width    = panel.Width - 13,
                Height   = 35,
                ShowTint = true
            };

            var abilityDetailsView = new AbilityDetailsView(action, index);
            abilityDetailsView.Remove += (_, e) => {
                panel.Children.Remove(abilityDetails);
                rotation.RemoveAt(e.Value);
                this.Invalidate(panel, rotation);
            };

            abilityDetailsView.Add += (_, e) => {
                this.AddAbility(rotation, panel, e.Value);
            };
            abilityDetails.Show(abilityDetailsView);
            panel.Children.Remove(abilityDetails);
            panel.Children.Insert(index, abilityDetails);
            panel.Invalidate();
        }

        private void AddAbility(Rotation rotation, FlowPanel panel, int index) {
            var newAbility = new Action(GuildWarsAction.None);
            this.CreateAbilityView(rotation, newAbility, panel, index);
            rotation.Insert(index, newAbility);
            this.Invalidate(panel, rotation);
        }

        private void Invalidate(FlowPanel panel, Rotation rotation) {
            for (int i = 0; !_cancelToken.IsCancellationRequested && i < rotation.Count(); i++) {
                var ctrl = (AbilityDetailsView)((ViewContainer)panel.Children[i]).CurrentView;
                ctrl.Index = i;
            }
            panel.Invalidate();
        }
    }
}
