using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.Common
{
    public class WorldManager
    {
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly FolderManager _folderManager;
        private readonly JsonManager _jsonManager;
        private HashSet<string> _existingWorldIds;
        private ObservableCollection<WorldModel> _worlds;

        public ObservableCollection<WorldModel> Worlds
        {
            get => _worlds;
            private set
            {
                _worlds = value;
                OnPropertyChanged(nameof(Worlds));
            }
        }


        public WorldManager()
        {
            _vrChatAPIService = (VRChatAPIService)App.Current.Resources["VRChatAPIService"];
            _folderManager = (FolderManager)App.Current.Resources["FolderManager"];
            _jsonManager = new JsonManager();
            _worlds = new ObservableCollection<WorldModel>();
            _existingWorldIds = new HashSet<string>();
        }

        public async void LoadWorldsAsync()
        {
            if (_jsonManager.WorldConfigExists())
            {
                var worlds = _jsonManager.LoadWorlds();
                foreach (var world in worlds)
                {
                    if (world.WorldId != "???")
                    {
                        _worlds.Add(world);
                        _existingWorldIds.Add(world.WorldId);
                    }
                }

                if (worlds.Count > 0)
                {
                    await CheckForNewWorldsAsync();
                }
                else
                {
                    Debug.WriteLine("No worlds found in the config file. Performing initial scan.");
                    await InitialScanAsync();
                }
            }
            else
            {
                Debug.WriteLine("No config file found. Performing initial scan.");
                await InitialScanAsync();
            }
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
                    if(world.WorldId != "???")
                    {
                        _worlds.Add(world);
                        _existingWorldIds.Add(world.WorldId);
                    }
                }

                if (worlds.Count < 100)
                {
                    hasMore = false;
                }

                page++;
            }
            _folderManager.InitializeFolders(_worlds);

            SaveWorlds();
        }

        public async Task CheckForNewWorldsAsync()
        {
            if (_existingWorldIds == null)
            {
                _existingWorldIds = new HashSet<string>();
            }

            // Add existing worlds' IDs to the set
            foreach (var world in _worlds)
            {
                _existingWorldIds.Add(world.WorldId);
            }

            var worlds = await _vrChatAPIService.GetFavoriteWorldsAsync(100, 0);
            foreach (var world in worlds)
            {
                if (!_existingWorldIds.Contains(world.WorldId))
                {
                    if (world.WorldId != "???")
                    {
                        _worlds.Add(world);
                        _existingWorldIds.Add(world.WorldId);
                        _folderManager.AddToFolder(world, "Unclassified");
                    }
                }
                else
                {
                    break;
                }
            }

            SaveWorlds();
        }

        public void RemoveWorld(WorldModel world)
        {
            if (_worlds.Contains(world))
            {
                _worlds.Remove(world);
                _existingWorldIds.Remove(world.WorldId);
                SaveWorlds();
            }
        }

        private void SaveWorlds()
        {
            _jsonManager.SaveWorlds(_worlds);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ResetWorlds()
        {
            _worlds.Clear();
            _existingWorldIds.Clear();
            SaveWorlds();
        }
    }
}