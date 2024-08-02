using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AddToFolderPopup : ContentDialog
    {
        public AddToFolderPopup(WorldModel selectedWorld)
        {
            this.InitializeComponent();
            this.DataContext = new AddToFolderPopupViewModel(selectedWorld);
        }


        private void CloseButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var viewModel = (AddToFolderPopupViewModel)this.DataContext;
            viewModel.CancelSelection();
            this.Hide();
        }

        private void AddFolderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (AddToFolderPopupViewModel)this.DataContext;
            viewModel.AddFolder();
        }

        private void ConfirmButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var viewModel = (AddToFolderPopupViewModel)this.DataContext;
            viewModel.ConfirmSelection();

            this.Hide();
        }
    }
}