using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class RemovePopup : ContentDialog
    {

        public RemovePopup(WorldModel selectedWorld, string folderName)
        {
            this.InitializeComponent();
            this.DataContext = new RemovePopupViewModel(selectedWorld, folderName);

            if (Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.ConfirmButton.Content = "削除";
                this.DeleteWorlds.Text = "ワールドを削除";
                this.SelectedWorld.Text = "選択されたワールド： " + selectedWorld.WorldName;
                this.ConfirmMessage.Text = "選択したワールドをフォルダから削除しますか？";
                this.ConfirmExplanation.Text = "削除されたワールドは「未分類」フォルダに移動されます。未分類フォルダから削除されたワールドは設定＞からアクセスできます。";
            }
            else
            {
                this.ConfirmButton.Content = "Confirm";
                this.DeleteWorlds.Text = "Delete Worlds";
                this.SelectedWorld.Text = "Selected World: " + selectedWorld.WorldName;
                this.ConfirmMessage.Text = "Do you want to remove the selected world from the folder?";
                this.ConfirmExplanation.Text = "Removed worlds will be moved to the 'Unclassified' folder. Worlds removed from the 'Unclassified' folder can be accessed from Settings > Trash Bin.";
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