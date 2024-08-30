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

namespace VRC_Favourite_Manager.ViewModels
{
    public class DeletePopupViewModel : ViewModelBase
    {
        private readonly FolderManager _folderManager;
        public DeletePopupViewModel()
        {
            Debug.WriteLine("MainViewModel created");
            _folderManager = Application.Current.Resources["FolderManager"] as FolderManager;

        }

        public void Delete_Click(string folderName)
        {
            _folderManager.RemoveFolder(folderName);
        }
    }
}
