using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class matchdetails_fixtureid_field_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataSource",
                table: "MatchDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FixtureId",
                table: "MatchDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "OriginalResponse",
                table: "MatchDetails",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MatchDetails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataSource",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "FixtureId",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "OriginalResponse",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MatchDetails");
        }
    }
}
