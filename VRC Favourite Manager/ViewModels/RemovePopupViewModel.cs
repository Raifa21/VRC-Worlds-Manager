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

        private WorldModel _selectedWorld;
        
        private string _selectedFolder;

        public RemovePopupViewModel(WorldModel selectedWorld, string folderName)
        {
            _selectedWorld = selectedWorld;
            _selectedFolder = folderName;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
        }
        
        public void RemoveFromFolder()
        {
            if (_selectedFolder != "Unclassified")
            {
                _folderManager.RemoveFromFolder(_selectedWorld, _selectedFolder);
            }
            else
            {
                _folderManager.MoveToHiddenFolder(_selectedWorld);
            }
        }
    }
}