using ZyphraTrades.Presentation.ViewModels;

namespace ZyphraTrades.Presentation.Models;

/// <summary>
/// View-model for a single partial take-profit entry in the trade form.
/// </summary>
public sealed class PartialTakeProfitItem : ViewModelBase
{
    private int _index;
    public int Index { get => _index; set => SetProperty(ref _index, value); }

    private decimal _exitPrice;
    public decimal ExitPrice { get => _exitPrice; set => SetProperty(ref _exitPrice, value); }

    private decimal _quantity;
    public decimal Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }

    private decimal _realizedPnl;
    public decimal RealizedPnl { get => _realizedPnl; set => SetProperty(ref _realizedPnl, value); }

    private decimal _percentClosed;
    public decimal PercentClosed { get => _percentClosed; set => SetProperty(ref _percentClosed, value); }

    private DateTime _closedDate = DateTime.UtcNow.Date;
    public DateTime ClosedDate { get => _closedDate; set => SetProperty(ref _closedDate, value); }

    private bool _movedToBreakeven;
    public bool MovedToBreakeven { get => _movedToBreakeven; set => SetProperty(ref _movedToBreakeven, value); }

    private string? _notes;
    public string? Notes { get => _notes; set => SetProperty(ref _notes, value); }
}
