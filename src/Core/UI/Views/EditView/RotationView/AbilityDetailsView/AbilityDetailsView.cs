using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Microsoft.Xna.Framework.Graphics;
using Nekres.RotationTrainer.Core.Controls;
using Nekres.RotationTrainer.Core.UI.Controls;
using System;
using System.Linq;
using Nekres.RotationTrainer.Core.Player.Models;
using Action = Nekres.RotationTrainer.Core.Player.Models.Action;
using HorizontalAlignment = Blish_HUD.Controls.HorizontalAlignment;
using Label = Blish_HUD.Controls.Label;
using TextBox = Blish_HUD.Controls.TextBox;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class AbilityDetailsView : View {
        public event EventHandler<ValueEventArgs<int>> Remove;
        public event EventHandler<ValueEventArgs<int>> Add;
        
        private static Texture2D _icon = RotationTrainerModule.Instance.ContentsManager.GetTexture("skill_frame.png");

        private Action _model;

        private int _index;
        public int Index {
            get => _index;
            set {
                _index = value;
                if (_numberLabel != null) {
                    _numberLabel.Text = $"{_index + 1}.";
                }
            }
        }

        private Label _numberLabel;

        public AbilityDetailsView(Action model, int index) {
            _model = model;
            _index = index;
        }

        protected override void Build(Container buildPanel) {

            _numberLabel = new Label {
                Parent              = buildPanel,
                Width               = 24,
                Height              = 24,
                Left                = 5,
                Top                 = 5,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment   = VerticalAlignment.Middle,
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size20, ContentService.FontStyle.Regular),
                StrokeText          = true,
                Text                = $"{_index + 1}."
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
                Width               = 75,
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
                Width               = 40,
                Height              = 24,
                Left                = repetitionsInput.Right + 5,
                Top                 = repetitionsInput.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Middle,
                Font                = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size20, ContentService.FontStyle.Regular),
                Text                = _model.Repetitions.ToString()
            };

            var messageInput = new TextBox {
                Parent = buildPanel,
                Width = 120,
                Height = 24,
                Top = repetitionsLabel.Top,
                Left = repetitionsLabel.Right + 5,
                Text = _model.Message,
                BasicTooltipText = "Add or Change Message"
            };
            messageInput.InputFocusChanged += OnMessageInputFocusChanged;

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

            var addButton = new AddButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent = buildPanel,
                Width  = 32,
                Height = 32,
                Right  = buildPanel.ContentRegion.Width - 16,
                Top    = repetitionsLabel.Top - 4,
                BasicTooltipText = "Insert a new action here."
            };
            addButton.Click += OnAddClicked;

            var removeButton = new RemoveButton(RotationTrainerModule.Instance.ContentsManager) {
                Parent           = buildPanel,
                Width            = 24,
                Height           = 24,
                Right            = addButton.Left - 10,
                Top              = repetitionsLabel.Top,
                BasicTooltipText = "Remove this action."
            };
            removeButton.Click += OnRemoveClicked;
            base.Build(buildPanel);
        }

        private void OnAddClicked(object o, MouseEventArgs e) {
            Add?.Invoke(this, new ValueEventArgs<int>(this.Index));
        }

        private void OnRemoveClicked(object o, MouseEventArgs e) {
            Remove?.Invoke(this, new ValueEventArgs<int>(this.Index));
        }

        private void OnActionChanged(object o, ValueChangedEventArgs e) {
            if (!Enum.TryParse<GuildWarsAction>(e.CurrentValue.Replace(" ", string.Empty), out var newAction)) {
                return;
            }
            _model.Action = newAction;
        }

        private void OnMessageInputFocusChanged(object o, ValueEventArgs<bool> e) {
            if (e.Value) {
                return;
            }
            _model.Message = ((TextBox)o).Text;
        }

    }
}
