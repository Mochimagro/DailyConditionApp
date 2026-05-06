using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public interface INotificationService
    {
        void ShowPersistentNotification(string title, string message);
    }
}
