using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Tomlyn;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Views;
using VRChat.API.Client;

namespace VRC_Favourite_Manager
{
    public sealed partial class App : Application
    {
        private VRChatService _VRChatService;
        private string apiKey;
        private MainWindow mainWindow;

        public App()
        {
            this.InitializeComponent();

            
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            InitialiseService();

            ReadConfig();
            


            mainWindow = new MainWindow();
            Frame rootFrame = new Frame();

            if (_VRChatService.CheckAuthentication())
            {
                rootFrame.Navigate(typeof(MainPage), args.Arguments);
            }
            else
            {
                rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
            }
            mainWindow.Content = rootFrame;
            mainWindow.Activate();
        }

        /// <summary>
        /// Reads the user's local config file to retrieve cached information.
        /// </summary>
        private void ReadConfig()
        {
            var toml = Toml.ToModel(Toml.Parse(File.ReadAllText("Config.toml")));
            if (toml.ContainsKey("auth"))
            {
                try
                {
                    this.apiKey = toml["auth"].ToString();
                }
                catch (System.Exception)
                {
                    this.apiKey = "";
                }
            }
        }

        /// <summary>
        /// Initialises the VRChatService with the provided username and password.
        /// </summary>
        private void InitialiseService()
        {
            Application.Current.Resources["VRChatService"] = new VRChatService();

            _VRChatService = (VRChatService)Application.Current.Resources["VRChatService"];
        }

    }
}
