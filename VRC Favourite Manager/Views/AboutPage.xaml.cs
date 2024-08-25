using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Documents;
using VRC_Favourite_Manager.Common;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
            }
            
        }
    }
}
