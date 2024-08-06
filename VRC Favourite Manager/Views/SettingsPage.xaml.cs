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

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void Language_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                string languageCode = string.Empty;

                switch (radioButton.Content.ToString())
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
                    RefreshPage();
                }
            }
        }

        private void ChangeApplicationLanguage(string languageCode)
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(languageCode));
        }
        private void RefreshPage()
        {
            var frame = Window.Current.Content as Frame;
            if (frame != null)
            {
                frame.Navigate(frame.Content.GetType());
            }
        }
    }
}
