using ZyphraTrades.Domain.Trading;

namespace ZyphraTrades.Application.DTOs;

public sealed class TradeFilterCriteria
{
    public string? SymbolFilter { get; set; }
    public TradeSide? SideFilter { get; set; }
    public TradeStatus? StatusFilter { get; set; }
    public string? SetupFilter { get; set; }
    public SessionType? SessionFilter { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? WinnersOnly { get; set; }
    public decimal? MinPnl { get; set; }
    public decimal? MaxPnl { get; set; }
}
