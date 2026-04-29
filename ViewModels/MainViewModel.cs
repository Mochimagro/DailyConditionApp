using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DailyConditionApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _greetingMessage = "Tap Button";

        // サイドメニュー項目
        public ObservableCollection<string> MenuItems { get; } = new ObservableCollection<string>
        {
            "サンプルA",            // DailyConditionResult に対応
            "サンプルB",            // DailyInput に対応
            "DailyConditionResult",
            "DailyInputView"
        };

        [ObservableProperty]
        private string _selectedMenu;

        [ObservableProperty]
        private bool _isMenuVisible;

        [RelayCommand]
        private void ToggleMenu()
        {
            IsMenuVisible = !IsMenuVisible;
        }

        [RelayCommand]
        private void ShowHelloMessage()
        {
            GreetingMessage = "Hello,world";
        }
    }
}
