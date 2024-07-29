
using System.Collections.ObjectModel;

namespace VRC_Favourite_Manager.Models
{
    public class FolderModel
    {
        public string Name { get; set; }
        public ObservableCollection<WorldModel> Worlds { get; set; }
        public bool IsSelected { get; set; }

        public FolderModel(string name)
        {
            Name = name;
            Worlds = new ObservableCollection<WorldModel>();
            IsSelected = false;
        }
    }
}