using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class leagueexternalmap_table_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Leagues_ApiFootballLeagueId",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "ApiFootballLeagueId",
                table: "Leagues");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Leagues",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Countries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "LeagueExternalMap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeagueId = table.Column<int>(type: "integer", nullable: false),
                    DataSource = table.Column<string>(type: "text", nullable: false),
                    ExternalLeagueId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueExternalMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeagueExternalMap_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LeagueExternalMap",
                columns: new[] { "Id", "CreatedAt", "DataSource", "ExternalLeagueId", "LeagueId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 39, 1 },
                    { 2, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 40, 2 },
                    { 3, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 41, 3 },
                    { 4, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 42, 4 },
                    { 5, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 43, 5 },
                    { 6, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 179, 6 },
                    { 7, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 180, 7 },
                    { 8, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 183, 8 },
                    { 9, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 184, 9 },
                    { 10, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 78, 10 },
                    { 11, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 79, 11 },
                    { 12, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 135, 12 },
                    { 13, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 136, 13 },
                    { 14, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 140, 14 },
                    { 15, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 141, 15 },
                    { 16, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 61, 16 },
                    { 17, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 62, 17 },
                    { 18, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 88, 18 },
                    { 19, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 144, 19 },
                    { 20, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 94, 20 },
                    { 21, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 203, 21 },
                    { 22, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "apifootball", 197, 22 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueExternalMap_ExternalLeagueId_DataSource",
                table: "LeagueExternalMap",
                columns: new[] { "ExternalLeagueId", "DataSource" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueExternalMap_LeagueId_DataSource",
                table: "LeagueExternalMap",
                columns: new[] { "LeagueId", "DataSource" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeagueExternalMap");

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Leagues",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApiFootballLeagueId",
                table: "Leagues",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortCode",
                table: "Countries",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

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
                column: "ApiFootballLeagueId",
                value: 41);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 4,
                column: "ApiFootballLeagueId",
                value: 42);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 5,
                column: "ApiFootballLeagueId",
                value: 43);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 6,
                column: "ApiFootballLeagueId",
                value: 179);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 7,
                column: "ApiFootballLeagueId",
                value: 180);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 8,
                column: "ApiFootballLeagueId",
                value: 183);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 9,
                column: "ApiFootballLeagueId",
                value: 184);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 10,
                column: "ApiFootballLeagueId",
                value: 78);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 11,
                column: "ApiFootballLeagueId",
                value: 79);

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
                column: "ApiFootballLeagueId",
                value: 140);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 15,
                column: "ApiFootballLeagueId",
                value: 141);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 16,
                column: "ApiFootballLeagueId",
                value: 61);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 17,
                column: "ApiFootballLeagueId",
                value: 62);

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
                column: "ApiFootballLeagueId",
                value: 144);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 20,
                column: "ApiFootballLeagueId",
                value: 94);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 21,
                column: "ApiFootballLeagueId",
                value: 203);

            migrationBuilder.UpdateData(
                table: "Leagues",
                keyColumn: "Id",
                keyValue: 22,
                column: "ApiFootballLeagueId",
                value: 197);

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_ApiFootballLeagueId",
                table: "Leagues",
                column: "ApiFootballLeagueId",
                unique: true);
        }
    }
}
