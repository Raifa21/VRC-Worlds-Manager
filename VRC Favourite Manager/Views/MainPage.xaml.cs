using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using VRC_Favourite_Manager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.Messaging;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MainPage : Page
    {
        int previousSelectedIndex = 0;
        private readonly MainViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;

            NavigateToFolderPage();
            RefreshPage("ja-JP");

            WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("Language changed to " + m.LanguageCode);
                RefreshPage(m.LanguageCode);
            });
            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {

            });
        }

        private void RefreshPage(string languageCode)
        {
            if (languageCode == "ja-JP")
            {
                this.AllWorldsItem.Content = "すべてのワールド";
                this.FoldersItem.Content = "フォルダ";
                this.SettingsItem.Content = "設定";
                this.LogoutItem.Content = "ログアウト";
            }
            else
            {
                this.AllWorldsItem.Content = "All Worlds";
                this.FoldersItem.Content = "Folders";
                this.SettingsItem.Content = "Settings";
                this.LogoutItem.Content = "Logout";
            }
        }


        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer.Tag is string selectedItem)
            {
                switch (selectedItem)
                {
                    case "SettingsPage":
                        ContentFrame.Navigate(typeof(SettingsPage));
                        break;
                    case "AboutPage":
                        //ContentFrame.Navigate(typeof(AboutPage));
                        break;
                    case "Logout":
                        viewModel.LogoutCommand.Execute(null);
                        break;
                }
            }
            else if(args.SelectedItemContainer.Tag is FolderModel selectedFolder)
            {
                viewModel.SelectedFolderChanged(selectedFolder);
                ContentFrame.Navigate(typeof(FolderPage));
            }
        }
        private void NavigateToFolderPage()
        {
            ContentFrame.Navigate(typeof(FolderPage));
        }
    }
}