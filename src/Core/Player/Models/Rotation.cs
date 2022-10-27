using Blish_HUD.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nekres.RotationTrainer.Player.Models {
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
        SpecialAction,
        Dodge,
        Interact
    }
    internal class Rotation : IEnumerable<Ability> {
        private static Regex _syntaxPattern = new Regex(@"(?<repetitions>(?<=\*)[1-9]{1}[0-9]*)|(?<duration>(?<=\/)[1-9]{1}[0-9]*)|(?<action>^[^\*\/]+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        private static Dictionary<string, GuildWarsAction> _map = new() {
            {"swap", GuildWarsAction.SwapWeapons},
            {"drop", GuildWarsAction.SwapWeapons},
            {"1", GuildWarsAction.WeaponSkill1},
            {"auto", GuildWarsAction.WeaponSkill1},
            {"aa", GuildWarsAction.WeaponSkill1},
            {"2", GuildWarsAction.WeaponSkill2},
            {"3", GuildWarsAction.WeaponSkill3},
            {"4", GuildWarsAction.WeaponSkill4},
            {"5", GuildWarsAction.WeaponSkill5},
            {"heal", GuildWarsAction.HealingSkill},
            {"6", GuildWarsAction.HealingSkill},
            {"7", GuildWarsAction.UtilitySkill1},
            {"u1", GuildWarsAction.UtilitySkill1},
            { "8", GuildWarsAction.UtilitySkill2},
            { "u2", GuildWarsAction.UtilitySkill2 },
            { "9", GuildWarsAction.UtilitySkill3},
            { "u3", GuildWarsAction.UtilitySkill3 },
            { "0", GuildWarsAction.EliteSkill},
            {"elite", GuildWarsAction.EliteSkill},
            {"f1", GuildWarsAction.ProfessionSkill1},
            {"f2", GuildWarsAction.ProfessionSkill2},
            {"f3", GuildWarsAction.ProfessionSkill3},
            {"f4", GuildWarsAction.ProfessionSkill4},
            {"f5", GuildWarsAction.ProfessionSkill5},
            {"special", GuildWarsAction.SpecialAction},
            {"s", GuildWarsAction.SpecialAction},
            {"dodge", GuildWarsAction.Dodge},
            {"interact", GuildWarsAction.Interact},
            {"use", GuildWarsAction.Interact},
            {"take", GuildWarsAction.Interact}
        };
        private static GuildWarsAction[] _utilityRemappable = new GuildWarsAction[3]
        {
            GuildWarsAction.UtilitySkill1,
            GuildWarsAction.UtilitySkill2,
            GuildWarsAction.UtilitySkill3
        };
        private static GuildWarsAction[] _toolbeltRemappable = new GuildWarsAction[3]
        {
            GuildWarsAction.ProfessionSkill2,
            GuildWarsAction.ProfessionSkill3,
            GuildWarsAction.ProfessionSkill4
        };

        public event EventHandler<EventArgs>  Changed;
        
        private ObservableCollection<Ability> _abilities;

        public Rotation(IEnumerable<Ability> abilities) {
            _abilities                   =  new ObservableCollection<Ability>(abilities);
            _abilities.CollectionChanged += OnAbilitiesChanged;
            this.RenewChangeHandlers();
        }

        private void OnAbilitiesChanged(object o, NotifyCollectionChangedEventArgs e) {
            Changed?.Invoke(this, EventArgs.Empty);
            this.RenewChangeHandlers();
        }

        private void RenewChangeHandlers() {
            foreach (var ability in _abilities) {
                ability.Changed -= OnAbilityChanged;
                ability.Changed += OnAbilityChanged;
            }
        }

        private void OnAbilityChanged(object o, EventArgs e) {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public static bool TryParse(string rawRotation, out Rotation rotation, int[] utilOrder = null) {

            utilOrder ??= new[] { 0,1,2 };
            rotation = null;

            if (string.IsNullOrEmpty(rawRotation)) {
                return false;
            }

            var abilities = new List<Ability>();

            string[] actions = rawRotation.Split(' ');

            foreach (string s in actions) {

                string expression  = s.ToLowerInvariant();
                int    duration    = 0;
                int    repetitions = 0;
                var    action      = GuildWarsAction.None;

                var matchCollection = _syntaxPattern.Matches(expression);

                foreach (Match match in matchCollection) {
                    if (match.Groups["action"].Success && !_map.TryGetValue(match.Groups["action"].Value, out action)) {
                        ScreenNotification.ShowNotification($"The action \"{match.Groups["action"].Value}\" doesn't exist.");
                        return false;
                    }

                    // We return if invalid arguments are found.
                    if (match.Groups["duration"].Success && !int.TryParse(match.Groups["duration"].Value, out duration)) {
                        ScreenNotification.ShowNotification($"Invalid duration \"{match.Groups["action"].Value}\".");
                        return false;
                    }

                    if (match.Groups["repetitions"].Success && !int.TryParse(match.Groups["repetitions"].Value, out repetitions)) {
                        ScreenNotification.ShowNotification($"Invalid repetitions \"{match.Groups["action"].Value}\".");
                        return false;
                    }
                }

                // Applying user-defined utility order.
                if (_utilityRemappable.Contains(action)) {
                    action = _utilityRemappable[utilOrder[Array.IndexOf(_utilityRemappable, action)]];
                } else if (_toolbeltRemappable.Contains(action)) {
                    action = _toolbeltRemappable[utilOrder[Array.IndexOf(_toolbeltRemappable, action)]];
                }

                // Add ability to rotation
                abilities.Add(new Ability(action, duration, repetitions));
            }

            rotation = new Rotation(abilities);
            return true;
        }

        public IEnumerator<Ability> GetEnumerator() {
            return _abilities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            if (!_abilities.Any()) {
                return base.ToString();
            }
            return string.Join(" ", _abilities.Select(x => x.ToString()));
        }

    }
}
