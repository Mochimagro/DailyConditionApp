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
        }
    }
}