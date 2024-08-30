using Microsoft.UI.Xaml;
using System.Collections.Generic;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.ViewModels
{
    public class RemovePopupViewModel
    {
        private readonly FolderManager _folderManager;

        private List<WorldModel> _selectedWorld;
        
        private string _selectedFolder;

        public RemovePopupViewModel(List<WorldModel> selectedWorld)
        {
            _selectedWorld = selectedWorld;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
        }
        
        public void RemoveFromFolder()
        {
            foreach(var world in _selectedWorld)
            {
                _folderManager.MoveToHiddenFolder(world);
            }
        }
    }
}