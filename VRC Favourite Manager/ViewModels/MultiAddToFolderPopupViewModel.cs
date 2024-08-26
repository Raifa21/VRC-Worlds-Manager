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

        private ObservableCollection<FolderSelection_Indeterminatable> _selectedFolders;

        public ObservableCollection<FolderSelection_Indeterminatable> SelectedFolders
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
            _selectedFolders = new ObservableCollection<FolderSelection_Indeterminatable>();

            SelectedWorlds = selectedWorlds;

            UpdateFolderSelection(_folderManager.Folders);

            Debug.WriteLine(selectedWorlds.Count);

        }

        public void UpdateFolderSelection(ObservableCollection<FolderModel> Folders)
        {
            _selectedFolders.Clear();
            foreach (var folder in Folders)
            {
                // Don't show the unclassified folder amd the Hidden folder in the list
                if (folder.Name != "Unclassified" && folder.Name != "Hidden")
                {
                    int count = 0;
                    bool flag_checked = true;
                    bool flag_indeterminate = false;
                    foreach (var SelectedWorld in SelectedWorlds)
                    {
                        if (folder.Worlds.Any(w => w.WorldId == SelectedWorld.WorldId))
                        {
                            count++;
                        }
                    }
                    if(count == SelectedWorlds.Count)
                    {
                        flag_checked = true;
                    }
                    else if(count == 0)
                    {
                        flag_checked = false;
                    }
                    else
                    {
                        flag_indeterminate = true;
                    }
                    
                    
                    _selectedFolders.Add(new FolderSelection_Indeterminatable()
                    {
                        FolderName = folder.Name,
                        IsChecked = flag_checked,
                        IsIndeterminate = flag_indeterminate
                    });
                }
            }
        }

        public void AddFolder()
        {
            var newFolderName = "New Folder";
            newFolderName = _folderManager.AddFolder(newFolderName);
            _selectedFolders.Add(new FolderSelection_Indeterminatable()
            {
                FolderName = newFolderName,
                IsChecked = false,
                IsIndeterminate = false
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
                if (folder.Name != "Unclassified" && folder.Name != "Hidden")
                {
                    if (_selectedFolders.FirstOrDefault(x => x.FolderName == folder.Name).IsIndeterminate)
                    {
                        continue;
                    }
                    else if (_selectedFolders.FirstOrDefault(x => x.FolderName == folder.Name).IsChecked == true)
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

    public class FolderSelection_Indeterminatable
    {
        public string FolderName { get; set; }

        public bool IsIndeterminate { get; set; }

        public bool IsChecked { get; set; }
    }
}