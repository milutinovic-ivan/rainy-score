using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeWeatherConditiontopluralname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeatherCondition_MatchDetails_MatchDetailsId",
                table: "WeatherCondition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherCondition",
                table: "WeatherCondition");

            migrationBuilder.RenameTable(
                name: "WeatherCondition",
                newName: "WeatherConditions");

            migrationBuilder.RenameIndex(
                name: "IX_WeatherCondition_MatchDetailsId",
                table: "WeatherConditions",
                newName: "IX_WeatherConditions_MatchDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherConditions",
                table: "WeatherConditions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeatherConditions_MatchDetails_MatchDetailsId",
                table: "WeatherConditions",
                column: "MatchDetailsId",
                principalTable: "MatchDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeatherConditions_MatchDetails_MatchDetailsId",
                table: "WeatherConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherConditions",
                table: "WeatherConditions");

            migrationBuilder.RenameTable(
                name: "WeatherConditions",
                newName: "WeatherCondition");

            migrationBuilder.RenameIndex(
                name: "IX_WeatherConditions_MatchDetailsId",
                table: "WeatherCondition",
                newName: "IX_WeatherCondition_MatchDetailsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherCondition",
                table: "WeatherCondition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeatherCondition_MatchDetails_MatchDetailsId",
                table: "WeatherCondition",
                column: "MatchDetailsId",
                principalTable: "MatchDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
