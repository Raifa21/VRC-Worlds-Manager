using System;
using System.IO;
using Tomlyn;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.Common
{
    public class ConfigManager
    {
        private readonly string _configFilePath;

        public ConfigManager()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataFolder, "VRC_Worlds_Manager");
            Directory.CreateDirectory(appFolder);
            _configFilePath = Path.Combine(appFolder, "Config.toml");
        }

        public string ReadConfig()
        {
            var _configService = new ConfigService();
            return _configService.LoadToken(_configFilePath);
        }

        public bool ConfigExists()
        {
            return File.Exists(_configFilePath);
        }

        public void WriteToConfig(string key, string token)
        {
            var _configService = new ConfigService();

            if (!ConfigExists())
            {
                _configService.SaveToken($"{key} = \"{token}\"", _configFilePath);
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
                _configService.SaveToken(Toml.FromModel(toml), _configFilePath);
            }
        }

        /// <summary>
        /// Logs the user out. Only the auth token is removed.
        /// </summary>
        public void Logout()
        {
            if (ConfigExists())
            {
                var toml = Toml.ToModel(Toml.Parse(ReadConfig()));
                if (toml.ContainsKey("auth"))
                {
                    toml.Remove("auth");
                    var _configService = new ConfigService();
                    _configService.SaveToken(Toml.FromModel(toml), _configFilePath);
                }
            }
        }
    }
}