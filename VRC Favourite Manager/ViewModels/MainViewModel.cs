using System;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;

        public ICommand LogoutCommand { get; }
        public IEnumerable<NavigationViewItemBase> FoldersNavigationViewItems { get; set; }

        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");
            _vrChatAPIService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            LogoutCommand = new RelayCommand(async () => await LogoutCommandAsync());

            _worldManager.LoadWorldsAsync();

            FoldersNavigationViewItems = GetFoldersNavigationViewItems(_folderManager.Folders);

            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("FolderUpdatedMessage received");
                FoldersNavigationViewItems = GetFoldersNavigationViewItems(m.Folders);
            });
        }

        private async Task LogoutCommandAsync()
        {
            await _vrChatAPIService.LogoutAsync();
            ((App)Application.Current).mainWindow.NavigateToAuthenticationPage();
        }

        private IEnumerable<NavigationViewItemBase> GetFoldersNavigationViewItems(ObservableCollection<FolderModel> Folders)
        {
            return Folders.Select(folder => new NavigationViewItem
            {
                Content = folder.Name,
                Tag = folder
            }).Cast<NavigationViewItemBase>();
        }

        public void SelectedFolderChanged(FolderModel folder)
        {
            _folderManager.ChangeSelectedFolder(folder.Name);
        }
    }
}
