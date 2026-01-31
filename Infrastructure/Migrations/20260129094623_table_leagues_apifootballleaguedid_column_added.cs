using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class table_leagues_apifootballleaguedid_column_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApiFootballLeagueId",
                table: "Leagues",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 1,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 2,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 3,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 4,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 5,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 6,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 7,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 8,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 9,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 10,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 11,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 12,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 13,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 14,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 15,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 16,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 17,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 18,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 19,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 20,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 21,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 22,
                column: "ApiFootballLeagueId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_ApiFootballLeagueId",
                table: "Leagues",
                column: "ApiFootballLeagueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leagues_ApiFootballLeagueId",
                table: "Leagues");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Name",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "ApiFootballLeagueId",
                table: "Leagues");
        }
    }
}
