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
using Microsoft.UI.Xaml.Navigation;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MainPage : Page
    {
        int previousSelectedIndex = 0;
        private readonly MainViewModel viewModel;
        private object _previouslySelectedItem;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;

            NavigateToAllWorldsPage();
            RefreshPage(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride);

            var folders = viewModel.FoldersNavigationViewItems;
            foreach (var folder in folders)
            {
                FoldersItem.MenuItems.Add(folder);
            }

            GenerateFolders();

            WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("Language changed to " + m.LanguageCode);
                RefreshPage(m.LanguageCode);
                GenerateFolders();
            });
            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("Folder updated");
                GenerateFolders();
            });
        }

        private void RefreshPage(string languageCode)
        {
            if (languageCode == "ja")
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

        private void GenerateFolders()
        {
            var folders = viewModel.GetFoldersNavigationViewItems();
            FoldersItem.MenuItems.Clear();
            foreach (var folder in folders)
            {
                if((string)folder.Content == "Hidden")
                {
                    continue;
                }
                if((string)folder.Content == "Unclassified" && Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
                {
                    folder.Content = "未分類";
                }
                else if((string)folder.Content == "Unclassified" && Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "en")
                {
                    folder.Content = "Unclassified";
                }
                FoldersItem.MenuItems.Add(folder);
                if (folder.IsSelected)
                {
                    NavView.SelectedItem = folder;
                }
            }
        }


        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if( args.SelectedItemContainer == null)
            {
                return;
            }
            if (args.SelectedItemContainer.Tag is string selectedItem)
            {
                switch (selectedItem)
                {
                    case "AllWorldsPage":
                        ContentFrame.Navigate(typeof(AllWorldsPage));
                        break;
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
        private void NavigateToAllWorldsPage()
        {
            ContentFrame.Navigate(typeof(AllWorldsPage));
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Unregister the message subscription
            WeakReferenceMessenger.Default.Unregister<LanguageChangedMessage>(this);
            WeakReferenceMessenger.Default.Unregister<FolderUpdatedMessage>(this);
        }
    }
}