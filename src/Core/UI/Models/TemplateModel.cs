using Blish_HUD;
using Gw2Sharp.ChatLinks;
using Gw2Sharp.WebApi.V2.Models;
using Nekres.RotationTrainer.Player.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Text;

namespace Nekres.RotationTrainer.Core.UI.Models {
    internal class TemplateModel 
    {
        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Unique identifier of this build.
        /// </summary>
        [JsonProperty("uid")]
        public Guid Id { get; private init; }

        /// <summary>
        /// Indicates when this rotation has been created (UTC).
        /// </summary>
        [JsonProperty("cat")]
        public DateTime CreationDate { get; private init; }

        /// <summary>
        /// Indicates when this rotation was last modified (UTC).
        /// </summary>
        [JsonProperty("mat")]
        public DateTime ModifiedDate { get; private init; }

        /// <summary>
        /// The Guild Wars 2 client build id.
        /// </summary>
        [JsonProperty("gw2")]
        public int ClientBuildId { get; private init; }

        private string _title;
        /// <summary>
        /// Custom title of this build.
        /// </summary>
        [JsonProperty("tit")]
        public string Title 
        {
            get => _title;
            set {
                if (_title != null && _title.Equals(value)) {
                    return;
                }
                _title = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _buildTemplate;
        /// <summary>
        /// The build template code.
        /// </summary>
        [JsonProperty("tem")]
        public string BuildTemplate 
        { 
            get => _buildTemplate;
            set {
                if (_buildTemplate != null && _buildTemplate.Equals(value)) {
                    return;
                }
                _buildTemplate = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private WeaponSet _primaryWeaponSet;
        /// <summary>
        /// The 1st weapon set.
        /// </summary>
        [JsonProperty("prw")]
        public WeaponSet PrimaryWeaponSet {
            get => _primaryWeaponSet;
            set {
                if (_primaryWeaponSet != null && _primaryWeaponSet.Equals(value)) {
                    return;
                }
                _primaryWeaponSet = value;
                Changed?.Invoke(this, EventArgs.Empty);
                if (value != null) {
                    value.Changed -= OnBuildChanged;
                    value.Changed += OnBuildChanged;
                }
            }
        }

        private WeaponSet _secondaryWeaponSet;
        /// <summary>
        /// The 2nd weapon set.
        /// </summary>
        [JsonProperty("snw")]
        public WeaponSet SecondaryWeaponSet {
            get => _secondaryWeaponSet;
            set {
                if (_secondaryWeaponSet != null && _secondaryWeaponSet.Equals(value)) {
                    return;
                }
                _secondaryWeaponSet = value;
                Changed?.Invoke(this, EventArgs.Empty);
                if (value != null) {
                    value.Changed -= OnBuildChanged;
                    value.Changed += OnBuildChanged;
                }
            }
        }

        private BuildRotation _rotation;
        /// <summary>
        /// The rotation.
        /// </summary>
        [JsonProperty("rot")]
        public BuildRotation Rotation 
        { 
            get => _rotation;
            set {
                if (_rotation != null && _rotation.Equals(value)) {
                    return;
                }
                _rotation = value;
                Changed?.Invoke(this, EventArgs.Empty);
                if (value != null) {
                    value.Changed -= OnBuildChanged;
                    value.Changed += OnBuildChanged;
                }
            }
        }

        private int[] _utilityOrder;
        /// <summary>
        /// Remappings of the order of utility keys in the <see cref="BuildTemplate"/>. 
        /// </summary>
        /// <remarks>
        /// Each index represents a utility skill from left to right. Values (1-3) represent the reordering.
        /// </remarks>
        [JsonIgnore]
        public int[] UtilityOrder 
        { 
            get => _utilityOrder;
            set {
                if (_utilityOrder != null && _utilityOrder.SequenceEqual(value)) {
                    return;
                }
                _utilityOrder = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public TemplateModel(Guid id, DateTime creationDate, DateTime modifiedDate, int clientBuildId) {
            this.Id            = id;
            this.CreationDate  = creationDate;
            this.ModifiedDate  = modifiedDate;
            this.ClientBuildId = clientBuildId;

            _title = string.Empty;

            _buildTemplate    = string.Empty;
            _primaryWeaponSet   = new WeaponSet(SkillWeaponType.None, SkillWeaponType.None);
            _secondaryWeaponSet = new WeaponSet(SkillWeaponType.None, SkillWeaponType.None);

            _rotation     = new BuildRotation(string.Empty, string.Empty);
            _utilityOrder = new int[3] { 0, 1, 2 };
        }

        public TemplateModel() : this(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow, GameService.Gw2Mumble.Info.BuildId) {
            /* NOOP */
        }

        /// <summary>
        /// Converts the <see cref="BuildTemplate"/> to a .NET Object.
        /// </summary>
        /// <returns>A <see cref="BuildChatLink"/> object representing the <see cref="BuildTemplate"/>.</returns>
        public bool TryGetBuildChatLink(out BuildChatLink buildChatLink) {
            buildChatLink = null;
            if (string.IsNullOrEmpty(this.BuildTemplate)) {
                return false;
            }
            return Gw2ChatLink.TryParse(this.BuildTemplate, out var chatLink) && chatLink.TryCast(out buildChatLink);
        }

        public override string ToString() {
            return $"[&{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, new StringEnumConverter())))}]";
        }

        public static bool TryParse(string code, out TemplateModel model) {
            if (string.IsNullOrEmpty(code)) {
                model = null;
                return false;
            }

            if (code.StartsWith("[", StringComparison.OrdinalIgnoreCase)) {
                code = code.Substring(1);
            }

            if (code.StartsWith("&", StringComparison.OrdinalIgnoreCase)) {
                code = code.Substring(1);
            }

            if (code.EndsWith("]", StringComparison.OrdinalIgnoreCase)) {
                code = code.Remove(code.Length - 1);
            }

            try {
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(code));
                return TaskUtil.TryParseJson(json, out model);
            } catch (Exception e) when (e is ArgumentException or FormatException) {
                model = null;
                return false;
            }
        }

        private void OnBuildChanged(object o, EventArgs e) {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public sealed class WeaponSet {

            public event EventHandler<EventArgs> Changed;

            private SkillWeaponType _mainHand;
            [JsonProperty("mhd"), JsonConverter(typeof(StringEnumConverter))]
            public SkillWeaponType MainHand {
                get => _mainHand;
                set {
                    if (_mainHand == value) {
                        return;
                    }
                    _mainHand = value;
                    Changed?.Invoke(this, EventArgs.Empty);
                }
            }

            private SkillWeaponType _offHand;
            [JsonProperty("ohd"), JsonConverter(typeof(StringEnumConverter))]
            public SkillWeaponType OffHand {
                get => _offHand;
                set {
                    if (_offHand == value) {
                        return;
                    }
                    _offHand = value;
                    Changed?.Invoke(this, EventArgs.Empty);
                }
            }

            public WeaponSet(SkillWeaponType mainHand, SkillWeaponType offHand) {
                _mainHand = mainHand;
                _offHand = offHand;
            }
        }

        public sealed class BuildRotation {

            public event EventHandler<EventArgs> Changed;

            private Rotation _opener;
            [JsonProperty("ope"), JsonConverter(typeof(RotationJsonConverter))]
            public Rotation Opener {
                get => _opener;
                set {
                    if (_opener != null && _opener.Equals(value)) {
                        return;
                    }
                    _opener = value;
                    Changed?.Invoke(this, EventArgs.Empty);

                    if (value != null) {
                        value.Changed += OnRotationChanged;
                    }
                }
            }

            private Rotation _loop;
            [JsonProperty("loo"), JsonConverter(typeof(RotationJsonConverter))]
            public Rotation Loop {
                get => _loop;
                set {
                    if (_loop != null && _loop.Equals(value)) {
                        return;
                    }
                    _loop = value;
                    Changed?.Invoke(this, EventArgs.Empty);

                    if (value != null) {
                        value.Changed += OnRotationChanged;
                    }
                }
            }

            public BuildRotation(string opener, string loop) {
                _opener = Player.Models.Rotation.TryParse(opener, out var o) ? o : new Rotation();
                _loop = Player.Models.Rotation.TryParse(loop, out var l) ? l : new Rotation();
            }

            private void OnRotationChanged(object sender, EventArgs e) {
                Changed?.Invoke(this, EventArgs.Empty);
            }

        }
    }
}
