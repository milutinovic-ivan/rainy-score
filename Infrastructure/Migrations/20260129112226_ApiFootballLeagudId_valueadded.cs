using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApiFootballLeagudId_valueadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 1,
                column: "ApiFootballLeagueId",
                value: 39);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 2,
                column: "ApiFootballLeagueId",
                value: 40);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 41, "League One" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 42, "League Two" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 43, "National League" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 179, "Premiership" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 180, "Championship" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 183, "League One" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 184, "League Two" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 78, "Bundesliga" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 79, "2. Bundesliga" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 12,
                column: "ApiFootballLeagueId",
                value: 135);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 13,
                column: "ApiFootballLeagueId",
                value: 136);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 140, "La Liga" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 141, "Segunda División" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 61, "Ligue 1" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 62, "Ligue 2" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 18,
                column: "ApiFootballLeagueId",
                value: 88);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 144, "Jupiler Pro League" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 94, "Primeira Liga" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 203, "Süper Lig" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { 197, "Super League 1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "League 1" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "League 2" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Conference" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Premier League" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Division 1" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Division 2" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Division 3" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Bundesliga 1" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Bundesliga 2" });

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
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "La Liga Primera Division" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "La Liga Segunda Division" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Le Championnat" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Division 2" });

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
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Jupiler League" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Liga I" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Futbol Ligi 1" });

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ApiFootballLeagueId", "Name" },
                values: new object[] { null, "Ethniki Katigoria" });
        }
    }
}
