using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZyphraTrades.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DynamicFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BreakevenAt",
                table: "Trades",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BreakevenPrice",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChecklistScore",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChecklistTotal",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImprovementCommitment",
                table: "Trades",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBreakevenActive",
                table: "Trades",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PostTradeObservation",
                table: "Trades",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChecklistRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradePartials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    RealizedPnl = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PercentClosed = table.Column<decimal>(type: "TEXT", precision: 8, scale: 2, nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MovedToBreakeven = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePartials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradePartials_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeTimeframeAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timeframe = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ScreenshotPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Analysis = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeTimeframeAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeTimeframeAnalyses_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimeframesJson = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DefaultRiskPercent = table.Column<decimal>(type: "TEXT", precision: 8, scale: 4, nullable: false),
                    DefaultAccountBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradeChecklistEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TradeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChecklistRuleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsChecked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeChecklistEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeChecklistEntries_ChecklistRules_ChecklistRuleId",
                        column: x => x.ChecklistRuleId,
                        principalTable: "ChecklistRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeChecklistEntries_Trades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "Trades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeChecklistEntries_ChecklistRuleId",
                table: "TradeChecklistEntries",
                column: "ChecklistRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeChecklistEntries_TradeId",
                table: "TradeChecklistEntries",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_TradePartials_TradeId",
                table: "TradePartials",
                column: "TradeId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeTimeframeAnalyses_TradeId",
                table: "TradeTimeframeAnalyses",
                column: "TradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeChecklistEntries");

            migrationBuilder.DropTable(
                name: "TradePartials");

            migrationBuilder.DropTable(
                name: "TradeTimeframeAnalyses");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "ChecklistRules");

            migrationBuilder.DropColumn(
                name: "BreakevenAt",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "BreakevenPrice",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ChecklistScore",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ChecklistTotal",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ImprovementCommitment",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "IsBreakevenActive",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "PostTradeObservation",
                table: "Trades");
        }
    }
}
