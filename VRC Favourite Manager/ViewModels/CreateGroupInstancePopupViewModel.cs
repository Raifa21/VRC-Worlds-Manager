using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using VRChat.API.Model;

namespace VRC_Favourite_Manager.ViewModels
{
    public class CreateGroupInstancePopupViewModel : ObservableObject
    {
        private readonly VRChatAPIService _vrChatApiService;
        private readonly WorldModel _selectedWorld;
        public string Region;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private bool _showCancelButton;
        public bool ShowCancelButton
        {
            get => _showCancelButton;
            set => SetProperty(ref _showCancelButton, value);
        }

        private bool _isGroupRolesLoadingComplete;
        public bool IsGroupRolesLoadingComplete
        {
            get => _isGroupRolesLoadingComplete;
            set => SetProperty(ref _isGroupRolesLoadingComplete, value);
        }


        private bool _canCreateGroupInstance;
        public bool CanCreateGroupInstance
        {
            get => _canCreateGroupInstance;
            set => SetProperty(ref _canCreateGroupInstance, value);
        }

        private bool _canCreateRestricted;
        public bool CanCreateRestricted
        {
            get => _canCreateRestricted;
            set => SetProperty(ref _canCreateRestricted, value);
        }

        private bool _canCreateGroupOnly;

        public bool CanCreateGroupOnly
        {
            get => _canCreateGroupOnly;
            set => SetProperty(ref _canCreateGroupOnly, value);
        }

        private bool _canCreateGroupPlus;

        public bool CanCreateGroupPlus
        {
            get => _canCreateGroupPlus;
            set => SetProperty(ref _canCreateGroupPlus, value);
        }
        
        private bool _canCreateGroupPublic;

        public bool CanCreateGroupPublic
        {
            get => _canCreateGroupPublic;
            set => SetProperty(ref _canCreateGroupPublic, value);
        }

        private bool _isQueueEnabled;
        public bool IsQueueEnabled
        {
            get => _isQueueEnabled;
            set => SetProperty(ref _isQueueEnabled, value);
        }


        private GroupModel _selectedGroup;
        private List<string> _userRoles;
        private List<string> _selectedRoles;

        public ICommand CancelLoadingCommand { get; }

        public ObservableCollection<GroupModel> Groups { get; set; }

        public ObservableCollection<GroupRolesModel> GroupRoles { get; set; }

        private ObservableCollection<SelectRolesModel> _selectRoles;
        public ObservableCollection<SelectRolesModel> SelectRoles
        {
            get => _selectRoles;
            set => SetProperty(ref _selectRoles, value);
        }

        private bool _everyoneIsSelected;
        public bool EveryoneIsSelected
        {
            get => _everyoneIsSelected;
            set => SetProperty(ref _everyoneIsSelected, value);
        }

        // These are for displaying content on the UI
        public string ThumbnailImageUrl { get; set; }
        private string _groupIcon;
        public string GroupIcon
        {
            get => _groupIcon;
            set => SetProperty(ref _groupIcon, value);
        }
        
        private string _groupName;
        public string GroupName
        {
            get => _groupName;
            set => SetProperty(ref _groupName, value);
        }

        private string _groupAccessType;
        public string GroupAccessType
        {
            get => _groupAccessType;
            set => SetProperty(ref _groupAccessType, value);
        }

        private bool _isRoleRestricted;
        public bool IsRoleRestricted
        {
            get => _isRoleRestricted;
            set => SetProperty(ref _isRoleRestricted, value);
        }
        
        private string _rolesThatHaveAccess;
        public string RolesThatHaveAccess
        {
            get => _rolesThatHaveAccess;
            set => SetProperty(ref _rolesThatHaveAccess, value);
        }
        

        public CreateGroupInstancePopupViewModel(WorldModel selectedWorld, string region)
        {
            _vrChatApiService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _selectedWorld = selectedWorld;
            ThumbnailImageUrl = selectedWorld.ThumbnailImageUrl;
            Region = region;

            Groups = new ObservableCollection<GroupModel>();
            _selectedRoles = new List<string>();
            GetGroups();


            CanCreateGroupInstance = false;
            CancelLoadingCommand = new RelayCommand(CancelLoading);

            SelectRoles = new ObservableCollection<SelectRolesModel>();
            EveryoneIsSelected = false;
            IsQueueEnabled = false;

            IsGroupRolesLoadingComplete = false;

            IsRoleRestricted = false;
        }

        private async void GetGroups()
        {
            IsLoading = true;
            ShowCancelButton = false;
            Message = "Loading...";

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                var delayTask = Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                var groupsTask = _vrChatApiService.GetGroupsAsync();

                if (await Task.WhenAny(groupsTask, delayTask) == delayTask)
                {
                    Message = "Please use the official site. Loading is taking too long.";
                    ShowCancelButton = true;
                    return;
                }

                var groups = await groupsTask;

                if (groups == null || !groups.Any())
                {
                    Message = "You have no groups, please create or join a group to create a group instance.";
                }
                else
                {
                    Groups.Clear();
                    foreach (var group in groups)
                    {
                        Groups.Add(new GroupModel
                        {
                            Name = group.Name,
                            Id = group.GroupId,
                            Privacy = group.Privacy,
                            Icon = group.IconUrl,
                            GroupRoles = new List<GroupRolesModel>()
                        });
                    }
                    Message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void CancelLoading()
        {
            _cancellationTokenSource?.Cancel();
            Message = "Loading canceled.";
            IsLoading = false;
        }

        public async void GroupSelected(string groupName)
        {
            EveryoneIsSelected = true;
            IsLoading = true;
            IsGroupRolesLoadingComplete = false;
            ShowCancelButton = false;
            Message = "Loading group roles...";

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            try
            {
                _selectedGroup = Groups.First(group => group.Name == groupName);
                GroupIcon = _selectedGroup.Icon;
                GroupName = _selectedGroup.Name;
                GroupRoles = new ObservableCollection<GroupRolesModel>();
                foreach(var groupRole in _selectedGroup.GroupRoles)
                {
                    GroupRoles.Add(new GroupRolesModel
                    {
                        Name = groupRole.Name,
                        Id = groupRole.Id,
                        Permissions = groupRole.Permissions,
                        IsManagementRole = groupRole.IsManagementRole,
                        Order = groupRole.Order,
                        IsSelected = false
                    });
                }
                if (_selectedGroup == null)
                {
                    Message = "Group not found.";
                    return;
                }

                Debug.WriteLine($"Selected group: {_selectedGroup.Name}");
                Debug.WriteLine($"Selected group ID: {_selectedGroup.Id}");

                var delayTask = Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                var groupRolesTask = _vrChatApiService.GetGroupRolesAsync(_selectedGroup.Id);
                var userRolesTask = _vrChatApiService.GetUserRoleAsync(_selectedGroup.Id);

                var allTasks = Task.WhenAll(groupRolesTask, userRolesTask);

                if (await Task.WhenAny(allTasks, delayTask) == delayTask)
                {
                    Message = "Please use the official site. Loading is taking too long.";
                    ShowCancelButton = true;
                    return;
                }

                _selectedGroup.GroupRoles = await groupRolesTask;
                _userRoles = await userRolesTask;

                Debug.WriteLine($"Group roles: {_selectedGroup.GroupRoles.Count}");
                Debug.WriteLine($"User roles: {_userRoles.Count}");

                _userRoles.Add(_selectedGroup.GroupRoles.Find(gr => gr.Name == "Everyone").Id);


                UpdatePermissions();
                
                var allRoles = new ObservableCollection<SelectRolesModel>();
                foreach (var groupRole in _selectedGroup.GroupRoles)
                {
                    if (groupRole.Name != "Everyone")
                    {
                        allRoles.Add(new SelectRolesModel
                        {
                            Name = groupRole.Name,
                            IsManagementRole = groupRole.IsManagementRole,
                            IsSelected = false,
                            IsDisabled = false,
                            Order = groupRole.Order,
                        });
                    }
                }


                SelectRoles = allRoles;

                Message = string.Empty;

                if (!CanCreateGroupInstance)
                {
                    Debug.WriteLine("User does not have the required permissions to create a group instance.");
                    Message = "You do not have the required permissions to create a group instance.";
                }
                else
                {
                    Debug.WriteLine("User has the required permissions to create a group instance.");
                }
            }
            catch (Exception ex)
            {
                Message = $"An error occurred: {ex.Message}";
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                IsGroupRolesLoadingComplete = true;
                Debug.WriteLine("Group roles loaded.");
                Debug.WriteLine($"User roles: {_userRoles.Count}");
                Debug.WriteLine(CanCreateGroupInstance
                    ? "User can create group instance."
                    : "User cannot create group instance.");
                Debug.WriteLine(CanCreateRestricted
                    ? "User can create restricted group instance."
                    : "User cannot create restricted group instance.");
                Debug.WriteLine(CanCreateGroupOnly
                    ? "User can create group only group instance."
                    : "User cannot create group only group instance.");
                Debug.WriteLine(CanCreateGroupPlus
                    ? "User can create group plus group instance."
                    : "User cannot create group plus group instance.");
                Debug.WriteLine(CanCreateGroupPublic
                    ? "User can create public group instance."
                    : "User cannot create public group instance.");

            }
        }

        private void UpdatePermissions()
        {
            var permissions = new List<string>();
            foreach (var role in _userRoles)
            {
                var groupRole = _selectedGroup.GroupRoles.Find(gr => gr.Id == role);
                if (groupRole != null)
                {

                    foreach (var permission in groupRole.Permissions)
                    {
                        if (!permissions.Contains(permission))
                        {
                            permissions.Add(permission);
                        }
                    }
                }
            }
            foreach(var permission in permissions)
            {
                Debug.WriteLine($"Permission: {permission}");
            }

            CanCreateRestricted = permissions.Contains("group-instance-restricted-create") || permissions.Contains("*");
            CanCreateGroupOnly = permissions.Contains("group-instance-open-create") || permissions.Contains("*");
            CanCreateGroupPlus = permissions.Contains("group-instance-plus-create") || permissions.Contains("*");
            CanCreateGroupPublic = (permissions.Contains("group-instance-public-create") || permissions.Contains("*")) && _selectedGroup.Privacy == "default";

            CanCreateGroupInstance = (_canCreateGroupOnly || _canCreateGroupPlus || _canCreateGroupPublic || _canCreateRestricted) &&
                                     (permissions.Contains("group-instance-join") || permissions.Contains("*"));
        }


        public void SelectRolesChanged()
        {
            Debug.WriteLine("Checkbox updated");

            EveryoneIsSelected = false;

            var isAnyRoleSelected = SelectRoles.Any(role => role.IsSelected && role.Order != 0);
            var isNonManagementRoleSelected =
                SelectRoles.Where(role => !role.IsManagementRole).Any(role => role.IsSelected);

            Debug.WriteLine($"Any role selected: {isAnyRoleSelected}");
            Debug.WriteLine($"Non management role selected: {isNonManagementRoleSelected}");

            if (isAnyRoleSelected)
            {
                var owner = SelectRoles.First(role => role.Order == 0);
                owner.IsSelected = true;
                owner.IsDisabled = true;
            }
            else
            {
                var owner = SelectRoles.First(role => role.Order == 0);
                if (owner.IsSelected && owner.IsDisabled)
                {
                    owner.IsSelected = false;
                    owner.IsDisabled = false;
                }
                if(!owner.IsSelected && !owner.IsDisabled)
                {
                    EveryoneIsSelected = true;
                }
            }

            if (isNonManagementRoleSelected)
            {
                var managementRoles = SelectRoles.Where(role => role.IsManagementRole);
                foreach (var role in managementRoles)
                {
                    role.IsSelected = true;
                    role.IsDisabled = true;
                }
            }
            else
            {
                var managementRoles = SelectRoles.Where(role => role.IsManagementRole && role.Order != 0);
                foreach (var role in managementRoles)
                {
                    if (role.IsSelected && role.IsDisabled)
                    {
                        role.IsSelected = false;
                        role.IsDisabled = false;
                    }
                }
            }

        }

        public void AccessTypeSelected(string instanceType)
        {
            Debug.WriteLine("Access type selected" + instanceType);
            _groupAccessType = instanceType;
        }

        public void RolesSelected()
        {
            Debug.WriteLine("Roles selected");
            _selectedRoles.Clear();
            if(EveryoneIsSelected)
            {
                _selectedRoles.Add(_selectedGroup.GroupRoles.Find(gr => gr.Name == "Everyone").Id);
            }
            else
            {
                foreach (var role in SelectRoles)
                {
                    if (role.IsSelected)
                    {
                        _selectedRoles.Add(_selectedGroup.GroupRoles.Find(gr => gr.Name == role.Name).Id);
                    }
                }
            }

            IsRoleRestricted = true;
            RolesThatHaveAccess = "";
            foreach (var role in _selectedRoles)
            {
                RolesThatHaveAccess += _selectedGroup.GroupRoles.Find(gr => gr.Id == role).Name + ", ";
            } 
            //remove the last comma
            if (RolesThatHaveAccess.Length > 2)
            {
                RolesThatHaveAccess = RolesThatHaveAccess.Remove(RolesThatHaveAccess.Length - 2);
            }
        }

        public void InvertInstanceQueue()
        {
            IsQueueEnabled = !IsQueueEnabled;
        }

        public async void CreateInstanceAsync()
        {
            if (!_selectedRoles.Any())
            {
                _selectedRoles.Add(_selectedGroup.GroupRoles.Find(gr => gr.Name == "Everyone").Id);
            }
            await _vrChatApiService.CreateGroupInstanceAsync(_selectedWorld.WorldId, _selectedGroup.Id, Region,
                _groupAccessType.ToLower(), _selectedRoles, _isQueueEnabled);
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

    public class SelectRolesModel : ObservableObject
    {
        public string Name { get; set; }
        public bool IsManagementRole { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
        private bool _isDisabled;
        public bool IsDisabled
        {
            get => _isDisabled;
            set => SetProperty(ref _isDisabled, value);
        }
        public int Order { get; set; }
    }
}