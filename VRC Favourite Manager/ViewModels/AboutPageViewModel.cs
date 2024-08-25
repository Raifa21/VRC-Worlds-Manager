using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using VRC_Favourite_Manager.Common;
using VRC_Favourite_Manager.Services;
using VRC_Favourite_Manager.Views;
using VRChat.API.Model;

namespace VRC_Favourite_Manager.ViewModels
{
    public class AboutPageViewModel : ViewModelBase
    {
        public SupportersData supportersData { get; private set; }


        public async Task LoadSupportersAsync()
        {
            var service = new GitHubSupporterService();
            supportersData = await service.GetSupportersAsync("Raifa21", "VRC-Worlds-Manager", "supporters.json");

            // Example: Iterate through the supporters and print their names
            foreach (var supporter in supportersData.Supporters)
            {
                Console.WriteLine($"{supporter.Name} (Tier {supporter.Tier}): {supporter.Color}");
            }

            // Example: Check the tiers without supporters
            Console.WriteLine($"Tier 1: {supportersData.TiersWithoutSupporters.Tier1.Note}");
            Console.WriteLine($"Tier 5: {supportersData.TiersWithoutSupporters.Tier5.Note}");
        }
    }
}