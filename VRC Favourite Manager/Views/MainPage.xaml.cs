using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using VRC_Favourite_Manager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MainPage : Page
    {
        int previousSelectedIndex = 0;
        private MainViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;
            LoadFoldersToDropDownButton();

            ((INotifyCollectionChanged)viewModel.Folders).CollectionChanged += Folders_CollectionChanged;
        }
        private void Folders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LoadFoldersToDropDownButton();
        }

        private void LoadFoldersToDropDownButton()
        {
            FoldersFlyout.Items.Clear();

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
            var index = (int)menuFlyoutItem.Tag;
            if (index != previousSelectedIndex)
            {
                var folder = viewModel.Folders[index];
                viewModel.SelectedFolder = folder;
                previousSelectedIndex = index;
            }


        }
    }
}