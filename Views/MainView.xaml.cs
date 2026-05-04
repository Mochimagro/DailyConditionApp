using System.ComponentModel;
using DailyConditionApp.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace DailyConditionApp.Views
{
    public partial class MainView : ContentPage
    {
        public MainView(ViewModels.MainViewModel viewModel,DailyConditionResultViewModel resultViewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            ResultView.BindingContext = resultViewModel;
        }
    }
}