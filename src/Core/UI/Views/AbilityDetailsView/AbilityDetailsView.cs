using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework.Graphics;
using Nekres.RotationTrainer.Player.Models;
using System;
using System.Linq;
using Nekres.RotationTrainer.Core.Controls;
using HorizontalAlignment = Blish_HUD.Controls.HorizontalAlignment;
using Label = Blish_HUD.Controls.Label;
using TextBox = Blish_HUD.Controls.TextBox;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class AbilityDetailsView : View<AbilityDetailsPresenter> {

        private static Texture2D _icon = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");

        private int _number;

        public AbilityDetailsView(Ability model, int number) {
            _number = number;
            this.WithPresenter(new AbilityDetailsPresenter(this, model));
        }

        protected override void Build(Container buildPanel) {

            var number = new Label {
                Parent              = buildPanel,
                Width               = 24,
                Height              = 24,
                Left = 5,
                Top = 5,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment   = VerticalAlignment.Middle,
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size20, ContentService.FontStyle.Regular),
                StrokeText          = true,
                Text                = $"{_number}."
            };

            var icon = new Image {
                Parent  = buildPanel,
                Texture = _icon,
                Left    = number.Right + 10,
                Top     = number.Top,
                Width   = 24,
                Height  = 24
            };

            var actionDropdown = new Dropdown {
                Parent = buildPanel,
                Width  = 120,
                Height = 24,
                Left   = icon.Right + 5,
                Top    = icon.Top
            };

            foreach (var action in Enum.GetValues(typeof(GuildWarsAction)).Cast<GuildWarsAction>()) {
                if (action == GuildWarsAction.None) {
                    continue;
                }
                actionDropdown.Items.Add(action.ToString()); //TODO: Dropdown with display value (friendly name)
            }
            actionDropdown.SelectedItem =  this.Presenter.Model.Action.ToString();
            actionDropdown.ValueChanged += OnActionChanged;

            var durationInput = new StandardButton {
                Parent           = buildPanel,
                Width            = 24,
                Height           = 24,
                Left             = actionDropdown.Right + 5,
                Top              = actionDropdown.Top,
                BasicTooltipText = "Add or Change Duration",
                Text             = "/"
            };

            var durationLabel = new Label {
                Parent              = buildPanel,
                Width               = 80,
                Height              = 24,
                Left                = durationInput.Right + 5,
                Top                 = durationInput.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Middle,
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size20, ContentService.FontStyle.Regular),
                Text                = $"{this.Presenter.Model.Duration}ms"
            };

            var repetitionsInput = new StandardButton {
                Parent           = buildPanel,
                Width            = 24,
                Height           = 24,
                Left             = durationLabel.Right + 5,
                Top              = durationLabel.Top,
                BasicTooltipText = "Add or Change Repetitions",
                Text             = "*"
            };

            var repetitionsLabel = new Label {
                Parent              = buildPanel,
                Width               = 80,
                Height              = 24,
                Left                = repetitionsInput.Right + 5,
                Top                 = repetitionsInput.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Middle,
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size20, ContentService.FontStyle.Regular),
                Text                = this.Presenter.Model.Repetitions.ToString()
            };

            durationInput.Click += (o, e) => NumericInputPrompt.ShowPrompt((confirmed, n) => {
                if (!confirmed) {
                    return;
                }

                this.Presenter.Model.Duration = n;
                durationLabel.Text            = $"{n}ms";
            }, "Enter a Duration in Milliseconds:", this.Presenter.Model.Duration);

            repetitionsInput.Click += (o, e) => NumericInputPrompt.ShowPrompt((confirmed, n) => {
                if (!confirmed) {
                    return;
                }

                this.Presenter.Model.Repetitions = n;
                repetitionsLabel.Text            = n.ToString();
            }, "Enter a Number of Repetitions:", this.Presenter.Model.Repetitions);

            base.Build(buildPanel);
        }

        private void OnActionChanged(object o, ValueChangedEventArgs e) {
            if (!Enum.TryParse<GuildWarsAction>(e.CurrentValue, out var newAction)) {
                return;
            }
            this.Presenter.Model.Action = newAction;
        }
    }
}
