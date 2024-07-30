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
            _jsonManager = new JsonManager();
            _favoriteWorlds = new HashSet<WorldModel>();
            _existingWorldIds = new HashSet<string>();

            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());
            LogoutCommand = new RelayCommand(async () => await LogoutCommandAsync());
            MoveWorldCommand = new RelayCommand<WorldModel>(MoveWorld);
            ResetCommand = new RelayCommand(ResetWorlds);

            Folders = new ObservableCollection<FolderModel>
            {
                new FolderModel("Unclassified")
            };
            SelectedFolder = Folders.First();
            

            var task = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (_jsonManager.ConfigExists())
            {
                Debug.WriteLine("Loading worlds from file");
                var worlds = _jsonManager.LoadWorlds();
                foreach (var world in worlds)
                {
                    _favoriteWorlds.Add(world);
                    if (!Folders.Any(f => f.Worlds.Contains(world)))
                    {
                        Folders.First(f => f.Name == "Unclassified").Worlds.Add(world);
                    }
                }
                PrintFolders();
                if (_favoriteWorlds.Count == 0)
                {
                    Debug.WriteLine("No worlds found in file");
                    await InitialScanAsync();
                    Debug.WriteLine("Found " + _favoriteWorlds.Count + " worlds");
                    Debug.WriteLine("Done checking for new worlds");
                }
                else
                {
                    Debug.WriteLine("Checking for new worlds");
                    await CheckForNewWorldsAsync();
                    Debug.WriteLine("Found " + _favoriteWorlds.Count + " worlds");
                    Debug.WriteLine("Done checking for new worlds");
                }
            }
            else
            {
                Debug.WriteLine("No config file found, scanning for worlds");
                await InitialScanAsync();
            }

            PrintFolders();

            UpdateWorldsCollection();
        }
        private async Task InitialScanAsync()
        {
            int page = 0;
            bool hasMore = true;
            while (hasMore)
            {
                var worlds = await _vrChatAPIService.GetFavoriteWorldsAsync(100, page * 100);
                foreach (var world in worlds)
                {
                    _favoriteWorlds.Add(world);
                    if (world.Folder.Count == 0)
                    {
                        Folders.First(f => f.Name == "Unclassified").Worlds.Add(world);
                    }
                    else
                    {
                        foreach (var folder in world.Folder)
                        {
                            if (Folders.All(f => f.Name != folder))
                            {
                                Folders.Add(new FolderModel(folder));
                            }
                            Folders.First(f => f.Name == folder).Worlds.Add(world);
                        }
                    }
                    
                }
                if (worlds.Count < 100)
                {
                    hasMore = false;
                }
                page++;
            }
            _jsonManager.SaveWorlds(_favoriteWorlds);
        }
        private async Task CheckForNewWorldsAsync()
        {
            if (_existingWorldIds == null)
            {
                _existingWorldIds = new HashSet<string>();
            }

            // Add existing worlds' IDs to the set
            foreach (var world in _favoriteWorlds)
            {
                _existingWorldIds.Add(world.WorldId);
            }

            var worlds = await _vrChatAPIService.GetFavoriteWorldsAsync(100, 0);
            foreach (var world in worlds)
            {
                if (!_existingWorldIds.Contains(world.WorldId))
                {
                    _favoriteWorlds.Add(world);
                    _existingWorldIds.Add(world.WorldId);
                    if (world.Folder.Count == 0)
                    {
                        Folders.First(f => f.Name == "Unclassified").Worlds.Add(world);
                    }
                    else
                    {
                        foreach (var folder in world.Folder)
                        {
                            if (Folders.All(f => f.Name != folder))
                            {
                                Folders.Add(new FolderModel(folder));
                            }
                            Folders.First(f => f.Name == folder).Worlds.Add(world);
                        }
                    }
                }
            }
            _jsonManager.SaveWorlds(_favoriteWorlds);
        }

        private void UpdateWorldsCollection()
        {
            Worlds.Clear();
            if (SelectedFolder != null)
            {
                foreach (var world in SelectedFolder.Worlds)
                {
                    Worlds.Add(world);
                }
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
        private void PrintFolders()
        {
            Debug.WriteLine("Folders and their contents:");
            foreach (var folder in Folders)
            {
                Debug.WriteLine($"Folder: {folder.Name}");
                foreach (var world in folder.Worlds)
                {
                    Debug.WriteLine($"  World: {world.WorldName} (ID: {world.WorldId})");
                }
            }
        }

    }
}
