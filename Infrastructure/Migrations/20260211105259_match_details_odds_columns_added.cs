using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class match_details_odds_columns_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalResponse",
                table: "MatchDetails",
                newName: "OriginalResponseOdds");

            migrationBuilder.AddColumn<int>(
                name: "BookmakerId",
                table: "MatchDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BookmakerName",
                table: "MatchDetails",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookmakerId",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "BookmakerName",
                table: "MatchDetails");

            migrationBuilder.RenameColumn(
                name: "OriginalResponseOdds",
                table: "MatchDetails",
                newName: "OriginalResponse");
        }
    }
}
