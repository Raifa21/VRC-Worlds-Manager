using System;
using System.IO;
using Tomlyn;

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

        public string ReadConfig()
        {
            return File.ReadAllText(_configFilePath);
        }

        public bool ConfigExists()
        {
            return File.Exists(_configFilePath);
        }

        public void WriteToConfig(string key, string token)
        {
            if (!ConfigExists())
            {
                File.WriteAllText(_configFilePath, $"{key} = \"{token}\"");
            }
            else
            {
                var toml = Toml.ToModel(Toml.Parse(ReadConfig()));
                if (toml.ContainsKey(key))
                {
                    toml[key] = token;
                }
                else
                {
                    toml.Add(key, token);
                }

                File.WriteAllText(_configFilePath, (Toml.FromModel(toml)));
            }
        }
    }
}