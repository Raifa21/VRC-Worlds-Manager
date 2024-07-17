using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.ViewModels;
using Windows.Networking.Sockets;


namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AuthenticationPage : Page
    {
        private readonly VRChatService _vrChatService;
        private string username;
        private string password;
        private string twoFactorAuthentication;
        public AuthenticationPage()
        {
            this.InitializeComponent();
            _vrChatService = Application.Current.Resources["VRChatService"] as VRChatService;
            
            
        }
    }
}