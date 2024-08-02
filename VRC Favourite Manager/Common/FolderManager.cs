using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public FolderManager(JsonManager jsonManager)
        {
            _jsonManager = jsonManager;
            _folders = new ObservableCollection<FolderModel>();

            LoadFolders();

            _selectedFolder = _folders.FirstOrDefault();
        }

        public void LoadFolders()
        {
            if (_jsonManager.FolderConfigExists())
            {
                var savedFolders = _jsonManager.LoadFolders();
                if (savedFolders != null)
                {
                    foreach (var folder in savedFolders)
                    {
                        _folders.Add(folder);
                    }
                }
                else
                {
                    _folders.Add(new FolderModel("Unclassified"));
                    SaveFolders();
                }
            }
            else
            {
                _folders.Add(new FolderModel("Unclassified"));
                SaveFolders();
            }
        }

        public void AddToFolder(WorldModel world, string folderName)
        {
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            if (folder == null)
            {
                folder = new FolderModel(folderName);
                _folders.Add(folder);
            }
            folder.Worlds.Add(world);

            folder = _folders.FirstOrDefault(f => f.Name == "Unclassified");
            folder?.Worlds.Remove(world);

            SaveFolders();
        }
        public void RemoveFromFolder(WorldModel world, string folderName)
        {
            var folder = _folders.FirstOrDefault(f => f.Name == folderName);
            folder?.Worlds.Remove(world);
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
            _folders.Remove(folder);
            SaveFolders();
        }

        private void SaveFolders()
        {
            _jsonManager.SaveFolders(_folders);
        }

        public void ResetFolders()
        {
            _folders.Clear();
            _folders.Add(new FolderModel("Unclassified"));
            SaveFolders();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}