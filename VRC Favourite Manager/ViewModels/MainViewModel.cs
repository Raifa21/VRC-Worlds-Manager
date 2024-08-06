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

namespace VRC_Favourite_Manager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _timer;
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;
        public IEnumerable<NavigationViewItemBase> FoldersNavigationViewItems { get; set; }

        public MainViewModel()
        {
            Debug.WriteLine("MainViewModel created");
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;
            
            _worldManager.LoadWorldsAsync();
            FoldersNavigationViewItems = GetFoldersNavigationViewItems();
            _folderManager.PropertyChanged += OnFolderManagerPropertyChanged;
        }

        private void OnFolderManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FoldersNavigationViewItems = GetFoldersNavigationViewItems();
        }

        private IEnumerable<NavigationViewItemBase> GetFoldersNavigationViewItems()
        {
            return _folderManager.Folders.Select(folder => new NavigationViewItem
            {
                Content = folder.Name,
                Tag = folder
            }).Cast<NavigationViewItemBase>();
        }

        public void SelectedFolderChanged(FolderModel folder)
        {
            _folderManager.ChangeSelectedFolder(folder);
        }
    }
}
