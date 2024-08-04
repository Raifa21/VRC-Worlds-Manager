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
        private ObservableCollection<string> _folderNames;

        public ObservableCollection<string> FolderNames
        {
            get => _folderNames;
            set
            {
                _folderNames = value;
                OnPropertyChanged(nameof(FolderNames));
            }
        }
        
        public ICommand LogoutCommand { get; }
        public ICommand ResetCommand { get; }


        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");
            _vrChatAPIService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;
            
            _worldManager.LoadWorldsAsync();
            _folderNames = new ObservableCollection<string>();
            foreach (var folder in _folderManager.Folders)
            {
                _folderNames.Add(folder.Name);
            }

            _folderManager.PropertyChanged += OnFolderManagerPropertyChanged;

            LogoutCommand = new RelayCommand(async () => await LogoutCommandAsync());
            ResetCommand = new RelayCommand(Reset);
        }

        private void OnFolderManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _folderNames.Clear();
            foreach (var folder in _folderManager.Folders)
            {
                _folderNames.Add(folder.Name);
            }
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

        public void SelectedFolderChanged(FolderModel folder)
        {
            _folderManager.ChangeSelectedFolder(folder);
        }
    }
}
