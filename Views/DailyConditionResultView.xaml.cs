using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views;

public partial class DailyConditionResultView : ContentView
{
    public DailyConditionResultView()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is DailyConditionResultViewModel) return;

        if (MauiProgram.CurrentServiceProvider != null)
        {
            var vm = MauiProgram.CurrentServiceProvider.GetService<DailyConditionResultViewModel>();
            BindingContext = vm;

            // 配置されたタイミングで自動的にNotionへデータを読み込みに行く
            if (vm != null && vm.EnvironmentScoreText == "--")
            {
                vm.LoadResultCommand.Execute(null);
            }
        }
    }
}