using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZyphraTrades.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThemeAndDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultContext",
                table: "UserSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultSetup",
                table: "UserSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultStrategy",
                table: "UserSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDarkMode",
                table: "UserSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultContext",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "DefaultSetup",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "DefaultStrategy",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "IsDarkMode",
                table: "UserSettings");
        }
    }
}
