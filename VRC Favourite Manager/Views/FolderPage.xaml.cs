// FolderPage.xaml.cs

using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Input;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class FolderPage : Page
    {
        private readonly FolderPageViewModel _viewModel;
        public FolderPage()
        {
            this.InitializeComponent();
            _viewModel = new FolderPageViewModel();
            this.DataContext = _viewModel;
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is WorldModel selectedWorld)
            {
                var dialog = new WorldDetailsPopup(selectedWorld)
                {
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private void ViewDetails_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                var dialog = new WorldDetailsPopup(selectedWorld)
                {
                    XamlRoot = this.XamlRoot
                };
                dialog.ShowAsync();
            }
        }

        private async void MoveToAnotherFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                var addToFolderPopup = new AddToFolderPopup(selectedWorld)
                {
                    XamlRoot = this.Content.XamlRoot
                };
                await addToFolderPopup.ShowAsync();
            }
        }

        private void Remove_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                _viewModel.RemoveFromFolder(selectedWorld);
            }
        }
    }
}