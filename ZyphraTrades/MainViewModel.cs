using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
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

    private string _statusText = "Listo";
    public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

    // ══════════════════════ Chart Data ══════════════════════

    private ISeries[] _equitySeries = Array.Empty<ISeries>();
    public ISeries[] EquitySeries { get => _equitySeries; set => SetProperty(ref _equitySeries, value); }

    private Axis[] _equityXAxes = Array.Empty<Axis>();
    public Axis[] EquityXAxes { get => _equityXAxes; set => SetProperty(ref _equityXAxes, value); }

    private ISeries[] _pnlBarSeries = Array.Empty<ISeries>();
    public ISeries[] PnlBarSeries { get => _pnlBarSeries; set => SetProperty(ref _pnlBarSeries, value); }

    private ISeries[] _monthlyPnlSeries = Array.Empty<ISeries>();
    public ISeries[] MonthlyPnlSeries { get => _monthlyPnlSeries; set => SetProperty(ref _monthlyPnlSeries, value); }

    private Axis[] _monthlyXAxes = Array.Empty<Axis>();
    public Axis[] MonthlyXAxes { get => _monthlyXAxes; set => SetProperty(ref _monthlyXAxes, value); }

    private ISeries[] _winLossSeries = Array.Empty<ISeries>();
    public ISeries[] WinLossSeries { get => _winLossSeries; set => SetProperty(ref _winLossSeries, value); }

    private ISeries[] _rDistributionSeries = Array.Empty<ISeries>();
    public ISeries[] RDistributionSeries { get => _rDistributionSeries; set => SetProperty(ref _rDistributionSeries, value); }

    private Axis[] _rDistXAxes = Array.Empty<Axis>();
    public Axis[] RDistXAxes { get => _rDistXAxes; set => SetProperty(ref _rDistXAxes, value); }

    private ISeries[] _dayOfWeekSeries = Array.Empty<ISeries>();
    public ISeries[] DayOfWeekSeries { get => _dayOfWeekSeries; set => SetProperty(ref _dayOfWeekSeries, value); }

    private Axis[] _dayOfWeekXAxes = Array.Empty<Axis>();
    public Axis[] DayOfWeekXAxes { get => _dayOfWeekXAxes; set => SetProperty(ref _dayOfWeekXAxes, value); }

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
                StatusText = "Trade guardado ✔";
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
            StatusText = "Cargando trades…";
            Trades.Clear();

            var items = await _svc.GetAllAsync();
            foreach (var t in items.OrderByDescending(t => t.OpenedAt))
                Trades.Add(t);

            SelectedTrade = Trades.FirstOrDefault();
            TradesView.Refresh();
            RecalculateStats();
            StatusText = $"{Trades.Count} trades cargados";
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
        BuildChartData(visible);
    }

    // ══════════════════════ Chart Builders ══════════════════════

    private static readonly SKColor WinColor = new(34, 197, 94);    // verde
    private static readonly SKColor LossColor = new(239, 68, 68);   // rojo
    private static readonly SKColor BeColor = new(234, 179, 8);     // amarillo
    private static readonly SKColor AccentColor = new(155, 58, 74); // wine (#9B3A4A)
    private static readonly SKColor WineLight = new(212, 85, 107);  // wine-light (#D4556B)

    private void BuildChartData(List<Trade> trades)
    {
        var closed = trades.Where(t => t.Status == TradeStatus.Closed)
                           .OrderBy(t => t.ClosedAt ?? t.OpenedAt)
                           .ToList();

        // ── 1. Curva de Equidad (línea) ──
        var equityPoints = new List<ObservablePoint>();
        decimal cumPnl = 0;
        for (int i = 0; i < closed.Count; i++)
        {
            cumPnl += closed[i].NetPnl;
            equityPoints.Add(new ObservablePoint(i + 1, (double)cumPnl));
        }
        EquitySeries = new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = equityPoints,
                Fill = new SolidColorPaint(WineLight.WithAlpha(30)),
                Stroke = new SolidColorPaint(WineLight, 2),
                GeometrySize = 0,
                LineSmoothness = 0.3,
                Name = "Equidad ($)"
            }
        };

        // ── 2. PnL por Trade (barras) ──
        var pnlBars = closed.Select(t =>
            new ObservableValue((double)t.NetPnl)).ToList();
        PnlBarSeries = new ISeries[]
        {
            new ColumnSeries<ObservableValue>
            {
                Values = pnlBars,
                Name = "PnL por Trade",
                Mapping = (v, i) => new LiveChartsCore.Kernel.Coordinate(i, v.Value ?? 0),
                Fill = null, // will use conditional below
            }
        };
        // Color dinámico: verde si > 0, rojo si < 0
        var pnlBarColors = closed.Select(t => t.NetPnl >= 0
            ? new SolidColorPaint(WinColor)
            : new SolidColorPaint(LossColor)).ToArray();
        PnlBarSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = closed.Select(t => (double)t.NetPnl).ToArray(),
                Name = "PnL",
                Fill = new SolidColorPaint(AccentColor),
            }
        };

        // ── 3. Rendimiento Mensual (barras) ──
        var monthGroups = closed
            .GroupBy(t => (t.ClosedAt ?? t.OpenedAt).ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .ToList();
        var monthLabels = monthGroups.Select(g => g.Key).ToArray();
        var monthValues = monthGroups.Select(g => (double)g.Sum(t => t.NetPnl)).ToArray();
        MonthlyPnlSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = monthValues,
                Name = "PnL Mensual",
                Fill = new SolidColorPaint(AccentColor),
            }
        };
        MonthlyXAxes = new Axis[]
        {
            new Axis
            {
                Labels = monthLabels,
                LabelsRotation = 45,
                TextSize = 10,
                LabelsPaint = new SolidColorPaint(new SKColor(160, 160, 160))
            }
        };

        // ── 4. Win / Loss / BE (pie) ──
        var wins = closed.Count(t => t.NetPnl > 0);
        var losses = closed.Count(t => t.NetPnl < 0);
        var bes = closed.Count(t => t.NetPnl == 0);
        WinLossSeries = new ISeries[]
        {
            new PieSeries<int> { Values = new[] { wins }, Name = "Ganadas", Fill = new SolidColorPaint(WinColor) },
            new PieSeries<int> { Values = new[] { losses }, Name = "Perdidas", Fill = new SolidColorPaint(LossColor) },
            new PieSeries<int> { Values = new[] { bes }, Name = "Break-Even", Fill = new SolidColorPaint(BeColor) }
        };

        // ── 5. Distribución de R (histograma) ──
        var rValues = closed.Where(t => t.ResultR.HasValue).Select(t => t.ResultR!.Value).ToList();
        if (rValues.Count > 0)
        {
            var minR = Math.Floor((double)rValues.Min());
            var maxR = Math.Ceiling((double)rValues.Max());
            var buckets = new Dictionary<string, int>();
            for (double b = minR; b < maxR; b += 0.5)
            {
                var label = $"{b:0.0}";
                var count = rValues.Count(r => (double)r >= b && (double)r < b + 0.5);
                buckets[label] = count;
            }
            RDistributionSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = buckets.Values.ToArray(),
                    Name = "Frecuencia R",
                    Fill = new SolidColorPaint(AccentColor),
                }
            };
            RDistXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = buckets.Keys.ToArray(),
                    LabelsRotation = 45,
                    TextSize = 10,
                    LabelsPaint = new SolidColorPaint(new SKColor(160, 160, 160))
                }
            };
        }

        // ── 6. PnL por Día de la Semana ──
        var dayNames = new[] { "Lun", "Mar", "Mié", "Jue", "Vie" };
        var dayValues = new double[5];
        foreach (var t in closed)
        {
            var dow = (t.ClosedAt ?? t.OpenedAt).DayOfWeek;
            var idx = dow switch
            {
                DayOfWeek.Monday => 0,
                DayOfWeek.Tuesday => 1,
                DayOfWeek.Wednesday => 2,
                DayOfWeek.Thursday => 3,
                DayOfWeek.Friday => 4,
                _ => -1
            };
            if (idx >= 0) dayValues[idx] += (double)t.NetPnl;
        }
        DayOfWeekSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = dayValues,
                Name = "PnL por Día",
                Fill = new SolidColorPaint(AccentColor),
            }
        };
        DayOfWeekXAxes = new Axis[]
        {
            new Axis
            {
                Labels = dayNames,
                TextSize = 11,
                LabelsPaint = new SolidColorPaint(new SKColor(160, 160, 160))
            }
        };
    }

    // ══════════════════════ Filters ══════════════════════

    private Task ApplyFilterAsync()
    {
        TradesView.Refresh();
        RecalculateStats();
        StatusText = "Filtro aplicado";
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
        StatusText = "Filtros limpiados";
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
            StatusText = "Eliminando trade…";
            await _svc.DeleteAsync(SelectedTrade.Id);
            await LoadAsync();
            StatusText = "Trade eliminado ✔";
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
        StatusText = $"Editando trade de {SelectedTrade.Symbol}";
    }

    // ══════════════════════ Export ══════════════════════

    private async Task ExportCsvAsync()
    {
        try
        {
            StatusText = "Exportando CSV…";

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

            StatusText = $"CSV exportado: {file}";
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
