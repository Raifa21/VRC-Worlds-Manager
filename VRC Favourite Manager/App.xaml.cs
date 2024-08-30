﻿using System.Diagnostics;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Tomlyn;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager
{
    public sealed partial class App : Application
    {
        private VRChatAPIService _VRChatAPIService;
        private string authToken;
        private string twoFactorAuthToken;
        public MainWindow MainWindow { get; private set; }
        private string languageCode;

        public App()
        {
            this.InitializeComponent();

            
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Application Started.");
            InitialiseService();

            ReadConfig();



            MainWindow = new MainWindow();
            Frame rootFrame = new Frame();
            try
            {
                if (string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(twoFactorAuthToken))
                {
                    rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                }
                else
                {
                    if (await _VRChatAPIService.VerifyAuthTokenAsync(authToken, twoFactorAuthToken))
                    {
                        if(await _VRChatAPIService.VerifyLoginWithAuthTokenAsync(authToken, twoFactorAuthToken))
                        {
                            System.Diagnostics.Debug.WriteLine("Login successful.");
                            rootFrame.Navigate(typeof(MainPage), args.Arguments);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error verifying API key.");
                        rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                    }
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error reading API key from config file.");
                rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
            }


            MainWindow.Content = rootFrame;
            MainWindow.Activate();
        }

        /// <summary>
        /// Reads the user's local config file to retrieve cached information.
        /// </summary>
        private void ReadConfig()
        {
            var configManager = new ConfigManager();
            var configService = new ConfigService();

            if (!configManager.ConfigExists())
            {
                System.Diagnostics.Debug.WriteLine("Config file not found.");
                return;
            }
            try
            {
                var toml = Toml.ToModel(Toml.Parse(configManager.ReadConfig()));
                if (toml.ContainsKey("auth") && toml.ContainsKey("twoFactorAuth"))
                {
                    try
                    {
                        this.authToken = toml["auth"].ToString();
                        this.twoFactorAuthToken = toml["twoFactorAuth"].ToString();
                    }
                    catch (System.Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Error reading API key from config file.");
                        Debug.WriteLine(e.Message);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("API key not found in config file.");
                }

            }
            catch (FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("Config file not found.");
            }

        }

        /// <summary>
        /// Initialise services as singleton.
        /// </summary>
        private void InitialiseService()
        {
            Application.Current.Resources["VRChatAPIService"] = new VRChatAPIService();
            Application.Current.Resources["FolderManager"] = new FolderManager();
            Application.Current.Resources["WorldManager"] = new WorldManager();
            _VRChatAPIService = (VRChatAPIService)Application.Current.Resources["VRChatAPIService"];
        }

    }
    public class ViewModelLocator
    {
        private static AuthenticationViewModel _authenticationViewModel = new AuthenticationViewModel();

        public AuthenticationViewModel AuthenticationViewModel => _authenticationViewModel;
    }
    public class LanguageChangedMessage
    {
        public string LanguageCode { get; set; }
        public LanguageChangedMessage(string languageCode)
        {
            LanguageCode = languageCode;
        }
    }
}
