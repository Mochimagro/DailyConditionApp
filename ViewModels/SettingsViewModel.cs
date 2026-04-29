using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage;

namespace DailyConditionApp.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _apiKey;

        public SettingsViewModel()
        {
            ApiKey = "入力して下さい";
        }

    }
}
