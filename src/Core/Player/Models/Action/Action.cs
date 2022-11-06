using System;
using System.Linq;
using System.Text.RegularExpressions;
using Blish_HUD.Controls;

namespace Nekres.RotationTrainer.Core.Player.Models {
    internal class Action : ActionBase {

        private static Regex _pattern = new Regex(@"\[(?<message>.*?)\]\((?<action>[^\*\/]+)(\*(?<repetitions>[1-9]{1}[0-9]*))?(\/(?<duration>[1-9]{1}[0-9]*))?\)", RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        private        int   _duration;
        public int Duration {
            get => _duration;
            set {
                if (_duration == value) {
                    return;
                }
                _duration = value;
                OnChanged(EventArgs.Empty);
            }
        }

        private int _repetitions;
        public int Repetitions {
            get => _repetitions;
            set {
                if (_repetitions == value) {
                    return;
                }
                _repetitions = value;
                OnChanged(EventArgs.Empty);
            }
        }

        public Action(GuildWarsAction action, int duration = 0, int repetitions = 0, string message = "") : base(action, message) {
            _duration = duration;
            _repetitions = repetitions;
        }

        public static bool TryParse(string input, out Action action, int[] utilOrder = null) {
            action = null;

            string message     = string.Empty;
            var    gwAction    = GuildWarsAction.None;
            int    duration    = 0;
            int    repetitions = 0;

            var matchCollection = _pattern.Matches(input);

            foreach (Match match in matchCollection) {
                if (match.Groups["message"].Success) {
                    message = match.Groups["message"].Value;
                }

                if (match.Groups["action"].Success && !GwActionUtil.TryParse(match.Groups["action"].Value, out gwAction)) {
                    ScreenNotification.ShowNotification($"The action \"{match.Groups["action"].Value}\" doesn't exist.");
                    return false;
                }

                // We return if invalid arguments are found.
                if (match.Groups["duration"].Success && !int.TryParse(match.Groups["duration"].Value, out duration)) {
                    ScreenNotification.ShowNotification($"Invalid duration \"{match.Groups["duration"].Value}\".");
                    return false;
                }

                if (match.Groups["repetitions"].Success && !int.TryParse(match.Groups["repetitions"].Value, out repetitions)) {
                    ScreenNotification.ShowNotification($"Invalid repetitions \"{match.Groups["repetitions"].Value}\".");
                    return false;
                }
            }

            // Applying user-defined utility order.
            utilOrder ??= new[] { 0, 1, 2 };
            if (UtilityRemappable.Contains(gwAction)) {
                gwAction = UtilityRemappable[utilOrder[Array.IndexOf(UtilityRemappable, gwAction)]];
            } else if (ToolbeltRemappable.Contains(gwAction)) {
                gwAction = ToolbeltRemappable[utilOrder[Array.IndexOf(ToolbeltRemappable, gwAction)]];
            }

            action = new Action(gwAction, duration, repetitions, message);
            return true;
        }

        public override string ToString() {
            string str = base.ToString();

            if (this.Repetitions > 0) {
                str += $"*{this.Repetitions}";
            }

            if (this.Duration > 0) {
                str += $"/{this.Duration}";
            }

            str += ')';
            return str;
        }

    }
}
