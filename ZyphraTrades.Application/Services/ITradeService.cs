using ZyphraTrades.Application.DTOs;
using ZyphraTrades.Domain.Entities;

namespace ZyphraTrades.Application.Services;

public interface ITradeService
{
    Task<IReadOnlyList<Trade>> GetAllAsync(CancellationToken ct = default);
    Task<Trade?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Trade> CreateAsync(CreateTradeRequest request, CancellationToken ct = default);
    Task<Trade> UpdateAsync(Guid id, CreateTradeRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    PortfolioStatistics CalculateStatistics(IReadOnlyList<Trade> trades);
    IReadOnlyList<EquityCurvePoint> CalculateEquityCurve(IReadOnlyList<Trade> trades);
}
