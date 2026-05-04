using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views;

public partial class PostedDailyView : ContentPage
{
	public PostedDailyView(PostedDailyViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
    }
}