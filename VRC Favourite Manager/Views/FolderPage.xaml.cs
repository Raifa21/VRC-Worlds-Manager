// FolderPage.xaml.cs

using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Windows.UI.Core;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class FolderPage : Page
    {
        private FolderPageViewModel _viewModel => (FolderPageViewModel)this.DataContext;
        private CoreWindow _coreWindow;
        public FolderPage()
        {
            this.InitializeComponent();
            this.DataContext = _viewModel;
            this.DataContextChanged += FolderPage_DataContextChanged;

            RenameButton.Visibility = Visibility.Collapsed;
        }
        private void FolderPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is FolderPageViewModel viewModel)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
                UpdateVisibility(viewModel.IsRenaming);
            }
        }
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FolderPageViewModel.IsRenaming))
            {
                var viewModel = sender as FolderPageViewModel;
                UpdateVisibility(viewModel.IsRenaming);
            }
        }
        private void UpdateVisibility(bool isRenaming)
        {
            FolderNameTextBlock.Visibility = isRenaming ? Visibility.Collapsed : Visibility.Visible;
            FolderNameTextBox.Visibility = isRenaming ? Visibility.Visible : Visibility.Collapsed;
            if(_viewModel.FolderName != "Unclassified")
            {
                RenameButton.Visibility = isRenaming ? Visibility.Collapsed : Visibility.Visible;
            }
            
        }
        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                _viewModel.RenameFolder(FolderNameTextBox.Text);
                _viewModel.IsRenaming = false;
            }
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