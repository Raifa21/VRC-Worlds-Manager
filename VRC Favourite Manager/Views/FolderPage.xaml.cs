using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class FolderPage : Page
    {
        public FolderPage()
        {
            this.InitializeComponent();
            this.DataContext = (Application.Current as App).MainViewModel;
        }
    }
}