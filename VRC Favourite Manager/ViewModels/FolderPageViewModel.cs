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
        public ObservableCollection<WorldModel> Worlds => _folderManager.SelectedFolder.Worlds;
        public string FolderName => _folderManager.SelectedFolder.Name;


        public ICommand MoveWorldCommand { get; }
        public ICommand AddFolderCommand { get; }

        public FolderPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;


            MoveWorldCommand = new RelayCommand<Tuple<WorldModel,string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);
        }

        private void MoveWorld(Tuple<WorldModel,string> tuple)
        {
            _folderManager.AddToFolder(tuple.Item1, tuple.Item2);
        }
        private void AddFolder(string folderName)
        {
            _folderManager.AddFolder(folderName);
        }
        public void RemoveFromFolder(WorldModel world)
        {
            _folderManager.RemoveFromFolder(world, FolderName);
        }
    }
}
