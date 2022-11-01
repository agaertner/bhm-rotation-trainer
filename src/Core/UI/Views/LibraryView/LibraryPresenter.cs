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
using Blish_HUD.Content;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class LibraryPresenter : Presenter<LibraryView, LibraryModel>
    {
        public LibraryPresenter(LibraryView view, LibraryModel model) : base(view, model) {
            model.TemplateModels               =  RotationTrainerModule.Instance.DataService.FindAll();
            this.View.AddNewClick              += OnAddNewClicked;
            this.View.ImportFromClipboardClick += OnImportFromClipboardClicked;
        }

        protected override void Unload() {
            this.View.AddNewClick              -= OnAddNewClicked;
            this.View.ImportFromClipboardClick -= OnImportFromClipboardClicked;
            base.Unload();
        }

        private async void OnImportFromClipboardClicked(object o, EventArgs e) {
            var code = await ClipboardUtil.WindowsClipboardService.GetTextAsync();
            if (!TemplateModel.TryParse(code, out var model)) {
                GameService.Content.PlaySoundEffectByName("error");
                ScreenNotification.ShowNotification("Your clipboard does not contain a valid template.", ScreenNotification.NotificationType.Error);
                return;
            }
            this.AddTemplate(model);
            RotationTrainerModule.Instance.DataService.Upsert(model);
        }

        private void OnAddNewClicked(object o, EventArgs e) {
            var model = new TemplateModel();
            this.AddTemplate(model);
            RotationTrainerModule.Instance.DataService.Upsert(model);
        }

        internal void AddTemplate(TemplateModel model, AsyncTexture2D icon = null) {
            string    name = string.Empty;

            if (model.TryGetBuildChatLink(out var build)) {
                var prof    = build.Profession;
                var elite   = build.Specialization3Id;
                var isElite = RotationTrainerModule.Instance.RenderService.IsEliteSpec(elite); 
                icon = isElite ? RotationTrainerModule.Instance.RenderService.GetEliteRender(elite) 
                          : RotationTrainerModule.Instance.RenderService.GetProfessionRender(prof); 
                name = isElite ? RotationTrainerModule.Instance.RenderService.GetEliteSpecName(elite)
                               : RotationTrainerModule.Instance.RenderService.GetProfessionName(prof);
            }

            var button = new TemplateButton(model) {
                Parent = this.View.TemplatePanel,
                Size   = new Point(345, 100),
                Icon       = icon ?? ContentService.Textures.TransparentPixel,
                IconSize   = DetailsIconSize.Small,
                Text       = model.Title,
                BottomText = name
            };
            button.PlayClick += delegate {
                RotationTrainerModule.Instance.TemplatePlayer.Play(model.Rotation);
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
