using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AddToFolderPopupViewModel : INotifyPropertyChanged
    {
        private readonly FolderManager _folderManager;

        private ObservableCollection<FolderSelection> _selectedFolders;

        public ObservableCollection<FolderSelection> SelectedFolders
        {
            get => _selectedFolders;
            set
            {
                _selectedFolders = value;
                OnPropertyChanged(nameof(SelectedFolders));
            }
        }

        public WorldModel SelectedWorld { get; set; }
        public ObservableCollection<FolderModel> Folders;

        public AddToFolderPopupViewModel(WorldModel selectedWorld)
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _selectedFolders = new ObservableCollection<FolderSelection>();

            UpdateFolderSelection(_folderManager.Folders);

            SelectedWorld = selectedWorld;
        }

        public void UpdateFolderSelection(ObservableCollection<FolderModel> Folders)
        {
            _selectedFolders.Clear();
            foreach (var folder in Folders)
            {
                // Don't show the unclassified folder in the list
                if (folder.Name != "Unclassified")
                {
                    _selectedFolders.Add(new FolderSelection()
                    {
                        FolderName = folder.Name,
                        IsChecked = folder.Worlds.Contains(SelectedWorld)
                    });
                }
            }
        }

        public void AddFolder()
        {
            var newFolderName = "New Folder";
            newFolderName = _folderManager.AddFolder(newFolderName);
            _selectedFolders.Add(new FolderSelection()
            {
                FolderName = newFolderName,
                IsChecked = false
            });
        }

        public void CancelSelection()
        {
            UpdateFolderSelection(_folderManager.Folders);
        }

        public void ConfirmSelection()
        {
            foreach (var folder in Folders)
            {
                if (folder.Name != "Unclassified")
                {
                    if (_selectedFolders.FirstOrDefault(x => x.FolderName == folder.Name).IsChecked)
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
    public class FolderSelection : INotifyPropertyChanged
    {
        private bool _isChecked;

        public string FolderName { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}