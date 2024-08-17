using System.Collections.Generic;
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
        private GroupModel _selectedGroup;
        private List<GroupModel> Groups { get; set; }

        public CreateGroupInstancePopupViewModel(WorldModel selectedWorld, string region)
        {
            _vrChatApiService = Application.Current.Resources["VRChatApiService"] as VRChatAPIService;
            _selectedWorld = selectedWorld;
            _region = region;

            Groups = new List<GroupModel>();
            GetGroups();
        }

        private async void GetGroups()
        {
            var groups = await _vrChatApiService.GetGroupsAsync();
            foreach (var group in groups)
            {
                Groups.Add(new GroupModel
                {
                    Name = group.Name,
                    Id = group.Id,
                    Icon = group.Icon,
                    GroupRoles = group.Roles
                });
            }
        }
        public async void CreateInstanceAsync()
        {
            await _vrChatApiService.CreateGroupInstanceAsync(_selectedWorld.WorldId, _region);
        }

        public void GroupSelected(string groupName)
        {
            _selectedGroup = group;
        }

    }
    public class GroupModel()
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Icon { get; set; }
        public List<string> GroupRoles { get; set; }
    }
}