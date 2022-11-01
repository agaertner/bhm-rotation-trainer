using System;
using System.Collections.Generic;

namespace Nekres.RotationTrainer.Player.Models {
    internal class Ability {
        private static Dictionary<GuildWarsAction, string> _map = new() {
            { GuildWarsAction.SwapWeapons, "swap" },
            { GuildWarsAction.WeaponSkill1, "1" },
            { GuildWarsAction.WeaponSkill2, "2" },
            { GuildWarsAction.WeaponSkill3, "3" },
            { GuildWarsAction.WeaponSkill4, "4" },
            { GuildWarsAction.WeaponSkill5, "5" },
            { GuildWarsAction.HealingSkill, "heal" },
            { GuildWarsAction.UtilitySkill1, "u1" },
            { GuildWarsAction.UtilitySkill2, "u2" },
            { GuildWarsAction.UtilitySkill3, "u3" },
            { GuildWarsAction.EliteSkill, "elite" },
            { GuildWarsAction.ProfessionSkill1, "f1" },
            { GuildWarsAction.ProfessionSkill2, "f2" },
            { GuildWarsAction.ProfessionSkill3, "f3" },
            { GuildWarsAction.ProfessionSkill4, "f4" },
            { GuildWarsAction.ProfessionSkill5, "f5" },
            { GuildWarsAction.SpecialAction, "special" },
            { GuildWarsAction.Dodge, "dodge" },
            { GuildWarsAction.Interact, "use" }
        };

        public event EventHandler<EventArgs> Changed;

        private GuildWarsAction _action;
        public GuildWarsAction Action {
            get => _action;
            set {
                if (_action == value) {
                    return;
                }
                _action = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _duration;
        public int Duration {
            get => _duration;
            set {
                if (_duration == value) {
                    return;
                }
                _duration = value;
                Changed?.Invoke(this, EventArgs.Empty);
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
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _message;
        public string Message {
            get => _message;
            set {
                if (_message != null && _message.Equals(value)) {
                    return;
                }
                _message = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public Ability(GuildWarsAction action, int duration = 0, int repetitions = 0, string message = "") {
            _action = action;
            _duration = duration;
            _repetitions = repetitions;
            _message = message;
        }

        public override string ToString() {

            string str = "[";

            if (!string.IsNullOrEmpty(this.Message)) {
                str += this.Message ?? string.Empty;
            }

            str += "](";

            if (!_map.TryGetValue(this.Action, out string action)) {
                return string.Empty;
            }

            str += action;

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
