using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Documents;
using VRC_Favourite_Manager.Common;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            string languageCode = Application.Current.Resources["languageCode"] as string;

            RefreshPage(languageCode);
        }

        private void Language_Checked(object sender, RoutedEventArgs e)
        {
            var _configManager = new ConfigManager();
            if (sender is RadioButton radioButton)
            {
                string languageCode = string.Empty;

                switch (radioButton.Tag.ToString())
                {
                    case "Japanese":
                        languageCode = "ja";
                        break;
                    case "English":
                        languageCode = "en";
                        break;
                }

                if (!string.IsNullOrEmpty(languageCode))
                {
                    ChangeApplicationLanguage(languageCode);
                }
            }

        }

        private async void RefreshPage(string languageCode)
        {
            if (this.SettingsTitle == null)
            {
                await Task.Delay(100);
            }
            if (languageCode == "ja")
            {
                this.SettingsTitle.Text = "設定";
                this.LanguageTitle.Text = "言語";
                this.JapaneseRadioButton.Content = "日本語";
                this.EnglishRadioButton.Content = "英語";
                this.JapaneseRadioButton.IsChecked = true;
                this.EnglishRadioButton.IsChecked = false;
                this.LookingForTranslators.Text = "他言語の対応は後日追加予定です。対応言語の要望は";
                this.HyperlinkText.Text = "こちら";
                this.WorldManagementTitle.Text = "ワールド管理";
                this.HiddenFolder.Content = "非表示フォルダ";
                this.ResetButton.Content = "リセット";
            }
            else
            {
                this.SettingsTitle.Text = "Settings";
                this.LanguageTitle.Text = "Language";
                this.JapaneseRadioButton.Content = "Japanese";
                this.EnglishRadioButton.Content = "English";
                this.EnglishRadioButton.IsChecked = true;
                this.JapaneseRadioButton.IsChecked = false;
                this.LookingForTranslators.Text = "Support for other languages will be added later. Requests for supported languages can be made";
                this.HyperlinkText.Text = "here.";
                this.WorldManagementTitle.Text = "Manage Worlds";
                this.HiddenFolder.Content = "Hidden Folder"; 
                this.ResetButton.Content = "Reset";
            }
        }

        private void ChangeApplicationLanguage(string languageCode)
        { 
            Application.Current.Resources["languageCode"] = languageCode;
            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(languageCode));
            RefreshPage(languageCode);
        }

        private void HiddenFolder_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HiddenFolderPage));
        }

        private void ResetButton_Clicked(object sender, RoutedEventArgs e)
        {
            var resetPopup = new ResetPopup();
            resetPopup.XamlRoot = this.Content.XamlRoot;
            resetPopup.ShowAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Unregister the message subscription
            WeakReferenceMessenger.Default.Unregister<LanguageChangedMessage>(this);
        }

        private void Hyperlink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            // Launch the URI
            Windows.System.Launcher.LaunchUriAsync(new Uri("https://forms.gle/vDGEFjz9PQJaHbxk8"));
        }
    }
}
