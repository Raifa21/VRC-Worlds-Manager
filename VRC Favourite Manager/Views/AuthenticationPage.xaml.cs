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
        public string Username { get; set; }
        public string Password { get; set; }
        public string TwoFactorAuth { get; set; }
        public AuthenticationPage()
        {
            this.InitializeComponent();
        }
    }
}