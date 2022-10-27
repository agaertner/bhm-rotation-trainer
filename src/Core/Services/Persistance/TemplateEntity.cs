using LiteDB;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nekres.RotationTrainer.Core.Services.Persistance {
    internal class TemplateEntity
    {
        /// <summary>
        /// Internal database id.
        /// </summary>
        [BsonId(true)]
        public int _id { get; set; }

        /// <summary>
        /// Unique identifier of this build.
        /// </summary>
        [BsonField("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Custom title of this build.
        /// </summary>
        [BsonField("title")]
        public string Title { get; set; }

        /// <summary>
        /// The Guild Wars 2 client build id.
        /// </summary>
        [BsonField("buildId")] 
        public int BuildId { get; set; }

        /// <summary>
        /// The build template code.
        /// </summary>
        [BsonField("template")] 
        public string Template { get; set; }

        /// <summary>
        /// The rotation.
        /// </summary>
        [BsonField("rotation")] 
        public string Rotation { get; set; }

        /// <summary>
        /// Remappings of the order of utility keys in the <see cref="Template"/>. 
        /// </summary>
        /// <remarks>
        /// Each index represents a utility skill from left to right. Values represent the reordering.
        /// </remarks>
        [BsonField("utilityOrder")] 
        public int[] UtilityOrder { get; set; }

        public TemplateEntity(Guid id) {
            this.Id             = id;
            this.Title          = string.Empty;
            this.BuildId        = 0;
            this.Rotation       = string.Empty;
            this.Template       = string.Empty;
            this.UtilityOrder = new [] {0, 1, 2};
        }

        public TemplateModel ToModel() {
            return new TemplateModel(this.Id) {
                Title          = this.Title,
                BuildId        = this.BuildId,
                UtilityOrder = new ObservableCollection<int>(this.UtilityOrder),
                Rotation       = this.Rotation,
                Template       = this.Template,
            };
        }

        public static TemplateEntity FromModel(TemplateModel model) {
            return new TemplateEntity(model.Id) {
                BuildId        = model.BuildId,
                Rotation       = model.Rotation,
                Template       = model.Template,
                Title          = model.Title,
                UtilityOrder   = model.UtilityOrder.ToArray()
            };
        }
    }
}