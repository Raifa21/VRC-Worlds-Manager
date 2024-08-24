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
        private readonly VRChatAPIService _vrChatAPIService;
        private readonly Window _mainWindow;
        private MainWindow mainWindow;
        private string _errorMessage;

        public AuthenticationViewModel()
        {
            _vrChatAPIService = Application.Current.Resources["VRChatAPIService"] as VRChatAPIService;
            _mainWindow = ((App)Application.Current).mainWindow;
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
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        // Adjusted constructor to accept NavigationService directly

        private async void Login()
        {
            try
            {
                await _vrChatAPIService.VerifyLoginAsync(Username, Password);
                System.Diagnostics.Debug.WriteLine("Login successful.");
                DisplayMainView();
            }
            catch (VRCRequiresTwoFactorAuthException e)
            {
                if (await DoTwoFactorAuthenticationAsync(e.TwoFactorAuthType))
                {
                    DisplayMainView();
                }
                else
                {
                    ErrorMessage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "2段階認証に失敗しました。もう一度お試しください。" : "Failed to authenticate with 2FA. Please try again.";
                }
            }
            catch (VRCIncorrectCredentialsException)
            {
                ErrorMessage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "ユーザー名またはパスワードが間違っています。" : "Incorrect username or password.";
            }
            catch (Exception ex)
            {
                ErrorMessage = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja" ? "エラーが発生しました。もう一度お試しください。" : "An error occurred. Please try again.";
            }
        }

        private async Task<bool> DoTwoFactorAuthenticationAsync(string twoFactorAuthType)
        {
            var otpDialog = new TwoFactorAuthPopup(_mainWindow.Content.XamlRoot);
            var result = await otpDialog.ShowAsync();
            if (result != ContentDialogResult.Primary || string.IsNullOrEmpty(otpDialog.OtpCode))
            {
                System.Diagnostics.Debug.WriteLine("OTP Dialog was cancelled or empty");
            }
            return await _vrChatAPIService.Authenticate2FAAsync(otpDialog.OtpCode, twoFactorAuthType);
        }

        private void DisplayMainView()
        {
            var rootFrame = new Frame();
            mainWindow = new MainWindow();
            rootFrame.Navigate(typeof(MainPage));
            mainWindow.Content = rootFrame;
            mainWindow.Activate();

            ((App)Application.Current).mainWindow.Close();
        }

    }
}
