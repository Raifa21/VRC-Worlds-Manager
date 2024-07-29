using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AddToFolderPopup : ContentDialog
    {
        public AddToFolderPopup()
        {
            this.InitializeComponent();
            this.DataContext = new AddToFolderPopupViewModel();
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
    }
}