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
        private List<string> _userRoles;
        private string _groupAccessType;
        private List<string> _selectedRoles;
        private bool _isQueueEnabled;
        private List<GroupModel> Groups { get; set; }

        public CreateGroupInstancePopupViewModel(WorldModel selectedWorld, string region)
        {
            _vrChatApiService = Application.Current.Resources["VRChatApiService"] as VRChatAPIService;
            _selectedWorld = selectedWorld;
            _region = region;

            Groups = new List<GroupModel>();
            _selectedRoles = new List<string>();
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
                    Icon = group.IconUrl,
                    GroupRoles = new List<GroupRolesModel>()
                });
            }
        }
        public async void GroupSelected(string groupName)
        {
            _selectedGroup = Groups.Find(group => group.Name == groupName);
            _selectedGroup.GroupRoles = await _vrChatApiService.GetGroupRolesAsync(_selectedGroup.Id);
            _userRoles = await _vrChatApiService.GetUserRoleAsync(_selectedGroup.Id);
            
        }

        public void AccessTypeSelected(string instanceType)
        {
            _groupAccessType = instanceType.ToLower();
        }

        public async void CreateInstanceAsync()
        {
            await _vrChatApiService.CreateGroupInstanceAsync(_selectedWorld.WorldId, _selectedGroup.Id, _region,
                _groupAccessType, _selectedRoles, _isQueueEnabled);
        }


    }
    public class GroupModel()
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Icon { get; set; }
        public List<GroupRolesModel> GroupRoles { get; set; }
    }

    public class GroupRolesModel()
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public List<string> Permissions { get; set; }

        public bool IsManagementRole { get; set; }

        public int Order { get; set; }
    }
}