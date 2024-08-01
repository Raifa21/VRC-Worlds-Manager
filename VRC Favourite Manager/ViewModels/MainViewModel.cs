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
        private readonly JsonManager _jsonManager;
        private string _folderName;
        private FolderModel _selectedFolder;
        private ObservableCollection<FolderModel> _folders;

        private HashSet<WorldModel> _favoriteWorlds;
        private HashSet<string> _existingWorldIds;

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

        public ObservableCollection<FolderModel> Folders
        {
            get => _folders;
            set
            {
                _folders = value;
                OnPropertyChanged(nameof(Folders));
            }
        }
        public string FolderName
        {
            get => _folderName;
            set
            {
                _folderName = value;
                OnPropertyChanged();
            }
        }
        public FolderModel SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                UpdateWorldsCollection();
            }
        }

        public ObservableCollection<WorldModel> Worlds
        {
            get => _worlds;
            set
            {
                _worlds = value;
                OnPropertyChanged(nameof(Worlds));
            }
        }



        public ICommand AddFolderCommand { get; }
        public ICommand MoveWorldCommand { get; }

        public ICommand RefreshCommand { get; }

        public ICommand LogoutCommand { get; }

        public ICommand ResetCommand { get; }



        public MainViewModel()
        {

            Worlds = new ObservableCollection<WorldModel>();
            _vrChatAPIService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;

            _jsonManager = new JsonManager();
            _favoriteWorlds = new HashSet<WorldModel>();
            _existingWorldIds = new HashSet<string>();

            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());
            LogoutCommand = new RelayCommand(async () => await LogoutCommandAsync());
            MoveWorldCommand = new RelayCommand<WorldModel>(MoveWorld);
            ResetCommand = new RelayCommand(ResetWorlds);

            Folders = new ObservableCollection<FolderModel>();
            var task = InitializeAsync();


            SelectedFolder = Folders.First();
        }

        private async Task InitializeAsync()
        {
            if (_jsonManager.FolderConfigExists())
            {
                var savedFolders = _jsonManager.LoadFolders();
                if (savedFolders != null)
                {
                    Folders = new ObservableCollection<FolderModel>(savedFolders);
                }
                else
                {
                    Debug.WriteLine("No folder in file, creating default folders");
                    Folders = new ObservableCollection<FolderModel>
                    {
                        new FolderModel("Unclassified")
                    };
                    _jsonManager.SaveFolders(Folders);
                }
            }
            else
            {
                Debug.WriteLine("No folder file found, creating default folders");
                Folders = new ObservableCollection<FolderModel>
                {
                    new FolderModel("Unclassified")
                };
                _jsonManager.SaveFolders(Folders);
            }
        }


        private async Task RefreshWorldsAsync()
        {
            await CheckForNewWorldsAsync();
        }
        private async Task LogoutCommandAsync()
        {
            await _vrChatAPIService.LogoutAsync();
        }
        private void MoveWorld(WorldModel world)
        {
            // Remove the world from the current folder
            var unclassifiedFolder = Folders.FirstOrDefault(f => f.Name == "Unclassified");
            if (unclassifiedFolder?.Worlds.Contains(world) == true)
            {
                unclassifiedFolder.Worlds.Remove(world);
            }

            SelectedFolder?.Worlds.Add(world);
        }
        private void ResetWorlds()
        {
            _favoriteWorlds.Clear();
            _existingWorldIds.Clear();
            foreach (var folder in Folders)
            {
                folder.Worlds.Clear();
            }
            _jsonManager.SaveWorlds(_favoriteWorlds);
        }
        public void AddFolder(string folderName)
        {
            if (!string.IsNullOrWhiteSpace(folderName) && !Folders.Any(f => f.Name == folderName))
            {
                var newFolder = new FolderModel(folderName);
                Folders.Add(newFolder);
                _jsonManager.SaveFolders(Folders);
            }
        }
    }
}
