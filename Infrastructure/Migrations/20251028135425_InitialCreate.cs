using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stadiums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stadiums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leagues_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortCode = table.Column<string>(type: "text", nullable: false),
                    StadiumId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Stadiums_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeagueId = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MatchTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HomeTeamId = table.Column<int>(type: "integer", nullable: false),
                    AwayTeamId = table.Column<int>(type: "integer", nullable: false),
                    FullTimeHomeGoals = table.Column<int>(type: "integer", nullable: false),
                    FullTimeAwayGoals = table.Column<int>(type: "integer", nullable: false),
                    FullTimeWiner = table.Column<char>(type: "character(1)", nullable: false),
                    HalfTimeHomeGoals = table.Column<int>(type: "integer", nullable: false),
                    HalfTimeAwayGoals = table.Column<int>(type: "integer", nullable: false),
                    HalfTimeWiner = table.Column<char>(type: "character(1)", nullable: false),
                    HomeShots = table.Column<int>(type: "integer", nullable: false),
                    AwayShots = table.Column<int>(type: "integer", nullable: false),
                    HomeShotsOnTarget = table.Column<int>(type: "integer", nullable: false),
                    AwayShotsOnTarget = table.Column<int>(type: "integer", nullable: false),
                    HomeWinOdds = table.Column<decimal>(type: "numeric", nullable: false),
                    DrawWinOdds = table.Column<decimal>(type: "numeric", nullable: false),
                    AwayWinOdds = table.Column<decimal>(type: "numeric", nullable: false),
                    GoalsOver25 = table.Column<decimal>(type: "numeric", nullable: false),
                    GoalsUnder25 = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchDetails_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchDetails_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchDetails_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CreatedAt", "Name", "ShortCode" },
                values: new object[] { 1, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "England", "ENG" });

            migrationBuilder.InsertData(
                table: "Leagues",
                columns: new[] { "Id", "CountryId", "CreatedAt", "Name", "ShortCode" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Premier League", "E0" },
                    { 2, 1, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Championship", "E1" },
                    { 3, 1, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "League 1", "E2" },
                    { 4, 1, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "League 2", "E3" },
                    { 5, 1, new DateTime(2025, 10, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Conference", "EC" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_CountryId",
                table: "Leagues",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchDetails_AwayTeamId",
                table: "MatchDetails",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchDetails_HomeTeamId",
                table: "MatchDetails",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchDetails_LeagueId",
                table: "MatchDetails",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_StadiumId",
                table: "Teams",
                column: "StadiumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchDetails");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Stadiums");
        }
    }
}
