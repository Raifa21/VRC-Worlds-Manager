
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class WorldDetailsPopupViewModel
    {
        private readonly VRChatAPIService _vrChatApiService;

        public WorldDetailsPopupViewModel(WorldModel selectedWorld)
        {
            _vrChatApiService = Application.Current.Resources["VRChatApiService"] as VRChatAPIService;
        }

        public void CreateInstance(WorldModel world, string instanceType, string region)
        {
            await _vrChatApiService.CreateInstanceAsync(world.WorldId, instanceType, region);
        }

    }
}