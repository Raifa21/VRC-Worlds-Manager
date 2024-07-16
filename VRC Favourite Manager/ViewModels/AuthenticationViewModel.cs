using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Views;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AuthenticationViewModel : ViewModelBase
    {
        private readonly VRChatService _vrChatService;
        private readonly NavigationService _navigationService;

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
        public AuthenticationViewModel(NavigationService navigationService)
        {
            _vrChatService = new VRChatService();
            _navigationService = navigationService; // Use the passed NavigationService
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            if (await _vrChatService.LoginAwait(Username, Password))
            {
                // Navigate to main application page upon successful login
                _navigationService.Navigate(typeof(MainPage));
            }
            else
            {
                Debug.WriteLine("Authentication failed.");
            }
        }
    }
}
