using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AddWorldPopup : ContentDialog
    {
        private readonly FolderPageViewModel _viewModel;

        public AddWorldPopup(FolderPageViewModel viewModel)
        {
            this.InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;
            string languageCode = Application.Current.Resources["languageCode"] as string;
            AddWorld.Text = languageCode == "ja" ? "ワールドを追加" : "Add World";
            AddWorldText.Text = languageCode == "ja" ? "追加したいワールドのURLを入力してください。" : "Please enter the URL of the world to add.";
            ConfirmButton.Content = languageCode == "ja" ? "追加" : "Add";

        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = InputTextBox.Text;

            if (!string.IsNullOrEmpty(folderName))
            {
                //sanitize input using UserInputHandler in Services
                var _userInputHandler = new UserInputHandler();
                folderName = _userInputHandler.SanitizeUserInput(folderName);

                folderName = folderName.Replace("https://vrchat.com/home/world/", "");
                folderName = folderName.Replace("https://vrchat.com/home/launch?worldId=", "");
                _viewModel.AddWorld(folderName);
            }
            this.Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }

}