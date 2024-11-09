using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using Windows.ApplicationModel.DataTransfer;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class ShareFolderPopup : ContentDialog
    {
        private readonly FolderPageViewModel _viewModel;

        public ShareFolderPopup(FolderPageViewModel viewModel)
        {
            this.InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;
            string languageCode = Application.Current.Resources["languageCode"] as string;
            AddWorld.Text = languageCode == "ja" ? "�t�H���_�����L" : "Share Folder";
            ShareFolderText.Text = languageCode == "ja" ? "���L�̃R�[�h�����L���邱�Ƃő��̃��[�U�[�Ƃ��̃t�H���_�����L�ł��܂��B" : "Copy and share the code below to share this folder with others.";
            CopiedText.Text = languageCode == "ja" ? "�N���b�v�{�[�h�ɕۑ�����܂���" : "Copied to clipboard";
            GenerateShareCode();

        }
    
        private void GenerateShareCode()
        {
            string shareCode = _viewModel.GenerateShareCode();
            CopyTextBox.Text = shareCode;
            CopyTextBox.SelectAll();
        }

        private void CopyTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CopyTextBox.SelectAll();
            // Copy to clipboard
            var package = new DataPackage();
            package.SetText(CopyTextBox.Text);
            Clipboard.SetContent(package);

            CopiedText.Visibility = Visibility.Visible;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                CopiedText.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }

}