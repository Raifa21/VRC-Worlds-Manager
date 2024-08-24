﻿using System;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using System.Linq;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace VRC_Favourite_Manager.ViewModels
{
    public class HiddenFolderPageViewModel : ViewModelBase
    {
        private readonly FolderManager _folderManager;
        private readonly WorldManager _worldManager;
        public ObservableCollection<WorldModel> Worlds { get; private set; }


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

        public string ViewDetailsText { get; set; }
        public string RestoreText { get; set; }

        public HiddenFolderPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            Worlds = new ObservableCollection<WorldModel>();


            UpdateWorlds();

            ViewDetailsText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "詳細" : "View Details";
            RestoreText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "復元" : "Restore";


            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("Folder updated");
                UpdateWorlds();
            });
        }

        public void UpdateWorlds()
        {
            Worlds.Clear();
            var hiddenWorlds = _folderManager.Folders.First(f => f.Name == "Hidden").Worlds;
            foreach (var world in hiddenWorlds)
            {
                Worlds.Add(world);
            }
        }

        public void RestoreWorld(WorldModel world)
        {
            _folderManager.RemoveFromFolder(world, "Hidden");
            _folderManager.AddToFolder(world, "Unclassified");
            UpdateWorlds();
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default.Unregister<FolderUpdatedMessage>(this);
        }
    }
}