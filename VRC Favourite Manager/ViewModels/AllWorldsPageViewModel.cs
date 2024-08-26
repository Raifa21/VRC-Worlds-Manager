using System;
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
    public class AllWorldsPageViewModel : ViewModelBase
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
        public ICommand MoveWorldCommand { get; }
        public ICommand AddFolderCommand { get; }

        public string ViewDetailsText { get; set; }
        public string MoveToAnotherFolderText { get; set; }
        public string RemoveFromFolderText { get; set; }

        public AllWorldsPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            Worlds = new ObservableCollection<WorldModel>();


            MoveWorldCommand = new RelayCommand<Tuple<WorldModel, string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);

            UpdateWorlds();

            ViewDetailsText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "詳細" : "View Details";
            MoveToAnotherFolderText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "別のフォルダに移動" : "Move to another folder";
            RemoveFromFolderText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "フォルダから削除" : "Remove from folder";


            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("Folder updated");
                UpdateWorlds();
            });
        }

        public void UpdateWorlds()
        {
            Worlds.Clear();
            foreach (var folder in _folderManager.Folders)
            {
                if (folder.Name == "Hidden")
                {
                    continue;
                }
                foreach (var world in folder.Worlds)
                {
                    Worlds.Add(world);
                }
            }
        }

        private void MoveWorld(Tuple<WorldModel,string> tuple)
        {
            _folderManager.AddToFolder(tuple.Item1, tuple.Item2);
        }
        private void AddFolder(string folderName)
        {
            _folderManager.AddFolder(folderName);
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
