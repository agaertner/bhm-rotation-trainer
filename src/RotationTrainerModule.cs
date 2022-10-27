using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nekres.RotationTrainer.Core.Services;
using Nekres.RotationTrainer.Core.UI.Views;
using Nekres.RotationTrainer.Player;
using Nekres.RotationTrainer.Player.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Nekres.RotationTrainer {
    [Export(typeof(Module))]
    public class RotationTrainerModule : Module
    {
        internal static readonly Logger Logger = Logger.GetLogger(typeof(RotationTrainerModule));

        internal static RotationTrainerModule Instance;

        #region Service Managers

        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

        #endregion

        internal SettingEntry<bool>                                    LibraryShowAll;
        internal Dictionary<GuildWarsAction, SettingEntry<KeyBinding>> ActionBindings;

        private  Texture2D      _cornerTexture;
        private  Texture2D      _backgroundTexture;
        private  CornerIcon     _cornerIcon;
        internal Texture2D      EditTexture;

        private  StandardWindow _moduleWindow;

        internal DataService    DataService { get; set; }
        internal RenderService  RenderService;

        internal TemplatePlayer TemplatePlayer;

        [ImportingConstructor]
        public RotationTrainerModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(
            moduleParameters)
        {
            ActionBindings = new Dictionary<GuildWarsAction, SettingEntry<KeyBinding>>();
            Instance = this;
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            var selfManagedSettings = settings.AddSubCollection("ManagedSettings", false, false);
            LibraryShowAll = selfManagedSettings.DefineSetting("LibraryShowAll", false, 
                () => "Show All Templates",
                () => "Show all templates no matter your current profession.");

            var skillBindingSettings        = settings.AddSubCollection("Skills");
            skillBindingSettings.RenderInUi = true;

            foreach (GuildWarsAction skill in Enum.GetValues(typeof(GuildWarsAction)))
            {
                if (skill == GuildWarsAction.None) {
                    continue;
                }

                string friendlyName = skill.ToFriendlyString();
                ActionBindings.Add(skill, skillBindingSettings.DefineSetting(skill.ToString(), new KeyBinding(Keys.None) {Enabled = true},
                                                                            () => friendlyName,
                                                                            () => "Your key binding for " + friendlyName));
            }
        }

        protected override void Initialize()
        {
            _cornerTexture     = ContentsManager.GetTexture("corner_icon.png");
            _backgroundTexture = ContentsManager.GetTexture("background.png");
            this.EditTexture   = ContentService.Textures.TransparentPixel;
            DataService        = new DataService(this.DirectoriesManager.GetFullDirectoryPath("rotation_trainer"));

            TemplatePlayer = new TemplatePlayer();
            RenderService  = new RenderService(GetModuleProgressHandler());
        }

        private void UpdateModuleLoading(string loadingMessage)
        {
            if (_cornerIcon == null) {
                return;
            }
            _cornerIcon.LoadingMessage = loadingMessage;
        }

        public IProgress<string> GetModuleProgressHandler()
        {
            return new Progress<string>(UpdateModuleLoading);
        }

        protected override async Task LoadAsync()
        {
        }

        protected override void OnModuleLoaded(EventArgs e)
        {
            var windowRegion  = new Rectangle(40, 26, 423, 780 - 56);
            var contentRegion = new Rectangle(70, 41, 380, 780 - 42);
            _moduleWindow = new StandardWindow(_backgroundTexture, windowRegion, contentRegion)
            {
                Parent = GameService.Graphics.SpriteScreen,
                Emblem = _cornerTexture,
                Location = new Point((GameService.Graphics.SpriteScreen.Width - windowRegion.Width) / 2, (GameService.Graphics.SpriteScreen.Height) / 2),
                SavesPosition = true,
                Title = this.Name,
                Id = $"{nameof(RotationTrainerModule)}_0c60c5e9-02bd-4098-8050-287c6a2cba5d"
            };

            _cornerIcon = new CornerIcon(_cornerTexture, this.Name);
            _cornerIcon.Click += OnModuleIconClick;

            this.RenderService.DownloadIcons();
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        public void OnModuleIconClick(object o, MouseEventArgs e)
        {
            if (this.RenderService.IsLoading) {
                ScreenNotification.ShowNotification("Loading. Please wait a few seconds and try again.");
                return;
            }
            _moduleWindow.ToggleWindow(new LibraryView(new LibraryModel()));
        }

        protected override void Update(GameTime gameTime)
        {
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            // Unload
            this.RenderService?.Dispose();
            if (_cornerIcon != null)
            {
                _cornerIcon.Click -= OnModuleIconClick;
                _cornerIcon.Dispose();
            }

            this._moduleWindow?.Dispose();
            _backgroundTexture?.Dispose();

            // All static members must be manually unset
            Instance = null;
        }
    }
}