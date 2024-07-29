using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AddToFolderPopupViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FolderModel> Folders { get; set; }
        public WorldModel SelectedWorld { get; set; }

        public AddToFolderPopupViewModel(ObservableCollection<FolderModel> folders, WorldModel selectedWorld)
        {
            Folders = folders;
            SelectedWorld = selectedWorld;
            LoadFolderSelections();
        }

        private void LoadFolderSelections()
        {
            foreach (var folder in Folders)
            {
                if (SelectedWorld.Folder.Contains(folder.Name))
                {
                    folder.IsSelected = true;
                }
            }
        }

        public void AddFolder()
        {
            Folders.Add(new FolderModel("New Folder"));
        }

        public List<string> GetSelectedFolders()
        {
            var selectedFolders = new List<string>();
            foreach (var folder in Folders)
            {
                if (folder.IsSelected)
                {
                    selectedFolders.Add(folder.Name);
                }
            }
            return selectedFolders;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}