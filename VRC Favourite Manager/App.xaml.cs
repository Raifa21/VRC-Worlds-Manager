using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager
{
    public sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;

                // Initialize NavigationService with the root frame
                var navigationService = new NavigationService(rootFrame);

                // Pass NavigationService to your AuthenticationViewModel
                rootFrame.Navigate(typeof(LoginPage), navigationService);
            }

            Window.Current.Activate();
        }
    }
}
