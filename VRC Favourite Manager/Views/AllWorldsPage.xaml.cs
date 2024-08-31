// FolderPage.xaml.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AllWorldsPage : Page
    {
        private AllWorldsPageViewModel _viewModel => (AllWorldsPageViewModel)this.DataContext;
        private List<WorldModel> selectedItems;
        private string folderName;


        public AllWorldsPage()
        {
            this.InitializeComponent();
            this.DataContext = _viewModel;
            
            selectedItems = new List<WorldModel>();
            MultiClickGrid.Visibility = Visibility.Collapsed;

            string languageCode = Application.Current.Resources["languageCode"] as string;

            if (languageCode == "ja")
            {
                this.MultiSelectButton.Content = "選択";
                this.MultiSelectButton_Cancel.Content = "キャンセル";
                this.FolderNameTextBlock.Text = "ワールド一覧";
            }
            else
            {
                this.MultiSelectButton.Content = "Select";
                this.MultiSelectButton_Cancel.Content = "Cancel";
                this.FolderNameTextBlock.Text = "All Worlds";
            }
        }

        private async void Refresh_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.RefreshButton.IsEnabled = false;

                List<Task> tasks = new List<Task>();
                tasks.Add(_viewModel.RefreshWorldsAsync());
                tasks.Add(Task.Delay(5000));
                await Task.WhenAll(tasks);
            }
            finally
            {
                this.RefreshButton.IsEnabled = true;
            }
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is WorldModel selectedWorld)
            {
                var dialog = new WorldDetailsPopup(selectedWorld)
                {
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItems = MultiClickGrid.SelectedItems.Cast<WorldModel>().ToList();
        }

        private void GridView_ClearSelection(object sender, RoutedEventArgs e)
        {
            MultiClickGrid.SelectedItems.Clear();
            selectedItems.Clear();
        }


        private void ViewDetails_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Debug.WriteLine("ViewDetails_Click");

            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                var dialog = new WorldDetailsPopup(selectedWorld)
                {
                    XamlRoot = this.XamlRoot
                };
                dialog.ShowAsync();
            }
        }


        private async void MoveToAnotherFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                var addToFolderPopup = new AddToFolderPopup(selectedWorld)
                {
                    XamlRoot = this.Content.XamlRoot
                };
                await addToFolderPopup.ShowAsync();
            }
        }
        private async void MultiMoveToAnotherFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if(sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                if(selectedItems.Count <= 1)
                {
                    var addToFolderPopup = new AddToFolderPopup(selectedWorld)
                    {
                        XamlRoot = this.Content.XamlRoot
                    };
                    await addToFolderPopup.ShowAsync();
                }
                else
                {
                    var addToFolderPopup = new MultiAddToFolderPopup(selectedItems)
                    {
                        XamlRoot = this.Content.XamlRoot
                    };
                    await addToFolderPopup.ShowAsync();
                }
            }
        }

        private async void Remove_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: WorldModel selectedWorld })
            {
                var selectedWorldList = new List<WorldModel> { selectedWorld };
                var removePopup = new RemovePopup(selectedWorldList)
                {
                    XamlRoot = this.Content.XamlRoot
                };
                await removePopup.ShowAsync();
            }
        }

        private async void MultiRemove_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var removePopup = new RemovePopup(selectedItems)
            {
                XamlRoot = this.Content.XamlRoot
            };
            await removePopup.ShowAsync();
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel.Dispose();
        }

    }
}