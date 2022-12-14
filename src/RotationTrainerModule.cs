using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Controls.Intern;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nekres.RotationTrainer.Core.Services;
using Nekres.RotationTrainer.Core.Services.Persistance;
using Nekres.RotationTrainer.Core.UI.Views.HomeTab;
using Nekres.RotationTrainer.Player;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Nekres.RotationTrainer
{
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

        internal SettingEntry<bool> LibraryShowAll;
        internal Dictionary<GuildWarsControls, SettingEntry<KeyBinding>> SkillBindings = new Dictionary<GuildWarsControls, SettingEntry<KeyBinding>>();
        internal SettingEntry<KeyBinding> InteractionBinding;
        internal SettingEntry<KeyBinding> DodgeBinding;

        private Texture2D _cornerTexture;
        private Texture2D _backgroundTexture;
        private CornerIcon _cornerIcon;

        internal TabbedWindow2 Window;
        internal RenderService RenderService;
        internal TemplatePlayer TemplatePlayer;
        internal TemplateReader TemplateReader;

        [ImportingConstructor]
        public RotationTrainerModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(
            moduleParameters)
        {
            Instance = this;
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            var selfManagedSettings = settings.AddSubCollection("ManagedSettings", false, false);
            LibraryShowAll = selfManagedSettings.DefineSetting("LibraryShowAll", false, 
                () => "Show All Templates",
                () => "Show all templates no matter your current profession.");

            var skillBindingSettings = settings.AddSubCollection("Skills", true, false);
            foreach (GuildWarsControls skill in Enum.GetValues(typeof(GuildWarsControls)))
            {
                if (skill == GuildWarsControls.None) continue;
                var friendlyName = Regex.Replace(skill.ToString(), "([A-Z]|[1-9])", " $1").Trim();
                SkillBindings.Add(skill,
                    skillBindingSettings.DefineSetting(skill.ToString(), new KeyBinding(Keys.None) {Enabled = true},
                        () => friendlyName,
                        () => "Your key binding for " + friendlyName));
            }

            InteractionBinding = skillBindingSettings.DefineSetting("InteractionKey", new KeyBinding(Keys.F) {Enabled = true},
                () => "Interact",
                () => "General context-sensitive interact prompt. Used for\ninteracting with the environment, including Talk,\nLoot Revive, etc.");

            DodgeBinding = skillBindingSettings.DefineSetting("DodgeKey", new KeyBinding(Keys.V) {Enabled = true},
                () => "Dodge",
                () => "Do an evasive dodge roll, negating damage, in the\ndirection your character is moving (backward if\nstationary).");
        }

        protected override void Initialize()
        {
            _cornerTexture = ContentsManager.GetTexture("corner_icon.png");
            _backgroundTexture = GameService.Content.GetTexture("controls/window/502049");

            TemplateReader = new TemplateReader();
            TemplatePlayer = new TemplatePlayer();
            RenderService = new RenderService(GetModuleProgressHandler());
        }

        private void UpdateModuleLoading(string loadingMessage)
        {
            if (_cornerIcon == null) return;
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
            var windowRegion = new Rectangle(40, 26, 895 + 38, 780 - 56);
            var contentRegion = new Rectangle(70, 41, 895 - 43, 780 - 142);
            this.Window = new TabbedWindow2(_backgroundTexture, windowRegion, contentRegion)
            {
                Parent = GameService.Graphics.SpriteScreen,
                Emblem = _cornerTexture,
                Location = new Point((GameService.Graphics.SpriteScreen.Width - windowRegion.Width) / 2, (GameService.Graphics.SpriteScreen.Height) / 2),
                SavesPosition = true,
                Title = this.Name,
                Id = $"{nameof(RotationTrainerModule)}_5f6e0f0b-5dc8-4c21-8f74-e4874b9d5822"
            };

            _cornerIcon = new CornerIcon(_cornerTexture, "Special Forces");
            _cornerIcon.Click += OnModuleIconClick;

            this.RenderService.DownloadIcons();

            this.RenderService.LoadingChanged += OnLoadingChanged;
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        private void OnLoadingChanged(object o, ValueEventArgs<bool> e)
        {
            if (e.Value) return;
            this.RenderService.LoadingChanged -= OnLoadingChanged;
            this.Window.Tabs.Add(new Tab(_cornerTexture, () => new HomeView(new HomeModel()), "Home"));
        }

        public void OnModuleIconClick(object o, MouseEventArgs e)
        {
            if (this.RenderService.IsLoading) return;
            this.Window?.ToggleWindow();
        }

        protected override void Update(GameTime gameTime)
        {
        }

        /// <inheritdoc />
        protected override void Unload()
        {
            // Unload
            if (this.RenderService != null)
            {
                this.RenderService.LoadingChanged -= OnLoadingChanged;
                this.RenderService.Dispose();
            }
            if (_cornerIcon != null)
            {
                _cornerIcon.Click -= OnModuleIconClick;
                _cornerIcon.Dispose();
            }
            this.TemplatePlayer?.Dispose();
            this.Window?.Dispose();
            _backgroundTexture?.Dispose();

            // All static members must be manually unset
            Instance = null;
        }
    }
}