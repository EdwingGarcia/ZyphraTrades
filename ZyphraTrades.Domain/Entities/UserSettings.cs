namespace ZyphraTrades.Domain.Entities;

/// <summary>
/// Stores user configuration: custom timeframes, preferences, etc.
/// Single-row pattern — only one UserSettings row exists.
/// </summary>
public class UserSettings
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// JSON-serialized list of timeframes the user trades on.
    /// e.g. ["1m","5m","15m","1H","4H","D"]
    /// </summary>
    public string TimeframesJson { get; set; } = "[\"M1\",\"M5\",\"M15\",\"H1\",\"H4\",\"D1\"]";

    /// <summary>Default risk per trade (% of account).</summary>
    public decimal DefaultRiskPercent { get; set; } = 1.0m;

    /// <summary>Default account balance for new trades.</summary>
    public decimal? DefaultAccountBalance { get; set; }

    /// <summary>Preferred base currency (USD, EUR, etc.).</summary>
    public string BaseCurrency { get; set; } = "USD";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // ── Helpers ──

    public List<string> GetTimeframes()
        => System.Text.Json.JsonSerializer.Deserialize<List<string>>(TimeframesJson ?? "[]") ?? new();

    public void SetTimeframes(IEnumerable<string> timeframes)
    {
        TimeframesJson = System.Text.Json.JsonSerializer.Serialize(timeframes.ToList());
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
