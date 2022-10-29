using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Nekres.RotationTrainer.Core.UI.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nekres.RotationTrainer.Core.UI.Views {
    internal class LibraryView : View<LibraryPresenter>
    {
        internal event EventHandler<EventArgs> AddNewClick;

        private const int    MARGIN_BOTTOM = 10;
        private const string FILTER_ALL    = "All";
        private const string FILTER_BUILD_ID   = "Game Version";

        public FlowPanel TemplatePanel;

        public LibraryView(LibraryModel model)
        {
            this.WithPresenter(new LibraryPresenter(this, model));
            RotationTrainerModule.Instance.DataService.TemplateDeleted += OnTemplateDeleted;
        }

        protected override void Unload() {
            RotationTrainerModule.Instance.DataService.TemplateDeleted -= OnTemplateDeleted;
            base.Unload();
        }

        protected override async Task<bool> Load(IProgress<string> progress)
        {
            return !RotationTrainerModule.Instance.RenderService.IsLoading;
        }

        private void OnTemplateDeleted(object o, ValueEventArgs<Guid> e) {
            var ctrl = this.TemplatePanel?.Children
                           .Where(x => x.GetType() == typeof(TemplateButton))
                           .Cast<TemplateButton>().FirstOrDefault(x => x.TemplateModel.Id.Equals(e.Value));
            ctrl?.Dispose();
        }

        protected override void Build(Container buildPanel)
        {
            var searchBar = new TextBox {
                Parent          = buildPanel,
                MaxLength       = 256,
                Location        = new Point(0,   5),
                Size            = new Point(150, 32),
                PlaceholderText = "Search..."
            };
            searchBar.EnterPressed += OnSearchFilterChanged;
            // Sort drop down
            var ddSortMethod = new Dropdown {
                Parent   = buildPanel,
                Location = new Point(buildPanel.ContentRegion.Width - 5 - 150, 5),
                Width    = 150
            };
            ddSortMethod.Items.Add(FILTER_ALL);
            ddSortMethod.Items.Add(FILTER_BUILD_ID);
            ddSortMethod.SelectedItem =  FILTER_BUILD_ID;
            ddSortMethod.ValueChanged += OnSortChanged;

            this.TemplatePanel = new FlowPanel
            {
                Parent         = buildPanel,
                Size           = new Point(buildPanel.ContentRegion.Width - 10, buildPanel.ContentRegion.Height - 150),
                Location       = new Point(0,                                   ddSortMethod.Bottom             + MARGIN_BOTTOM),
                FlowDirection  = ControlFlowDirection.LeftToRight,
                ControlPadding = new Vector2(5, 5),
                CanCollapse    = false,
                CanScroll      = true,
                Collapsed      = false,
                ShowTint       = true,
                ShowBorder     = true
            };

            var btnAddNew = new StandardButton {
                Parent   = buildPanel,
                Location = new Point((buildPanel.ContentRegion.Width - 100) / 2, this.TemplatePanel.Bottom + MARGIN_BOTTOM),
                Size     = new Point(100,                                        35),
                Text     = "Add Template"
            };
            btnAddNew.Click += BtnAddNew_Click;

            foreach (var template in this.Presenter.Model.TemplateModels)
            {
                this.Presenter.AddTemplate(template);
            }

            base.Build(buildPanel);
        }

        private void BtnAddNew_Click(object o, MouseEventArgs e) {
            AddNewClick?.Invoke(this, EventArgs.Empty);
        }

        private void OnSearchFilterChanged(object o, EventArgs e) {
            string text = ((TextBox)o).Text;
            text = string.IsNullOrEmpty(text) ? text : text.ToLowerInvariant();
            this.TemplatePanel.SortChildren<TemplateButton>((x, y) => {
                x.Visible = string.IsNullOrEmpty(text) || x.Title.ToLowerInvariant().Contains(text);
                y.Visible = string.IsNullOrEmpty(text) || y.Title.ToLowerInvariant().Contains(text);
                if (!x.Visible || !y.Visible) {
                    return 0;
                }

                return string.Compare(x.Title, y.Title, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        private void OnSortChanged(object o, ValueChangedEventArgs e) {
            string filter = ((Dropdown)o).SelectedItem;
            this.TemplatePanel.SortChildren<TemplateButton>((x, y) => {
                x.Visible = filter.Equals(FILTER_ALL) || x.TemplateModel.ClientBuildId.Equals(GameService.Gw2Mumble.Info.BuildId);
                y.Visible = filter.Equals(FILTER_ALL) || y.TemplateModel.ClientBuildId.Equals(GameService.Gw2Mumble.Info.BuildId); ;
                if (!x.Visible || !y.Visible) {
                    return 0;
                }
                return string.Compare(x.Title, y.Title, StringComparison.InvariantCultureIgnoreCase);
            });
        }
    }
}
