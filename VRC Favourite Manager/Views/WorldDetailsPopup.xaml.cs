// WorldDetailsPopup.xaml.cs
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
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
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void CreateInstanceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Handle instance creation logic here
            var world = (WorldModel)this.DataContext;
            // Implement your logic for creating an instance
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