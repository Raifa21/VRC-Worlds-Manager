using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
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

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.SelectFolders.Text = "フォルダに追加";
                this.SubSelectFoldersText.Text = "フォルダを選択";
                this.ConfirmButton.Content = "追加";
                this.AddFolderButton.Content = "フォルダを追加";
            }
            else
            {
                this.SelectFolders.Text = "Add to Folder";
                this.SubSelectFoldersText.Text = "Select folders";
                this.ConfirmButton.Content = "Confirm";
                this.AddFolderButton.Content = "Add Folder";
            }
        }


        private void CloseButton_Click(object o, RoutedEventArgs routedEventArgs)
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

        private void ConfirmButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            var viewModel = (AddToFolderPopupViewModel)this.DataContext;
            viewModel.ConfirmSelection();

            this.Hide();
        }
    }
}