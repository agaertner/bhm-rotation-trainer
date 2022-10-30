using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework.Graphics;
using Nekres.RotationTrainer.Player.Models;
using System;
using System.Linq;
using Blish_HUD.Input;
using Nekres.RotationTrainer.Core.Controls;
using Nekres.RotationTrainer.Core.UI.Controls;
using HorizontalAlignment = Blish_HUD.Controls.HorizontalAlignment;
using Label = Blish_HUD.Controls.Label;
using TextBox = Blish_HUD.Controls.TextBox;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class AbilityDetailsView : View {
        public event EventHandler<ValueEventArgs<int>> Remove;

        private static Texture2D _icon = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");

        private Ability _model;

        private int     _number;
        public int Number {
            get => _number;
            set {
                _number = value;
                if (_numberLabel != null) {
                    _numberLabel.Text = $"{_number}.";
                }
            }
        }

        private Label _numberLabel;

        public AbilityDetailsView(Ability model, int number) {
            _model = model;
            _number = number;
        }

        protected override void Build(Container buildPanel) {

            _numberLabel = new Label {
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
                Left    = _numberLabel.Right + 10,
                Top     = _numberLabel.Top,
                Width   = 24,
                Height  = 24
            };

            var actionDropdown = new Dropdown {
                Parent = buildPanel,
                Width  = 126,
                Height = 24,
                Left   = icon.Right + 5,
                Top    = icon.Top
            };

            foreach (var action in Enum.GetValues(typeof(GuildWarsAction)).Cast<GuildWarsAction>()) {
                if (action == GuildWarsAction.None) {
                    continue;
                }
                actionDropdown.Items.Add(action.ToFriendlyString());
            }
            actionDropdown.SelectedItem =  _model.Action.ToFriendlyString();
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
                Text                = $"{_model.Duration}ms"
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
                Text                = _model.Repetitions.ToString()
            };

            durationInput.Click += (o, e) => NumericInputPrompt.ShowPrompt((confirmed, n) => {
                if (!confirmed) {
                    return;
                }
                _model.Duration = n;
                durationLabel.Text            = $"{n}ms";
            }, "Enter a Duration in Milliseconds:", _model.Duration.ToString());

            repetitionsInput.Click += (o, e) => NumericInputPrompt.ShowPrompt((confirmed, n) => {
                if (!confirmed) {
                    return;
                }
                _model.Repetitions = n;
                repetitionsLabel.Text            = n.ToString();
            }, "Enter a Number of Repetitions:", _model.Repetitions.ToString());

            var removeButton = new RemoveButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent = buildPanel,
                Width  = 24,
                Height = 24,
                Right  = buildPanel.ContentRegion.Width - 13,
                Top    = repetitionsLabel.Top
            };
            removeButton.Click += OnRemoveClicked;
            base.Build(buildPanel);
        }

        private void OnRemoveClicked(object o, MouseEventArgs e) {
            Remove?.Invoke(this, new ValueEventArgs<int>(Number));
        }

        private void OnActionChanged(object o, ValueChangedEventArgs e) {
            if (!Enum.TryParse<GuildWarsAction>(e.CurrentValue.Replace(" ", string.Empty), out var newAction)) {
                return;
            }
            _model.Action = newAction;
        }
    }
}
