// WorldDetailsPopup.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Diagnostics;
using VRC_Favourite_Manager.Models;
using VRChat.API.Model;

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
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void CreateInstanceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var world = (WorldModel)this.DataContext;
            
        }
        private void InstanceType_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedInstanceType = radioButton.Content.ToString();
                Debug.WriteLine("Selected Instance Type: " + selectedInstanceType);
                // Handle the selection change, e.g., update the view model
            }
        }
        private void AuthorLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = $"https://vrchat.com/home/user/{World.AuthorId}";
            Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }
        private void WorldLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = $"https://vrchat.com/home/world/{World.WorldId}";
            Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }

        private void Region_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedRegion = radioButton.Content.ToString();
                Debug.WriteLine("Selected Region: " + selectedRegion);
                // Handle the selection change, e.g., update the view model
            }
        }
    }
}