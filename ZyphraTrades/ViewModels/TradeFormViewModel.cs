using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using ZyphraTrades.Application.DTOs;
using ZyphraTrades.Application.Services;
using ZyphraTrades.Domain.Entities;
using ZyphraTrades.Domain.Trading;
using ZyphraTrades.Presentation.Commands;
using ZyphraTrades.Presentation.Models;

namespace ZyphraTrades.Presentation.ViewModels;

public sealed class TradeFormViewModel : ViewModelBase
{
    private readonly ITradeService _svc;
    private readonly ISettingsService _settings;

    /// <summary>Invoked after a successful save or cancel.</summary>
    public Action? OnCompleted { get; set; }

    /// <summary>Invoked when an error occurs.</summary>
    public Action<string>? OnError { get; set; }

    // ══════════════════════ State ══════════════════════

    private bool _isEditing;
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

    private Guid? _editingTradeId;

    private string _formTitle = "New Trade";
    public string FormTitle { get => _formTitle; set => SetProperty(ref _formTitle, value); }

    // ══════════════════════ Basic Info ══════════════════════

    private string _symbol = "";
    public string Symbol { get => _symbol; set => SetProperty(ref _symbol, value); }

    private TradeSide _selectedSide = TradeSide.Buy;
    public TradeSide SelectedSide
    {
        get => _selectedSide;
        set { if (SetProperty(ref _selectedSide, value)) RecalculatePnL(); }
    }

    private string? _selectedTimeframe;
    public string? SelectedTimeframe { get => _selectedTimeframe; set => SetProperty(ref _selectedTimeframe, value); }

    private TradeStatus _selectedStatus = TradeStatus.Closed;
    public TradeStatus SelectedStatus { get => _selectedStatus; set => SetProperty(ref _selectedStatus, value); }

    private SessionType? _selectedSession;
    public SessionType? SelectedSession { get => _selectedSession; set => SetProperty(ref _selectedSession, value); }

    // ══════════════════════ Timing ══════════════════════

    private DateTime _openDate = DateTime.UtcNow.Date;
    public DateTime OpenDate { get => _openDate; set => SetProperty(ref _openDate, value); }

    private DateTime? _closeDate = DateTime.UtcNow.Date;
    public DateTime? CloseDate { get => _closeDate; set => SetProperty(ref _closeDate, value); }

    // ══════════════════════ Prices ══════════════════════

    private decimal _entryPrice;
    public decimal EntryPrice
    {
        get => _entryPrice;
        set { if (SetProperty(ref _entryPrice, value)) { RecalculateRR(); RecalculatePnL(); } }
    }

    private decimal? _exitPrice;
    public decimal? ExitPrice
    {
        get => _exitPrice;
        set { if (SetProperty(ref _exitPrice, value)) RecalculatePnL(); }
    }

    private decimal? _stopLoss;
    public decimal? StopLoss
    {
        get => _stopLoss;
        set { if (SetProperty(ref _stopLoss, value)) { RecalculateRR(); RecalculatePnL(); } }
    }

    private decimal? _takeProfit;
    public decimal? TakeProfit
    {
        get => _takeProfit;
        set { if (SetProperty(ref _takeProfit, value)) RecalculateRR(); }
    }

    private decimal? _riskRewardDisplay;
    public decimal? RiskRewardDisplay { get => _riskRewardDisplay; private set => SetProperty(ref _riskRewardDisplay, value); }

    // ══════════════════════ PnL (auto-calculado) ══════════════════════

    private decimal _grossPnl;
    public decimal GrossPnl { get => _grossPnl; private set => SetProperty(ref _grossPnl, value); }

    private decimal _netPnl;
    public decimal NetPnl { get => _netPnl; private set => SetProperty(ref _netPnl, value); }

    // ══════════════════════ Risk Management ══════════════════════

    /// <summary>true = risk is entered as % of account, false = risk as $ amount.</summary>
    private bool _riskAsPercent;
    public bool RiskAsPercent
    {
        get => _riskAsPercent;
        set
        {
            if (SetProperty(ref _riskAsPercent, value))
            {
                OnPropertyChanged(nameof(RiskAsPercent));
                RecalculateRiskFromMode();
            }
        }
    }

    private decimal? _riskPercent;
    public decimal? RiskPercent
    {
        get => _riskPercent;
        set { if (SetProperty(ref _riskPercent, value)) RecalculateRiskFromPercent(); }
    }

    private decimal? _riskAmount;
    public decimal? RiskAmount
    {
        get => _riskAmount;
        set { if (SetProperty(ref _riskAmount, value)) RecalculatePnL(); }
    }

    private decimal? _riskR;
    public decimal? RiskR { get => _riskR; private set => SetProperty(ref _riskR, value); }

    private decimal? _resultR;
    public decimal? ResultR { get => _resultR; private set => SetProperty(ref _resultR, value); }

    // ══════════════════════ Account (auto-calculado) ══════════════════════

    private decimal? _accountBalanceBefore;
    public decimal? AccountBalanceBefore { get => _accountBalanceBefore; set { if (SetProperty(ref _accountBalanceBefore, value)) { RecalculateBalance(); RecalculateRiskFromPercent(); } } }

    private decimal? _accountBalanceAfter;
    public decimal? AccountBalanceAfter { get => _accountBalanceAfter; private set => SetProperty(ref _accountBalanceAfter, value); }

    // ══════════════════════ Psychology (PNL-focused) ══════════════════════

    private EmotionalState? _emotionBefore;
    public EmotionalState? EmotionBefore { get => _emotionBefore; set => SetProperty(ref _emotionBefore, value); }

    private EmotionalState? _emotionAfter;
    public EmotionalState? EmotionAfter { get => _emotionAfter; set => SetProperty(ref _emotionAfter, value); }

    private int? _confidenceLevel;
    public int? ConfidenceLevel { get => _confidenceLevel; set => SetProperty(ref _confidenceLevel, value); }

    private string? _postTradeObservation;
    public string? PostTradeObservation { get => _postTradeObservation; set => SetProperty(ref _postTradeObservation, value); }

    private string? _improvementCommitment;
    public string? ImprovementCommitment { get => _improvementCommitment; set => SetProperty(ref _improvementCommitment, value); }

    // ══════════════════════ Context ══════════════════════

    private string? _setup;
    public string? Setup { get => _setup; set => SetProperty(ref _setup, value); }

    private string? _strategy;
    public string? Strategy { get => _strategy; set => SetProperty(ref _strategy, value); }

    private MarketCondition? _marketCondition;
    public MarketCondition? MarketCondition { get => _marketCondition; set => SetProperty(ref _marketCondition, value); }

    private ConfluenceLevel? _confluence;
    public ConfluenceLevel? Confluence { get => _confluence; set => SetProperty(ref _confluence, value); }

    // ══════════════════════ Journal ══════════════════════

    private string? _notes;
    public string? Notes { get => _notes; set => SetProperty(ref _notes, value); }

    private string? _mistakeNotes;
    public string? MistakeNotes { get => _mistakeNotes; set => SetProperty(ref _mistakeNotes, value); }

    private string? _lessonLearned;
    public string? LessonLearned { get => _lessonLearned; set => SetProperty(ref _lessonLearned, value); }

    private int? _rating;
    public int? Rating { get => _rating; set => SetProperty(ref _rating, value); }

    private string? _screenshotPath;
    public string? ScreenshotPath { get => _screenshotPath; set => SetProperty(ref _screenshotPath, value); }

    private string? _tags;
    public string? Tags { get => _tags; set => SetProperty(ref _tags, value); }

    // ══════════════════════ Dynamic Collections ══════════════════════

    /// <summary>Per-timeframe screenshot + analysis (generated from user config).</summary>
    public ObservableCollection<TimeframeAnalysisItem> TimeframeAnalyses { get; } = new();

    /// <summary>Dynamic checklist from user-defined trading plan rules.</summary>
    public ObservableCollection<ChecklistItem> ChecklistItems { get; } = new();

    /// <summary>Partial take-profits for this trade.</summary>
    public ObservableCollection<PartialTakeProfitItem> Partials { get; } = new();

    // ── Secciones colapsables ──
    private bool _showPsychology;
    public bool ShowPsychology { get => _showPsychology; set => SetProperty(ref _showPsychology, value); }

    private bool _showJournal;
    public bool ShowJournal { get => _showJournal; set => SetProperty(ref _showJournal, value); }

    private bool _showPartials;
    public bool ShowPartials { get => _showPartials; set => SetProperty(ref _showPartials, value); }

    public ICommand TogglePsychologyCommand { get; private set; } = null!;
    public ICommand ToggleJournalCommand { get; private set; } = null!;
    public ICommand TogglePartialsCommand { get; private set; } = null!;

    // ── Checklist summary ──
    public int CheckedCount => ChecklistItems.Count(i => i.IsChecked);
    public int ChecklistTotal => ChecklistItems.Count;
    public string ChecklistSummary => ChecklistTotal > 0
        ? $"{CheckedCount}/{ChecklistTotal} ({(ChecklistTotal > 0 ? Math.Round((double)CheckedCount / ChecklistTotal * 100, 0) : 0)}%)"
        : "No rules configured";

    // ══════════════════════ Dropdown Sources ══════════════════════

    public IReadOnlyList<TradeSide> SideOptions { get; } = Enum.GetValues<TradeSide>();
    public IReadOnlyList<TradeStatus> StatusOptions { get; } = Enum.GetValues<TradeStatus>();
    public IReadOnlyList<EmotionalState> EmotionOptions { get; } = Enum.GetValues<EmotionalState>();
    public IReadOnlyList<MarketCondition> MarketConditionOptions { get; } = Enum.GetValues<MarketCondition>();
    public IReadOnlyList<SessionType> SessionOptions { get; } = Enum.GetValues<SessionType>();
    public IReadOnlyList<ConfluenceLevel> ConfluenceOptions { get; } = Enum.GetValues<ConfluenceLevel>();

    /// <summary>Dynamic — rebuilt from user settings.</summary>
    private ObservableCollection<string> _timeframeOptions = new(new[] { "M1", "M5", "M15", "M30", "H1", "H4", "D1", "W1", "MN" });
    public ObservableCollection<string> TimeframeOptions { get => _timeframeOptions; set => SetProperty(ref _timeframeOptions, value); }

    public IReadOnlyList<int> RatingOptions { get; } = new[] { 1, 2, 3, 4, 5 };
    public IReadOnlyList<int> ConfidenceLevelOptions { get; } = Enumerable.Range(1, 5).ToArray();

    // ══════════════════════ Commands ══════════════════════

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand AddPartialCommand { get; }
    public ICommand RemovePartialCommand { get; }
    public ICommand BrowseScreenshotCommand { get; }

    // ══════════════════════ Constructor ══════════════════════

    public TradeFormViewModel(ITradeService tradeService, ISettingsService settingsService)
    {
        _svc = tradeService;
        _settings = settingsService;

        SaveCommand = new AsyncCommand(SaveAsync);
        CancelCommand = new RelayCommand(_ => OnCompleted?.Invoke());
        ResetCommand = new RelayCommand(_ => Reset());
        AddPartialCommand = new RelayCommand(_ => AddPartial());
        RemovePartialCommand = new RelayCommand(RemovePartial);
        BrowseScreenshotCommand = new RelayCommand(BrowseScreenshot);

        // Toggles para secciones colapsables
        TogglePsychologyCommand = new RelayCommand(_ => ShowPsychology = !ShowPsychology);
        ToggleJournalCommand = new RelayCommand(_ => ShowJournal = !ShowJournal);
        TogglePartialsCommand = new RelayCommand(_ => ShowPartials = !ShowPartials);
    }

    // ══════════════════════ Dynamic Loading ══════════════════════

    /// <summary>
    /// Loads dynamic form sections from user configuration.
    /// Must be called after construction and whenever settings change.
    /// </summary>
    public async Task LoadDynamicSectionsAsync()
    {
        // Load timeframes from settings
        var timeframes = await _settings.GetTimeframesAsync();

        TimeframeOptions.Clear();
        foreach (var tf in timeframes)
            TimeframeOptions.Add(tf);

        // Generate TF analysis rows
        TimeframeAnalyses.Clear();
        for (int i = 0; i < timeframes.Count; i++)
            TimeframeAnalyses.Add(new TimeframeAnalysisItem(timeframes[i], i));

        // Load checklist rules
        var rules = await _settings.GetActiveRulesAsync();
        ChecklistItems.Clear();
        foreach (var rule in rules)
        {
            var item = new ChecklistItem(rule.Id, rule.Name, rule.Category, rule.Description);
            item.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ChecklistItem.IsChecked))
                {
                    OnPropertyChanged(nameof(CheckedCount));
                    OnPropertyChanged(nameof(ChecklistSummary));
                }
            };
            ChecklistItems.Add(item);
        }

        OnPropertyChanged(nameof(CheckedCount));
        OnPropertyChanged(nameof(ChecklistTotal));
        OnPropertyChanged(nameof(ChecklistSummary));

        // Load default account balance
        var settings = await _settings.GetSettingsAsync();
        if (AccountBalanceBefore is null && settings.DefaultAccountBalance.HasValue)
            AccountBalanceBefore = settings.DefaultAccountBalance;

        // Aplicar parámetros dinámicos desde ajustes (si el usuario no los ha modificado)
        if (string.IsNullOrWhiteSpace(Setup) && !string.IsNullOrWhiteSpace(settings.DefaultSetup))
            Setup = settings.DefaultSetup;
        if (string.IsNullOrWhiteSpace(Strategy) && !string.IsNullOrWhiteSpace(settings.DefaultStrategy))
            Strategy = settings.DefaultStrategy;
    }

    // ══════════════════════ Public API ══════════════════════

    public async Task LoadForEdit(Trade trade)
    {
        IsEditing = true;
        _editingTradeId = trade.Id;
        FormTitle = $"Edit Trade · {trade.Symbol}";

        Symbol = trade.Symbol;
        SelectedSide = trade.Side;
        SelectedTimeframe = trade.Timeframe;
        SelectedStatus = trade.Status;
        SelectedSession = trade.Session;

        OpenDate = trade.OpenedAt.UtcDateTime.Date;
        CloseDate = trade.ClosedAt?.UtcDateTime.Date;

        EntryPrice = trade.EntryPrice;
        ExitPrice = trade.ExitPrice;
        StopLoss = trade.StopLoss;
        TakeProfit = trade.TakeProfit;

        RiskAmount = trade.RiskAmount;
        AccountBalanceBefore = trade.AccountBalanceBefore;

        // PnL, ResultR y BalanceAfter se auto-calculan desde precios + riesgo

        EmotionBefore = trade.EmotionBefore;
        EmotionAfter = trade.EmotionAfter;
        ConfidenceLevel = trade.ConfidenceLevel;
        PostTradeObservation = trade.PostTradeObservation;
        ImprovementCommitment = trade.ImprovementCommitment;

        Setup = trade.Setup;
        Strategy = trade.Strategy;
        MarketCondition = trade.MarketCondition;
        Confluence = trade.Confluence;

        Notes = trade.Notes;
        MistakeNotes = trade.MistakeNotes;
        LessonLearned = trade.LessonLearned;
        Rating = trade.Rating;
        ScreenshotPath = trade.ScreenshotPath;
        Tags = trade.Tags;

        // Load dynamic sections first
        await LoadDynamicSectionsAsync();

        // Populate existing TF analyses
        foreach (var analysis in trade.TimeframeAnalyses.OrderBy(a => a.SortOrder))
        {
            var match = TimeframeAnalyses.FirstOrDefault(ta => ta.Timeframe == analysis.Timeframe);
            if (match is not null)
            {
                match.ScreenshotPath = analysis.ScreenshotPath;
                match.Analysis = analysis.Analysis;
            }
        }

        // Populate existing checklist entries
        foreach (var entry in trade.ChecklistEntries)
        {
            var match = ChecklistItems.FirstOrDefault(ci => ci.RuleId == entry.ChecklistRuleId);
            if (match is not null)
            {
                match.IsChecked = entry.IsChecked;
                match.Notes = entry.Notes;
            }
        }

        // Populate existing partials
        Partials.Clear();
        int idx = 1;
        foreach (var p in trade.Partials.OrderBy(p => p.ClosedAt))
        {
            Partials.Add(new PartialTakeProfitItem
            {
                Index = idx++,
                ExitPrice = p.ExitPrice,
                PercentClosed = p.PercentClosed,
                Notes = p.Notes
            });
        }
    }

    public void Reset()
    {
        IsEditing = false;
        _editingTradeId = null;
        FormTitle = "New Trade";

        Symbol = "";
        SelectedSide = TradeSide.Buy;
        SelectedTimeframe = null;
        SelectedStatus = TradeStatus.Closed;
        SelectedSession = null;

        OpenDate = DateTime.UtcNow.Date;
        CloseDate = DateTime.UtcNow.Date;

        EntryPrice = 0;
        ExitPrice = null;
        StopLoss = null;
        TakeProfit = null;
        RiskRewardDisplay = null;

        _grossPnl = 0;
        _netPnl = 0;
        _resultR = null;
        _riskR = null;
        _accountBalanceAfter = null;
        OnPropertyChanged(nameof(GrossPnl));
        OnPropertyChanged(nameof(NetPnl));
        OnPropertyChanged(nameof(ResultR));
        OnPropertyChanged(nameof(RiskR));
        OnPropertyChanged(nameof(AccountBalanceAfter));

        RiskAmount = null;
        _riskPercent = null;
        OnPropertyChanged(nameof(RiskPercent));
        AccountBalanceBefore = null;

        EmotionBefore = null;
        EmotionAfter = null;
        ConfidenceLevel = null;
        PostTradeObservation = null;
        ImprovementCommitment = null;

        Setup = null;
        Strategy = null;
        MarketCondition = null;
        Confluence = null;

        Notes = null;
        MistakeNotes = null;
        LessonLearned = null;
        Rating = null;
        ScreenshotPath = null;
        Tags = null;

        Partials.Clear();

        // Reset checklist checks
        foreach (var item in ChecklistItems)
        {
            item.IsChecked = false;
            item.Notes = null;
        }

        // Reset TF analyses
        foreach (var ta in TimeframeAnalyses)
        {
            ta.ScreenshotPath = null;
            ta.Analysis = null;
        }
    }

    // ══════════════════════ Partial TP Operations ══════════════════════

    private void AddPartial()
    {
        Partials.Add(new PartialTakeProfitItem
        {
            Index = Partials.Count + 1
        });
    }

    private void RemovePartial(object? param)
    {
        if (param is PartialTakeProfitItem item)
        {
            Partials.Remove(item);
            for (int i = 0; i < Partials.Count; i++)
                Partials[i].Index = i + 1;
        }
    }

    // ══════════════════════ Screenshot Browse ══════════════════════

    private void BrowseScreenshot(object? param)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*",
            Title = "Select Screenshot"
        };

        if (dialog.ShowDialog() == true)
        {
            if (param is TimeframeAnalysisItem tfItem)
                tfItem.ScreenshotPath = dialog.FileName;
            else
                ScreenshotPath = dialog.FileName;
        }
    }

    // ══════════════════════ Private ══════════════════════

    private async Task SaveAsync()
    {
        try
        {
            var request = BuildRequest();

            if (IsEditing && _editingTradeId.HasValue)
                await _svc.UpdateAsync(_editingTradeId.Value, request);
            else
                await _svc.CreateAsync(request);

            Reset();
            OnCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex.Message);
        }
    }

    private CreateTradeRequest BuildRequest()
    {
        var req = new CreateTradeRequest
        {
            Symbol = Symbol,
            Side = SelectedSide,
            Timeframe = SelectedTimeframe,
            Status = SelectedStatus,
            Session = SelectedSession,

            OpenedAt = new DateTimeOffset(OpenDate, TimeSpan.Zero),
            ClosedAt = CloseDate.HasValue ? new DateTimeOffset(CloseDate.Value, TimeSpan.Zero) : null,

            EntryPrice = EntryPrice,
            ExitPrice = ExitPrice,
            StopLoss = StopLoss,
            TakeProfit = TakeProfit,

            GrossPnl = GrossPnl,
            NetPnl = NetPnl,

            RiskAmount = RiskAmount,
            RiskR = RiskRewardDisplay,
            ResultR = ResultR,

            AccountBalanceBefore = AccountBalanceBefore,
            AccountBalanceAfter = AccountBalanceAfter,

            EmotionBefore = EmotionBefore,
            EmotionAfter = EmotionAfter,
            ConfidenceLevel = ConfidenceLevel,
            PostTradeObservation = PostTradeObservation,
            ImprovementCommitment = ImprovementCommitment,

            Setup = Setup,
            Strategy = Strategy,
            MarketCondition = MarketCondition,
            Confluence = Confluence,

            Notes = Notes,
            MistakeNotes = MistakeNotes,
            LessonLearned = LessonLearned,
            Rating = Rating,
            ScreenshotPath = ScreenshotPath,
            Tags = Tags,

            ChecklistScore = ChecklistItems.Count(i => i.IsChecked),
            ChecklistTotal = ChecklistItems.Count
        };

        // Partials
        foreach (var p in Partials)
        {
            req.Partials.Add(new TradePartialDto
            {
                ExitPrice = p.ExitPrice,
                Quantity = 0,
                RealizedPnl = 0,
                PercentClosed = p.PercentClosed,
                ClosedAt = CloseDate.HasValue ? new DateTimeOffset(CloseDate.Value, TimeSpan.Zero) : DateTimeOffset.UtcNow,
                Notes = p.Notes
            });
        }

        // Timeframe analyses
        foreach (var ta in TimeframeAnalyses.Where(a => !string.IsNullOrWhiteSpace(a.Analysis) || !string.IsNullOrWhiteSpace(a.ScreenshotPath)))
        {
            req.TimeframeAnalyses.Add(new TradeTimeframeAnalysisDto
            {
                Timeframe = ta.Timeframe,
                ScreenshotPath = ta.ScreenshotPath,
                Analysis = ta.Analysis,
                SortOrder = ta.SortOrder
            });
        }

        // Checklist entries
        foreach (var ci in ChecklistItems)
        {
            req.ChecklistEntries.Add(new TradeChecklistEntryDto
            {
                ChecklistRuleId = ci.RuleId,
                RuleName = ci.RuleName,
                Category = ci.Category,
                IsChecked = ci.IsChecked,
                Notes = ci.Notes
            });
        }

        return req;
    }

    private void RecalculateRR()
    {
        if (!StopLoss.HasValue || !TakeProfit.HasValue || EntryPrice == 0)
        {
            RiskRewardDisplay = null;
            return;
        }

        var risk = Math.Abs(EntryPrice - StopLoss.Value);
        var reward = Math.Abs(TakeProfit.Value - EntryPrice);

        RiskRewardDisplay = risk > 0 ? Math.Round(reward / risk, 2) : null;
    }

    /// <summary>
    /// Auto-calcula PnL bruto/neto y ResultR a partir de precios y riesgo.
    /// Fórmula: ResultR = movimiento / distancia_riesgo, PnL = ResultR × RiskAmount.
    /// </summary>
    private void RecalculatePnL()
    {
        if (!ExitPrice.HasValue || !StopLoss.HasValue || EntryPrice == 0)
        {
            GrossPnl = 0;
            NetPnl = 0;
            ResultR = null;
            RecalculateBalance();
            return;
        }

        var riskDistance = Math.Abs(EntryPrice - StopLoss.Value);
        if (riskDistance == 0)
        {
            GrossPnl = 0;
            NetPnl = 0;
            ResultR = null;
            RecalculateBalance();
            return;
        }

        // Movimiento según dirección del trade
        var moveDistance = SelectedSide == TradeSide.Buy
            ? ExitPrice.Value - EntryPrice
            : EntryPrice - ExitPrice.Value;

        // Resultado en múltiplos de R
        var resultR = Math.Round(moveDistance / riskDistance, 4);
        ResultR = resultR;

        // Si el usuario proporcionó el monto de riesgo en $, calculamos PnL en $
        if (RiskAmount.HasValue && RiskAmount.Value > 0)
        {
            GrossPnl = Math.Round(resultR * RiskAmount.Value, 2);
            NetPnl = GrossPnl; // Sin costos para traders comunes
        }
        else
        {
            GrossPnl = 0;
            NetPnl = 0;
        }

        RecalculateBalance();
    }

    /// <summary>
    /// Auto-calcula el balance posterior: BalanceAfter = BalanceBefore + NetPnl.
    /// </summary>
    private void RecalculateBalance()
    {
        if (AccountBalanceBefore.HasValue && NetPnl != 0)
            AccountBalanceAfter = Math.Round(AccountBalanceBefore.Value + NetPnl, 2);
        else
            AccountBalanceAfter = null;
    }

    /// <summary>
    /// Cuando el usuario cambia el % de riesgo, calcula el monto en $.
    /// </summary>
    private void RecalculateRiskFromPercent()
    {
        if (!RiskAsPercent) return;
        if (RiskPercent.HasValue && AccountBalanceBefore.HasValue && AccountBalanceBefore.Value > 0)
            RiskAmount = Math.Round(AccountBalanceBefore.Value * RiskPercent.Value / 100m, 2);
    }

    /// <summary>
    /// Cuando el usuario cambia el modo de riesgo, recalcula según corresponda.
    /// </summary>
    private void RecalculateRiskFromMode()
    {
        if (RiskAsPercent)
            RecalculateRiskFromPercent();
    }
}
