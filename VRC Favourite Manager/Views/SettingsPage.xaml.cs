using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using Microsoft.UI.Xaml.Documents;
using VRC_Favourite_Manager.Common;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            RefreshPage(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride);
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

        private void RefreshPage(string languageCode)
        {
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
                this.HiddenTitle.Text = "非表示フォルダ";
                this.HiddenFolder.Content = "非表示";
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
                this.LookingForTranslators.Text = "Support for other languages will be added later. Requests for supported languages can be made ";
                this.HyperlinkText.Text = "here";
                this.HiddenTitle.Text = "Hidden Folder";
                this.HiddenFolder.Content = "Hidden";
                this.ResetButton.Content = "Reset";
            }
        }

        private void ChangeApplicationLanguage(string languageCode)
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(languageCode));
            RefreshPage(languageCode);
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
