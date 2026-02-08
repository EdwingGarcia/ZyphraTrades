using ZyphraTrades.Presentation.ViewModels;

namespace ZyphraTrades.Presentation.Models;

/// <summary>
/// View-model for a single timeframe analysis row in the trade form.
/// Dynamically generated based on user's configured timeframes.
/// </summary>
public sealed class TimeframeAnalysisItem : ViewModelBase
{
    public string Timeframe { get; }
    public int SortOrder { get; }

    private string? _screenshotPath;
    public string? ScreenshotPath { get => _screenshotPath; set => SetProperty(ref _screenshotPath, value); }

    private string? _analysis;
    public string? Analysis { get => _analysis; set => SetProperty(ref _analysis, value); }

    public TimeframeAnalysisItem(string timeframe, int sortOrder)
    {
        Timeframe = timeframe;
        SortOrder = sortOrder;
    }
}
