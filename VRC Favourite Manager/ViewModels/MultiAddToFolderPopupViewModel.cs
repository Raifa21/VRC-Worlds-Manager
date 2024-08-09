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
    public class MultiAddToFolderPopupViewModel : INotifyPropertyChanged
    {
        private readonly FolderManager _folderManager;

        private ObservableCollection<FolderSelectionNullable> _selectedFolders;

        public ObservableCollection<FolderSelectionNullable> SelectedFolders
        {
            get => _selectedFolders;
            set
            {
                _selectedFolders = value;
                OnPropertyChanged(nameof(SelectedFolders));
            }
        }

        private List<WorldModel> SelectedWorlds { get; set; }

        public MultiAddToFolderPopupViewModel(List<WorldModel> selectedWorlds)
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _selectedFolders = new ObservableCollection<FolderSelectionNullable>();

            SelectedWorlds = selectedWorlds;

            UpdateFolderSelection(_folderManager.Folders);

            Debug.WriteLine(selectedWorlds.Count);
        }

        public void UpdateFolderSelection(ObservableCollection<FolderModel> Folders)
        {
            _selectedFolders.Clear();
            foreach (var folder in Folders)
            {
                // Don't show the unclassified folder in the list
                if (folder.Name != "Unclassified")
                {
                    int count = 0;
                    bool? flag = null;
                    foreach (var SelectedWorld in SelectedWorlds)
                    {
                        if (folder.Worlds.Any(w => w.WorldId == SelectedWorld.WorldId))
                        {
                            count++;
                        }
                    }
                    if(count == SelectedWorlds.Count)
                    {
                        flag = true;
                    }
                    else if(count == 0)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = null;
                    }
                    
                    
                    _selectedFolders.Add(new FolderSelectionNullable()
                    {
                        FolderName = folder.Name,
                        IsCheckedNullable = flag
                    });
                }
            }
        }

        public void AddFolder()
        {
            var newFolderName = "New Folder";
            newFolderName = _folderManager.AddFolder(newFolderName);
            _selectedFolders.Add(new FolderSelectionNullable()
            {
                FolderName = newFolderName,
                IsCheckedNullable = false
            });
        }

        public void CancelSelection()
        {
            UpdateFolderSelection(_folderManager.Folders);
        }

        public void ConfirmSelection()
        {
            foreach (var folder in _folderManager.Folders)
            {   
                if (folder.Name != "Unclassified")
                {
                    if (_selectedFolders.FirstOrDefault(x => x.FolderName == folder.Name).IsCheckedNullable == null)
                    {
                        Debug.WriteLine("Folder " + folder.Name + " is null");
                        continue;
                    }
                    else if (_selectedFolders.FirstOrDefault(x => x.FolderName == folder.Name).IsCheckedNullable == true)
                    {
                        foreach (var SelectedWorld in SelectedWorlds)
                        {
                            Debug.WriteLine("Adding " + SelectedWorld.WorldName + " to " + folder.Name);
                            _folderManager.AddToFolder(SelectedWorld, folder.Name);
                        }
                    }
                    else
                    {
                        foreach (var SelectedWorld in SelectedWorlds)
                        {
                            _folderManager.RemoveFromFolder(SelectedWorld, folder.Name);
                        }
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

    public class FolderSelectionNullable : INotifyPropertyChanged
    {
        private bool? _isCheckedNullable;

        public string FolderName { get; set; }

        public bool? IsCheckedNullable
        {
            get => _isCheckedNullable;
            set
            {
                if (_isCheckedNullable != value)
                {
                    _isCheckedNullable = value;
                    OnPropertyChanged(nameof(_isCheckedNullable));
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