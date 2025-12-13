using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZyphraTrades.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OpenedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Side = table.Column<int>(type: "INTEGER", nullable: false),
                    Timeframe = table.Column<string>(type: "TEXT", nullable: true),
                    EntryPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ExitPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    StopLoss = table.Column<decimal>(type: "TEXT", nullable: true),
                    TakeProfit = table.Column<decimal>(type: "TEXT", nullable: true),
                    PositionSize = table.Column<decimal>(type: "TEXT", nullable: true),
                    Fees = table.Column<decimal>(type: "TEXT", nullable: true),
                    GrossPnl = table.Column<decimal>(type: "TEXT", nullable: false),
                    NetPnl = table.Column<decimal>(type: "TEXT", nullable: false),
                    RiskR = table.Column<decimal>(type: "TEXT", nullable: true),
                    ResultR = table.Column<decimal>(type: "TEXT", nullable: true),
                    Setup = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
