using System;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Common;
using CommunityToolkit.Mvvm.Messaging;

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
            _folderName = _folderManager?.SelectedFolder?.Name;
            _isRenaming = false;

            ChangeFolderNameLang = (_folderName == "Unclassified" && Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja");

            MoveWorldCommand = new RelayCommand<Tuple<WorldModel, string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);

            UpdateWorlds();

            ViewDetailsText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "詳細" : "View Details";
            MoveToAnotherFolderText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "別のフォルダに移動" : "Move to another folder";
            RemoveFromFolderText = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "フォルダから削除" : "Remove from folder";
            IsUnclassified = _folderName == "Unclassified";


            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Debug.WriteLine("Folder updated");
                OnFolderUpdated();
            });
        }
        public void RenameFolder(string newFolderName)
        {
            _folderManager.RenameFolder(newFolderName, _folderName);
            Debug.WriteLine("Renamed folder: " + newFolderName);
        }

        private void OnFolderUpdated()
        {
            Debug.WriteLine("Selected folder changed");
            _folderName = _folderManager.SelectedFolder?.Name;
            UpdateWorlds();
        }

        public void UpdateWorlds()
        {
            Worlds.Clear();
            if (_folderManager.SelectedFolder != null)
            {
                foreach (var world in _folderManager.SelectedFolder.Worlds)
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
