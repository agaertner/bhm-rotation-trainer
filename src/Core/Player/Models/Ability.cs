using System;
using System.Collections.Generic;

namespace Nekres.RotationTrainer.Player.Models {
    internal class Ability : IDisposable {
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

        public GuildWarsAction Action { get; }

        public int Duration { get; }

        public int Repetitions { get; }

        public Ability(GuildWarsAction action, int duration = 0, int repetitions = 0) {
            Action = action;
            Duration = duration;
            Repetitions = repetitions;
        }

        public virtual void Dispose() {
            // NOOP
        }

        public override string ToString() {
            if (!_map.TryGetValue(this.Action, out string str)) {
                return base.ToString();
            }
            if (this.Repetitions > 0) {
                str += '*' + this.Repetitions;
            }
            if (this.Duration > 0) {
                str += '/' + this.Duration;
            }
            return str;
        }

    }
}
