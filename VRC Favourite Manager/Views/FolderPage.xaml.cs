using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class FolderPage : Page
    {
        public FolderPage()
        {
            this.InitializeComponent();
            this.DataContext = (MainViewModel)Application.Current.Resources["MainViewModel"];
        }
    }
}