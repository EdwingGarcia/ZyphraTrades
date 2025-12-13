using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyphraTrades.Domain.Trading;
namespace ZyphraTrades.Domain.Entities;

public class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset OpenedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ClosedAt { get; set; }

    public string Symbol { get; set; } = "AUDUSD";
    public TradeSide Side { get; set; }

    public string? Timeframe { get; set; } // "M15", "H1", etc.

    public decimal EntryPrice { get; set; }
    public decimal? ExitPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }

    public decimal? PositionSize { get; set; } // lots / units
    public decimal? Fees { get; set; }

    public decimal GrossPnl { get; set; }
    public decimal NetPnl { get; set; } // Gross - Fees

    // R-multiple / riesgo relativo (clave para expectancy)
    public decimal? RiskR { get; set; }   // 1R = lo que arriesgaste
    public decimal? ResultR { get; set; } // p.ej. +2.5R

    public string? Setup { get; set; }
    public string? Notes { get; set; }
}