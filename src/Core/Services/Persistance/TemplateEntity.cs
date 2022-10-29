using Gw2Sharp.WebApi.V2.Models;
using LiteDB;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Linq;
using Blish_HUD;

namespace Nekres.RotationTrainer.Core.Services.Persistance {
    internal class TemplateEntity
    {
        /// <summary>
        /// Unique identifier of this build.
        /// </summary>
        [BsonId(true)]
        public Guid Id { get; }

        /// <summary>
        /// Indicates when this entity has been created (UTC).
        /// </summary>
        [BsonField("CreationDate")]
        public DateTime CreationDate { get; }

        /// <summary>
        /// Indicates when this entity was last modified (UTC).
        /// </summary>
        [BsonField("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// The Guild Wars 2 client build id.
        /// </summary>
        [BsonField("BuildId")]
        public int ClientBuildId { get; }

        /// <summary>
        /// Custom title of this build.
        /// </summary>
        [BsonField("Title")]
        public string Title { get; set; }

        /// <summary>
        /// The build template code.
        /// </summary>
        [BsonField("BuildTemplate")] 
        public string BuildTemplate { get; set; }

        /// <summary>
        /// Primary Weapon - Main Hand
        /// </summary>
        [BsonField("PrimaryWeaponMainHand")]
        public SkillWeaponType PrimaryWeaponMainHand { get; set; }

        /// <summary>
        /// Primary Weapon - Off Hand
        /// </summary>
        [BsonField("PrimaryWeaponOffHand")]
        public SkillWeaponType PrimaryWeaponOffHand { get; set; }

        /// <summary>
        /// Secondary Weapon - Main Hand
        /// </summary>
        [BsonField("SecondaryWeaponMainHand")]
        public SkillWeaponType SecondaryWeaponMainHand { get; set; }

        /// <summary>
        /// Secondary Weapon - Off Hand
        /// </summary>
        [BsonField("SecondaryWeaponOffHand")]
        public SkillWeaponType SecondaryWeaponOffHand { get; set; }

        /// <summary>
        /// The rotation.
        /// </summary>
        [BsonField("Rotation")] 
        public string Rotation { get; set; }

        /// <summary>
        /// Remappings of the order of utility keys in the <see cref="BuildTemplate"/>. 
        /// </summary>
        /// <remarks>
        /// Each index represents a utility skill from left to right. Values represent the reordering.
        /// </remarks>
        [BsonField("UtilityOrder")] 
        public int[] UtilityOrder { get; set; }

        public TemplateEntity() : this(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow, 0, 
                                       string.Empty, string.Empty, SkillWeaponType.None, SkillWeaponType.None,
                                       SkillWeaponType.None, SkillWeaponType.None, string.Empty, new [] {0,1,2}) {
            /* NOOP */
        }

        public TemplateEntity(Guid id, DateTime creationDate, DateTime modifiedDate, int clientBuildId, string title, string buildTemplate, 
                              SkillWeaponType primaryWeaponMainHand,   SkillWeaponType primaryWeaponOffHand, 
                              SkillWeaponType secondaryWeaponMainHand, SkillWeaponType secondaryWeaponOffHand, string rotation, int[] utilityOrder) {
            this.Id           = id;
            this.CreationDate = creationDate;
            this.ModifiedDate = modifiedDate;

            this.ClientBuildId = clientBuildId;
            this.Title         = title;

            this.BuildTemplate         = buildTemplate;
            this.PrimaryWeaponMainHand = primaryWeaponMainHand;
            this.PrimaryWeaponOffHand  = primaryWeaponOffHand;
            this.SecondaryWeaponMainHand = secondaryWeaponMainHand;
            this.SecondaryWeaponOffHand = secondaryWeaponOffHand;

            this.Rotation     = rotation;
            this.UtilityOrder = utilityOrder;
        }

        public TemplateModel ToModel() {
            return new TemplateModel(this.Id, this.CreationDate, this.ModifiedDate, this.ClientBuildId) {
                Title         = this.Title,

                BuildTemplate    = this.BuildTemplate,
                PrimaryWeaponSet   = new TemplateModel.WeaponSet(SkillWeaponType.None, SkillWeaponType.None),
                SecondaryWeaponSet = new TemplateModel.WeaponSet(SkillWeaponType.None, SkillWeaponType.None),

                Rotation      = this.Rotation,
                UtilityOrder  = this.UtilityOrder,
            };
        }

        public static TemplateEntity FromModel(TemplateModel model) {
            return new TemplateEntity(model.Id, model.CreationDate, model.ModifiedDate, model.ClientBuildId,
                                      model.Title, model.BuildTemplate, model.PrimaryWeaponSet.MainHand, model.PrimaryWeaponSet.OffHand, 
                                      model.SecondaryWeaponSet.MainHand, model.SecondaryWeaponSet.OffHand, model.Rotation, model.UtilityOrder.ToArray());
        }
    }
}