using System.ComponentModel;
using DailyConditionApp.ViewModels;
using Microsoft.Maui.ApplicationModel;

namespace DailyConditionApp.Views
{
    public partial class MainView : BaseContentPage
    {
        private System.IDisposable? _backHandlerSubscription;

        public MainView(ViewModels.MainViewModel viewModel,DailyConditionResultViewModel resultViewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            ResultView.BindingContext = resultViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // DIコンテナからViewModelを取得してコマンドを実行
            if (MauiProgram.CurrentServiceProvider != null)
            {
                var resultVm = MauiProgram.CurrentServiceProvider.GetService<DailyConditionResultViewModel>();
                // すでに読み込み中でなければ実行
                if (resultVm != null && !resultVm.IsBusy)
                {
                    resultVm.LoadResultCommand.Execute(null);
                }
            }

            // MainViewではバック操作をアプリ終了へ委ねるため、ハンドラを登録しても常に false を返す
            // (NavigationService は登録ハンドラを順に呼び出すため、ここで true を返すと終了が抑制されてしまう)
            if (NavigationService != null)
            {
                _backHandlerSubscription = NavigationService.RegisterBackHandler(() => System.Threading.Tasks.Task.FromResult(false));
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // 登録解除
            _backHandlerSubscription?.Dispose();
            _backHandlerSubscription = null;
        }
    }
}