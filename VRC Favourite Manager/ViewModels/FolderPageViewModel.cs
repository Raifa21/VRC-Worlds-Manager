﻿using System;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Common;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using System.Linq;

namespace VRC_Favourite_Manager.ViewModels
{
    public class FolderPageViewModel : ViewModelBase
    {
        private readonly FolderManager _folderManager;
        private readonly WorldManager _worldManager;
        public ObservableCollection<WorldModel> Worlds { get; private set; }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set
            {
                _isRenaming = value;
                OnPropertyChanged(nameof(IsRenaming));
            }
        }

        private bool _isSelecting;
        public bool IsSelecting
        {
            get => _isSelecting;
            set
            {
                _isSelecting = value;
                OnPropertyChanged(nameof(IsSelecting));
            }
        }

        private string _folderName;
        public string FolderName
        {
            get => _folderName;
            set
            {
                _folderName = value;
                OnPropertyChanged(nameof(FolderName));
            }
        }
        public ICommand MoveWorldCommand { get; }
        public ICommand AddFolderCommand { get; }

        public bool ChangeFolderNameLang{ get; set; }

        public string ViewDetailsText { get; set; }
        public string MoveToAnotherFolderText { get; set; }
        public string RemoveFromFolderText { get; set; }
        public bool IsUnclassified { get; set; }

        public FolderPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            Worlds = new ObservableCollection<WorldModel>();
            FolderName = _folderManager?.SelectedFolder?.Name;
            _isRenaming = false;

            string languageCode = Application.Current.Resources["languageCode"] as string;
            ChangeFolderNameLang = (FolderName == "Unclassified" && languageCode == "ja");

            MoveWorldCommand = new RelayCommand<Tuple<WorldModel, string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);

            UpdateWorlds();

            ViewDetailsText = languageCode == "ja" ? "詳細" : "View Details";
            MoveToAnotherFolderText = languageCode == "ja" ? "別のフォルダに移動" : "Move to another folder";
            RemoveFromFolderText = languageCode == "ja" ? "フォルダから削除" : "Remove from folder";
            IsUnclassified = FolderName == "Unclassified";


            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Log.Information("Folder updated");
                OnFolderUpdated();
            });
        }

        public void RenameCancel()
        {
            IsRenaming = false;
            FolderName = _folderManager.SelectedFolder.Name;
        }
        public void RenameFolder(string newFolderName)
        {
            var newName = _folderManager.RenameFolder(newFolderName, _folderName);
            Log.Information("Renamed folder: " + newName);
            FolderName = newName;
            IsRenaming = false;
        }

        private void OnFolderUpdated()
        {
            Log.Information("Selected folder changed");
            UpdateWorlds();
        }

        public void UpdateWorlds()
        {
            if (_folderManager.SelectedFolder == null) return;

            var updatedWorlds = _folderManager.SelectedFolder.Worlds.ToList();

            // Remove any worlds that are no longer in the updated list
            for (int i = Worlds.Count - 1; i >= 0; i--)
            {
                if (!updatedWorlds.Contains(Worlds[i]))
                {
                    Worlds.RemoveAt(i);
                }
            }

            // Add new worlds that don't exist in the current list
            foreach (var world in updatedWorlds)
            {
                if (!Worlds.Contains(world))
                {
                    Worlds.Add(world);
                }
            }
        }

        private void MoveWorld(Tuple<WorldModel,string> tuple)
        {
            _folderManager.AddToFolder(tuple.Item1, tuple.Item2);
            UpdateWorlds();

        }
        private void AddFolder(string folderName)
        {
            _folderManager.AddFolder(folderName);
            UpdateWorlds();
        }

        public async Task RefreshWorldsAsync()
        {
            await _worldManager.CheckForNewWorldsAsync();
            UpdateWorlds();
        }
        public void Dispose()
        {
            WeakReferenceMessenger.Default.Unregister<FolderUpdatedMessage>(this);
        }
    }
}
