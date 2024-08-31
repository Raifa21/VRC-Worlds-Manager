using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


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

            ReloadPage();
        }

        public void ReloadPage()
        {
            string languageCode = Application.Current.Resources["languageCode"] as string;

            if (languageCode == "ja")
            {
                this.Subtitle.Text = "VRChatにログイン";
                this.UsernameTextBox.Header = "メール/ユーザー名";
                this.PasswordBox.Header = "パスワード";
                this.Legal1.Text = "ログイン情報は保存されず、認証目的のみに使用されます。";
                this.Legal2.Text = "このアプリの使用により、VRChatのAPI利用規約に同意したものとみなされます。";
                this.Legal3.Text = "このアプリケーションの使用によって生じる問題については、一切責任を負いません。ご了承ください。";
                this.LoginButton.Content = "ログイン";
            }
            else
            {
                this.Subtitle.Text = "Log into VRChat";
                this.UsernameTextBox.Header = "Email/Username";
                this.PasswordBox.Header = "Password";
                this.Legal1.Text =
                    "Your login credentials are not stored and are only used for authentication purposes.";
                this.Legal2.Text = "By using this app, you agree to VRChat's API Terms of Service.";
                this.Legal3.Text = "Please be aware that I am not responsible for any issues that may arise from using this application.";
                this.LoginButton.Content = "Login";
            }
        }

        public void ChangeLang_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources["languageCode"] = Application.Current.Resources["languageCode"] == "ja" ? "en" : "ja";
            ReloadPage();
        }
    }
}