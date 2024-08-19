using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        private bool _canCreateGroupInstance;
        public bool CanCreateGroupInstance
        {
            get => _canCreateGroupInstance;
            set
            {
                _canCreateGroupInstance = value;
                OnPropertyChanged();
            }
        }

        private bool _canCreateRestricted;
        public bool CanCreateRestricted
        {
            get => _canCreateRestricted;
            set
            {
                _canCreateRestricted = value;
                OnPropertyChanged();
            }
        }

        private bool _canCreateGroupOnly;

        public bool CanCreateGroupOnly
        {
            get => _canCreateGroupOnly;
            set
            {
                _canCreateGroupOnly = value;
                OnPropertyChanged();
            }
        }

        private bool _canCreateGroupPlus;

        public bool CanCreateGroupPlus
        {
            get => _canCreateGroupPlus;
            set
            {
                _canCreateGroupPlus = value;
                OnPropertyChanged();
            }
        }
        
        private bool _canCreateGroupPublic;

        public bool CanCreateGroupPublic
        {
            get => _canCreateGroupPublic;
            set
            {
                _canCreateGroupPublic = value;
                OnPropertyChanged();
            }
        }

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

            CanCreateGroupInstance = false;
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
                    Privacy = group.Privacy,
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

            var permissions = new List<string>();
            foreach (var role in _userRoles)
            {
                var groupRole = _selectedGroup.GroupRoles.Find(groupRole => groupRole.Name == role);
                if (groupRole != null)
                {
                    foreach(var permission in groupRole.Permissions)
                    {
                        if(!permissions.Contains(permission))
                        {
                            permissions.Add(permission);
                        }
                    }
                }
            }

            _canCreateRestricted = permissions.Contains("group-instance-restricted-create");
            _canCreateGroupOnly = permissions.Contains("group-instance-open-create");
            _canCreateGroupPlus = permissions.Contains("group-instance-plus-create");
            _canCreateGroupPublic = permissions.Contains("group-instance-public-create") && _selectedGroup.Privacy == "default";

            _canCreateGroupInstance =
                (_canCreateGroupOnly || _canCreateGroupPlus || _canCreateGroupPublic || _canCreateRestricted) &&
                permissions.Contains("group-instance-join");
        }

        public void AccessTypeSelected(string instanceType)
        {
            _groupAccessType = instanceType.ToLower();
        }

        public void RolesSelected(List<string> roles)
        {
            _selectedRoles = roles;
        }

        public async void CreateInstanceAsync()
        {
            await _vrChatApiService.CreateGroupInstanceAsync(_selectedWorld.WorldId, _selectedGroup.Id, _region,
                _groupAccessType, _selectedRoles, _isQueueEnabled);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class GroupModel()
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Privacy { get; set; }
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

        public bool IsSelected { get; set; }
    }
}