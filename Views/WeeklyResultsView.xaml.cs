using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views;
public partial class WeeklyResultsView : BaseContentPage
{
    public WeeklyResultsView(WeeklyResultsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // ページ表示時にデータをロードする例
        // ViewModel 側でロードコマンドを公開しているため、OnAppearingで呼ぶのが安全
        this.Appearing += async (s, e) =>
        {
            if (!viewModel.IsBusy && viewModel.WeeklyScores.Count == 0)
            {
                await viewModel.LoadWeeklyDataAsync();
            }
        };
    }

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();

    //    // 既に BindingContext に ViewModel がセットされていなければ DI から取得してセット
    //    if (BindingContext is not WeeklyResultsViewModel vm)
    //    {
    //        if (MauiProgram.CurrentServiceProvider != null)
    //        {
    //            vm = MauiProgram.CurrentServiceProvider.GetService<WeeklyResultsViewModel>();
    //            if (vm != null)
    //            {
    //                BindingContext = vm;

    //                if (!vm.IsBusy)
    //                {
    //                    // CommunityToolkit の RelayCommand は Async メソッドの Async サフィックスを取り除いた名前でコマンドを生成するため
    //                    // LoadWeeklyDataAsync -> LoadWeeklyDataCommand が存在する想定
    //                    vm.LoadWeeklyDataCommand.Execute(null);
    //                }
    //            }
    //        }
    //    }
    //}
}