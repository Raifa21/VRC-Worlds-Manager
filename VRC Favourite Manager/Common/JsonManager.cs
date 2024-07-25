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
        private readonly string _filePath;

        public JsonManager()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localFolder, "VRC_Favourite_Manager");
            Directory.CreateDirectory(appFolder);
            _filePath = Path.Combine(appFolder, "worlds.json");
            Debug.WriteLine("Config file path: " + _filePath);
        }

        public bool ConfigExists()
        {
            return File.Exists(_filePath);
        }

        public List<WorldModel> LoadWorlds()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<WorldModel>>(json);
        }

        public void SaveWorlds(IEnumerable<WorldModel> worlds)
        {
            var json = JsonSerializer.Serialize(worlds);
            File.WriteAllText(_filePath, json);
            Debug.WriteLine("File written to: " + _filePath);
        }
    }
}