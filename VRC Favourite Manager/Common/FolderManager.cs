using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.Common
{
    public class FolderManager : INotifyPropertyChanged
    {
        private readonly JsonManager _jsonManager;

        public event PropertyChangedEventHandler PropertyChanged;


        private ObservableCollection<FolderModel> _folders;

        public ObservableCollection<FolderModel> Folders
        {
            get => _folders;
            set
            {
                _folders = value;
                OnPropertyChanged(nameof(Folders));
            }
        }


        private FolderModel _selectedFolder;
        public FolderModel SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (_selectedFolder != value)
                {
                    _selectedFolder = value;
                    OnPropertyChanged(nameof(SelectedFolder));
                }
            }
        }

        public FolderManager()
        {
            _jsonManager = new JsonManager();
            _folders = new ObservableCollection<FolderModel>();
            LoadFolders();
            foreach (var folder in _folders)
            {
                Debug.WriteLine(folder.Name);
            }
            _selectedFolder = _folders.FirstOrDefault();

            PrintFolders();
        }

        public void LoadFolders()
        {
            if (_jsonManager.FolderConfigExists())
            {
                var savedFolders = _jsonManager.LoadFolders();
                if (savedFolders != null && savedFolders.Count > 0)
                {
                    Debug.WriteLine("Loading folders");
                    foreach (var folder in savedFolders)
                    {
                        _folders.Add(folder);
                    }
                }
                else
                {
                    Debug.WriteLine("No folders found");
                    AddFolder("Unclassified");
                    SaveFolders();
                }
            }
            else
            {
                Debug.WriteLine("File not found");
                AddFolder("Unclassified");
                SaveFolders();
            }

            PrintFolders();
        }
        public void InitializeFolders(ObservableCollection<WorldModel> worlds)
        {
            var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
            if (unclassifiedFolder != null)
            {
                foreach (var world in worlds)
                {
                    if (!unclassifiedFolder.Worlds.Contains(world))
                    {
                        unclassifiedFolder.Worlds.Add(world);
                    }
                }
            }
            else
            {
                AddFolder("Unclassified");
                InitializeFolders(worlds);
            }
            SaveFolders();
            PrintFolders();
        }

        public void AddToFolder(WorldModel world, string folderName)
        {
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            if (folder != null)
            {
                if (!folder.Worlds.Contains(world))
                {
                    folder.Worlds.Add(world);
                }
                var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                unclassifiedFolder?.Worlds.Remove(world);
                SaveFolders();
            }
            else
            {
                AddFolder(folderName);
                AddToFolder(world, folderName);
            }
        }

        public void RemoveFromFolder(WorldModel world, string folderName)
        {
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            folder?.Worlds.Remove(world);
            var PlaceWorldInUnclassified = true;
            foreach(var f in _folders)
            {
                if(f.Worlds.Contains(world))
                {
                    PlaceWorldInUnclassified = false;
                    break;
                }
            }
            if (PlaceWorldInUnclassified)
            {
                var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                unclassifiedFolder?.Worlds.Add(world);
            }
            SaveFolders();
        }

        public void AddFolder(string folderName)
        {
            var index = 1;
            while (_folders.Any(f => f.Name == folderName))
            {
                index++;
                folderName = $"{folderName} ({index})";
            }
            _folders.Add(new FolderModel(folderName));
            SaveFolders();
        }

        public void RemoveFolder(FolderModel folder)
        {
            foreach (var world in folder.Worlds)
            {
                var PlaceWorldInUnclassified = true;
                foreach (var f in _folders)
                {
                    if(f != folder && f.Worlds.Contains(world))
                    {
                        PlaceWorldInUnclassified = false;
                        break;
                    }
                }
            }
            var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
            foreach (var world in folder.Worlds)
            {
                unclassifiedFolder?.Worlds.Add(world);
            }
            _folders.Remove(folder);
            SaveFolders();
            if(_selectedFolder == folder)
            {
                _selectedFolder = _folders.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        private void SaveFolders()
        {
            _jsonManager.SaveFolders(_folders);
        }

        public void RenameFolder(string newName)
        {
            if (_selectedFolder.Name != "Unclassified")
            {
                _selectedFolder.Name = newName;
                SaveFolders();
            }
        }

        public void ResetFolders()
        {
            _folders.Clear();
            SaveFolders();
            _folders.Add(new FolderModel("Unclassified"));
        }

        public void PrintFolders()
        {
            foreach (var folder in _folders) {
                Debug.WriteLine(folder.Name);
                foreach (var world in folder.Worlds)
                {
                    Debug.WriteLine(world.WorldName);
                }
            }
        }

        public void ChangeSelectedFolder(FolderModel folder)
        {
            _selectedFolder = folder;
            OnPropertyChanged(nameof(SelectedFolder));
            Debug.WriteLine($"Selected folder: {SelectedFolder.Name}");
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}