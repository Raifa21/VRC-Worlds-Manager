using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRChat.API.Model;

namespace VRC_Favourite_Manager.ViewModels
{
    public class RemovePopupViewModel
    {
        private readonly FolderManager _folderManager;

        private List<WorldModel> _selectedWorld;
        
        private string _selectedFolder;

        public RemovePopupViewModel(List<WorldModel> selectedWorld, string folderName)
        {
            _selectedWorld = selectedWorld;
            _selectedFolder = folderName;
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