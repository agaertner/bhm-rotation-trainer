using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Gw2Sharp.WebApi.V2.Models;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Linq;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class WeaponSetView : View {
        private TemplateModel.WeaponSet _model;
        private bool                    _isPrimary;
        public WeaponSetView(TemplateModel.WeaponSet model, bool isPrimaryWeaponSet) {
            _model = model;
            _isPrimary = isPrimaryWeaponSet;
        }

        protected override void Build(Container buildPanel) {

            var header = new Label {
                Parent = buildPanel,
                Text = (_isPrimary ? "Primary" : "Secondary") + " Weapon Set",
                Width = buildPanel.ContentRegion.Width,
                Height = 24,
                Left = 7,
                Top = 0,
                Font = GameService.Content.DefaultFont18,
                ShowShadow = true,
                StrokeText = true
            };

            var mainHandLabel = new Label {
                Parent              = buildPanel,
                Text                = "Main Hand",
                Width               = 100,
                Height              = 24,
                HorizontalAlignment = HorizontalAlignment.Right,
                Left                = 12,
                Top                 = header.Bottom + 5,
            };

            var mainHandSelect = new Dropdown {
                Parent       = buildPanel,
                Width        = 100,
                Height       = 24,
                Left         = mainHandLabel.Right + 5,
                Top          = mainHandLabel.Top,
                SelectedItem = _model.MainHand.ToFriendlyString()
            };

            var offHandLabel = new Label {
                Parent = buildPanel,
                Text   = "Off Hand",
                Width  = 100,
                Height = 24,
                HorizontalAlignment = HorizontalAlignment.Right,
                Left   = mainHandLabel.Left,
                Top    = mainHandLabel.Bottom + 5,
            };

            var offHandSelect = new Dropdown {
                Parent       = buildPanel,
                Width        = 100,
                Height       = 24,
                Left         = offHandLabel.Right + 5,
                Top          = offHandLabel.Top,
                SelectedItem = _model.OffHand.ToFriendlyString()
            };

            foreach (var type in Enum.GetValues(typeof(SkillWeaponType)).Cast<SkillWeaponType>()) {
                if (type == SkillWeaponType.Unknown) {
                    continue;
                }
                mainHandSelect.Items.Add(type.ToFriendlyString());
                offHandSelect.Items.Add(type.ToFriendlyString());
            }
            mainHandSelect.ValueChanged += OnMainHandChanged;
            offHandSelect.ValueChanged  += OnOffHandChanged;
            base.Build(buildPanel);
        }

        private void OnMainHandChanged(object o, ValueChangedEventArgs e) {
            if (!Enum.TryParse<SkillWeaponType>(e.CurrentValue.Replace(" ", ""), out var weapon)) {
                return;
            }
            _model.MainHand = weapon;
        }

        private void OnOffHandChanged(object o, ValueChangedEventArgs e) {
            if (!Enum.TryParse<SkillWeaponType>(e.CurrentValue.Replace(" ", ""), out var weapon)) {
                return;
            }
            _model.OffHand = weapon;
        }
    }
}
