using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;
        private MainWindow mainWindow;

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
            FoldersNavigationViewItems = GetFoldersNavigationViewItems();
            _folderManager.PropertyChanged += OnFolderManagerPropertyChanged;
        }

        private async Task LogoutCommandAsync()
        {
            await _vrChatAPIService.LogoutAsync();
            var rootFrame = new Frame();
            mainWindow = new MainWindow();
            rootFrame.Navigate(typeof(AuthenticationPage));
            mainWindow.Content = rootFrame;
            mainWindow.Activate();


            ((App)Application.Current).mainWindow.Close();
        }

        private void OnFolderManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FoldersNavigationViewItems = GetFoldersNavigationViewItems();
        }

        public IEnumerable<NavigationViewItemBase> GetFoldersNavigationViewItems()
        {
            return _folderManager.Folders.Select(folder => new NavigationViewItem
            {
                Content = folder.Name,
                Tag = folder,
                IsSelected = (folder == _folderManager.SelectedFolder)
            }).Cast<NavigationViewItemBase>();
        }

        public void SelectedFolderChanged(FolderModel folder)
        {
            _folderManager.ChangeSelectedFolder(folder);
        }
    }
}
