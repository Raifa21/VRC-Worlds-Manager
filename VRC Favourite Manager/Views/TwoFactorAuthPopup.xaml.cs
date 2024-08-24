using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class TwoFactorAuthPopup : ContentDialog
    {
        public string OtpCode { get; private set; }

        public TwoFactorAuthPopup(XamlRoot xamlRoot)
        {
            this.InitializeComponent();
            this.XamlRoot = xamlRoot;

            if(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride == "ja")
            {
                this.Subtitle.Text = "2段階認証が有効になっています。";
                this.Description.Text = "認証コードを入力してください：";
                this.PrimaryButtonText = "キャンセル";
                this.SecondaryButtonText = "送信";
            }
            else {
                this.Subtitle.Text = "2FA is enabled.";
                this.Description.Text = "Please enter a 2FA code:";
                this.PrimaryButtonText = "Cancel";
                this.SecondaryButtonText = "Submit";
            }


        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OtpCode = null;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OtpCode = OtpTextBox.Text;
        }
    }
}