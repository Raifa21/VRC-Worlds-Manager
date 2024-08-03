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
            _coreWindow = Window.Current.CoreWindow;
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