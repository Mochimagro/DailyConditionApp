using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views;

public partial class DailyInputView : ContentPage
{
	public DailyInputView(DailyInputViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}