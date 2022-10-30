using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Linq;
using MouseEventArgs = Blish_HUD.Input.MouseEventArgs;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class ConfigView : View<ConfigPresenter> {

        public event EventHandler<EventArgs> DeleteClick;

        public ConfigView(TemplateModel model) {
            this.WithPresenter(new ConfigPresenter(this, model));
        }

        protected override void Build(Container buildPanel) {

            var flowPanel = new FlowPanel {
                Parent         = buildPanel,
                Width          = buildPanel.ContentRegion.Width,
                Height         = buildPanel.ContentRegion.Height,
                Left           = 0,
                Top            = 0,
                ControlPadding = new Vector2(5, 5)
            };

            var primaryWeaponSetContainer = new ViewContainer {
                Parent = flowPanel,
                Width  = flowPanel.ContentRegion.Width,
                Height = 100,
            };
            primaryWeaponSetContainer.Show(new WeaponSetView(this.Presenter.Model.PrimaryWeaponSet, true));

            var secondaryWeaponSetContainer = new ViewContainer {
                Parent = flowPanel,
                Width  = flowPanel.ContentRegion.Width,
                Height = 100,
            };
            secondaryWeaponSetContainer.Show(new WeaponSetView(this.Presenter.Model.SecondaryWeaponSet, false));

            var utilRemapper = new UtilityRemapper(this.Presenter.Model.UtilityOrder.ToArray()) {
                Parent   = flowPanel,
            };
            utilRemapper.Reordered += OnUtilitiesReordered;

            // Delete button
            var delBtn = new DeleteButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent           = flowPanel,
                Size             = new Point(42, 42),
                BasicTooltipText = "Delete"
            };
            delBtn.Click += DeleteButton_Click;

            base.Build(buildPanel);
        }

        private void DeleteButton_Click(object o, MouseEventArgs e) {
            DeleteClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnUtilitiesReordered(object o, ValueEventArgs<int[]> e) {
            this.Presenter.Model.UtilityOrder = e.Value;
        }
    }
}
