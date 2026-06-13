using System;
using DailyConditionApp.Services;
using Microsoft.Maui.Controls;

namespace DailyConditionApp.Views
{
    public class BaseContentPage : ContentPage
    {
        protected readonly INavigationService? NavigationService;

        public BaseContentPage()
        {
            if (MauiProgram.CurrentServiceProvider != null)
            {
                NavigationService = MauiProgram.CurrentServiceProvider.GetService(typeof(INavigationService)) as INavigationService;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (NavigationService == null)
            {
                return base.OnBackButtonPressed();
            }

            // Block and wait for async handler
            var handled = NavigationService.OnBackPressedAsync().GetAwaiter().GetResult();
            if (handled)
            {
                // Handled - suppress default behavior
                return true;
            }

            // Not handled - let OS perform default behavior (possibly exit app)
            return base.OnBackButtonPressed();
        }
    }
}
