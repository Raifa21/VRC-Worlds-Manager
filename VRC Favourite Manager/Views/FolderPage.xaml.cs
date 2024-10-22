// FolderPage.xaml.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Models;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.UI.Xaml.Media;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class FolderPage : Page
    {
        private FolderPageViewModel _viewModel => (FolderPageViewModel)this.DataContext;
        private List<WorldModel> selectedItems;


        public FolderPage()
        {
            this.InitializeComponent();
            this.DataContext = _viewModel;
            this.DataContextChanged += FolderPage_DataContextChanged;
            
            selectedItems = new List<WorldModel>();

            RenameButton.Visibility = Visibility.Collapsed;

            string languageCode = Application.Current.Resources["languageCode"] as string;

            if (languageCode == "ja")
            {
                this.MultiSelectButton.Content = "‘I‘ð";
                this.MultiSelectButton_Cancel.Content = "ƒLƒƒƒ“ƒZƒ‹";
            }
            else
            {
                this.MultiSelectButton.Content = "Select";
                this.MultiSelectButton_Cancel.Content = "Cancel";
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
        private void FolderPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue is FolderPageViewModel viewModel)
            {
                UpdateVisibility();
            }
        }


        ScrollViewer _singleClickScrollViewer;
        ScrollViewer _multiClickScrollViewer;

        // Sync scroll position when the single select GridView is scrolled
        private void SingleClickGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _singleClickScrollViewer = GetScrollViewer(SingleClickGrid);
            if (_singleClickScrollViewer != null)
            {
                _singleClickScrollViewer.ViewChanged += SingleClickScrollViewer_ViewChanged;
            }
        }

        // Sync scroll position when the multi select GridView is scrolled
        private void MultiClickGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _multiClickScrollViewer = GetScrollViewer(MultiClickGrid);
            if (_multiClickScrollViewer != null)
            {
                _multiClickScrollViewer.ViewChanged += MultiClickScrollViewer_ViewChanged;
            }
        }

        // Update MultiClickGrid's scroll position based on SingleClickGrid's scroll
        private void SingleClickScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_multiClickScrollViewer != null && !e.IsIntermediate)
            {
                _multiClickScrollViewer.ChangeView(_singleClickScrollViewer.HorizontalOffset, _singleClickScrollViewer.VerticalOffset, null);
            }
        }

        // Update SingleClickGrid's scroll position based on MultiClickGrid's scroll
        private void MultiClickScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_singleClickScrollViewer != null && !e.IsIntermediate)
            {
                _singleClickScrollViewer.ChangeView(_multiClickScrollViewer.HorizontalOffset, _multiClickScrollViewer.VerticalOffset, null);
            }
        }

        // Helper function to extract ScrollViewer from GridView
        private ScrollViewer GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer)
            {
                return element as ScrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }


        private void UpdateVisibility()
        {
            if(_viewModel.FolderName != "Unclassified")
            {
                RenameButton.Visibility = Visibility.Visible;
            }
        }
        private void FolderRename_Start(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _viewModel.IsRenaming = true;
        }

        private void FolderRename_Cancel(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _viewModel.RenameCancel();

        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                _viewModel.RenameFolder(FolderNameTextBox.Text);
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
            Log.Information("ViewDetails_Click");

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