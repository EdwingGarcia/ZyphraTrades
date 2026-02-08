namespace ZyphraTrades.Application.DTOs;

public sealed record EquityCurvePoint(
    DateTimeOffset Date,
    decimal CumulativePnl,
    decimal Drawdown,
    int TradeNumber);
