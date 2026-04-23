using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DigitalGarden.Migrations
{
    /// <inheritdoc />
    public partial class DailyMusicIngestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "music_snapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CapturedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Period = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_music_snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "top_artists_snapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PlayCount = table.Column<int>(type: "integer", nullable: false),
                    SnapshotId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_top_artists_snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_top_artists_snapshots_music_snapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "music_snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "top_tracks_snapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Rank = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ArtistName = table.Column<string>(type: "text", nullable: false),
                    PlayCount = table.Column<int>(type: "integer", nullable: false),
                    SnapshotId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_top_tracks_snapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_top_tracks_snapshots_music_snapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "music_snapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_music_snapshots_CapturedAtUtc",
                table: "music_snapshots",
                column: "CapturedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_top_artists_snapshots_Name",
                table: "top_artists_snapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_top_artists_snapshots_SnapshotId_Rank",
                table: "top_artists_snapshots",
                columns: new[] { "SnapshotId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_top_tracks_snapshots_ArtistName_Name",
                table: "top_tracks_snapshots",
                columns: new[] { "ArtistName", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_top_tracks_snapshots_SnapshotId_Rank",
                table: "top_tracks_snapshots",
                columns: new[] { "SnapshotId", "Rank" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "top_artists_snapshots");

            migrationBuilder.DropTable(
                name: "top_tracks_snapshots");

            migrationBuilder.DropTable(
                name: "music_snapshots");
        }
    }
}
