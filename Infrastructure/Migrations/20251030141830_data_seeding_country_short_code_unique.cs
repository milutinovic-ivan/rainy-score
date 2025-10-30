using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class data_seeding_country_short_code_unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedAt", "Name", "ShortCode" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Scotland", "SCT" },
                    { 3, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Germany", "GER" },
                    { 4, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Italy", "ITA" },
                    { 5, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Spain", "ESP" },
                    { 6, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "France", "FRA" },
                    { 7, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Netherlands", "NL" },
                    { 8, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Belgium", "BEL" },
                    { 9, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Portugal", "PRT" },
                    { 10, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Turkey", "TUR" },
                    { 11, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Greece", "GRC" }
                });

            migrationBuilder.InsertData(
                table: "Leagues",
                columns: new[] { "Id", "CountryId", "CreatedAt", "Name", "ShortCode" },
                values: new object[,]
                {
                    { 6, 2, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Premier League", "SC0" },
                    { 7, 2, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Division 1", "SC1" },
                    { 8, 2, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Division 2", "SC2" },
                    { 9, 2, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Division 3", "SC3" },
                    { 10, 3, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Bundesliga 1", "D1" },
                    { 11, 3, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Bundesliga 2", "D2" },
                    { 12, 4, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Serie A", "I1" },
                    { 13, 4, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Serie B", "I2" },
                    { 14, 5, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "La Liga Primera Division", "SP1" },
                    { 15, 5, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "La Liga Segunda Division", "SP2" },
                    { 16, 6, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Le Championnat", "F1" },
                    { 17, 6, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Division 2", "F2" },
                    { 18, 7, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Eredivisie", "N1" },
                    { 19, 8, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Jupiler League", "B1" },
                    { 20, 9, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Liga I", "P1" },
                    { 21, 10, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Futbol Ligi 1", "T1" },
                    { 22, 11, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Ethniki Katigoria", "G1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ShortCode",
                table: "Countries",
                column: "ShortCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Countries_ShortCode",
                table: "Countries");

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}
