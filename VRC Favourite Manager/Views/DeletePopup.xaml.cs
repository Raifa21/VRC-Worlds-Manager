using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.ViewModels;
using Windows.Networking.Sockets;
using VRC_Favourite_Manager.Models;
using static System.Net.Mime.MediaTypeNames;


namespace VRC_Favourite_Manager.Views
{
    public sealed partial class DeletePopup : ContentDialog
    {
        public string _folderName;
        public DeletePopup(string folderName)
        {
            this.InitializeComponent();
            _folderName = folderName;

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.DeleteWorlds.Text = "フォルダを削除";
                this.ConfirmButton.Content = "削除";
                this.ConfirmMessage.Text = "選択したフォルダを削除しますか？";
                this.ConfirmExplanation.Text = "選択したフォルダは削除されます。この操作は元に戻すことができません。";
            }
            else
            {
                this.DeleteWorlds.Text = "Delete Folder";
                this.ConfirmButton.Content = "Delete";
                this.ConfirmMessage.Text = "Do you want to remove the selected folder?";
                this.ConfirmExplanation.Text = "The selected folder will be removed. This action cannot be undone.";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new DeletePopupViewModel();
            viewModel.Delete_Click(_folderName);
            this.Hide();
        }

    }
}