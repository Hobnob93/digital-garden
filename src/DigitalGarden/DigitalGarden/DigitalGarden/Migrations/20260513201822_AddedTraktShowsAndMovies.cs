using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalGarden.Migrations
{
    /// <inheritdoc />
    public partial class AddedTraktShowsAndMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trakt_movies",
                columns: table => new
                {
                    TraktId = table.Column<int>(type: "integer", nullable: false),
                    ImdbId = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    TmdbId = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ReleaseYear = table.Column<int>(type: "integer", nullable: false),
                    LastWatchedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlayCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trakt_movies", x => x.TraktId);
                });

            migrationBuilder.CreateTable(
                name: "trakt_shows",
                columns: table => new
                {
                    TraktId = table.Column<int>(type: "integer", nullable: false),
                    TvdbId = table.Column<int>(type: "integer", nullable: false),
                    ImdbId = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    TmdbId = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ReleaseYear = table.Column<int>(type: "integer", nullable: false),
                    LastWatchedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlayCount = table.Column<int>(type: "integer", nullable: false),
                    SeasonsWatched = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trakt_shows", x => x.TraktId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trakt_movies");

            migrationBuilder.DropTable(
                name: "trakt_shows");
        }
    }
}
