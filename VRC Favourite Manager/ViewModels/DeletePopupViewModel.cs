using Microsoft.UI.Xaml;
using System.Diagnostics;
using VRC_Favourite_Manager.Common;

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
        }
    }
}
