using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MultiAddToFolderPopup : ContentDialog
    {
        public MultiAddToFolderPopup(List<WorldModel> selectedWorlds)
        {
            this.InitializeComponent();
            this.DataContext = new MultiAddToFolderPopupViewModel(selectedWorlds);


            if(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.SelectFolders.Text = "フォルダに追加";
                this.SelectedText.Text = "選択されたワールド： ";
                this.SubSelectFoldersText.Text = "フォルダを選択";
                this.ConfirmButton.Content = "追加";
                this.AddFolderButton.Content = "フォルダを追加";
            }
            else
            {
                this.SelectFolders.Text = "Add to Folder";
                this.SelectedText.Text = "Selected Worlds: ";
                this.SubSelectFoldersText.Text = "Select folders";
                this.ConfirmButton.Content = "Confirm";
                this.AddFolderButton.Content = "Add Folder";
            }
            foreach (var world in selectedWorlds)
            {
                this.SelectedText.Text += world.WorldName + ", ";
            }
            //remove the last comma
            this.SelectedText.Text = this.SelectedText.Text.Remove(this.SelectedText.Text.Length - 2);
        }


        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (MultiAddToFolderPopupViewModel)this.DataContext;
            viewModel.CancelSelection();
            this.Hide();
        }

        private void AddFolderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (MultiAddToFolderPopupViewModel)this.DataContext;
            viewModel.AddFolder();
        }

        private void ConfirmButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            var viewModel = (MultiAddToFolderPopupViewModel)this.DataContext;
            viewModel.ConfirmSelection();

            this.Hide();
        }
        private void IndeterminateButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is FolderSelection_Indeterminatable folder)
            {
                folder.IsIndeterminate = false;
                folder.IsChecked = true;
            }
        }
    }
}