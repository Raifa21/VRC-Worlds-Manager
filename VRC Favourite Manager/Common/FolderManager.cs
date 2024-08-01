using System.Collections.ObjectModel;
using System.Linq;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.Common
{
    public class FolderManager
    {
        private readonly JsonManager _jsonManager;
        public ObservableCollection<FolderModel> Folders { get; private set; }

        public FolderManager(JsonManager jsonManager)
        {
            _jsonManager = jsonManager;
            Folders = new ObservableCollection<FolderModel>();

            LoadFolders();
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

        public void AddFolder(string folderName)
        {
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
    }
}