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
                viewModel.GroupSelected(button.CommandParameter as string);
                if (button.Name == "Restricted")
                {
                    viewModel.IsRoleRestricted = true;
                }
            }
        }

        private void SelectType(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (sender != null)
            {
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                viewModel.AccessTypeSelected(button.Name);
                viewModel.IsGroupRolesLoadingComplete = false;
            }
        }

        private void ShowInstanceSelect(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.IsGroupRolesLoadingComplete = true;
        }

        private void HideInstanceSelect(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.IsGroupRolesLoadingComplete = false;
        }

        private void SelectRolesChanged_Checked(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.SelectRolesChanged();
        }
        private void SelectRolesChanged_Unchecked(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.SelectRolesChanged();
        }

        private void RolesSelected(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.RolesSelected();
        }

        private void InvertInstanceQueue(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.InvertInstanceQueue();
        }

        private void Region_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string selectedRegion = radioButton.Content.ToString();
                var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
                if(viewModel != null)
                {
                    viewModel.Region = selectedRegion;
                }
            }
        }
        private void CreateInstance(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateGroupInstancePopupViewModel)this.DataContext;
            viewModel.CreateInstanceAsync();

            this.Hide();
        }
    }
}
