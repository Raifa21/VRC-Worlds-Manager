using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MainPage : Page
    {
        int previousSelectedIndex = 0;

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            LoadFoldersToSelectorBar();
        }

        private void LoadFoldersToSelectorBar()
        {
            var viewModel = (MainViewModel)this.DataContext;

            foreach (var folder in viewModel.Folders)
            {
                SelectorBar2.Items.Add(new SelectorBarItem { Text = folder.Name });
            }
        }

        private void SelectorBar2_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            var selectedItem = sender.SelectedItem as SelectorBarItem;
            int currentSelectedIndex = sender.Items.IndexOf(selectedItem);

            System.Type pageType = typeof(FolderPage); // Default to FolderPage

            // Determine the page type based on the selected folder
            var viewModel = (MainViewModel)this.DataContext;
            if (currentSelectedIndex < viewModel.Folders.Count)
            {
                viewModel.SelectedFolder = viewModel.Folders[currentSelectedIndex];
                pageType = typeof(FolderPage);
            }

            var slideNavigationTransitionEffect =
                currentSelectedIndex - previousSelectedIndex > 0 ?
                    SlideNavigationTransitionEffect.FromRight :
                    SlideNavigationTransitionEffect.FromLeft;

            ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo()
                { Effect = slideNavigationTransitionEffect });

            previousSelectedIndex = currentSelectedIndex;
        }
    }
}