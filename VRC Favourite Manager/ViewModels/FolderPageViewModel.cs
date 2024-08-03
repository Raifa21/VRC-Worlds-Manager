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
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager.ViewModels
{
    public class FolderPageViewModel : ViewModelBase
    {
        private readonly FolderManager _folderManager;
        private readonly WorldManager _worldManager;
        public ObservableCollection<WorldModel> Worlds { get; private set; }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set
            {
                _isRenaming = value;
                OnPropertyChanged(nameof(IsRenaming));
            }
        }

        private string _folderName;
        public string FolderName
        {
            get => _folderName;
            set
            {
                _folderName = value;
                OnPropertyChanged(nameof(FolderName));
            }
        }

        public ICommand MoveWorldCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand RenamingCommand { get; }
        public ICommand RefreshCommand { get; }

        public FolderPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            Worlds = new ObservableCollection<WorldModel>();
            _folderName = _folderManager?.SelectedFolder?.Name;
            _isRenaming = false;

            _folderManager.PropertyChanged += OnFolderManagerPropertyChanged;

            MoveWorldCommand = new RelayCommand<Tuple<WorldModel, string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);
            RenamingCommand = new RelayCommand(RenamingFolder);
            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());

            UpdateWorlds();
        }
        public void RemoveFromFolder(WorldModel world)
        {
            _folderManager.RemoveFromFolder(world, _folderName);
            UpdateWorlds();
        }
        public void RenameFolder()
        {
            _folderManager.RenameFolder(FolderName);
        }

        private void OnFolderManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FolderManager.SelectedFolder))
            {
                _folderName = _folderManager.SelectedFolder?.Name;
                OnPropertyChanged(nameof(FolderName));
                UpdateWorlds();
            }
        }

        private void UpdateWorlds()
        {
            Worlds.Clear();
            if (_folderManager.SelectedFolder != null)
            {
                foreach (var world in _folderManager.SelectedFolder.Worlds)
                {
                    Worlds.Add(world);
                }
            }
        }

        private void MoveWorld(Tuple<WorldModel,string> tuple)
        {
            _folderManager.AddToFolder(tuple.Item1, tuple.Item2);
            UpdateWorlds();

        }
        private void AddFolder(string folderName)
        {
            _folderManager.AddFolder(folderName);
            UpdateWorlds();
        }
        private void RenamingFolder()
        {
            _isRenaming = true;
        }
        private async Task RefreshWorldsAsync()
        {
            await _worldManager.CheckForNewWorldsAsync();
        }
    }
}
