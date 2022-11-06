using System.Collections.Generic;
using System.Linq;

namespace Nekres.RotationTrainer.Core.Player.Models {
    public enum GuildWarsAction {
        None,
        SwapWeapons,
        WeaponSkill1,
        WeaponSkill2,
        WeaponSkill3,
        WeaponSkill4,
        WeaponSkill5,
        HealingSkill,
        UtilitySkill1,
        UtilitySkill2,
        UtilitySkill3,
        EliteSkill,
        ProfessionSkill1,
        ProfessionSkill2,
        ProfessionSkill3,
        ProfessionSkill4,
        ProfessionSkill5,
        ProfessionSkill6,
        ProfessionSkill7,
        SpecialAction,
        Dodge,
        Interact
    }

    public static class GwActionUtil {
        private static readonly Dictionary<GuildWarsAction, string> _action2Str = new() {
            { GuildWarsAction.None, string.Empty },
            { GuildWarsAction.SwapWeapons, "^" },
            { GuildWarsAction.WeaponSkill1, "1" },
            { GuildWarsAction.WeaponSkill2, "2" },
            { GuildWarsAction.WeaponSkill3, "3" },
            { GuildWarsAction.WeaponSkill4, "4" },
            { GuildWarsAction.WeaponSkill5, "5" },
            { GuildWarsAction.HealingSkill, "6" },
            { GuildWarsAction.UtilitySkill1, "7" },
            { GuildWarsAction.UtilitySkill2, "8" },
            { GuildWarsAction.UtilitySkill3, "9" },
            { GuildWarsAction.EliteSkill, "0" },
            { GuildWarsAction.ProfessionSkill1, "f1" },
            { GuildWarsAction.ProfessionSkill2, "f2" },
            { GuildWarsAction.ProfessionSkill3, "f3" },
            { GuildWarsAction.ProfessionSkill4, "f4" },
            { GuildWarsAction.ProfessionSkill5, "f5" },
            { GuildWarsAction.ProfessionSkill6, "f6" },
            { GuildWarsAction.ProfessionSkill7, "f7" },
            { GuildWarsAction.SpecialAction, "n" },
            { GuildWarsAction.Dodge, "v" },
            { GuildWarsAction.Interact, "f" }
        };

        private static readonly Dictionary<string, GuildWarsAction> _str2Action = _action2Str.ToDictionary(x => x.Value, x => x.Key);
        
        public static string Serialize(GuildWarsAction action) {
            return _action2Str[action];
        }

        public static bool TryParse(string input, out GuildWarsAction action) {
            return _str2Action.TryGetValue(input, out action);
        }
    }

}
