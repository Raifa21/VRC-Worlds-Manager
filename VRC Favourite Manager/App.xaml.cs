using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager
{
    public sealed partial class App : Application
    {
        private readonly VRChatService _VRChatService;
        public App()
        {
            // Read config file
            // This is a placeholder for now
            string username = "username";
            string password = "password";

            _VRChatService = new VRChatService(username, password);
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = new Frame();
            if (_VRChatService.CheckAuthentication())
            {
                rootFrame.Navigate(typeof(MainPage));
            }
            else
            {
                rootFrame.Navigate(typeof(AuthenticationPage));
            }
            rootFrame.Navigate(typeof(AuthenticationPage));
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }
    }
}
