using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views;
public partial class WeeklyResultsView : ContentPage
{
    public WeeklyResultsView()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is WeeklyResultsViewModel) return;

        if (MauiProgram.CurrentServiceProvider != null)
        {
            var vm = MauiProgram.CurrentServiceProvider.GetService<WeeklyResultsViewModel>();
            BindingContext = vm;
        }
    }
}