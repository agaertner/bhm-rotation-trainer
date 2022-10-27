using Blish_HUD.Graphics.UI;
using Nekres.RotationTrainer.Player.Models;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class AbilityDetailsPresenter : Presenter<AbilityDetailsView, Ability> {
        public AbilityDetailsPresenter(AbilityDetailsView view, Ability model) : base(view, model) {

        }

    }
}
