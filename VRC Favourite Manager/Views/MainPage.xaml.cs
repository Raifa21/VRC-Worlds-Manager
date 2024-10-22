using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using Serilog;
using Microsoft.UI.Xaml.Media.Imaging;

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
            string languageCode = Application.Current.Resources["languageCode"] as string;

            RefreshPage(languageCode);

            var folders = viewModel.FoldersNavigationViewItems;
            foreach (var folder in folders)
            {
                FoldersItem.MenuItems.Add(folder);
            }
            string exeFolder = AppContext.BaseDirectory;
            Log.Information("Exe folder: " + exeFolder);
            string svgPath = Path.Combine(exeFolder, "Icons", "Saturn.svg");
            Log.Information("SVG path: " + svgPath);
            var svgImageSource = new SvgImageSource(new Uri(svgPath));
            Saturn.Source = svgImageSource;

            string gearPath = Path.Combine(exeFolder, "Icons", "Gear.svg");
            var gearImageSource = new SvgImageSource(new Uri(gearPath));
            Gear.Source = gearImageSource;

            string logoutPath = Path.Combine(exeFolder, "Icons", "Logout.svg");
            var logoutImageSource = new SvgImageSource(new Uri(logoutPath));
            Logout.Source = logoutImageSource;



            GenerateFolders();
            foreach(var folder in folders)
            {
                if(folder.IsSelected)
                {
                    NavView.SelectedItem = folder;
                }
            }
            

            WeakReferenceMessenger.Default.Register<LanguageChangedMessage>(this, (r, m) =>
            {
                Log.Information("Language changed to " + m.LanguageCode);
                RefreshPage(m.LanguageCode);
                GenerateFolders();
            });
            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Log.Information("Folder updated");
                GenerateFolders();
            });
        }

        private void RefreshPage(string languageCode)
        {
            try
            {
                if (languageCode == "ja")
                {
                    this.AllWorldsItem.Content = "ワールド一覧";
                    this.FoldersItem.Content = "フォルダ";
                    this.AboutItem.Content = "このアプリについて";
                    this.SettingsItem.Content = "設定";
                    this.LogoutItem.Content = "ログアウト";
                }
                else
                {
                    this.AllWorldsItem.Content = "All Worlds";
                    this.FoldersItem.Content = "Folders";
                    this.AboutItem.Content = "About";
                    this.SettingsItem.Content = "Settings";
                    this.LogoutItem.Content = "Logout";
                }
            }
            catch (System.Exception)
            {
                Log.Information("Error refreshing page.");
            }
        }

        private void GenerateFolders()
        {
            var folders = viewModel.GetFoldersNavigationViewItems();
            FoldersItem.MenuItems.Clear();
            foreach (var folder in folders)
            {
                string languageCode = Application.Current.Resources["languageCode"] as string;

                if ((string)folder.Content == "Hidden")
                {
                    continue;
                }
                if((string)folder.Content == "Unclassified" && languageCode == "ja")
                {
                    folder.Content = "未分類";
                }
                else if((string)folder.Content == "Unclassified" && languageCode == "en")
                {
                    folder.Content = "Unclassified";
                }
                else
                {
                    MenuFlyout flyout = new MenuFlyout();
                    string folderName = folder.Content as string;

                    MenuFlyoutItem delete = new MenuFlyoutItem
                    {
                        Text = "Delete folder",
                        Tag = folderName
                    };
                    delete.Click += Delete_Click;

                    flyout.Items.Add(delete);

                    folder.ContextFlyout = flyout;
                }

                FoldersItem.MenuItems.Add(folder);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            var folderName = menuItem?.Tag as string;

            if (!string.IsNullOrEmpty(folderName))
            {
                var deletePopup = new DeletePopup(folderName);
                deletePopup.XamlRoot = this.Content.XamlRoot;
                await deletePopup.ShowAsync();
                GenerateFolders();
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
                        Log.Information("SettingsPage");
                        ContentFrame.Navigate(typeof(SettingsPage));
                        break;
                    case "AboutPage":
                        ContentFrame.Navigate(typeof(AboutPage));
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