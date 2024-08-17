using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class CreateGroupInstancePopupViewModel
    {
        private readonly VRChatAPIService _vrChatApiService;
        private WorldModel _selectedWorld;
        private string _region;

        public CreateGroupInstancePopupViewModel(WorldModel selectedWorld, string region)
        {
            _vrChatApiService = Application.Current.Resources["VRChatApiService"] as VRChatAPIService;
            _selectedWorld = selectedWorld;
            _region = region;
        }

        public async void CreateInstanceAsync()
        {
            await _vrChatApiService.CreateGroupInstanceAsync(_selectedWorld.WorldId, _region);
        }
    }
}