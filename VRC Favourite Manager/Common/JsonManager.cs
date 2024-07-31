using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager
{
    public class JsonManager
    {
        private readonly string _worldPath;
        private readonly string _folderPath;

        public JsonManager()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localFolder, "VRC_Favourite_Manager");
            Directory.CreateDirectory(appFolder);
            _worldPath = Path.Combine(appFolder, "worlds.json");
            _folderPath = Path.Combine(appFolder, "folders.json");
        }

        public bool WorldConfigExists()
        {
            return File.Exists(_worldPath);
        }

        public List<WorldModel> LoadWorlds()
        {
            var json = File.ReadAllText(_worldPath);
            return JsonSerializer.Deserialize<List<WorldModel>>(json);
        }

        public void SaveWorlds(IEnumerable<WorldModel> worlds)
        {
            var json = JsonSerializer.Serialize(worlds);
            File.WriteAllText(_worldPath, json);
            Debug.WriteLine("File written to: " + _worldPath);
        }
        public bool FolderConfigExists()
        {
            return File.Exists(_folderPath);
        }

        public List<FolderModel> LoadFolders()
        {
            var json = File.ReadAllText(_folderPath);
            return JsonSerializer.Deserialize<List<FolderModel>>(json);
        }

        public void SaveFolders(IEnumerable<FolderModel> folders)
        {
            var json = JsonSerializer.Serialize(folders);
            File.WriteAllText(_folderPath, json);
            Debug.WriteLine("File written to: " + _folderPath);
        }

    }
}