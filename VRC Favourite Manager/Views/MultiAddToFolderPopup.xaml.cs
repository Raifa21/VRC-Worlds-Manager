using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MultiAddToFolderPopup : ContentDialog
    {
        public MultiAddToFolderPopup(List<WorldModel> selectedWorlds)
        {
            this.InitializeComponent();
            this.DataContext = new MultiAddToFolderPopupViewModel(selectedWorlds);
        }


        private void CloseButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var viewModel = (MultiAddToFolderPopupViewModel)this.DataContext;
            viewModel.CancelSelection();
            this.Hide();
        }

        private void AddFolderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (MultiAddToFolderPopupViewModel)this.DataContext;
            viewModel.AddFolder();
        }

        private void ConfirmButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var viewModel = (MultiAddToFolderPopupViewModel)this.DataContext;
            viewModel.ConfirmSelection();

            this.Hide();
        }
    }
}