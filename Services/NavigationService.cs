using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using DailyConditionApp.Views;

namespace DailyConditionApp.Services
{
    public class NavigationService : INavigationService
    {
        readonly List<Func<Task<bool>>> _backHandlers = new();

        public IDisposable RegisterBackHandler(Func<Task<bool>> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _backHandlers.Add(handler);
            return new Unsubscriber(_backHandlers, handler);
        }

        public async Task<bool> OnBackPressedAsync()
        {
            // Call last registered handler first
            for (int i = _backHandlers.Count - 1; i >= 0; i--)
            {
                var handler = _backHandlers[i];
                try
                {
                    var handled = await handler();
                    if (handled) return true; // handled -> suppress default
                }
                catch
                {
                    // ignore individual handler errors
                }
            }

            // No handler handled the back action - decide default behavior:
            // - If current page is MainView -> return false to allow OS to exit app
            // - Otherwise -> navigate to MainView (root) and suppress OS exit
            var shell = Shell.Current;
            if (shell == null)
            {
                return false;
            }

            var current = shell.CurrentPage;
            if (current is MainView)
            {
                // On MainView, let the OS handle the back (exit app)
                return false;
            }

            // For any other page, navigate back to MainView as a root
            try
            {
                await shell.GoToAsync("//MainView", true);
                return true; // handled -> suppress OS default
            }
            catch
            {
                // If navigation failed, fall back to letting OS handle it
                return false;
            }
        }

        public Task GoBackAsync()
        {
            var nav = Shell.Current?.Navigation;
            if (nav != null && nav.NavigationStack.Count > 1)
            {
                return nav.PopAsync();
            }
            return Task.CompletedTask;
        }

        public Task NavigateToAsync(string route, bool animate = true)
        {
            if (Shell.Current == null) return Task.CompletedTask;
            return Shell.Current.GoToAsync(route, animate);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<Func<Task<bool>>> _list;
            private readonly Func<Task<bool>> _handler;

            public Unsubscriber(List<Func<Task<bool>>> list, Func<Task<bool>> handler)
            {
                _list = list;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_list.Contains(_handler)) _list.Remove(_handler);
            }
        }
    }
}
