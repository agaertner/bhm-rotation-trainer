using Blish_HUD;
using LiteDB;
using Nekres.RotationTrainer.Core.Services.Persistance;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gw2Sharp.WebApi.V2.Models;

namespace Nekres.RotationTrainer.Core.Services {
    internal class DataService : IDisposable {
        public event EventHandler<ValueEventArgs<Guid>> TemplateDeleted;

        private LiteDatabase                            _db;
        private ILiteCollection<TemplateEntity>         _ctx;
        public  bool Loading { get; private set; }

        private string _cacheDir;

        public DataService(string cacheDir) {
            _cacheDir = cacheDir;
            _db = new LiteDatabase(new ConnectionString {
                Filename   = Path.Combine(_cacheDir, "data.db"),
                Connection = ConnectionType.Shared
            });
            _ctx = _db.GetCollection<TemplateEntity>("templates");

            Upsert(new TemplateModel(new Guid("97d7ddf8-06cc-4ae4-8a7a-45a2ea5ea712"), DateTime.UtcNow, DateTime.UtcNow, GameService.Gw2Mumble.Info.BuildId) {
                Title            = "Condition Untamed (Example)",
                BuildTemplate    = "[&DQQePSA2SBd5AAAAARsAALYAAAC/AAAAwAAAAD0AAAAAAAAAAAAAAAAAAAA=]",
                PrimaryWeaponSet   = new TemplateModel.WeaponSet(SkillWeaponType.Axe, SkillWeaponType.Axe),
                SecondaryWeaponSet = new TemplateModel.WeaponSet(SkillWeaponType.None, SkillWeaponType.None),
                Rotation         = "2 3 4 u1 1/4000 2 3 elite 4 u2 1/4000 2 heal"
            });
        }

        public void Upsert(TemplateModel model) {
            _ctx.EnsureIndex(x => x.Id);
            var e = _ctx.FindOne(x => x.Id.Equals(model.Id));

            if (e == null) {
                _ctx.Insert(TemplateEntity.FromModel(model));
                GameService.Content.PlaySoundEffectByName("color-change");
            } else {
                e.Title                   = model.Title;
                e.Rotation                = model.Rotation;
                e.BuildTemplate           = model.BuildTemplate;
                e.PrimaryWeaponMainHand   = model.PrimaryWeaponSet.MainHand;
                e.PrimaryWeaponOffHand    = model.PrimaryWeaponSet.OffHand;
                e.SecondaryWeaponMainHand = model.SecondaryWeaponSet.MainHand;
                e.SecondaryWeaponOffHand  = model.SecondaryWeaponSet.OffHand;
                e.UtilityOrder            = model.UtilityOrder.ToArray();
                e.ModifiedDate            = DateTime.UtcNow;
                _ctx.Update(e);
            }
        }

        public List<TemplateModel> FindAll() {
            return _ctx.FindAll().Select(x => x.ToModel()).ToList();
        }

        public void DeleteById(Guid id) {
            _ctx.DeleteMany(x => x.Id.Equals(id));
            TemplateDeleted?.Invoke(this, new ValueEventArgs<Guid>(id));
        }

        public void Dispose() {
            _db?.Dispose();
        }
    }
}
