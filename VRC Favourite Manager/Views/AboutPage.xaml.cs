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
                CreatorTextBlock.Text = "�쐬��: Raifa";
                SupportThankYouTextBlock.Text = "���x�����肪�Ƃ��������܂��I�x����";
                hyperlinktext.Text = "������";
                SupportThankYouTextSuffix.Text = "����I";
                MITLicenseTextBlock.Text = "���̃A�v���P�[�V������MIT���C�Z���X�̉��Ń��C�Z���X����Ă��܂��B";
                ComplianceTextBlock.Text = "���̃A�v���P�[�V�������g�p���邱�ƂŁA���ׂĂ̓K�p����郉�C�Z���X�ɏ]�����Ƃɓ��ӂ�����̂Ƃ��܂��B";
                CCBYNCTextBlock.Text = "�ꕔ�̃R���|�[�l���g��CC-BY-NC-4.0���C�Z���X�̉��Ń��C�Z���X����Ă���A�񏤗p�ړI�݂̂Ŏg�p�ł��܂��B";
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
