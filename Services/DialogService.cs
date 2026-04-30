using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public class DialogService : IDialogService
    {
        public async Task ShowToastAsync(string message)
        {
            await Toast.Make(message, ToastDuration.Short).Show();
        }
    }
}
