using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;
using Microsoft.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace DailyConditionApp.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;

        public SettingsViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        [RelayCommand]
        private async Task GoToApiSettingsAsync()
        {
            await Shell.Current.GoToAsync("ApiSettingsView");
        }

        [RelayCommand]
        private async Task GoToRewardSettingsAsync()
        {
            await Shell.Current.GoToAsync("RewardSettingsView");
        }
    }
}
