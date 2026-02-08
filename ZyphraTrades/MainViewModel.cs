using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using ZyphraTrades.Application.DTOs;
using ZyphraTrades.Application.Services;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Domain.Trading;
using ZyphraTrades.Presentation.Commands;
using ZyphraTrades.Presentation.ViewModels;

namespace ZyphraTrades.Presentation;

public class MainViewModel : ViewModelBase
{
    private readonly ITradeService _svc;
    private readonly ISettingsService _settingsSvc;

    // ══════════════════════ Sub-ViewModels ══════════════════════

    public TradeFormViewModel TradeForm { get; }
    public SettingsViewModel Settings { get; }

    // ══════════════════════ Navigation ══════════════════════

    private string _currentPage = "AddTrade";
    public string CurrentPage
    {
        get => _currentPage;
        set
        {
            if (!SetProperty(ref _currentPage, value)) return;
            if (value == "Dashboard") _ = RefreshDashboardAsync();
            if (value == "Settings") _ = Settings.LoadAsync();
            if (value == "AddTrade") _ = TradeForm.LoadDynamicSectionsAsync();
        }
    }

    public ICommand NavigateCommand { get; }

    // ══════════════════════ Trade Collection ══════════════════════

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
        set { SetProperty(ref _selectedTrade, value); CommandManager.InvalidateRequerySuggested(); }
    }

    // ══════════════════════ Filters ══════════════════════

    private string _symbolFilter = "";
    public string SymbolFilter { get => _symbolFilter; set => SetProperty(ref _symbolFilter, value); }

    public IReadOnlyList<string> SideFilterOptions { get; } = new[] { "All", "Buy", "Sell" };

    private string _sideFilter = "All";
    public string SideFilter { get => _sideFilter; set => SetProperty(ref _sideFilter, value); }

    public IReadOnlyList<string> StatusFilterOptions { get; } = new[] { "All", "Open", "Closed" };

    private string _statusFilter = "All";
    public string StatusFilter { get => _statusFilter; set => SetProperty(ref _statusFilter, value); }

    private DateTime? _fromDate;
    public DateTime? FromDate { get => _fromDate; set => SetProperty(ref _fromDate, value); }

    private DateTime? _toDate;
    public DateTime? ToDate { get => _toDate; set => SetProperty(ref _toDate, value); }

    // ══════════════════════ Dashboard / Statistics ══════════════════════

    private PortfolioStatistics _stats = PortfolioStatistics.Empty;
    public PortfolioStatistics Stats { get => _stats; set => SetProperty(ref _stats, value); }

    // Formatted helpers for dashboard display
    public string AvgHoldTimeText => Stats.AverageHoldTime?.ToString(@"d\.hh\:mm") ?? "--";
    public string LongestHoldText => Stats.LongestHoldTime?.ToString(@"d\.hh\:mm") ?? "--";
    public string ShortestHoldText => Stats.ShortestHoldTime?.ToString(@"d\.hh\:mm") ?? "--";

    // ══════════════════════ Status Bar ══════════════════════

    private string _statusText = "Ready";
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

    // ══════════════════════ Commands ══════════════════════

    public ICommand RefreshCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand DeleteSelectedCommand { get; }
    public ICommand EditSelectedCommand { get; }
    public ICommand ExportCsvCommand { get; }

    // ══════════════════════ Constructor ══════════════════════

    public MainViewModel(ITradeService tradeService, ISettingsService settingsService)
    {
        _svc = tradeService;
        _settingsSvc = settingsService;

        // Sub-VMs
        TradeForm = new TradeFormViewModel(tradeService, settingsService)
        {
            OnCompleted = () =>
            {
                _ = LoadAsync();
                CurrentPage = "Trades";
                StatusText = "Trade saved ✔";
            },
            OnError = msg => StatusText = $"Error: {msg}"
        };

        Settings = new SettingsViewModel(settingsService);

        // Navigation
        NavigateCommand = new RelayCommand(p =>
        {
            if (p is string page) CurrentPage = page;
        });

        // Data commands
        RefreshCommand = new AsyncCommand(LoadAsync);
        ApplyFilterCommand = new AsyncCommand(ApplyFilterAsync);
        ClearFilterCommand = new RelayCommand(_ => ClearFilters());
        DeleteSelectedCommand = new AsyncCommand(DeleteSelectedAsync, () => SelectedTrade != null);
        EditSelectedCommand = new AsyncCommand(EditSelectedAsync, () => SelectedTrade != null);
        ExportCsvCommand = new AsyncCommand(ExportCsvAsync, () => Trades.Count > 0);

        TradesView = CollectionViewSource.GetDefaultView(Trades);
        TradesView.Filter = FilterTrade;
    }

    // ══════════════════════ Load ══════════════════════

    public async Task LoadAsync()
    {
        try
        {
            StatusText = "Loading trades…";
            Trades.Clear();

            var items = await _svc.GetAllAsync();
            foreach (var t in items.OrderByDescending(t => t.OpenedAt))
                Trades.Add(t);

            SelectedTrade = Trades.FirstOrDefault();
            TradesView.Refresh();
            RecalculateStats();
            StatusText = $"Loaded {Trades.Count} trades";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    // ══════════════════════ Dashboard ══════════════════════

    private Task RefreshDashboardAsync()
    {
        RecalculateStats();
        return Task.CompletedTask;
    }

    private void RecalculateStats()
    {
        var visible = TradesView.Cast<Trade>().ToList();
        Stats = _svc.CalculateStatistics(visible);
        OnPropertyChanged(nameof(AvgHoldTimeText));
        OnPropertyChanged(nameof(LongestHoldText));
        OnPropertyChanged(nameof(ShortestHoldText));
    }

    // ══════════════════════ Filters ══════════════════════

    private Task ApplyFilterAsync()
    {
        TradesView.Refresh();
        RecalculateStats();
        StatusText = "Filter applied";
        return Task.CompletedTask;
    }

    private void ClearFilters()
    {
        SymbolFilter = "";
        SideFilter = "All";
        StatusFilter = "All";
        FromDate = null;
        ToDate = null;
        TradesView.Refresh();
        RecalculateStats();
        StatusText = "Filters cleared";
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

        if (StatusFilter == "Open" && t.Status != TradeStatus.Open) return false;
        if (StatusFilter == "Closed" && t.Status != TradeStatus.Closed) return false;

        if (FromDate.HasValue && t.OpenedAt.UtcDateTime.Date < FromDate.Value.Date) return false;
        if (ToDate.HasValue && t.OpenedAt.UtcDateTime.Date > ToDate.Value.Date) return false;

        return true;
    }

    // ══════════════════════ CRUD Commands ══════════════════════

    private async Task DeleteSelectedAsync()
    {
        if (SelectedTrade == null) return;
        try
        {
            StatusText = "Deleting trade…";
            await _svc.DeleteAsync(SelectedTrade.Id);
            await LoadAsync();
            StatusText = "Trade deleted ✔";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    private async Task EditSelectedAsync()
    {
        if (SelectedTrade == null) return;
        await TradeForm.LoadForEdit(SelectedTrade);
        CurrentPage = "AddTrade";
        StatusText = $"Editing {SelectedTrade.Symbol} trade";
    }

    // ══════════════════════ Export ══════════════════════

    private async Task ExportCsvAsync()
    {
        try
        {
            StatusText = "Exporting CSV…";

            var exportDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ZyphraTrades", "Exports");
            Directory.CreateDirectory(exportDir);

            var file = Path.Combine(exportDir, $"trades_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            var visible = TradesView.Cast<Trade>().ToList();

            using var sw = new StreamWriter(file);
            sw.WriteLine("OpenedAtUTC,ClosedAtUTC,Symbol,Side,Status,Timeframe,Entry,Exit,SL,TP,Size,NetPnl,ResultR,Setup,Strategy,Session,Emotion,Confluence,Rating,Notes");

            foreach (var t in visible)
            {
                var line = string.Join(",",
                    Esc(t.OpenedAt.UtcDateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)),
                    Esc(t.ClosedAt?.UtcDateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? ""),
                    Esc(t.Symbol),
                    Esc(t.Side.ToString()),
                    Esc(t.Status.ToString()),
                    Esc(t.Timeframe ?? ""),
                    t.EntryPrice.ToString(CultureInfo.InvariantCulture),
                    (t.ExitPrice ?? 0m).ToString(CultureInfo.InvariantCulture),
                    (t.StopLoss ?? 0m).ToString(CultureInfo.InvariantCulture),
                    (t.TakeProfit ?? 0m).ToString(CultureInfo.InvariantCulture),
                    (t.PositionSize ?? 0m).ToString(CultureInfo.InvariantCulture),
                    t.NetPnl.ToString(CultureInfo.InvariantCulture),
                    (t.ResultR ?? 0m).ToString(CultureInfo.InvariantCulture),
                    Esc(t.Setup ?? ""),
                    Esc(t.Strategy ?? ""),
                    Esc(t.Session?.ToString() ?? ""),
                    Esc(t.EmotionBefore?.ToString() ?? ""),
                    Esc(t.Confluence?.ToString() ?? ""),
                    (t.Rating ?? 0).ToString(),
                    Esc(t.Notes ?? ""));
                sw.WriteLine(line);
            }

            StatusText = $"CSV exported: {file}";
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
    }

    private static string Esc(string s)
    {
        if (s.Contains('"') || s.Contains(',') || s.Contains('\n'))
            return $"\"{s.Replace("\"", "\"\"")}\"";
        return s;
    }
}
