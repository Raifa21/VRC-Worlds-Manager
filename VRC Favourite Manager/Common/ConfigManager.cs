using System;
using System.IO;

namespace VRC_Favourite_Manager
{
    public class ConfigManager
    {
        private readonly string _configFilePath;

        public ConfigManager()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataFolder, "VRC_Favourite_Manager");
            Directory.CreateDirectory(appFolder);
            _configFilePath = Path.Combine(appFolder, "Config.toml");
        }

        public void WriteConfig(string content)
        {
            File.WriteAllText(_configFilePath, content);
        }

        public string ReadConfig()
        {
            return File.ReadAllText(_configFilePath);
        }

        public bool ConfigExists()
        {
            return File.Exists(_configFilePath);
        }
    }
}