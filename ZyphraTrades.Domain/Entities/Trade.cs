using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ZyphraTrades.Domain.Entities;

public class Trade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OpenTime { get; set; } = DateTime.UtcNow;
    public string Symbol { get; set; } = "AUDUSD";
    public decimal Entry { get; set; }
    public decimal? Exit { get; set; }
    public decimal PnL { get; set; }
    public string? Notes { get; set; }
}
