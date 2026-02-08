using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZyphraTrades.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedTradeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AccountBalanceAfter",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AccountBalanceBefore",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Commission",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConfidenceLevel",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Confluence",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmotionAfter",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmotionBefore",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LessonLearned",
                table: "Trades",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MarketCondition",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MistakeNotes",
                table: "Trades",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Outcome",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RiskAmount",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RiskRewardRatio",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotPath",
                table: "Trades",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Session",
                table: "Trades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Slippage",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Trades",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Strategy",
                table: "Trades",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Swap",
                table: "Trades",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Trades",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalanceAfter",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "AccountBalanceBefore",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Commission",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ConfidenceLevel",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Confluence",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "EmotionAfter",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "EmotionBefore",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "LessonLearned",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "MarketCondition",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "MistakeNotes",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "RiskAmount",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "RiskRewardRatio",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ScreenshotPath",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Session",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Slippage",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Strategy",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Swap",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Trades");
        }
    }
}
