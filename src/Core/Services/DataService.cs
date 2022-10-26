using Blish_HUD;
using LiteDB;
using Nekres.RotationTrainer.Core.Services.Persistance;
using Nekres.RotationTrainer.Core.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        }

        public void Upsert(TemplateModel model) {
            _ctx.EnsureIndex(x => x.Id);
            var e = _ctx.FindOne(x => x.Id.Equals(model.Id));

            if (e == null) {
                _ctx.Insert(TemplateEntity.FromModel(model));
                GameService.Content.PlaySoundEffectByName("color-change");
            } else {
                e.Title          = model.Title;
                e.Rotation       = model.Rotation;
                e.BuildId        = model.BuildId;
                e.Template       = model.Template;
                e.UtilityOrder = model.UtilityOrder.ToArray();
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
