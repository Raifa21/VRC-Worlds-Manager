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
        private readonly VRChatService _vrChatService;
        private readonly JsonStorageService _jsonStorageService;

        private ObservableCollection<WorldModel> _favoriteWorlds;
        public ICommand RefreshCommand { get; }

        public MainViewModel()
        {
            InitializeAsync();
            _vrChatService = Application.Current.Resources["VRChatService"] as VRChatService;
            _jsonStorageService = new JsonStorageService();

            _favoriteWorlds = new ObservableCollection<WorldModel>();
            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());


            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(10);
            _timer.Tick += async (sender, e) => await CheckForNewWorldsAsync();
            _timer.Start();
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
            var worlds = await _vrChatService.InitialGetFavoriteWorldsAsync();
            foreach (var world in worlds)
            {
                _favoriteWorlds.Add(world);
            }
            _jsonStorageService.SaveWorlds(_favoriteWorlds);
        }
        private async Task CheckForNewWorldsAsync()
        {
            // Implement the logic to check for new worlds here
        }

        private async Task RefreshWorldsAsync()
        {
            await CheckForNewWorldsAsync();
        }
    }
}
