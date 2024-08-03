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
        public ObservableCollection<WorldModel> Worlds { get; private set; }
        public string FolderName { get; private set; }


        public ICommand MoveWorldCommand { get; }
        public ICommand AddFolderCommand { get; }

        public FolderPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;

            Worlds = new ObservableCollection<WorldModel>();
            FolderName = _folderManager.SelectedFolder?.Name;

            _folderManager.PropertyChanged += OnFolderManagerPropertyChanged;

            MoveWorldCommand = new RelayCommand<Tuple<WorldModel, string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);

            UpdateWorlds();
        }

        private void OnFolderManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FolderManager.SelectedFolder))
            {
                FolderName = _folderManager.SelectedFolder?.Name;
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
        public void RemoveFromFolder(WorldModel world)
        {
            _folderManager.RemoveFromFolder(world, FolderName);
            UpdateWorlds();
        }
    }
}
