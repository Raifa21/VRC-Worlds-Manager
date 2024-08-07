using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager.Common
{
    public class FolderManager
    {
        private readonly JsonManager _jsonManager;
        private ObservableCollection<FolderModel> _folders;
        private string _selectedFolder;

        public FolderManager()
        {
            _jsonManager = new JsonManager();
            _folders = new ObservableCollection<FolderModel>();
            LoadFolders();
            foreach (var folder in _folders)
            {
                Debug.WriteLine(folder.Name);
            }
            _selectedFolder = "Unclassified";
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(_folders.FirstOrDefault(f => f.Name == _selectedFolder)));
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
                SaveFolders();
            }
            else
            {
                AddFolder("Unclassified");
                InitializeFolders(worlds);
            }
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(_folders.FirstOrDefault(f => f.Name == _selectedFolder)));
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
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));

            if (folderName == _selectedFolder)
            {
                WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(folder));
            }
        }

        public void RemoveFromFolder(WorldModel world, string folderName)
        {
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            folder?.Worlds.Remove(world);
            var placeWorldInUnclassified = true;
            foreach(var f in _folders)
            {
                if(f.Worlds.Contains(world))
                {
                    placeWorldInUnclassified = false;
                    break;
                }
            }
            if (placeWorldInUnclassified)
            {
                var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                unclassifiedFolder?.Worlds.Add(world);
                if(_selectedFolder == "Unclassified")
                {
                    WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(unclassifiedFolder));
                }
            }
            if (folderName == _selectedFolder)
            {
                WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(folder));
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
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            SaveFolders();
        }

        public void RemoveFolder(FolderModel folder)
        {
            foreach (var world in folder.Worlds)
            {
                var placeWorldInUnclassified = true;
                foreach (var f in _folders)
                {
                    if(f != folder && f.Worlds.Contains(world))
                    {
                        placeWorldInUnclassified = false;
                        break;
                    }
                }
                if (placeWorldInUnclassified)
                {
                    var unclassifiedFolder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
                    unclassifiedFolder?.Worlds.Add(world);
                }
            }
            _folders.Remove(folder);
            SaveFolders();
            if(_selectedFolder == folder.Name)
            {
                _selectedFolder = "Unclassified";
            }
            WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(_folders.FirstOrDefault(f => f.Name == "Unclassified")));
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
        }

        private void SaveFolders()
        {
            _jsonManager.SaveFolders(_folders);
        }

        public void RenameFolder(string newName, string oldName)
        {
            if (oldName != "Unclassified")
            {
                var index = 1;
                while (_folders.Any(f => f.Name == newName))
                {
                    index++;
                    newName = $"{newName} ({index})";
                }
                var folder = _folders.FirstOrDefault(f => f.Name == oldName);
                folder.Name = newName;
                WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
                if (oldName == _selectedFolder)
                {
                    _selectedFolder = newName;
                    WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(_folders.FirstOrDefault(f => f.Name == _selectedFolder)));
                }
                SaveFolders();
            }
        }

        public void ResetFolders()
        {
            _folders.Clear();
            SaveFolders();
            _folders.Add(new FolderModel("Unclassified"));
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(_folders.FirstOrDefault(f => f.Name == "Unclassified")));
        }

        public void ChangeSelectedFolder(string folderName)
        {
            _selectedFolder = folderName;
            WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(_folders.FirstOrDefault(f => f.Name == _selectedFolder)));
        }

        public void GetCurrentState()
        {
            WeakReferenceMessenger.Default.Send(new FolderUpdatedMessage(_folders));
            WeakReferenceMessenger.Default.Send(new SelectedFolderChangedMessage(_folders.FirstOrDefault(f => f.Name == _selectedFolder)));
        }
    }
    public class FolderUpdatedMessage(ObservableCollection<FolderModel> folders)
    {
        public ObservableCollection<FolderModel> Folders { get; set; } = folders;
    }
    public class SelectedFolderChangedMessage(FolderModel folder)
    {
        public FolderModel Folder { get; set; } = folder;
    }
}