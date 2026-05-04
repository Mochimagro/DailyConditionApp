using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DailyConditionApp.Services;

namespace DailyConditionApp.ViewModels
{
    public partial class DailyConditionResultViewModel : BaseViewModel
    {
        private readonly INotionService _notionService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty] private string _environmentScoreText = "--";
        [ObservableProperty] private string _conditionCommentText = "読み込み中...";

        public DailyConditionResultViewModel(INotionService notionService, ISettingsService settingsService)
        {
            _notionService = notionService;
            _settingsService = settingsService;
        }

        [RelayCommand]
        public async Task LoadResultAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var notionSettings = await _settingsService.LoadNotionKeyAsync();
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                var result = await _notionService.GetTodayConditionAsync(notionSettings.token, notionSettings.databaseId, today);

                if (result != null)
                {
                    EnvironmentScoreText = Math.Ceiling(result.EnvironmentScore).ToString();
                    ConditionCommentText = result.ConditionComment;
                }
                else
                {
                    ConditionCommentText = "本日のデータはまだ登録されていません。";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
