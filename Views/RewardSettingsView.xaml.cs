using DailyConditionApp.ViewModels;

namespace DailyConditionApp.Views
{
    public partial class RewardSettingsView : BaseContentPage
    {
        public RewardSettingsView(RewardSettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
