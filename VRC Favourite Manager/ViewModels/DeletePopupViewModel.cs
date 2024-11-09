using Microsoft.UI.Xaml;
using System.Diagnostics;
using Serilog;
using VRC_Favourite_Manager.Common;
using CommunityToolkit.Mvvm.Messaging;

namespace VRC_Favourite_Manager.ViewModels
{
    public class DeletePopupViewModel : ViewModelBase
    {
        private readonly FolderManager _folderManager;
        public DeletePopupViewModel()
        {
            Log.Information("MainViewModel created");
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;

        }

        public void Delete_Click(string folderName)
        {
            _folderManager.RemoveFolder(folderName);
            //send a SelectedFolderUpdatedMessage message
            WeakReferenceMessenger.Default.Send(new SelectedFolderUpdatedMessage());
        }
    }

    public class SelectedFolderUpdatedMessage
    {
        public string FolderName { get; set; }
    }
}