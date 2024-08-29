using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.Common
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
            return new List<WorldModel>();
        }

        public void SaveWorlds(IEnumerable<WorldModel> worlds)
        {
            var json = JsonSerializer.Serialize(worlds);
            var _configService = new ConfigService();
            _configService.SaveToken(json, _worldPath);
            Debug.WriteLine("File written to: " + _worldPath);
        }
        public bool FolderConfigExists()
        {
            return File.Exists(_folderPath);
        }

        public List<FolderModel> LoadFolders()
        {
            var _configService = new ConfigService();
            var json = _configService.LoadToken(_folderPath);
            return JsonSerializer.Deserialize<List<FolderModel>>(json);
        }

        public void SaveFolders(IEnumerable<FolderModel> folders)
        {
            var json = JsonSerializer.Serialize(folders);
            var _configService = new ConfigService();
            _configService.SaveToken(json, _folderPath);
            Debug.WriteLine("File written to: " + _folderPath);
        }

    }
}