using System.Diagnostics;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Common;
using System.IO;
using Tomlyn;
using System;

namespace VRC_Favourite_Manager.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly WorldManager _worldManager;
        private readonly FolderManager _folderManager;
        public ICommand ResetCommand { get; }

        public event EventHandler LanguageChanged;

        public SettingsPageViewModel()
        {
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;
            _worldManager = Application.Current.Resources["WorldManager"] as WorldManager;

            ResetCommand = new RelayCommand(Reset);

            Debug.WriteLine("SettingsPageViewModel created");
        }


        private void Reset()
        {
            _worldManager.ResetWorlds();
            _folderManager.ResetFolders();
        }
        protected virtual void OnLanguageChanged()
        {
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
