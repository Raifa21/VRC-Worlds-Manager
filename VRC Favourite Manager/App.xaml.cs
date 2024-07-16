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
            // Initialize your application here
            // For example, navigate to your main page
            var rootFrame = new Microsoft.UI.Xaml.Controls.Frame();
            rootFrame.Navigate(typeof(MainPage));
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }
    }
}
