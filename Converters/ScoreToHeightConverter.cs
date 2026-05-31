using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace DailyConditionApp.Converters
{
    public class ScoreToHeightConverter : IValueConverter
    {
        // Map score range 0..100 to exact pixel height in the chart area.
        // Set MinHeight to 0 so a score of 0 renders at the baseline.
        // Set MaxHeight to 200 to match the chart area HeightRequest in XAML.
        public double MinHeight { get; set; } = 0;
        public double MaxHeight { get; set; } = 200;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return MinHeight;

            if (value is int score)
            {
                // score is 0..100. Map to MinHeight..MaxHeight
                var clamped = Math.Max(0, Math.Min(100, score));
                double ratio = clamped / 100.0;
                return MinHeight + (MaxHeight - MinHeight) * ratio;
            }

            // if binding provides a boxed int
            if (value is int n2)
            {
                var clamped = Math.Max(0, Math.Min(100, n2));
                double ratio = clamped / 100.0;
                return MinHeight + (MaxHeight - MinHeight) * ratio;
            }

            return MinHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
