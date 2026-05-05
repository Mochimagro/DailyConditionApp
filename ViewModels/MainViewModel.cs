using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace DailyConditionApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "";


        public MainViewModel()
        {
            Title = $"{DateTime.Today.ToString("yyyy年MM月dd日")}";
        }

        // サイドメニューからではなく、画面内のボタンから直接PageAに遷移したい場合の例
        [RelayCommand]
        private async Task GoToDailyInput()
        {
            // AppShellで定義した Route="DailyInputView" の名前を使って遷移します
            await Shell.Current.GoToAsync("///DailyInputView");
        }
    }

}
