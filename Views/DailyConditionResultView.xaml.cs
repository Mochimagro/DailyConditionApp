using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views;

public partial class DailyConditionResultView : ContentView
{
    public DailyConditionResultView()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (BindingContext is DailyConditionResultViewModel) return;

        if (MauiProgram.CurrentServiceProvider != null)
        {
            var vm = MauiProgram.CurrentServiceProvider.GetService<DailyConditionResultViewModel>();
            BindingContext = vm;

            // 配置されたタイミングで自動的にNotionへデータを読み込みに行き、読み込み完了後に通知を表示
            if (vm != null && vm.EnvironmentScoreText == "--")
            {
                await vm.LoadResultAsync();
                if (vm.HasTodayData)
                {
                    vm.PushScoreNotificationCommand?.Execute(null);
                }
            }
        }
    }
}