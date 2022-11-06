using System;
using System.Linq;
using System.Text.RegularExpressions;
using Blish_HUD.Controls;

namespace Nekres.RotationTrainer.Core.Player.Models {
    internal class ActionHot : Action {

        private static Regex _pattern = new Regex(@"\[(?<message>.*?)\]\((?<action>[^\@\%]+)(\@(?<priority>[1-9]{1}[0-9]*))?(\%(?<cooldown>[1-9]{1}[0-9]*))?\)", RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        private        int   _priority;
        public int Priority {
            get => _priority;
            set {
                if (_priority == value) {
                    return;
                }
                _priority = value;
                OnChanged(EventArgs.Empty);
            }
        }

        private long _cooldownMs;
        public long CooldownMs {
            get => _cooldownMs;
            set {
                if (_cooldownMs == value) {
                    return;
                }
                _cooldownMs = value;
                OnChanged(EventArgs.Empty);
            }
        }

        public ActionHot(GuildWarsAction action, int priority, long cooldownMs, string message = "") : base(action, 0,0, message) {
            _priority = priority;
            _cooldownMs = cooldownMs;
        }

        public static bool TryParse(string input, out ActionHot action, int[] utilOrder = null) {
            action = null;

            string message         = string.Empty;
            var    gwAction        = GuildWarsAction.None;
            int    priority        = 0;
            long   cooldownMs      = 0;

            var    matchCollection = _pattern.Matches(input);

            foreach (Match match in matchCollection) {
                if (match.Groups["message"].Success) {
                    message = match.Groups["message"].Value;
                }

                if (match.Groups["action"].Success && !GwActionUtil.TryParse(match.Groups["action"].Value, out gwAction)) {
                    ScreenNotification.ShowNotification($"The action \"{match.Groups["action"].Value}\" doesn't exist.");
                    return false;
                }

                // We return if invalid arguments are found.
                if (match.Groups["priority"].Success && !int.TryParse(match.Groups["priority"].Value, out priority)) {
                    ScreenNotification.ShowNotification($"Invalid priority \"{match.Groups["priority"].Value}\".");
                    return false;
                }

                if (match.Groups["cooldown"].Success && !long.TryParse(match.Groups["cooldown"].Value, out cooldownMs)) {
                    ScreenNotification.ShowNotification($"Invalid cooldown \"{match.Groups["cooldown"].Value}\".");
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

            action = new ActionHot(gwAction, priority, cooldownMs, message);
            return true;
        }

        public override string ToString() {
            string str = base.ToString();

            if (this.Priority > 0) {
                str += $"@{this.Priority}";
            }

            if (this.CooldownMs > 0) {
                str += $"%{this.CooldownMs}";
            }

            str += ')';
            return str;
        }
    }
}
