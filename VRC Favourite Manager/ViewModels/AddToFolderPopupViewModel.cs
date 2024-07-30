using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AddToFolderPopupViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FolderModel> Folders { get; set; }
        public WorldModel SelectedWorld { get; set; }
        private string _newFolderName;

        public string NewFolderName
        {
            get => _newFolderName;
            set
            {
                _newFolderName = value;
                OnPropertyChanged();
            }
        }

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
            if (!string.IsNullOrWhiteSpace(NewFolderName))
            {
                if (Folders.All(f => f.Name != NewFolderName))
                {
                    Folders.Add(new FolderModel(NewFolderName));
                    NewFolderName = string.Empty; // Clear the text box after adding the folder
                }
                else
                {
                    // Handle duplicate folder name by adding a (num) suffix
                    int i = 1;
                    while (Folders.Any(f => f.Name == NewFolderName + $" ({i})"))
                    {
                        i++;
                    }
                    Folders.Add(new FolderModel(NewFolderName + $" ({i})"));
                }
            }
            else
            {
                Folders.Add(new FolderModel("New Folder"));
            }
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