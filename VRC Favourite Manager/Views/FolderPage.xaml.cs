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

            _coreWindow = Window.Current.CoreWindow;
            this.DataContext = _viewModel;
            this.DataContextChanged += MainPage_DataContextChanged;
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
        private void MainPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is FolderPageViewModel viewModel)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
                UpdateVisualState(viewModel.IsRenaming);
            }
        }
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FolderPageViewModel.IsRenaming))
            {
                UpdateVisualState(_viewModel.IsRenaming);
            }
        }
        private void UpdateVisualState(bool isRenaming)
        {
            if (isRenaming)
            {
                VisualStateManager.GoToState(this, "Renaming", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // Check if the Enter key is pressed and IME is not composing
            if (e.Key == Windows.System.VirtualKey.Enter && !IsImeComposing())
            {
                _viewModel.RenameFolder();
                _viewModel.IsRenaming = false;

            }
        }

        private bool IsImeComposing()
        {
            // Check if the IME is currently composing
            return _coreWindow.GetKeyState(Windows.System.VirtualKey.Space).HasFlag(CoreVirtualKeyStates.Down);
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