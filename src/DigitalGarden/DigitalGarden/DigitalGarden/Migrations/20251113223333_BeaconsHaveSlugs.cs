using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalGarden.Migrations
{
    /// <inheritdoc />
    public partial class BeaconsHaveSlugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "beacons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_beacons_Slug",
                table: "beacons",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_beacons_Slug",
                table: "beacons");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "beacons");
        }
    }
}
