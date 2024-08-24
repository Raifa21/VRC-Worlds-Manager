using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.ViewModels;
using Windows.Networking.Sockets;


namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AuthenticationPage : Page
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TwoFactorAuth { get; set; }
        public AuthenticationPage()
        {
            this.InitializeComponent();

            if(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.Subtitle.Text = "VRChatにログイン";
                this.UsernameTextBox.Header = "ユーザー名";
                this.PasswordBox.Header = "パスワード";
                this.Legal1.Text = "ログイン情報は保存されず、認証目的のみに使用されます。";
                this.Legal2.Text = "このアプリはVRChatとは一切関係ありません。VRChatはVRChat Inc.の商標です。";
                this.Legal3.Text = "このアプリケーションの使用によって生じる問題については、一切責任を負いません。ご了承ください。";
                this.LoginButton.Content = "ログイン";
            }
            else
            {
                this.Subtitle.Text = "Log into VRChat";
                this.UsernameTextBox.Header = "Username";
                this.PasswordBox.Header = "Password";
                this.Legal1.Text =
                    "Your login credentials are not stored and are only used for authentication purposes.";
                this.Legal2.Text = "We are not affiliated with VRChat in any way. VRChat is a trademark of VRChat Inc.";
                this.Legal3.Text = "Please be aware that I am not responsible for any issues that may arise from using this application.";
                this.LoginButton.Content = "Login";
            }
        }
    }
}