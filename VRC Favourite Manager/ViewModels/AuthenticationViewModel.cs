using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Services;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AuthenticationViewModel : ViewModelBase
    {
        public ICommand LoginCommand { get; }

        private readonly VRChatService _vrChatService;

        public AuthenticationViewModel()
        {
            _vrChatService = new VRChatService("200");
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            bool isAuthenticated = await _vrChatService.AuthenticateAsync();

            if (isAuthenticated)
            {
                // Navigate to main application page upon successful login
                NavigationService.Navigate(typeof(MainPage));
            }
            else
            {
                // Handle authentication failure
                // Display error message or handle as per your application flow
                Debug.WriteLine("Authentication failed.");
            }
        }
    }
}
