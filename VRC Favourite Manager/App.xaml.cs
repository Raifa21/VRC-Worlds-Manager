using Microsoft.UI.Xaml;
using System.Diagnostics;
using VRC_Favourite_Manager.Services;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;
using Windows.Media.Protection.PlayReady;
using System.Net;
using VRC_Favourite_Manager.ViewModels;

namespace VRC_Favourite_Manager
{
    public partial class App : Application
    {
        private readonly MainPage MainWindow;
        private readonly AuthenticationViewModel AuthenticationWindow;
        private readonly VRChatService _vrChatService;
        private readonly MainViewModel mainViewModel;

        public App()
        {
            InitializeComponent();


            // Check authentication status
            bool isAuthenticated = _vrChatService.CheckAuthentication();

            if (isAuthenticated)
            {
                // Navigate to main application page
                MainWindow = new MainPage();
                //display main window
            }
            else
            {
                // Navigate to authentication page
                AuthenticationWindow = new AuthenticationViewModel();
            }

        }

        
    }
}
