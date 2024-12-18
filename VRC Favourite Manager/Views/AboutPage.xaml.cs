using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            string exeFolder = AppContext.BaseDirectory;
            string imagePath = Path.Combine(exeFolder, "Assets", "SrkYocOB_400x400.jpg");
            Icon.ImageSource = new BitmapImage(new Uri(imagePath));

            string languageCode = Application.Current.Resources["languageCode"] as string;

            if (languageCode == "ja")
            {
                CreatorTextBlock.Text = "作成者: Raifa";
                SupportThankYouTextBlock.Text = "ご支援ありがとうございます！支援は";
                hyperlinktext.Text = "こちら";
                SupportThankYouTextSuffix.Text = "から！";
                MITLicenseTextBlock.Text = "このアプリケーションはMITライセンスの下でライセンスされています。";
                ComplianceTextBlock.Text = "このアプリケーションを使用することで、すべての適用されるライセンスに従うことに同意するものとします。";
                CCBYNCTextBlock.Text = "一部のコンポーネントはCC-BY-NC-4.0ライセンスの下でライセンスされており、非商用目的のみで使用できます。";
            }
            else
            {
                CreatorTextBlock.Text = "Created by: Raifa";
                SupportThankYouTextBlock.Text = "Thank you for your support! You can support me";
                hyperlinktext.Text = "here";
                SupportThankYouTextSuffix.Text = "";
                MITLicenseTextBlock.Text = "This application is licensed under the MIT License.";
                ComplianceTextBlock.Text =
                    "By using this application, you agree to comply with all applicable licenses.";
                CCBYNCTextBlock.Text =
                    "Some components are separately licensed under CC-BY-NC-4.0 and are for non-commercial use only.";

            }
        }
    }
}
