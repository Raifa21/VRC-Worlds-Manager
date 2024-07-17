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
        private string username;
        private string password;
        private MainWindow mainWindow;

        public App()
        {
            this.InitializeComponent();

            
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ReadConfig();
            InitialiseService();

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
            if (toml.ContainsKey("username") && toml.ContainsKey("password"))
            {
                try
                {
                    this.username = toml["username"].ToString();
                    this.password = toml["password"].ToString();
                }
                catch (System.Exception)
                {
                    this.username = "";
                    this.password = "";
                }
            }
        }

        /// <summary>
        /// Initialises the VRChatService with the provided username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void InitialiseService()
        {
            Application.Current.Resources["VRChatService"] = new VRChatService(username, password);

            _VRChatService = (VRChatService)Application.Current.Resources["VRChatService"];
        }

    }
}
