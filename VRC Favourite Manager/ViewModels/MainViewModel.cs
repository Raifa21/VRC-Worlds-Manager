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
        private World selectedWorld;
        private readonly DispatcherTimer _timer;
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;

        private ObservableCollection<WorldModel> _worlds;

        public World SelectedWorld
        {
            get { return selectedWorld; }
            set
            {
                if (selectedWorld != value)
                {
                    selectedWorld = value;
                    OnPropertyChanged();
                }
            }
        }

        private FolderModel _selectedFolder;

        public FolderModel SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (_selectedFolder != value)
                {
                    _selectedFolder = value;
                    OnPropertyChanged(nameof(SelectedFolder));
                    _folderManager.SelectedFolder = value;
                }
            }
        }

        public ObservableCollection<FolderModel> Folders => _folderManager.Folders;

        public ObservableCollection<WorldModel> Worlds
        {
            get => _worlds;
            set
            {
                _worlds = value;
                OnPropertyChanged();
            }
        }


        public ICommand MoveWorldCommand { get; }
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
