namespace DailyConditionApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // 明示的にルートを登録
            Routing.RegisterRoute("PostedDailyView", typeof(Views.PostedDailyView));
        }
    }
}
