using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AddToFolderPopupViewModel : INotifyPropertyChanged
    {
        private readonly FolderManager _folderManager;

        private ObservableCollection<KeyValuePair<string, bool>> _selectedFolders;

        public ObservableCollection<KeyValuePair<string, bool>> SelectedFolders
        {
            get => _selectedFolders;
            set
            {
                _selectedFolders = value;
                OnPropertyChanged(nameof(SelectedFolders));
            }
        }

        public WorldModel SelectedWorld { get; set; }
        public ObservableCollection<FolderModel> Folders => _folderManager.Folders;

        public AddToFolderPopupViewModel(WorldModel selectedWorld)
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _selectedFolders = new ObservableCollection<KeyValuePair<string, bool>>();
            foreach (var folder in Folders)
            {
                // Don't show the unclassified folder in the list
                if (folder.Name != "Unclassified")
                {
                    _selectedFolders.Add(new KeyValuePair<string, bool>(folder.Name,
                        folder.Worlds.Contains(selectedWorld)));
                }
            }

            SelectedWorld = selectedWorld;
        }


        public void AddFolder()
        {
            var newFolderName = "New Folder";
            _folderManager.AddFolder(newFolderName);
            _selectedFolders.Add(new KeyValuePair<string, bool>(newFolderName, false));
        }

        public void CancelSelection()
        {
            _selectedFolders = new ObservableCollection<KeyValuePair<string, bool>>();
            foreach (var folder in Folders)
            {
                // Don't show the unclassified folder in the list
                if (folder.Name != "Unclassified")
                {
                    _selectedFolders.Add(new KeyValuePair<string, bool>(folder.Name,
                        folder.Worlds.Contains(SelectedWorld)));
                }
            }
        }

        public void ConfirmSelection()
        {
            foreach (var folder in Folders)
            {
                if (folder.Name != "Unclassified")
                {
                    if (_selectedFolders.FirstOrDefault(f => f.Key == folder.Name).Value)
                    {
                        _folderManager.AddToFolder(SelectedWorld, folder.Name);
                    }
                    else
                    {
                        _folderManager.RemoveFromFolder(SelectedWorld, folder.Name);
                    }
                }
            }
        }



    public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}