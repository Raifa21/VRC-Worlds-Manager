using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using VRC_Favourite_Manager.ViewModels;
using Microsoft.UI.Xaml;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MainPage : Page
    {
        int previousSelectedIndex = 0;

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            LoadFoldersToDropDownButton();
        }

        private void LoadFoldersToDropDownButton()
        {
            var viewModel = (MainViewModel)this.DataContext;

            for (int i = 0; i < viewModel.Folders.Count; i++)
            {
                var folder = viewModel.Folders[i];
                var menuFlyoutItem = new MenuFlyoutItem { Text = folder.Name, Tag = i };
                menuFlyoutItem.Click += MenuFlyoutItem_Click;
                FoldersFlyout.Items.Add(menuFlyoutItem);
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var menuFlyoutItem = sender as MenuFlyoutItem;
            int selectedIndex = (int)menuFlyoutItem.Tag;
            var viewModel = (MainViewModel)this.DataContext;
            Debug.WriteLine($"Selected folder: {viewModel.Folders[selectedIndex].Name}");
            foreach (var folder in viewModel.Folders)
            {
                Debug.WriteLine(folder.Name);
            }
            if (selectedIndex < viewModel.Folders.Count)
            {
                viewModel.SelectedFolder = viewModel.Folders[selectedIndex];
            }

            var slideNavigationTransitionEffect =
                selectedIndex - previousSelectedIndex > 0 ?
                    SlideNavigationTransitionEffect.FromRight :
                    SlideNavigationTransitionEffect.FromLeft;

            ContentFrame.Navigate(typeof(FolderPage), null, new SlideNavigationTransitionInfo()
                { Effect = slideNavigationTransitionEffect });

            previousSelectedIndex = selectedIndex;
        }
    }
}