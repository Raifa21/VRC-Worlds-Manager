using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using VRC_Favourite_Manager.ViewModels;

public sealed partial class AuthenticationPage : Page
{
    public AuthenticationPage()
    {
        this.InitializeComponent();
        DataContext = new AuthenticationViewModel((Frame)Window.Current.Content);
    }
}