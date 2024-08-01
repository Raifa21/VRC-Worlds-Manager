using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.Common
{
    public class WorldManager
    {
        private readonly VRChatAPIService _vrChatAPIService;
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


        public WorldManager(VRChatAPIService vrChatAPIService, JsonManager jsonManager)
        {
            _vrChatAPIService = vrChatAPIService;
            _jsonManager = jsonManager;
            _worlds = new ObservableCollection<WorldModel>();
            _existingWorldIds = new HashSet<string>();

            LoadWorldsAsync();

        }

        private async void LoadWorldsAsync()
        {
            if (_jsonManager.WorldConfigExists())
            {
                var worlds = _jsonManager.LoadWorlds();
                foreach (var world in worlds)
                {
                    _worlds.Add(world);
                    _existingWorldIds.Add(world.WorldId);
                }

                if (worlds.Count > 0)
                {
                    await CheckForNewWorldsAsync();
                }
                else
                {
                    await InitialScanAsync();
                }
            }
            else
            {
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
                    _worlds.Add(world);
                }

                if (worlds.Count < 100)
                {
                    hasMore = false;
                }

                page++;
            }

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
                    _worlds.Add(world);
                    _existingWorldIds.Add(world.WorldId);
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