// WorldDetailsPopup.xaml.cs
using Microsoft.UI.Xaml.Controls;
using VRC_Favourite_Manager.Models;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class WorldDetailsPopup : ContentDialog
    {
        public WorldModel World { get; set; }

        public WorldDetailsPopup(WorldModel world)
        {
            this.InitializeComponent();
            this.World = world;
            this.DataContext = world;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Handle close button click
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Handle create instance button click
        }
    }
}