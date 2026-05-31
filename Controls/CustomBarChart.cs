using Microcharts;
using SkiaSharp;
using System;
using System.Linq;

namespace DailyConditionApp.Controls
{
    // Microcharts.BarChart を継承して、閾値ラインと棒上部の値表示を追加する
    public class CustomBarChart : BarChart
    {
        // 閾値値 (例: 80)
        public float Threshold { get; set; } = float.NaN;

        // 閾値ラインの色
        public SKColor ThresholdColor { get; set; } = SKColors.White;

        public float DashLength { get; set; } = 10f;
        public float GapLength { get; set; } = 6f;

        public new void Draw(SKCanvas canvas, int width, int height)
        {
            // まず標準描画
            base.Draw(canvas, width, height);

            if (Entries == null || !Entries.Any())
                return;

            // 値の範囲を取得
            float entriesMax = Entries.Max(e => e.Value ?? 0f);
            float entriesMin = Entries.Min(e => e.Value ?? 0f);
            var max = (!float.IsNaN(MaxValue) ? MaxValue : entriesMax);
            var min = (!float.IsNaN(MinValue) ? MinValue : entriesMin);
            var range = max - min;
            if (range <= 0)
                range = 1; // division guard

            // 棒の幅と間隔を簡易計算して、各棒の中心Xと上端Yを算出する
            int count = Entries.Count();
            float bandWidth = (float)width / count;

            // 値ラベル描画用のペイント
            using var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.Black,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName(null, SKFontStyleWeight.SemiBold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
                TextSize = Math.Max(12, height / 18f)
            };

            for (int i = 0; i < count; i++)
            {
                var entry = Entries.ElementAt(i);
                float value = entry.Value ?? 0f;
                // 中心X
                float xCenter = bandWidth * (i + 0.5f);
                // 棒の高さ
                float barHeight = (value - min) / range * height;
                float yTop = height - barHeight;

                // 値ラベル (上部に表示)
                if (!string.IsNullOrEmpty(entry.ValueLabel))
                {
                    // 少し余白を取る
                    float textY = yTop - 6f;
                    // テキストがキャンバス外に出る場合は内側に寄せる
                    if (textY < textPaint.TextSize)
                        textY = yTop + textPaint.TextSize + 4f; // 棒の内側に表示

                    canvas.DrawText(entry.ValueLabel, xCenter, textY, textPaint);
                }
            }

            // 閾値ラインを描画 (前面)
            if (!float.IsNaN(Threshold))
            {
                var y = height - (Threshold - min) / range * height;

                using var linePaint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    Color = ThresholdColor,
                    StrokeWidth = Math.Max(2, height / 120f),
                    PathEffect = SKPathEffect.CreateDash(new float[] { DashLength, GapLength }, 0)
                };

                canvas.DrawLine(0, y, width, y, linePaint);
            }
        }
    }
}
