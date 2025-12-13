using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using ZyphraTrades.Application.Abstractions;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Domain.Trading;

namespace ZyphraTrades.Presentation;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ITradeRepository _repo;

    public ObservableCollection<Trade> Trades { get; } = new();

    private ICollectionView? _tradesView;
    public ICollectionView TradesView
    {
        get => _tradesView ??= CollectionViewSource.GetDefaultView(Trades);
        private set { _tradesView = value; OnPropertyChanged(); }
    }

    private Trade? _selectedTrade;
    public Trade? SelectedTrade
    {
        get => _selectedTrade;
        set
        {
            if (_selectedTrade == value) return;
            _selectedTrade = value;
            OnPropertyChanged();
            RaiseCanExecuteChanged();
        }
    }

    // ===== Filters =====
    private string _symbolFilter = "";
    public string SymbolFilter
    {
        get => _symbolFilter;
        set { _symbolFilter = value; OnPropertyChanged(); }
    }

    public IReadOnlyList<string> SideFilterOptions { get; } = new[] { "All", "Buy", "Sell" };

    private string _sideFilter = "All";
    public string SideFilter
    {
        get => _sideFilter;
        set { _sideFilter = value; OnPropertyChanged(); }
    }

    private DateTime? _fromDate;
    public DateTime? FromDate
    {
        get => _fromDate;
        set { _fromDate = value; OnPropertyChanged(); }
    }

    private DateTime? _toDate;
    public DateTime? ToDate
    {
        get => _toDate;
        set { _toDate = value; OnPropertyChanged(); }
    }

    // ===== KPIs =====
    private int _totalTrades;
    public int TotalTrades
    {
        get => _totalTrades;
        private set { _totalTrades = value; OnPropertyChanged(); }
    }

    private decimal _netPnlTotal;
    public decimal NetPnlTotal
    {
        get => _netPnlTotal;
        private set { _netPnlTotal = value; OnPropertyChanged(); }
    }

    private string _winRateText = "0%";
    public string WinRateText
    {
        get => _winRateText;
        private set { _winRateText = value; OnPropertyChanged(); }
    }

    private string _avgRText = "0.00";
    public string AvgRText
    {
        get => _avgRText;
        private set { _avgRText = value; OnPropertyChanged(); }
    }

    private string _statusText = "Ready";
    public string StatusText
    {
        get => _statusText;
        private set { _statusText = value; OnPropertyChanged(); }
    }

    // ===== Commands =====
    public ICommand RefreshCommand { get; }
    public ICommand AddDummyTradeCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand DeleteSelectedCommand { get; }
    public ICommand ExportCsvCommand { get; }

    public MainViewModel(ITradeRepository repo)
    {
        _repo = repo;

        RefreshCommand = new AsyncCommand(LoadAsync);
        AddDummyTradeCommand = new AsyncCommand(AddDummyAsync);
        ApplyFilterCommand = new AsyncCommand(ApplyFilterAsync);
        DeleteSelectedCommand = new AsyncCommand(DeleteSelectedAsync, () => SelectedTrade != null);
        ExportCsvCommand = new AsyncCommand(ExportCsvAsync, () => Trades.Count > 0);

        // Setup view filtering
        TradesView = CollectionViewSource.GetDefaultView(Trades);
        TradesView.Filter = FilterTrade;
    }

    public async Task LoadAsync()
    {
        try
        {
            StatusText = "Loading trades…";
            Trades.Clear();

            var items = await _repo.GetAllAsync();

            // (Si aún no tienes converter para DateTimeOffset+SQLite, evita ordenar en SQL.
            //  Si ya lo tienes, puedes ordenar en el repo. Aquí ordenamos en memoria.)
            foreach (var t in items.OrderByDescending(t => t.OpenedAt))
                Trades.Add(t);

            SelectedTrade = Trades.FirstOrDefault();

            TradesView.Refresh();
            RecalculateKpis();
            StatusText = $"Loaded {Trades.Count} trades";
            RaiseCanExecuteChanged();
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            throw;
        }
    }

    public async Task AddDummyAsync()
    {
        try
        {
            StatusText = "Creating dummy trade…";

            var trade = new Trade
            {
                Symbol = "AUDUSD",
                Side = TradeSide.Buy,
                Timeframe = "H1",
                OpenedAt = DateTimeOffset.UtcNow,

                EntryPrice = 0.6550m,
                ExitPrice = 0.6580m,

                GrossPnl = 25.50m,
                NetPnl = 25.50m,

                ResultR = 1.50m,
                Setup = "Break & Retest",
                Notes = "Primera prueba de journal"
            };

            await _repo.AddAsync(trade);
            await _repo.SaveChangesAsync();

            await LoadAsync();
            StatusText = "Dummy trade added";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            throw;
        }
    }

    private Task ApplyFilterAsync()
    {
        TradesView.Refresh();
        RecalculateKpis();
        StatusText = "Filter applied";
        return Task.CompletedTask;
    }
    private async Task DeleteSelectedAsync()
    {
        if (SelectedTrade == null)
            return;

        try
        {
            StatusText = "Deleting trade…";

            await _repo.DeleteAsync(SelectedTrade.Id);
            await _repo.SaveChangesAsync();

            await LoadAsync();

            StatusText = "Trade deleted successfully";
        }
        catch (Exception ex)
        {
            StatusText = $"Error deleting trade: {ex.Message}";
            throw;
        }
    }


    private async Task ExportCsvAsync()
    {
        try
        {
            StatusText = "Exporting CSV…";

            var exportDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ZyphraTrades",
                "Exports");

            Directory.CreateDirectory(exportDir);

            var file = Path.Combine(exportDir, $"trades_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            // Exportamos lo que está visible por filtro
            var visible = TradesView.Cast<Trade>().ToList();

            using var sw = new StreamWriter(file);
            sw.WriteLine("OpenedAtUTC,Symbol,Side,Timeframe,Entry,Exit,NetPnl,ResultR,Setup,Notes");

            foreach (var t in visible)
            {
                var line = string.Join(",",
                    Escape(t.OpenedAt.UtcDateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)),
                    Escape(t.Symbol),
                    Escape(t.Side.ToString()),
                    Escape(t.Timeframe ?? ""),
                    t.EntryPrice.ToString(CultureInfo.InvariantCulture),
                    (t.ExitPrice ?? 0m).ToString(CultureInfo.InvariantCulture),
                    t.NetPnl.ToString(CultureInfo.InvariantCulture),
                    (t.ResultR ?? 0m).ToString(CultureInfo.InvariantCulture),
                    Escape(t.Setup ?? ""),
                    Escape(t.Notes ?? "")
                );

                sw.WriteLine(line);
            }

            StatusText = $"CSV exported: {file}";
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
            throw;
        }
    }

    private static string Escape(string s)
    {
        if (s.Contains('"') || s.Contains(',') || s.Contains('\n'))
            return $"\"{s.Replace("\"", "\"\"")}\"";
        return s;
    }

    private bool FilterTrade(object obj)
    {
        if (obj is not Trade t) return false;

        if (!string.IsNullOrWhiteSpace(SymbolFilter))
        {
            if (string.IsNullOrWhiteSpace(t.Symbol)) return false;
            if (!t.Symbol.Contains(SymbolFilter.Trim(), StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (SideFilter == "Buy" && t.Side != TradeSide.Buy) return false;
        if (SideFilter == "Sell" && t.Side != TradeSide.Sell) return false;

        if (FromDate.HasValue)
        {
            var from = FromDate.Value.Date;
            if (t.OpenedAt.UtcDateTime.Date < from) return false;
        }

        if (ToDate.HasValue)
        {
            var to = ToDate.Value.Date;
            if (t.OpenedAt.UtcDateTime.Date > to) return false;
        }

        return true;
    }

    private void RecalculateKpis()
    {
        var visible = TradesView.Cast<Trade>().ToList();

        TotalTrades = visible.Count;
        NetPnlTotal = visible.Sum(t => t.NetPnl);

        if (visible.Count == 0)
        {
            WinRateText = "0%";
            AvgRText = "0.00";
            return;
        }

        var wins = visible.Count(t => t.NetPnl > 0);
        var winRate = (double)wins / visible.Count;
        WinRateText = $"{winRate:P0}";

        var rValues = visible.Where(t => t.ResultR.HasValue).Select(t => t.ResultR!.Value).ToList();
        AvgRText = rValues.Count == 0 ? "0.00" : rValues.Average().ToString("0.00", CultureInfo.InvariantCulture);
    }

    private void RaiseCanExecuteChanged()
    {
        if (DeleteSelectedCommand is AsyncCommand ac1) ac1.RaiseCanExecuteChanged();
        if (ExportCsvCommand is AsyncCommand ac2) ac2.RaiseCanExecuteChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
