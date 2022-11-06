using System;

namespace Nekres.RotationTrainer.Core.Player.Models {
    internal abstract class ActionBase {

        protected static GuildWarsAction[] UtilityRemappable = new GuildWarsAction[3]
        {
            GuildWarsAction.UtilitySkill1,
            GuildWarsAction.UtilitySkill2,
            GuildWarsAction.UtilitySkill3
        };

        protected static GuildWarsAction[] ToolbeltRemappable = new GuildWarsAction[3]
        {
            GuildWarsAction.ProfessionSkill2,
            GuildWarsAction.ProfessionSkill3,
            GuildWarsAction.ProfessionSkill4
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

        protected ActionBase(GuildWarsAction action, string message) {
            _action = action;
            _message = message;
        }

        protected virtual void OnChanged(EventArgs e) {
            Changed?.Invoke(this, e);
        }

        public override string ToString() {

            string str = "[";

            if (!string.IsNullOrEmpty(this.Message)) {
                str += this.Message ?? string.Empty;
            }

            str += $"]({GwActionUtil.Serialize(this.Action)}";

            return str;
        }
    }
}
