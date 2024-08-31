using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class WorldDetailsPopupViewModel : ObservableObject
    {
        private readonly VRChatAPIService _vrChatApiService;
        private readonly FolderManager _folderManager;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }


        private bool _canCreateInstance;
        public bool CanCreateInstance
        {
            get => _canCreateInstance;
            set => SetProperty(ref _canCreateInstance, value);
        }

        private string _thumbnailImageUrl;
        public string ThumbnailImageUrl
        {
            get => _thumbnailImageUrl;
            set => SetProperty(ref _thumbnailImageUrl, value);
        }

        private string _worldName;
        public string WorldName
        {
            get => _worldName;
            set => SetProperty(ref _worldName, value);
        }

        private string _authorName;
        public string AuthorName
        {
            get => _authorName;
            set => SetProperty(ref _authorName, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private int _visits;
        public int Visits
        {
            get => _visits;
            set => SetProperty(ref _visits, value);
        }

        private int _favorites;
        public int Favorites
        {
            get => _favorites;
            set => SetProperty(ref _favorites, value);
        }

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set => SetProperty(ref _capacity, value);
        }

        private string _lastUpdate;
        public string LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }

        public WorldDetailsPopupViewModel(WorldModel selectedWorld)
        {
            _vrChatApiService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            CheckWorldUpdateAsync(selectedWorld.WorldId);
        }

        public async void CheckWorldUpdateAsync(string worldId)
        {
            IsLoading = true;
            var world = await _vrChatApiService.GetWorldByIdAsync(worldId);
            if (world == null)
            {
                IsLoading = false;
                CanCreateInstance = false;
                //get world from the folder manager
                foreach(var folder in _folderManager.Folders)
                {
                    var worldModel_temp = folder.Worlds.FirstOrDefault(w => w.WorldId == worldId);
                    if (worldModel_temp != null)
                    {
                        ThumbnailImageUrl = worldModel_temp.ThumbnailImageUrl;
                        WorldName = worldModel_temp.WorldName;
                        AuthorName = worldModel_temp.AuthorName;
                        Description = worldModel_temp.Description;
                        Visits = worldModel_temp.Visits ?? 0;
                        Favorites = worldModel_temp.Favorites;
                        Capacity = worldModel_temp.Capacity;
                        LastUpdate = worldModel_temp.LastUpdate;
                        break;
                    }
                }

                return;
            }
            CanCreateInstance = world.ReleaseStatus == "public";
            ThumbnailImageUrl = world.ThumbnailImageUrl;
            WorldName = world.Name;
            AuthorName = world.AuthorName;
            Description = world.Description;
            Visits = world.Visits ?? 0;
            Favorites = world.Favorites;
            Capacity = world.Capacity;
            LastUpdate = world.UpdatedAt.ToString(CultureInfo.InvariantCulture)?[..10];

            var worldModel = new WorldModel
            {
                ThumbnailImageUrl = ThumbnailImageUrl,
                WorldName = WorldName,
                WorldId = worldId,
                AuthorName = AuthorName,
                Capacity = Capacity,
                LastUpdate = LastUpdate,
                Description = Description,
                Visits = Visits,
                Favorites = Favorites
            };
            _folderManager.UpdateWorldInFolders(worldModel);

            IsLoading = false;
        }

        public async void CreateInstanceAsync(string worldId, string instanceType, string region)
        {
            await _vrChatApiService.CreateInstanceAsync(worldId, instanceType, region);
        }
    }
}
    