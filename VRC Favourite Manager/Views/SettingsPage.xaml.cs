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
using VRC_Favourite_Manager.Common;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            RefreshPage(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride);
            WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("Language changed to " + m.LanguageCode);
                RefreshPage(m.LanguageCode);
            });
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
                        languageCode = "ja-JP";
                        break;
                    case "English":
                        languageCode = "en-US";
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
            if (languageCode == "ja-JP")
            {
                this.SettingsTitle.Text = "設定";
                this.LanguageTitle.Text = "言語";
                this.JapaneseRadioButton.Content = "日本語";
                this.JapaneseRadioButton.IsChecked = true;
                this.EnglishRadioButton.Content = "英語";
                this.ResetButton.Content = "リセット";

            }
            else
            {
                this.SettingsTitle.Text = "Settings";
                this.LanguageTitle.Text = "Language";
                this.JapaneseRadioButton.Content = "Japanese";
                this.EnglishRadioButton.Content = "English";
                this.EnglishRadioButton.IsChecked = true;
                this.ResetButton.Content = "Reset";
            }
        }

        private void ChangeApplicationLanguage(string languageCode)
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(languageCode));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Unregister the message subscription
            WeakReferenceMessenger.Default.Unregister<LanguageChangedMessage>(this);
        }
    }
}
