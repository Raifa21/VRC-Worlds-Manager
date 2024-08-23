using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class RemovePopup : ContentDialog
    {
        
        public RemovePopup(List<WorldModel> selectedWorlds, string folderName)
        {
            this.InitializeComponent();
            this.DataContext = new RemovePopupViewModel(selectedWorlds, folderName);
            var text = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "選択されたワールド： " : "Selected Worlds: ";
            foreach (var selectedWorld in selectedWorlds)
            {
                text += selectedWorld.WorldName + ", ";
            }
            text = text.Substring(0, text.Length - 2);

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.ConfirmButton.Content = "削除";
                this.DeleteWorlds.Text = "ワールドを削除";
                this.SelectedWorld.Text = text;
                this.ConfirmMessage.Text = "選択したワールドを削除しますか？";
                this.ConfirmExplanation.Text = "選択されたワールドはすべてのフォルダから削除されます。削除されたワールドは設定＞非表示からアクセスできます。";
            }
            else
            {
                this.ConfirmButton.Content = "Confirm";
                this.DeleteWorlds.Text = "Delete Worlds";
                this.SelectedWorld.Text = text;
                this.ConfirmMessage.Text = "Do you want to remove the selected world(s)?";
                this.ConfirmExplanation.Text = "The selected world(s) will be removed from all folders. Removed worlds can be accessed from Settings > Hidden Worlds.";
            }
        }


        private void CloseButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            this.Hide();
        }

        private void ConfirmButton_Click(object o, RoutedEventArgs routedEventArgs)
        {
            var viewModel = (RemovePopupViewModel)this.DataContext;
            viewModel.RemoveFromFolder();
            this.Hide();
        }
    }
}