using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class ImportFolderPopup : ContentDialog
    {
        private readonly MainViewModel _viewModel;

        public ImportFolderPopup(MainViewModel viewModel)
        {
            this.InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;
            string languageCode = Application.Current.Resources["languageCode"] as string;
            AddWorld.Text = languageCode == "ja" ? "�t�H���_���C���|�[�g" : "Import Folder";
            ShareFolderText.Text = languageCode == "ja" ? "�R�[�h���g�p���ăt�H���_���C���|�[�g���܂��傤�B" : "Paste the code to import a folder.";
            ConfirmButton.Content = languageCode == "ja" ? "�C���|�[�g" : "Import";
            ErrorText.Text = languageCode == "ja" ? "�����ȃR�[�h�ł��B" : "Invalid code.";
            ErrorText.Visibility = Visibility.Collapsed;
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderCode = CodeTextBox.Text;

                if (!string.IsNullOrEmpty(folderCode))
                {
                    _viewModel.ImportFolder(folderCode);
                }
                this.Hide();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}