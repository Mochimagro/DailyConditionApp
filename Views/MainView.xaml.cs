using System.ComponentModel;
using Microsoft.Maui.ApplicationModel;

namespace DailyConditionApp.Views
{
    public partial class MainView : ContentPage
    {
        public MainView(ViewModels.MainViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}