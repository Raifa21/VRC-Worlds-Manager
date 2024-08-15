using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Views;

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
            Debug.WriteLine("Initializing folders");
            var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
            if (unclassifiedFolder != null)
            {
                foreach (var world in worlds)
                {
                    unclassifiedFolder.Worlds.Add(world);
                }
            }
            else
            {
                AddFolder("Unclassified");
                InitializeFolders(worlds);
            }
            SaveFolders();
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            Debug.WriteLine("Folders initialized");
        }

        public void AddToFolder(WorldModel world, string folderName)
        {
            Debug.WriteLine($"Adding {world.WorldName} to {folderName}");
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            if (folder != null)
            {
                if (folder.Worlds.All(w => w.WorldId != world.WorldId))
                {
                    Debug.WriteLine("Adding to folder");
                    folder.Worlds.Add(world);
                }
                else
                {
                    Debug.WriteLine("World already exists in folder");
                    var oldWorld = folder.Worlds.FirstOrDefault(w => w.WorldId == world.WorldId);
                    if (oldWorld != null)
                    {
                        oldWorld.AuthorName = world.AuthorName;
                        oldWorld.Capacity = world.Capacity;
                        oldWorld.Description = world.Description;
                        oldWorld.Favorites = world.Favorites;
                        oldWorld.LastUpdate = world.LastUpdate;
                        oldWorld.ThumbnailImageUrl = world.ThumbnailImageUrl;
                        oldWorld.Visits = world.Visits;
                        oldWorld.WorldName = world.WorldName;
                    }
                }

                if (folderName != "Unclassified")
                {
                    var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                    unclassifiedFolder?.Worlds.Remove(world);
                    if(SelectedFolder.Name == "Unclassified")
                    {
                        _selectedFolder = unclassifiedFolder;
                    }
                }

                if (SelectedFolder.Name == folderName)
                {
                    _selectedFolder = _folders.FirstOrDefault(f => f.Name == folderName);
                }
                SaveFolders();
            }
            else
            {
                AddFolder(folderName);
                AddToFolder(world, folderName);
            }

            if (folderName == SelectedFolder.Name)
            {
                _selectedFolder = folder;
            }

            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
        }

        public void RemoveFromFolder(WorldModel world, string folderName)
        {
            Debug.WriteLine($"Removing {world.WorldName} from {folderName}");
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            folder.Worlds.Remove(world);
            Debug.WriteLine($"Worlds in {folderName}: {folder.Worlds.Count}");
            var PlaceWorldInUnclassified = true;
            foreach(var f in _folders)
            {
                if (f.Worlds.Any(w => w.WorldId == world.WorldId))
                {
                    PlaceWorldInUnclassified = false;
                    break;
                }
            }
            if (PlaceWorldInUnclassified)
            {
                var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                unclassifiedFolder?.Worlds.Add(world);
                if (SelectedFolder.Name == "Unclassified")
                {
                    _selectedFolder = unclassifiedFolder;
                }
            }

            if (folderName == SelectedFolder.Name)
            {
                _selectedFolder = folder;
            }

            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));


            SaveFolders();
        }

        public string AddFolder(string folderName)
        {
            var index = 0;
            var name = folderName;
            while (_folders.Any(f => f.Name == folderName))
            {
                index++;
                folderName = $"{name} ({index})";
            }
            _folders.Add(new FolderModel(folderName));
            
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            SaveFolders();

            return folderName;
        }

        public void RemoveFolder(FolderModel folder)
        {
            foreach (var world in folder.Worlds)
            {
                var PlaceWorldInUnclassified = true;
                foreach (var f in _folders)
                {
                    if(f != folder && f.Worlds.Any(w => w.WorldId == world.WorldId))
                    {
                        PlaceWorldInUnclassified = false;
                        break;
                    }
                }
                if(PlaceWorldInUnclassified)
                {
                    var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                    unclassifiedFolder?.Worlds.Add(world);
                }
            }
            _folders.Remove(folder);
            SaveFolders();
            if(_selectedFolder == folder || _selectedFolder.Name == "Unclassified")
            {
                _selectedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
            }

            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
        }

        private void SaveFolders()
        {
            _jsonManager.SaveFolders(_folders);
        }

        public void RenameFolder(string newName, string oldName)
        {
            if(newName == oldName) return;
            if (oldName != "Unclassified")
            {
                var index = 0;
                var name = newName;
                while (_folders.Any(f => f.Name == newName))
                {
                    if (newName == oldName) return;
                    index++;
                    newName = $"{name} ({index})";
                }
                var folder = _folders.FirstOrDefault(f => f.Name == oldName);
                folder.Name = newName;

                if (_selectedFolder.Name == oldName)
                {
                    _selectedFolder = folder;
                }

                WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            }
        }

        public void ResetFolders()
        {
            _folders.Clear();
            SaveFolders();
            _folders.Add(new FolderModel("Unclassified"));
            _selectedFolder = _folders.FirstOrDefault();
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
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
    public class FolderUpdatedMessage(ObservableCollection<FolderModel> folders)
    {
        public ObservableCollection<FolderModel> Folders { get; set; } = folders;
    }
}