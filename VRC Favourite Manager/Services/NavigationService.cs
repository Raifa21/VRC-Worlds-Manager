using System;
using Microsoft.UI.Xaml.Controls;

namespace VRC_Favourite_Manager.Services
{
    public class NavigationService
    {
        private Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate(Type sourcePageType)
        {
            _frame.Navigate(sourcePageType);
        }
    }
}
