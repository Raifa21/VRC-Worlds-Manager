using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class ResetPopup : ContentDialog
    {
        
        public ResetPopup()
        {
            this.InitializeComponent();
            this.DataContext = new SettingsPageViewModel();

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.ConfirmButton.Content = "リセット";
                this.DeleteWorlds.Text = "フォルダをリセット";
                this.ConfirmMessage.Text = "すべてのデータをリセットしますか？";
                this.ConfirmMessage2.Text = "この操作は取り消せません。";
            }
            else
            {
                this.ConfirmButton.Content = "Confirm";
                this.DeleteWorlds.Text = "Reset Folders";
                this.ConfirmMessage.Text = "Are you sure you want to reset?";
                this.ConfirmMessage2.Text = "This action cannot be undone.";
            }
        }


        private void CloseButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            this.Hide();
        }

        private void ConfirmButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            var viewModel = (SettingsPageViewModel)this.DataContext;
            viewModel.Reset();
            this.Hide();
        }
    }
}