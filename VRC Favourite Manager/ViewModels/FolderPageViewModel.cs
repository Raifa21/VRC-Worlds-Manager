using System;
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
using System.Numerics;
using VRC_Favourite_Manager.Services;
using ABI.System;
using System.Globalization;

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

        private string _sortString;
        public string SortString
        {
            get => _sortString;
            set
            {
                _sortString = value;
                OnPropertyChanged(nameof(SortString));
            }
        }

        private bool _sortAscending;
        public bool SortAscending
        {
            get => _sortAscending;
            set
            {
                _sortAscending = value;
                OnPropertyChanged(nameof(SortAscending));
            }
        }

        public ObservableCollection<WorldModel> SearchWorldsCollection { get; private set; }

        public float DefaultRotation { get; } = 180;
        public float InverseRotation { get; } = 0;
        public Vector3 DefaultTransformation { get; } = new Vector3(16, 15, 0);
        public Vector3 InverseTransformation { get; } = new Vector3(0, 0, 0);


        public ICommand SortWorldsCommand { get; }
        public ICommand InverseSortCommand { get; }
        public ICommand MoveWorldCommand { get; }
        public ICommand AddFolderCommand { get; }


        public bool ChangeFolderNameLang{ get; set; }

        public string AddWorldFolderText { get; set; }
        public string ShareFolderText { get; set; }
        public string ViewDetailsText { get; set; }
        public string MoveToAnotherFolderText { get; set; }
        public string RemoveFromFolderText { get; set; }
        public bool IsUnclassified { get; set; }
        public string CurrentFolderTag { get; set; }
        public string SearchName { get; set; }


        public FolderPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            Worlds = new ObservableCollection<WorldModel>();
            SearchWorldsCollection = new ObservableCollection<WorldModel>();
            FolderName = _folderManager?.SelectedFolder?.Name;
            _isRenaming = false;

            string languageCode = Application.Current.Resources["languageCode"] as string;
            ChangeFolderNameLang = (FolderName == "Unclassified" && languageCode == "ja");
            SortWorldsCommand = new RelayCommand<string>(SortWorlds);
            InverseSortCommand = new RelayCommand(InverseSortOrder);
            MoveWorldCommand = new RelayCommand<Tuple<WorldModel, string>>(MoveWorld);
            AddFolderCommand = new RelayCommand<string>(AddFolder);

            UpdateWorlds();
            SearchWorld();

            CurrentFolderTag = "DateAdded";
            SortString = languageCode == "ja" ? "追加日付" : "Date Added";
            SortAscending = true;


            AddWorldFolderText = languageCode == "ja" ? "ワールドを追加" : "Add World";
            ShareFolderText = languageCode == "ja" ? "フォルダを共有" : "Share Folder";
            ViewDetailsText = languageCode == "ja" ? "詳細" : "View Details";
            MoveToAnotherFolderText = languageCode == "ja" ? "別のフォルダに移動" : "Move to another folder";
            RemoveFromFolderText = languageCode == "ja" ? "フォルダから削除" : "Remove from folder";
            IsUnclassified = FolderName == "Unclassified";


            WeakReferenceMessenger.Default.Register<FolderUpdatedMessage>(this, (r, m) =>
            {
                Log.Information("Folder updated");
                OnFolderUpdated();
            });

            WeakReferenceMessenger.Default.Register<SelectedFolderUpdatedMessage>(this, (r, m) =>
            {
                OnFolderUpdated();
                SearchWorld();
                CurrentFolderTag = "Name";
                SortAscending = true;
                SortWorlds("DateAdded");
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
            FolderName = _folderManager.SelectedFolder.Name;
            ChangeFolderNameLang = (FolderName == "Unclassified" && Application.Current.Resources["languageCode"] as string == "ja");
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

            // Remove any worlds from the SearchWorldsCollection
            for (int i = SearchWorldsCollection.Count - 1; i >= 0; i--)
            {
                if (!updatedWorlds.Contains(SearchWorldsCollection[i]))
                {
                    SearchWorldsCollection.RemoveAt(i);
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

        public void SearchWorld()
        {
            Debug.WriteLine("Searching for " + SearchName);
            SearchWorldsCollection.Clear();  // Clear the existing items

            if (string.IsNullOrEmpty(SearchName))
            {
                foreach (var world in Worlds)
                {
                    Debug.WriteLine("Adding " + world.WorldName);
                    SearchWorldsCollection.Add(world); // Add all items back if search is empty
                }

            }
            else
            {
                foreach (var world in Worlds)
                {
                    if (world.WorldName.ToLower().Contains(SearchName.ToLower()) ||
                        world.AuthorName.ToLower().Contains(SearchName.ToLower()))
                    {
                        SearchWorldsCollection.Add(world); // Add only matching items
                    }
                }
            }

            Debug.WriteLine("Search results: " + SearchWorldsCollection.Count);
        }


        public void SortWorlds(string tag)
        {
            Debug.WriteLine("Sorting worlds by " + tag);
            string languageCode = Application.Current.Resources["languageCode"] as string;
            switch (tag)
            {
                case "DateAdded":
                    SortString = languageCode == "ja" ? "追加日付" : "Date Added";
                    if(CurrentFolderTag == "DateAdded")
                    {
                        InverseSortOrder();
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => w.DateAdded));
                        SortAscending = true;
                        CurrentFolderTag = "DateAdded";
                    }
                    break;
                case "Name":
                    SortString = languageCode == "ja" ? "ワールド名" : "World Name";
                    if(CurrentFolderTag == "Name")
                    {
                        InverseSortOrder();
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderBy(w => w.WorldName));
                        SortAscending = true;
                        CurrentFolderTag = "Name";
                    }
                    break;
                case "Author":
                    SortString = languageCode == "ja" ? "作者名" : "Author";
                    if(CurrentFolderTag == "Author")
                    {
                        InverseSortOrder();
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderBy(w => w.AuthorName));
                        SortAscending = true;
                        CurrentFolderTag = "Author";
                    }
                    break;
                case "Favorites":
                    SortString = languageCode == "ja" ? "お気に入り数" : "Favorites";
                    if(CurrentFolderTag == "Favorites")
                    {
                        InverseSortOrder();
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => w.Favorites));
                        SortAscending = true;
                        CurrentFolderTag = "Favorites";
                    }
                    break;
                case "DateUpdated":
                    SortString = languageCode == "ja" ? "最終更新" : "Last Updated";
                    if(CurrentFolderTag == "DateUpdated")
                    {
                        InverseSortOrder();
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => DateTime.Parse(w.LastUpdate)));
                        SortAscending = true;
                        CurrentFolderTag = "DateUpdated";
                    }
                    break;
            }
            SearchWorld();
        }

        public void InverseSortOrder()
        {
            SortAscending = !SortAscending;
            switch (CurrentFolderTag)
            {
                case "DateAdded":
                    if (!SortAscending)
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderBy(w => w.DateAdded));
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => w.DateAdded));
                    }
                    break;
                case "Name":
                    if (!SortAscending)
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => w.WorldName));
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderBy(w => w.WorldName));
                    }
                    break;
                case "Author":
                    if (!SortAscending)
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => w.AuthorName));
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderBy(w => w.AuthorName));
                    }
                    break;
                case "Favorites":
                    if (!SortAscending)
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderBy(w => w.Favorites));
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => w.Favorites));
                    }
                    break;
                case "DateUpdated":
                    if (!SortAscending)
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderBy(w => DateTime.Parse(w.LastUpdate)));
                    }
                    else
                    {
                        Worlds = new ObservableCollection<WorldModel>(Worlds.OrderByDescending(w => DateTime.Parse(w.LastUpdate)));
                    }
                    break;
            }
            SearchWorld();
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
        public async void AddWorld(string worldUri)
        {
            // call the API to get the world by ID, async
            if (Application.Current.Resources["VRChatAPIService"] is VRChatAPIService vrChatApiService)
            {
                var result = await vrChatApiService.GetWorldByIdAsync(worldUri);
                if(result == null)
                {
                    Log.Information("World not found.");
                    return;
                }
                var worldModel = new Models.WorldModel
                {
                    ThumbnailImageUrl = result.ThumbnailImageUrl,
                    WorldName = result.Name,
                    WorldId = result.Id,
                    AuthorName = result.AuthorName,
                    AuthorId = result.AuthorId,
                    Capacity = result.Capacity,
                    LastUpdate = result.UpdatedAt.ToString(CultureInfo.InvariantCulture)?[..10],
                    Description = result.Description,
                    Visits = result.Visits,
                    Favorites = result.Favorites,
                    Platform = result.UnityPackages.Select(unityPackage => unityPackage.Platform).ToHashSet()
                };
                worldModel.DateAdded = DateTime.Now;
                if (_folderManager.SelectedFolder.Worlds.Any(w => w.WorldId == worldModel.WorldId))
                {
                    Debug.WriteLine("World already exists in folder");
                    return;
                }
                if (_folderManager.SelectedFolder.Name == "Unclassified")
                {
                    Debug.WriteLine("Adding world to unclassified");
                    _folderManager.AddToFolder(worldModel, "Unclassified");
                    //check if world exists in any other folder, if so remove it
                    foreach (var folder in _folderManager.Folders)
                    {
                        if (folder.Name != "Unclassified")
                        {
                            if (folder.Worlds.Any(w => w.WorldId == worldModel.WorldId))
                            {
                                Debug.WriteLine("Removing from: " + folder.Name);
                                _folderManager.RemoveFromFolder(worldModel, folder.Name);
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Adding world to selected folder");
                    //check if world exists in unclassified, if so remove it
                    _folderManager.AddToFolder(worldModel, _folderManager.SelectedFolder.Name);
                    if (_folderManager.Folders.Any(f => f.Name == "Unclassified"))
                    {
                        var unclassifiedFolder = _folderManager.Folders.First(f => f.Name == "Unclassified");
                        if (unclassifiedFolder.Worlds.Any(w => w.WorldId == worldModel.WorldId))
                        {
                            Debug.WriteLine("Removing from unclassified");
                            _folderManager.RemoveFromFolder(worldModel, "Unclassified");
                        }
                    }
                }
            }
            UpdateWorlds();
            SearchWorld();
            CurrentFolderTag = "Name";
            SortAscending = true;
            SortWorlds("DateAdded");
        }

        public async Task RefreshWorldsAsync()
        {
            await _worldManager.CheckForNewWorldsAsync();
            UpdateWorlds();
            SearchWorld();
            CurrentFolderTag = "Name";
            SortAscending = true;
            SortWorlds("DateAdded");
        }

        public string GenerateShareCode()
        {
            var configService = new ConfigService();
            return configService.EncodeFolderData(_folderManager.SelectedFolder);
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default.Unregister<FolderUpdatedMessage>(this);
            WeakReferenceMessenger.Default.Unregister<SelectedFolderUpdatedMessage>(this);
        }
    }
}
