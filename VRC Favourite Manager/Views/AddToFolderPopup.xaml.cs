using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AddToFolderPopup : ContentDialog
    {
        public AddToFolderPopup(ObservableCollection<FolderModel> folders, WorldModel selectedWorld)
        {
            this.InitializeComponent();
            this.DataContext = new AddToFolderPopupViewModel(folders, selectedWorld);
        }


        private void CloseButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
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
            var selectedFolders = viewModel.GetSelectedFolders();

            // Update the selected world folder property
            viewModel.SelectedWorld.Folder = selectedFolders;

            this.Hide();
        }
    }
}