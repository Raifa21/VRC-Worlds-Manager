using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AddToFolderPopupViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<KeyValuePair<string,bool>> Folders { get; set; }
        public WorldModel SelectedWorld { get; set; }

        public AddToFolderPopupViewModel(ObservableCollection<FolderModel> folders, WorldModel selectedWorld)
        {
            Folders = new ObservableCollection<KeyValuePair<string, bool>>();
            foreach (var folder in folders)
            {
                // Don't show the unclassified folder in the list
                if (folder.Name != "Unclassified")
                {
                    Folders.Add(new KeyValuePair<string, bool>(folder.Name, selectedWorld.Folder.Contains(folder.Name)));
                }
            }
            SelectedWorld = selectedWorld;
        }


        public void AddFolder()
        {
            string NewFolderName = "New Folder";
            int i = 1;
            while (Folders.Any(f => f.Key == NewFolderName))
            {
                NewFolderName = $"New Folder ({i})";
                i++;
            }
            Folders.Add(new KeyValuePair<string, bool>(NewFolderName, false));
        }

        public List<string> GetSelectedFolders()
        {
            var selectedFolders = new List<string>();
            foreach (var folder in Folders)
            {
                if (folder.Value)
                {
                    selectedFolders.Add(folder.Key);
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