using System.Diagnostics;
using System.IO;
using Windows.UI.WebUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog;
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
        public string languageCode { get; set; }

        public MainWindow MainWindow { get; private set; }

        public App()
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("app.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            this.InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            Log.Information("Application Started.");
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
                            Log.Information("Login successful.");
                            rootFrame.Navigate(typeof(MainPage), args.Arguments);
                        }
                    }
                    else
                    {
                        Log.Information("Error verifying API key.");
                        rootFrame.Navigate(typeof(AuthenticationPage), args.Arguments);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Information("TypeInitializationException: " + ex.Message);
                Log.Information("Stack Trace: " + ex.StackTrace);

                // Check if there is an inner exception
                if (ex.InnerException != null)
                {
                    // Log the inner exception details
                    Log.Information("Inner Exception: " + ex.InnerException.Message);
                    Log.Information("Inner Exception Stack Trace: " + ex.InnerException.StackTrace);
                }
                Log.Information("Error reading API key from config file.");
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
                Log.Information("Config file not found.");
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
                        Log.Information("Error reading API key from config file.");
                        Log.Information(e.Message);
                    }
                }
                else
                {
                    Log.Information("API key not found in config file.");
                }

            }
            catch (FileNotFoundException)
            {
                Log.Information("Config file not found.");
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
            Application.Current.Resources["languageCode"] = "ja";
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
