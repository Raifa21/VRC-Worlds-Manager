using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
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
        }

        public void LoadFolders()
        {
            if (_jsonManager.FolderConfigExists())
            {
                var savedFolders = _jsonManager.LoadFolders();
                if (savedFolders != null && savedFolders.Count > 0)
                {
                    Log.Information("Loading folders");
                    foreach (var folder in savedFolders)
                    {
                        _folders.Add(folder);
                    }
                    if (_folders.All(f => f.Name != "Hidden"))
                    {
                        AddFolder("Hidden");
                    }
                    if(_folders.All(f => f.Name != "Unclassified"))
                    {
                        AddFolder("Unclassified");
                    }
                }
                else
                {
                    Log.Information("No folders found");
                    AddFolder("Unclassified");
                    AddFolder("Hidden");
                    SaveFolders();
                }
            }
            else
            {
                Log.Information("File not found");
                AddFolder("Unclassified");
                AddFolder("Hidden");
                SaveFolders();
            }
        }
        public void InitializeFolders(ObservableCollection<WorldModel> worlds)
        {
            Log.Information("Initializing folders");
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
            if(_folders.All(f => f.Name != "Hidden"))
            {
                AddFolder("Hidden");
            }
            SaveFolders();
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            Log.Information("Folders initialized");
        }

        public void AddToFolder(WorldModel world, string folderName)
        {
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            if (folder != null)
            {
                var existingWorld = folder.Worlds.FirstOrDefault(w => w.WorldId == world.WorldId);
                if (existingWorld != null)
                {
                    // Update existing world details
                    existingWorld.AuthorName = world.AuthorName;
                    existingWorld.Capacity = world.Capacity;
                    existingWorld.Description = world.Description;
                    existingWorld.Favorites = world.Favorites;
                    existingWorld.LastUpdate = world.LastUpdate;
                    existingWorld.ThumbnailImageUrl = world.ThumbnailImageUrl;
                    existingWorld.Visits = world.Visits;
                    existingWorld.WorldName = world.WorldName;
                }
                else
                {
                    // Insert new world at the top
                    folder.Worlds.Insert(0, world);
                }

                if (folderName != "Unclassified")
                {
                    var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                    unclassifiedFolder?.Worlds.Remove(world);
                    if(SelectedFolder?.Name == "Unclassified")
                    {
                        _selectedFolder = unclassifiedFolder;
                    }
                }
                if (SelectedFolder?.Name == folderName)
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

            if (folderName == SelectedFolder?.Name)
            {
                _selectedFolder = folder;
            }

            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
        }
        public void UpdateWorldInFolders(WorldModel world)
        {
            foreach (var folder in _folders)
            {
                var existingWorld = folder.Worlds.FirstOrDefault(w => w.WorldId == world.WorldId);
                if (existingWorld != null)
                {
                    existingWorld.AuthorName = world.AuthorName;
                    existingWorld.Capacity = world.Capacity;
                    existingWorld.Description = world.Description;
                    existingWorld.Favorites = world.Favorites;
                    existingWorld.LastUpdate = world.LastUpdate;
                    existingWorld.ThumbnailImageUrl = world.ThumbnailImageUrl;
                    existingWorld.Visits = world.Visits;
                    existingWorld.WorldName = world.WorldName;
                    if (folder.Name == SelectedFolder?.Name)
                    {
                        SelectedFolder = folder;
                        WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
                    }
                }
            }
            
        }

        public void RemoveFromFolder(WorldModel world, string folderName)
        {
            var folder = _folders.First(f => f.Name == folderName);
            var worldToRemove = folder.Worlds.FirstOrDefault(w => w.WorldId == world.WorldId);
            if (worldToRemove != null)
            {
                folder.Worlds.Remove(worldToRemove);
            }
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
                unclassifiedFolder?.Worlds.Insert(0, world);
                if (SelectedFolder?.Name == "Unclassified")
                {
                    _selectedFolder = unclassifiedFolder;
                }
            }
            if(folderName == SelectedFolder?.Name)
            {
                _selectedFolder = folder;
            }

            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));


            SaveFolders();
        }

        public void MoveToHiddenFolder(WorldModel world)
        {
            //add to hidden folder
            _folders.First(f => f.Name == "Hidden").Worlds.Insert(0, world);


            foreach (var folder in _folders)
            {
                if (folder.Name == "Hidden")
                {
                    continue;
                }
                if (folder.Worlds.Any(w => w.WorldId == world.WorldId))
                {
                    folder.Worlds.Remove(world);
                }
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

        public void RemoveFolder(string folderName)
        {
            Log.Information($"Removing folder {folderName}");
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
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
                    unclassifiedFolder?.Worlds.Insert(0, world);
                }
            }
            _folders.Remove(folder);
            SaveFolders();
            if(SelectedFolder?.Name == folder.Name || SelectedFolder?.Name == "Unclassified")
            {
                SelectedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
            }

            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
        }

        public string RenameFolder(string newName, string oldName)
        {
            if (newName == oldName) return oldName;
            if (oldName != "Unclassified" && oldName != "Hidden")
            {
                var _userInputHandler = new UserInputHandler();
                newName = _userInputHandler.SanitizeUserInput(newName);
                if(string.IsNullOrWhiteSpace(newName))
                {
                    return oldName;
                }
                var index = 0;
                var name = newName;
                while (_folders.Any(f => f.Name == newName))
                {
                    if (newName == oldName) return oldName;
                    index++;
                    newName = $"{name} ({index})";
                }
                var folder = _folders.FirstOrDefault(f => f.Name == oldName);
                if (folder != null)
                {
                    folder.Name = newName;
                }

                if (SelectedFolder?.Name == oldName)
                {
                    SelectedFolder = folder;
                }

                WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            }
            Log.Information($"Renaming {oldName} to {newName}");
            SaveFolders();
            return newName;
        }

        private void SaveFolders()
        {
            _jsonManager.SaveFolders(_folders);
        }

        public void ResetFolders()
        {
            _folders.Clear();
            SaveFolders();
            _folders.Add(new FolderModel("Unclassified"));
            SelectedFolder = _folders.FirstOrDefault();
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
        }

        public void ChangeSelectedFolder(FolderModel folder)
        {
            SelectedFolder = folder;
            OnPropertyChanged(nameof(SelectedFolder));
            Log.Information($"Selected folder: {SelectedFolder.Name}");
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