using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DailyConditionApp.ViewModels
{
    public partial class PostedDailyViewModel:BaseViewModel
    {
        [RelayCommand]
        private async Task GoToMain()
        {
            await Shell.Current.GoToAsync("///MainView");
        }
    }
}
