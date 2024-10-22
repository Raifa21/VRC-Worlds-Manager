using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Serilog;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.Common
{
    public class JsonManager
    {
        private readonly string _worldPath;
        private readonly string _folderPath;

        public JsonManager()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localFolder, "VRC_Worlds_Manager");
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
            var _configService = new ConfigService();
            var json = _configService.LoadToken(_worldPath);
            return JsonSerializer.Deserialize<List<WorldModel>>(json);
        }

        public void SaveWorlds(IEnumerable<WorldModel> worlds)
        {
            var json = JsonSerializer.Serialize(worlds);
            var _configService = new ConfigService();
            _configService.SaveToken(json, _worldPath);
            Log.Information("File written to: " + _worldPath);
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
            Log.Information("File written to: " + _folderPath);
        }

    }
}