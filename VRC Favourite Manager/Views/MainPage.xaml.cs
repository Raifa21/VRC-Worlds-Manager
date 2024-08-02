using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using VRC_Favourite_Manager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MainPage : Page
    {
        int previousSelectedIndex = 0;
        private readonly MainViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;
            LoadFoldersToDropDownButton();

            ((INotifyCollectionChanged)viewModel.Folders).CollectionChanged += Folders_CollectionChanged;

            NavigateToFolderPage();
        }
        private void Folders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LoadFoldersToDropDownButton();
        }

        private void LoadFoldersToDropDownButton()
        {
            FoldersFlyout.Items.Clear();

            foreach (var folder in viewModel.Folders)
            {
                var menuFlyoutItem = new MenuFlyoutItem { Text = folder.Name, Tag = folder };
                menuFlyoutItem.Click += MenuFlyoutItem_Click;
                FoldersFlyout.Items.Add(menuFlyoutItem);
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            if (menuItem != null)
            {
                var selectedFolder = menuItem.Tag as FolderModel;
                if (selectedFolder != null)
                {
                    viewModel.SelectedFolder = selectedFolder;
                }
            }
        }
        private void NavigateToFolderPage()
        {
            ContentFrame.Navigate(typeof(FolderPage));
        }
    }
}