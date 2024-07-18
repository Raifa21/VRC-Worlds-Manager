using System;
using System.IO;
using System.Net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Tomlyn;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Views;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRC_Favourite_Manager
{
    public sealed partial class App : Application
    {
        private VRChatService _VRChatService;
        private string apiKey;
        public MainWindow mainWindow;

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
            try
            {
                if (apiKey == "")
                {
                    rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                }
                else
                {
                    ApiResponse<VerifyAuthTokenResult> response = _VRChatService.CheckAuthentication();
                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        if (response.Data.Ok)
                        {
                            rootFrame.Navigate(typeof(MainPage), args.Arguments);
                        }
                        else
                        {
                            Console.WriteLine("API key invalid. Redirecting to Login Page.");
                            rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                        }
                    }
                    else
                    {
                        Console.WriteLine("API is not reachable. Redirecting to Login Page.");
                        rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                    }
                }
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error reading API key from config file.");
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
            var configManager = new ConfigManager();

            if (!configManager.ConfigExists())
            {
                configManager.WriteConfig("auth = \"\"");
                this.apiKey = "";
                return;
            }
            try
            {
                var toml = Toml.ToModel(Toml.Parse(configManager.ReadConfig()));
                if (toml.ContainsKey("auth"))
                {
                    try
                    {
                        this.apiKey = toml["auth"].ToString();
                    }
                    catch (System.Exception)
                    {
                        Console.WriteLine("Error reading API key from config file.");
                        configManager.WriteConfig("auth = \"\"");
                        this.apiKey = "";
                    }
                }
                else
                {
                    Console.WriteLine("API key not found in config file.");
                    configManager.WriteConfig("auth = \"\"");
                    this.apiKey = "";
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Config file not found.");
                configManager.WriteConfig("auth = \"\"");
                this.apiKey = "";
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
    public class ViewModelLocator
    {
        private static AuthenticationViewModel _authenticationViewModel = new AuthenticationViewModel();

        public AuthenticationViewModel AuthenticationViewModel => _authenticationViewModel;
    }

}
