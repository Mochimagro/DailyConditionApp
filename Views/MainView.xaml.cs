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

            if (viewModel is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModels.MainViewModel.SelectedMenu))
            {
                var vm = BindingContext as ViewModels.MainViewModel;
                var selected = vm?.SelectedMenu;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (string.IsNullOrEmpty(selected))
                    {
                        DetailContent.Content = CreatePlaceholder();
                        return;
                    }

                    switch (selected)
                    {
                        case "DailyConditionResult":
                        case "サンプルA":
                            DetailContent.Content = new DailyConditionResultView().Content;
                            break;
                        case "DailyInputView":
                        case "サンプルB":
                            DetailContent.Content = new DailyInputView().Content;
                            break;
                        default:
                            DetailContent.Content = CreatePlaceholder(selected);
                            break;
                    }
                });
            }
        }

        private View CreatePlaceholder(string? title = null)
        {
            var stack = new VerticalStackLayout { Spacing = 12 };
            stack.Add(new Label { Text = title is null ? "サイドメニューから項目を選択してください。" : $"{title}（プレースホルダ）" });
            return stack;
        }
    }
}