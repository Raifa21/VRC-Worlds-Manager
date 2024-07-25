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

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _timer;
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly JsonManager _jsonManager;

        private HashSet<WorldModel> _favoriteWorlds;

        private ObservableCollection<WorldModel> _worlds;
        public ObservableCollection<WorldModel> Worlds
        {
            get => _worlds;
            set
            {
                _worlds = value;
                OnPropertyChanged(nameof(Worlds));
            }
        }

        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {

            Worlds = new ObservableCollection<WorldModel>();
            _vrChatAPIService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _jsonManager = new JsonManager();
            _favoriteWorlds = new HashSet<WorldModel>();
            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());

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
                }
                Debug.WriteLine("Found " + _favoriteWorlds.Count + " worlds");
                Debug.WriteLine("Checking for new worlds");
                await CheckForNewWorldsAsync();
                Debug.WriteLine("Done checking for new worlds");
            }
            else
            {
                Debug.WriteLine("No config file found, scanning for worlds");
                await InitialScanAsync();
            }

            UpdateWorldsCollection();
        }
        private async Task InitialScanAsync()
        {
            int page = 0;
            bool hasMore = true;
            while (hasMore)
            {
                var worlds = await _vrChatAPIService.GetFavoriteWorldsAsync(100, page*100);
                Debug.WriteLine($"Got {worlds.Count} worlds");
                foreach (var world in worlds)
                {
                    _favoriteWorlds.Add(world);
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
            var worlds = await _vrChatAPIService.GetFavoriteWorldsAsync(100,0);
            foreach (var world in worlds)
            {
                if (!_favoriteWorlds.Add(world))
                {
                    break;
                }
            }
            _jsonManager.SaveWorlds(_favoriteWorlds);
        }

        private async Task RefreshWorldsAsync()
        {
            await CheckForNewWorldsAsync();
        }

        private void UpdateWorldsCollection()
        {
            Worlds.Clear();
            foreach (var world in _favoriteWorlds)
            {
                Worlds.Add(world);
            }
        }

    }
}
