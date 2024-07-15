using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Views;
using VRChat.API.Api;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRC_Favourite_Manager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Check authentication status
            bool isAuthenticated = CheckAuthentication();

            if (isAuthenticated)
            {
                // Navigate to main application page
                MainWindow = new MainWindow();
            }
            else
            {
                // Navigate to authentication page
                MainWindow = new AuthenticationPage();
            }

            MainWindow.Activate();
        }

        private bool CheckAuthentication()
        {
            try
            {
                CurrrentUser result = apiInstance.GetCurrentUser();
                Debug.WriteLine("GetCurrentUser: " + result);

            }
        }
    }
}
