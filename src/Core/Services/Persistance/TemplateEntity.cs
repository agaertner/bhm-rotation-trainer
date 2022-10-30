using Gw2Sharp.WebApi.V2.Models;
using LiteDB;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Linq;

namespace Nekres.RotationTrainer.Core.Services.Persistance {
    internal class TemplateEntity
    {
        /// <summary>
        /// Unique identifier of this build.
        /// </summary>
        [BsonId(true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Indicates when this entity has been created (UTC).
        /// </summary>
        [BsonField("CreationDate")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Indicates when this entity was last modified (UTC).
        /// </summary>
        [BsonField("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Guild Wars 2 client build id with which this entity was created.
        /// </summary>
        [BsonField("BuildId")]
        public int ClientBuildId { get; set; }

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
        /// Opener of the rotation.
        /// </summary>
        [BsonField("RotationOpener")]
        public string RotationOpener { get; set; }

        /// <summary>
        /// Loop of the rotation.
        /// </summary>
        [BsonField("RotationLoop")] 
        public string RotationLoop { get; set; }

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
                                       SkillWeaponType.None, SkillWeaponType.None, string.Empty, string.Empty, new [] {0,1,2}) {
            /* NOOP */
        }

        public TemplateEntity(Guid id, DateTime creationDate, DateTime modifiedDate, int clientBuildId, string title, string buildTemplate, 
                              SkillWeaponType primaryWeaponMainHand,   SkillWeaponType primaryWeaponOffHand, 
                              SkillWeaponType secondaryWeaponMainHand, SkillWeaponType secondaryWeaponOffHand, 
                              string rotationOpener, string rotationLoop, int[] utilityOrder) {
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

            this.RotationOpener = rotationOpener;
            this.RotationLoop   = rotationLoop;
            this.UtilityOrder   = utilityOrder;
        }

        public TemplateModel ToModel() {
            return new TemplateModel(this.Id, this.CreationDate, this.ModifiedDate, this.ClientBuildId) {
                Title         = this.Title,

                BuildTemplate    = this.BuildTemplate,
                PrimaryWeaponSet   = new TemplateModel.WeaponSet(this.PrimaryWeaponMainHand, this.PrimaryWeaponOffHand),
                SecondaryWeaponSet = new TemplateModel.WeaponSet(this.SecondaryWeaponMainHand, this.SecondaryWeaponOffHand),

                Rotation     = new TemplateModel.BuildRotation(this.RotationOpener, this.RotationLoop),
                UtilityOrder = this.UtilityOrder,
            };
        }

        public static TemplateEntity FromModel(TemplateModel model) {
            return new TemplateEntity(model.Id, model.CreationDate, model.ModifiedDate, model.ClientBuildId,
                                      model.Title, model.BuildTemplate, model.PrimaryWeaponSet.MainHand, model.PrimaryWeaponSet.OffHand, 
                                      model.SecondaryWeaponSet.MainHand, model.SecondaryWeaponSet.OffHand, model.Rotation.Opener, model.Rotation.Loop, model.UtilityOrder.ToArray());
        }
    }
}