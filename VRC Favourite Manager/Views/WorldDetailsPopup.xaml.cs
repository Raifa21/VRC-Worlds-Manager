// WorldDetailsPopup.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Diagnostics;
using VRC_Favourite_Manager.Models;
using VRC_Favourite_Manager.ViewModels;
using VRChat.API.Model;

namespace VRC_Favourite_Manager.Views
{
    public sealed partial class WorldDetailsPopup : ContentDialog
    {
        public WorldModel World { get; set; }
        private string _selectedInstanceType;
        private string _selectedRegion;

        public WorldDetailsPopup(WorldModel world)
        {
            this.InitializeComponent();
            this.World = world;
            this.DataContext = world;
            
            _selectedInstanceType = "Public";
            _selectedRegion = "JP";
        }
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void CreateInstanceButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var world = (WorldModel)this.DataContext;
            if (_selectedInstanceType != "Group")
            {
                WorldDetailsPopupViewModel viewModel = new WorldDetailsPopupViewModel(world);
                viewModel.CreateInstanceAsync(world.WorldId, _selectedInstanceType, _selectedRegion);
            }
            else
            { 
                this.Hide();

                var createGroupInstance = new CreateGroupInstancePopup(world, _selectedRegion);
                createGroupInstance.ShowAsync();
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
        private void InstanceType_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedInstanceType = radioButton.Content.ToString();
                _selectedInstanceType = selectedInstanceType;
            }
        }
        private void Region_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedRegion = radioButton.Content.ToString();
                _selectedRegion = selectedRegion;
            }
        }
    }
}