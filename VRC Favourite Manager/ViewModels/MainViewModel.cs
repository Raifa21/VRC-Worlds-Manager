﻿using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly VRChatService _vrChatService;
        private ObservableCollection<WorldModel> _favoriteWorlds;

        public ObservableCollection<WorldModel> FavoriteWorlds
        {
            get => _favoriteWorlds;
            set
            {
                _favoriteWorlds = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _vrChatService = Application.Current.Resources["VRChatService"] as VRChatService;
            FavoriteWorlds = new ObservableCollection<WorldModel>();
        }

        public async Task InitializeAsync()
        {
            var favoriteModels = new List<FavouriteModel>
                {
                    new FavouriteModel { id = "1", type = "world", favouriteId = "wrld_a1071eb7-e16c-4a52-bd6e-c0efdb1b5ea5" }
                };

            var favoriteWorlds = await _vrChatService.GetFavoriteWorldsAsync(favoriteModels);

            foreach (var world in favoriteWorlds)
            {
                FavoriteWorlds.Add(world);
            }
        }
    }
}