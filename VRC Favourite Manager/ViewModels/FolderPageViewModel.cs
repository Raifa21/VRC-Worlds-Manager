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
using CommunityToolkit.Mvvm.Messaging;

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
            _isRenaming = false;

            MoveWorldCommand = new RelayCommand<Tuple<WorldModel, string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);
            RenamingCommand = new RelayCommand(RenamingFolder);
            RefreshCommand = new RelayCommand(async () => await RefreshWorldsAsync());

            _folderManager.GetCurrentState();

            WeakReferenceMessenger.Default.Register<SelectedFolderChangedMessage>(this, (r, m) =>
            {
                _folderName = m.Folder.Name;
                UpdateWorlds(m.Folder);
            });
        }
        public void RemoveFromFolder(WorldModel world)
        {
            _folderManager.RemoveFromFolder(world, _folderName);
        }
        public void RenameFolder(string newFolderName)
        {
            _folderManager.RenameFolder(newFolderName, _folderName);
            Debug.WriteLine("Renamed folder: " + newFolderName);
        }

        private void UpdateWorlds(FolderModel folder)
        {
            Worlds.Clear();
            foreach (var world in folder.Worlds)
            {
                Worlds.Add(world);
            }
        }

        private void MoveWorld(Tuple<WorldModel,string> tuple)
        {
            _folderManager.AddToFolder(tuple.Item1, tuple.Item2);

        }
        private void AddFolder(string folderName)
        {
            _folderManager.AddFolder(folderName);
        }
        private void RenamingFolder()
        {
            _isRenaming = true;
            Debug.WriteLine(IsRenaming);
        }
        private async Task RefreshWorldsAsync()
        {
            await _worldManager.CheckForNewWorldsAsync();
        }
    }
}
