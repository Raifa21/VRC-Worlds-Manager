using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AddToFolderPopupViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FolderViewModel> Folders { get; set; }

        public AddToFolderPopupViewModel()
        {
            // Load folders
            Folders = new ObservableCollection<FolderViewModel>
            {
                new FolderViewModel { Name = "Folder 1" },
                new FolderViewModel { Name = "Folder 2" },
                new FolderViewModel { Name = "Folder 3" }
            };
        }

        public void AddFolder()
        {
            // Logic to add a new folder
            Folders.Add(new FolderViewModel { Name = "New Folder" });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FolderViewModel : INotifyPropertyChanged
    {
        private string name;
        private bool isSelected;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
