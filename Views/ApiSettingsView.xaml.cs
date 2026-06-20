using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views
{
    public partial class ApiSettingsView : BaseContentPage
    {
        public ApiSettingsView(ApiSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
