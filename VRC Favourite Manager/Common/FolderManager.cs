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
        public ObservableCollection<FolderModel> Folders { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
            Folders = new ObservableCollection<FolderModel>();

            LoadFolders();

            _selectedFolder = Folders.FirstOrDefault();
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
                        Folders.Add(folder);
                    }
                }
                else
                {
                    Folders.Add(new FolderModel("Unclassified"));
                    SaveFolders();
                }
            }
            else
            {
                Folders.Add(new FolderModel("Unclassified"));
                SaveFolders();
            }
        }

        public void AddToFolder(WorldModel world, string folderName)
        {
            var folder = Folders.FirstOrDefault(f => f.Name == folderName);
            if (folder == null)
            {
                folder = new FolderModel(folderName);
                Folders.Add(folder);
            }
            folder.Worlds.Add(world);
            foreach (var f in Folders)
            {
                if (f.Name == "Unclassified")
                {
                    f.Worlds.Remove(world);
                }
            }
            SaveFolders();
        }
        public void RemoveFromFolder(WorldModel world, string folderName)
        {
            var folder = Folders.FirstOrDefault(f => f.Name == folderName);
            folder?.Worlds.Remove(world);
            SaveFolders();
        }

        public void AddFolder(string folderName)
        {
            var index = 0;
            //check for duplicates and add a (num) suffix if needed
            while (Folders.Any(f => f.Name == folderName))
            {
                index++;
                folderName = $"{folderName} ({index})";
            }
            Folders.Add(new FolderModel(folderName));
            SaveFolders();
        }

        public void RemoveFolder(FolderModel folder)
        {
            Folders.Remove(folder);
            SaveFolders();
        }

        private void SaveFolders()
        {
            _jsonManager.SaveFolders(Folders);
        }

        public void ResetFolders()
        {
            Folders.Clear();
            Folders.Add(new FolderModel("Unclassified"));
            SaveFolders();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}