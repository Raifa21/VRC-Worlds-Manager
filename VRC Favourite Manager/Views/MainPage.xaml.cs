using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly VRChatAPIService _vrChatAPIService;
        public MainPage()
        {
            this.InitializeComponent();
        }

    }
}
