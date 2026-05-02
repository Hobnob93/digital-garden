using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DigitalGarden.Migrations
{
    /// <inheritdoc />
    public partial class AddSteamGamesIngestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FullGameFetchAtUtc",
                table: "daily_ingests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "played_games",
                columns: table => new
                {
                    AppId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LastPlayedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalPlayTimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    HaveAllAchievements = table.Column<bool>(type: "boolean", nullable: false),
                    LastFullUpdateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_played_games", x => x.AppId);
                });

            migrationBuilder.CreateTable(
                name: "played_games_achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UnlockedIcon = table.Column<string>(type: "text", nullable: false),
                    LockedIcon = table.Column<string>(type: "text", nullable: false),
                    GlobalUnlockPercent = table.Column<double>(type: "double precision", nullable: true),
                    GlobalPercentUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsUnlocked = table.Column<bool>(type: "boolean", nullable: false),
                    UnlockedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GameAppId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_played_games_achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_played_games_achievements_played_games_GameAppId",
                        column: x => x.GameAppId,
                        principalTable: "played_games",
                        principalColumn: "AppId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_played_games_achievements_AppId",
                table: "played_games_achievements",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_played_games_achievements_GameAppId",
                table: "played_games_achievements",
                column: "GameAppId");

            migrationBuilder.CreateIndex(
                name: "IX_played_games_achievements_Name",
                table: "played_games_achievements",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "played_games_achievements");

            migrationBuilder.DropTable(
                name: "played_games");

            migrationBuilder.DropColumn(
                name: "FullGameFetchAtUtc",
                table: "daily_ingests");
        }
    }
}
