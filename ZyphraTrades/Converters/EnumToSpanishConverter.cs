using System.Globalization;
using System.Windows.Data;
using ZyphraTrades.Domain.Trading;

namespace ZyphraTrades.Presentation.Converters;

/// <summary>
/// Converts enum values to user-friendly Spanish display strings.
/// Used in ComboBox ItemTemplate to show Spanish labels while keeping enum values for binding.
/// </summary>
public sealed class EnumToSpanishConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            // TradeSide
            TradeSide.Buy => "Compra (Buy)",
            TradeSide.Sell => "Venta (Sell)",

            // TradeStatus
            TradeStatus.Open => "Abierto",
            TradeStatus.Closed => "Cerrado",
            TradeStatus.BreakEven => "Break-Even",
            TradeStatus.Cancelled => "Cancelado",

            // EmotionalState
            EmotionalState.Calm => "Calmado",
            EmotionalState.Confident => "Confiado",
            EmotionalState.Anxious => "Ansioso",
            EmotionalState.Fearful => "Temeroso",
            EmotionalState.Greedy => "Codicioso",
            EmotionalState.FOMO => "FOMO",
            EmotionalState.Revenge => "Venganza",
            EmotionalState.Frustrated => "Frustrado",
            EmotionalState.Euphoric => "Eufórico",
            EmotionalState.Indifferent => "Indiferente",

            // MarketCondition
            MarketCondition.Trending => "Tendencia",
            MarketCondition.Ranging => "Rango / Lateral",
            MarketCondition.Volatile => "Volátil",
            MarketCondition.LowVolatility => "Baja Volatilidad",
            MarketCondition.Breakout => "Ruptura (Breakout)",
            MarketCondition.Reversal => "Reversión",

            // SessionType
            SessionType.Asian => "Asia",
            SessionType.London => "Londres",
            SessionType.NewYork => "Nueva York",
            SessionType.LondonNewYorkOverlap => "Londres-NY (Overlap)",
            SessionType.Sydney => "Sídney",
            SessionType.OffHours => "Fuera de Horario",

            // ConfluenceLevel
            ConfluenceLevel.VeryLow => "Muy Baja",
            ConfluenceLevel.Low => "Baja",
            ConfluenceLevel.Medium => "Media",
            ConfluenceLevel.High => "Alta",
            ConfluenceLevel.VeryHigh => "Muy Alta",

            // TradeOutcome
            TradeOutcome.Win => "Ganancia",
            TradeOutcome.Loss => "Pérdida",
            TradeOutcome.BreakEven => "Break-Even",

            _ => value?.ToString() ?? ""
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
