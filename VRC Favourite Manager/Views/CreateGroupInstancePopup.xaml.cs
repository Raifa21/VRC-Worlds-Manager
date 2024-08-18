using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VRC_Favourite_Manager.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using VRC_Favourite_Manager.Models;


namespace VRC_Favourite_Manager.Views
{
    public sealed partial class CreateGroupInstancePopup : ContentDialog
    {
        public CreateGroupInstancePopup(WorldModel world, string region)
        {
            this.InitializeComponent();
            this.DataContext = new CreateGroupInstancePopupViewModel(world, region);
        }
        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            this.Hide();
        }
        private void GroupSelected(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (sender != null)
            {
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                viewModel.GroupSelected(button.Name);
            }
        }
    }
}
