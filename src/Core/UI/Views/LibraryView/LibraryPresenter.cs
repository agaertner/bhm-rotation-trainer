using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Core.UI.Models;
using Nekres.RotationTrainer.Player.Models;
using System;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class LibraryPresenter : Presenter<LibraryView, LibraryModel>
    {
        public LibraryPresenter(LibraryView view, LibraryModel model) : base(view, model) {
            model.TemplateModels = RotationTrainerModule.Instance.DataService.FindAll();
            this.View.AddNewClick += OnAddNewClicked;
        }

        private void OnAddNewClicked(object o, EventArgs e) {
            var model = new TemplateModel();
            this.AddTemplate(model, true);
            RotationTrainerModule.Instance.DataService.Upsert(model);
        }

        internal void AddTemplate(TemplateModel model, bool isNew = false) {
            Texture2D tex  = ContentService.Textures.Pixel;
            string    name = string.Empty;

            if (!isNew && model.TryGetBuildChatLink(out var build)) {
                var prof    = build.Profession;
                var elite   = build.Specialization3Id;
                var isElite = RotationTrainerModule.Instance.RenderService.IsEliteSpec(elite); 
                tex = isElite ? RotationTrainerModule.Instance.RenderService.GetEliteRender(elite) 
                          : RotationTrainerModule.Instance.RenderService.GetProfessionRender(prof); 
                name = isElite ? RotationTrainerModule.Instance.RenderService.GetEliteSpecName(elite)
                               : RotationTrainerModule.Instance.RenderService.GetProfessionName(prof);
            }

            var button = new TemplateButton(model) {
                Parent = this.View.TemplatePanel,
                Size   = new Point(345, 100),
                Icon       = tex,
                IconSize   = DetailsIconSize.Small,
                Text       = model.Title,
                BottomText = name
            };
            button.PlayClick += delegate {
                if (!Rotation.TryParse(model.Rotation, out var rotation)) {
                    ScreenNotification.ShowNotification("Unable to play rotation.", ScreenNotification.NotificationType.Error);
                    return;
                }
                RotationTrainerModule.Instance.TemplatePlayer.Play(rotation);
            };
            button.EditClick += OnEditMacroClicked;
        }

        private void OnEditMacroClicked(object o, MouseEventArgs e) {
            var ctrl = (TemplateButton)o;
            ctrl.Active = true;
            var bgTex = GameService.Content.GetTexture("controls/window/502049");
            var windowRegion = new Rectangle(40, 26, 895 + 38, 780 - 56);
            var contentRegion = new Rectangle(70, 41, 895 - 43, 780 - 142);
            var editWindow = new StandardWindow(bgTex, windowRegion, contentRegion) {
                Emblem = RotationTrainerModule.Instance.EditTexture,
                Parent = GameService.Graphics.SpriteScreen,
                Location = new Point((GameService.Graphics.SpriteScreen.Width - windowRegion.Width) / 2, (GameService.Graphics.SpriteScreen.Height - windowRegion.Height) / 2),
                Title = $"Edit Template - {ctrl.Title}",
                Id = $"{nameof(RotationTrainerModule)}_{nameof(EditView)}_f61def90-bdb1-423a-a4dd-478b02506460",
                SavesPosition = true
            };
            editWindow.Show(new EditView(ctrl.TemplateModel));
            editWindow.Hidden += (_, _) => editWindow.Dispose();
            editWindow.Disposed += (_, _) => ctrl.Active = false;
        }
    }
}
