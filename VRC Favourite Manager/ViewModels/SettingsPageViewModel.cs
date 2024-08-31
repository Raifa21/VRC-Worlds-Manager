﻿using System.Diagnostics;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Common;

namespace VRC_Favourite_Manager.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;
        public SettingsPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;


            Debug.WriteLine("SettingsPageViewModel created");
        }


        public async void Reset()
        {
            _folderManager.ResetFolders();
            _worldManager.ResetWorlds();

            await _worldManager.InitialScanAsync();
        }
    }
}
