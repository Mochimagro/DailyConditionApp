using DailyConditionApp.ViewModels;
using Microsoft.Maui.Controls;

namespace DailyConditionApp.Views;

public partial class SettingsView : BaseContentPage
{
	public SettingsView(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}