using System;
using Blish_HUD.Graphics.UI;
using Nekres.RotationTrainer.Core.UI.Models;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class EditPrersenter : Presenter<EditView, TemplateModel> {
        public EditPrersenter(EditView view, TemplateModel model) : base(view, model) {
            model.Changed += View_OnModelChanged;
        }

        public void Delete() {
            RotationTrainerModule.Instance.DataService.DeleteById(this.Model.Id);
        }

        private void View_OnModelChanged(object o, EventArgs e) {
            RotationTrainerModule.Instance.DataService.Upsert(this.Model);
        }
    }
}
