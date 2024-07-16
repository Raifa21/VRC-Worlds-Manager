using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using VRC_Favourite_Manager.ViewModels;
using Windows.Networking.Sockets;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AuthenticationPage : Page
    {
        public AuthenticationViewModel ViewModel => BindingContext as AuthenticationViewModel;
        public AuthenticationPage(AuthenticationViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            var newFrame = new AuthenticationViewModel(Frame);
            this.DataContext = newFrame;
        }
    }
}