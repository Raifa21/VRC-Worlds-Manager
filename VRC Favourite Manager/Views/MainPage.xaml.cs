using System.Diagnostics;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using VRC_Favourite_Manager.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using VRC_Favourite_Manager.Models;

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
        }
        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer.Tag is string selectedItem)
            {
                Debug.WriteLine(selectedItem);
            }
        }
        private void NavigateToFolderPage()
        {
            ContentFrame.Navigate(typeof(FolderPage));
        }
    }
}