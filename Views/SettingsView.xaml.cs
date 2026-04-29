using DailyConditionApp.ViewModels;
using Microsoft.Maui.Controls;

namespace DailyConditionApp.Views;

public partial class SettingsView : ContentPage
{
	public SettingsView(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}