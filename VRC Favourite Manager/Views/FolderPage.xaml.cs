// FolderPage.xaml.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Windows.UI.Core;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Navigation;
using VRC_Favourite_Manager.Common;
using System.Linq;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class FolderPage : Page
    {
        private FolderPageViewModel _viewModel => (FolderPageViewModel)this.DataContext;
        private List<WorldModel> selectedItems;
        private string folderName;
        public FolderPage()
        {
            this.InitializeComponent();
            this.DataContext = _viewModel;
            this.DataContextChanged += FolderPage_DataContextChanged;
            
            selectedItems = new List<WorldModel>();

            RenameButton.Visibility = Visibility.Collapsed;
            MultiClickGrid.Visibility = Visibility.Collapsed;
        }
        private void FolderPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is FolderPageViewModel viewModel)
            {
                UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            if(_viewModel.FolderName != "Unclassified")
            {
                RenameButton.Visibility = Visibility.Visible;
            }
        }
        private void FolderRename_Start(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _viewModel.IsRenaming = true;
            folderName = _viewModel.FolderName;
        }

        private void FolderRename_Cancel(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _viewModel.IsRenaming = false;
            _viewModel.FolderName = folderName;
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
        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItems = MultiClickGrid.SelectedItems.Cast<WorldModel>().ToList();
        }

        private void GridView_ClearSelection(object sender, RoutedEventArgs e)
        {
            MultiClickGrid.SelectedItems.Clear();
            selectedItems.Clear();
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
        private async void MultiMoveToAnotherFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                if(selectedItems.Count <= 1)
                {
                    var addToFolderPopup = new AddToFolderPopup(selectedWorld)
                    {
                        XamlRoot = this.Content.XamlRoot
                    };
                    await addToFolderPopup.ShowAsync();
                }
                else
                {
                    var addToFolderPopup = new MultiAddToFolderPopup(selectedItems)
                    {
                        XamlRoot = this.Content.XamlRoot
                    };
                    await addToFolderPopup.ShowAsync();
                }
            }
        }

        private void Remove_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                _viewModel.RemoveFromFolder(selectedWorld);
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.Dispose();
        }

    }
}