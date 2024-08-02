using System;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using System.Linq;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _timer;
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;
        public ObservableCollection<FolderModel> Folders => _folderManager.Folders;
        public FolderModel SelectedFolder
        {
            get => _folderManager.SelectedFolder;
            set
            {
                if (_folderManager.SelectedFolder != value)
                {
                    _folderManager.SelectedFolder = value;
                    OnPropertyChanged(nameof(SelectedFolder));
                }
            }
        }
        public ICommand RefreshCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ResetCommand { get; }


        public MainViewModel()
        {
            _vrChatAPIService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;

            Application.Current.Resources["WorldManager"] = new WorldManager(_vrChatAPIService, new JsonManager());
            Application.Current.Resources["FolderManager"] = new FolderManager(new JsonManager());
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;

            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());
            LogoutCommand = new RelayCommand(async () => await LogoutCommandAsync());
            ResetCommand = new RelayCommand(Reset);
        }

        private async Task RefreshWorldsAsync()
        {
            await _worldManager.CheckForNewWorldsAsync();
        }
        private async Task LogoutCommandAsync()
        {
            await _vrChatAPIService.LogoutAsync();
        }
        private void Reset()
        {
            _worldManager.ResetWorlds();
            _folderManager.ResetFolders();
        }
        public void AddFolder(string folderName)
        {
            _folderManager.AddFolder(folderName);
        }
    }
}
