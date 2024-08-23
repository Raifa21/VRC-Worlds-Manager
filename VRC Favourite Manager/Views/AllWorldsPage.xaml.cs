// FolderPage.xaml.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using Windows.UI.Core;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Navigation;
using VRC_Favourite_Manager.Common;
using System.Linq;
using VRChat.API.Model;
using Microsoft.UI.Xaml.Controls.Primitives;

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

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.MultiSelectButton.Content = "選択";
                this.MultiSelectButton_Cancel.Content = "キャンセル";
                this.FolderNameTextBlock.Text = "ワールド一覧";
            }
            else
            {
                this.MultiSelectButton.Content = "Select";
                this.MultiSelectButton_Cancel.Content = "Cancel";
                this.FolderNameTextBlock.Text = "Worlds";
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