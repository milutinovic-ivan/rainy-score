using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatchDetailsShotsNumberRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayShots",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "AwayShotsOnTarget",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "HomeShots",
                table: "MatchDetails");

            migrationBuilder.DropColumn(
                name: "HomeShotsOnTarget",
                table: "MatchDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IsHistory",
                table: "MatchDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHistory",
                table: "MatchDetails");

            migrationBuilder.AddColumn<int>(
                name: "AwayShots",
                table: "MatchDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AwayShotsOnTarget",
                table: "MatchDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeShots",
                table: "MatchDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeShotsOnTarget",
                table: "MatchDetails",
                type: "integer",
                nullable: true);
        }
    }
}
