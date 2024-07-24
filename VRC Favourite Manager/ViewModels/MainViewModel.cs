using System;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using VRChat.API.Model;

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _timer;
        private readonly VRChatAPIService _vrChatService;
        private readonly JsonStorageService _jsonStorageService;

        private HashSet<WorldModel> _favoriteWorlds;
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            
            _vrChatService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _jsonStorageService = new JsonStorageService();

            _favoriteWorlds = new HashSet<WorldModel>();
            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (_jsonStorageService.ConfigExists())
            {
                var worlds = _jsonStorageService.LoadWorlds();
                foreach (var world in worlds)
                {
                    _favoriteWorlds.Add(world);
                }
                await CheckForNewWorldsAsync();
            }
            else
            {
                await InitialScanAsync();
            }
        }
        private async Task InitialScanAsync()
        {
            var worlds = await _vrChatService.GetAllFavoriteWorldsAsync();
            foreach (var world in worlds)
            {
                _favoriteWorlds.Add(world);
            }
            _jsonStorageService.SaveWorlds(_favoriteWorlds);
        }
        private async Task CheckForNewWorldsAsync()
        {
            var worlds = await _vrChatService.GetFavoriteWorldsAsync();
            foreach (var world in worlds)
            {
                _favoriteWorlds.Add(world);
            }
            _jsonStorageService.SaveWorlds(_favoriteWorlds);
        }

        private async Task RefreshWorldsAsync()
        {
            await CheckForNewWorldsAsync();
        }
    }
}
