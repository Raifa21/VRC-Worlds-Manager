using System.Collections.ObjectModel;
using VRC_Favourite_Manager;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

public class FolderManager
{
    private readonly JsonManager _jsonManager;

    public ObservableCollection<FolderModel> Folders { get; private set; }

    public FolderManager(JsonManager jsonManager)
    {
        _jsonManager = jsonManager;
        Folders = new ObservableCollection<FolderModel>();

        var savedFolders = _jsonManager.LoadFolders();
        if (savedFolders != null)
        {
            foreach (var folder in savedFolders)
            {
                Folders.Add(folder);
            }
        }
        else
        {
            Folders.Add(new FolderModel("Unclassified"));
        }
    }

    public void AddFolder(string folderName)
    {
        Folders.Add(new FolderModel(folderName));
        SaveFolders();
    }

    public void RemoveFolder(FolderModel folder)
    {
        Folders.Remove(folder);
        SaveFolders();
    }

    private void SaveFolders()
    {
        _jsonManager.SaveFolders(Folders);
    }
}