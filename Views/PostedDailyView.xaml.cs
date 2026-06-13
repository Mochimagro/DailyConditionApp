using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views;

public partial class PostedDailyView : BaseContentPage
{
	public PostedDailyView(PostedDailyViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
    }
}