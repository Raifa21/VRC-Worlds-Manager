using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;

using System;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Views;
using System.IO;
using Tomlyn;

namespace VRC_Favourite_Manager.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;
        public ICommand LogoutCommand { get; }
        public ICommand ResetCommand { get; }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }


        public SettingsPageViewModel()
        {
            _vrChatAPIService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            LogoutCommand = new RelayCommand(async () => await LogoutCommandAsync());
            ResetCommand = new RelayCommand(Reset);

            GetUserDisplayName().Wait();
        }

        private async Task GetUserDisplayName()
        {
            if (ReadConfig())
            {
                return;
            }

            _displayName = await _vrChatAPIService.GetUserDisplayName();
            WriteToConfig("displayName", _displayName);
        }
        private bool ReadConfig()
        {
            var configManager = new ConfigManager();

            if (!configManager.ConfigExists())
            {
                System.Diagnostics.Debug.WriteLine("Config file not found.");
                return false;
            }
            try
            {
                var toml = Toml.ToModel(Toml.Parse(configManager.ReadConfig()));
                if (toml.TryGetValue("displayName", out var name))
                {
                    try
                    {
                        this._displayName = name.ToString();
                        return true;
                    }
                    catch (System.Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Error reading API key from config file.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("API key not found in config file.");
                }
            }
            catch (FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("Config file not found.");
            }
            return false;
        }

        private void WriteToConfig(string key, string token)
        {
            var configManager = new ConfigManager();
            configManager.WriteToConfig(key, token);
        }


        private async Task LogoutCommandAsync()
        {
            await _vrChatAPIService.LogoutAsync();
        }
        private void Reset()
        {
            _worldManager.ResetWorlds();
            _folderManager.ResetFolders();
        }
    }
}
