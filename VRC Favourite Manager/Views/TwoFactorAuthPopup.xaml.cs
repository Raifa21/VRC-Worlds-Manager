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