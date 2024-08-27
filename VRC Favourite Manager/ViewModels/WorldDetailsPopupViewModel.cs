using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class WorldDetailsPopupViewModel
    {
        private readonly VRChatAPIService _vrChatApiService;

        public WorldDetailsPopupViewModel(WorldModel selectedWorld)
        {
            _vrChatApiService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
        }

        public async void CreateInstanceAsync(string worldId, string instanceType, string region)
        {
            await _vrChatApiService.CreateInstanceAsync(worldId, instanceType, region);
        }
    }
}
    