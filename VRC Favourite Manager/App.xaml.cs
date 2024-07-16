using Microsoft.UI.Xaml;
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
            var rootFrame = Window.Current.Content as Microsoft.UI.Xaml.Controls.Frame;

            if (rootFrame == null)
            {
                rootFrame = new Microsoft.UI.Xaml.Controls.Frame();
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Navigate to the AuthenticationPage initially
                rootFrame.Navigate(typeof(AuthenticationPage));
            }

            Window.Current.Activate();
        }

    }
}
