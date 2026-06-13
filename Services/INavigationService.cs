using System;
using System.Threading.Tasks;

namespace DailyConditionApp.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Handle a back action from the device (hardware or software).
        /// Return true if the back action was handled (and default behavior should be suppressed),
        /// or false to allow the OS to perform the default behavior (e.g. exit the app).
        /// </summary>
        Task<bool> OnBackPressedAsync();

        Task GoBackAsync();

        Task NavigateToAsync(string route, bool animate = true);

        /// <summary>
        /// Register a back handler which will be invoked when a back action occurs.
        /// The handler should return true if it handled the back action, false otherwise.
        /// The returned IDisposable can be disposed to unregister the handler.
        /// </summary>
        IDisposable RegisterBackHandler(Func<Task<bool>> handler);
    }
}
