using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Views;
using VRChat.API.Model;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AuthenticationViewModel : ViewModelBase
    {
        private readonly VRChatService _vrChatService;
        private MainWindow mainWindow;

        public AuthenticationViewModel()
        {
            _vrChatService = Application.Current.Resources["VRChatService"] as VRChatService;
            LoginCommand = new RelayCommand(Login);

        }

        private string _username;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        // Adjusted constructor to accept NavigationService directly

        private async void Login()
        {
            Frame rootFrame = new Frame();
            mainWindow = new MainWindow();

            bool loginSuccessful = _vrChatService.Login(Username, Password);
            if (!loginSuccessful) throw new VRCIncorrectCredentialsException();
            var otpDialog = new TwoFactorAuthPopup();
            var result = await otpDialog.ShowAsync();
            if (result != ContentDialogResult.Primary || string.IsNullOrEmpty(otpDialog.OtpCode))
            {
                Console.WriteLine("OTP Dialog was cancelled or empty");
                return;
            }
            if (_vrChatService.RequiresEmailotp)
            {
                Verify2FAEmailCodeResult otpVerified = _vrChatService.VerifyEmail2FA(otpDialog.OtpCode);
                if (otpVerified.Verified)
                {
                    rootFrame.Navigate(typeof(MainPage));
                    mainWindow.Content = rootFrame;
                    mainWindow.Activate();
                }
                else
                {
                    throw new VRCIncorrectCredentialsException();
                }
            }
            else
            {
                Verify2FAResult otpVerified = _vrChatService.Verify2FA(otpDialog.OtpCode);
                if (otpVerified.Verified)
                {
                    rootFrame.Navigate(typeof(MainPage));
                    mainWindow.Content = rootFrame;
                    mainWindow.Activate();
                }
                else
                {
                    throw new VRCIncorrectCredentialsException();
                }
            }
        }
    }
}
