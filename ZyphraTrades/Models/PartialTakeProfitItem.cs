using ZyphraTrades.Presentation.ViewModels;

namespace ZyphraTrades.Presentation.Models;

/// <summary>
/// View-model for a single partial take-profit entry in the trade form.
/// Simplified: only % of position, price or RR, and optional notes.
/// </summary>
public sealed class PartialTakeProfitItem : ViewModelBase
{
    private int _index;
    public int Index { get => _index; set => SetProperty(ref _index, value); }

    /// <summary>Percentage of the position closed in this partial (0-100).</summary>
    private decimal _percentClosed;
    public decimal PercentClosed { get => _percentClosed; set => SetProperty(ref _percentClosed, value); }

    /// <summary>Exit price for this partial closure.</summary>
    private decimal _exitPrice;
    public decimal ExitPrice { get => _exitPrice; set => SetProperty(ref _exitPrice, value); }

    /// <summary>R-multiple at which this partial was taken (alternative to exit price).</summary>
    private decimal? _exitRR;
    public decimal? ExitRR { get => _exitRR; set => SetProperty(ref _exitRR, value); }

    /// <summary>Optional notes for this partial.</summary>
    private string? _notes;
    public string? Notes { get => _notes; set => SetProperty(ref _notes, value); }
}
