using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Tomlyn;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.ViewModels;
using VRC_Favourite_Manager.Views;
using VRChat.API.Client;
using VRChat.API.Model;

namespace VRC_Favourite_Manager
{
    public sealed partial class App : Application
    {
        private VRChatAPIService _VRChatAPIService;
        private string authToken;
        private string twoFactorAuthToken;
        public MainWindow mainWindow;
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
            


            mainWindow = new MainWindow();
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
                        try
                        {
                            if(await _VRChatAPIService.VerifyLoginWithAuthTokenAsync(authToken, twoFactorAuthToken))
                            {
                                System.Diagnostics.Debug.WriteLine("Login successful.");
                                rootFrame.Navigate(typeof(MainPage), args.Arguments);
                            }
                            
                        }
                        catch (VRCNotLoggedInException)
                        {
                            System.Diagnostics.Debug.WriteLine("Error verifying API key.");
                            rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                        }
                        
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error verifying API key.");
                        rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                    }
                }
            }
            catch (System.Exception)
            {
                System.Diagnostics.Debug.WriteLine("Error reading API key from config file.");
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
                    catch (System.Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Error reading API key from config file.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("API key not found in config file.");
                }

                if (toml.ContainsKey("language"))
                {
                    try
                    {
                        this.languageCode = toml["language"].ToString();
                        WeakReferenceMessenger.Default.Send(new LanguageChangedMessage(languageCode));
                    }
                    catch (System.Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Error reading language from config file.");
                        WeakReferenceMessenger.Default.Send(new LanguageChangedMessage("en-US"));
                    }
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
    public class LanguageChangedMessage(string languageCode)
    {
        public string LanguageCode { get; set; } = languageCode;
    }
}
