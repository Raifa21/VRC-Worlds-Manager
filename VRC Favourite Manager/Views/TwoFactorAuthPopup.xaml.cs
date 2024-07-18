using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class TwoFactorAuthPopup : ContentDialog
    {
        public string OtpCode { get; private set; }

        public TwoFactorAuthPopup()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OtpCode = OtpTextBox.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            OtpCode = null;
        }
    }
}