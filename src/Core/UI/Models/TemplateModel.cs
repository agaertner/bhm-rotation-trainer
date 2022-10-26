using Gw2Sharp.ChatLinks;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Blish_HUD;
using Blish_HUD.Content;

namespace Nekres.RotationTrainer.Core.UI.Models {
    internal class TemplateModel 
    {
        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Unique identifier of this build.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; private init; }

        private string _title;
        /// <summary>
        /// Custom title of this build.
        /// </summary>
        [JsonProperty("title")]
        public string Title 
        {
            get => _title;
            set {
                if (!string.IsNullOrEmpty(_title) && _title.Equals(value)) {
                    return;
                }
                _title = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _buildId;
        /// <summary>
        /// The Guild Wars 2 client build id.
        /// </summary>
        [JsonProperty("buildId")]
        public int BuildId 
        {
            get => _buildId;
            set {
                if (_buildId == value) {
                    return;
                }
                _buildId = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _template;
        /// <summary>
        /// The build template code.
        /// </summary>
        [JsonProperty("template")]
        public string Template 
        { 
            get => _template;
            set {
                if (!string.IsNullOrEmpty(_template) && _template.Equals(value)) {
                    return;
                }
                _template = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _rotation;
        /// <summary>
        /// The rotation.
        /// </summary>
        [JsonProperty("rotation")]
        public string Rotation 
        { 
            get => _rotation;
            set {
                if (!string.IsNullOrEmpty(_rotation) && _rotation.Equals(value)) {
                    return;
                }
                _rotation = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        private ObservableCollection<int> _utilityOrder;
        /// <summary>
        /// Remappings of the order of utility keys in the <see cref="Template"/>. 
        /// </summary>
        /// <remarks>
        /// Each index represents a utility skill from left to right. Values (1-3) represent the reordering.
        /// </remarks>
        [JsonIgnore]
        public ObservableCollection<int> UtilityOrder 
        { 
            get => _utilityOrder;
            set {
                if (_utilityOrder != null && _utilityOrder.Equals(value)) {
                    return;
                }

                _utilityOrder                   =  value;
                _utilityOrder.CollectionChanged += (_, _) => Changed?.Invoke(this, EventArgs.Empty);
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public TemplateModel(Guid id) {
            this.Id             = id;
            this.Title          = string.Empty;
            this.BuildId        = GameService.Gw2Mumble.Info.BuildId;
            this.Rotation       = string.Empty;
            this.Template       = string.Empty;
            this.UtilityOrder = new ObservableCollection<int>(new int[3] {0, 1, 2});
        }

        public TemplateModel() : this(Guid.NewGuid()) 
        {
        }

        /// <summary>
        /// Converts the <see cref="Template"/> to a .NET Object.
        /// </summary>
        /// <returns>A <see cref="BuildChatLink"/> object representing the <see cref="Template"/>.</returns>
        public bool TryGetBuildChatLink(out BuildChatLink buildChatLink) {
            buildChatLink = null;
            if (string.IsNullOrEmpty(this.Template)) {
                return false;
            }
            try {
                if (Gw2ChatLink.TryParse(this.Template, out var chatLink)) {
                    buildChatLink = (BuildChatLink)chatLink;
                    return true;
                }
            }
            catch (InvalidCastException) {
                return false;
            }
            return false;
        }

        public override string ToString() {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }

        public static bool TryParse(string code, out TemplateModel model) {
            string json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(code));
            return TaskUtil.TryParseJson(json, out model);
        }
    }
}
