using DailyConditionApp.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace DailyConditionApp.Views;

public partial class DailyInputView : ContentPage
{
	public DailyInputView(DailyInputViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

	protected async override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is DailyInputViewModel vm)
		{
			// Trigger weather fetch when the page appears
			// Use ExecuteAsync if available to await the async command
			if (vm.FetchWeatherCommand is IAsyncRelayCommand asyncCmd)
			{
				await asyncCmd.ExecuteAsync(null);
			}
			else
			{
				vm.FetchWeatherCommand.Execute(null);
			}
		}
	}
}