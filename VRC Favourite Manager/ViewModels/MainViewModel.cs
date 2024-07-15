using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
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
            _vrChatService = new VRChatService();
            FavoriteWorlds = new ObservableCollection<WorldModel>();
        }

        public async Task InitializeAsync()
        {
            if (await _vrChatService.LoginAsync())
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
