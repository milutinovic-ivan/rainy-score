using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tableaddedweather_condition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Stadiums",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Stadiums",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateTable(
                name: "WeatherCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchDetailsId = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Temperature2m = table.Column<decimal>(type: "numeric", nullable: false),
                    DewPoint2m = table.Column<decimal>(type: "numeric", nullable: false),
                    ApparentTemperature = table.Column<decimal>(type: "numeric", nullable: false),
                    SurfacePressure = table.Column<decimal>(type: "numeric", nullable: false),
                    Precipitation = table.Column<decimal>(type: "numeric", nullable: false),
                    Rain = table.Column<decimal>(type: "numeric", nullable: false),
                    Snowfall = table.Column<decimal>(type: "numeric", nullable: false),
                    CloudCover = table.Column<int>(type: "integer", nullable: false),
                    CloudCoverLow = table.Column<int>(type: "integer", nullable: false),
                    Et0FaoEvapotranspiration = table.Column<decimal>(type: "numeric", nullable: false),
                    WindSpeed10m = table.Column<decimal>(type: "numeric", nullable: false),
                    SunshineDuration = table.Column<decimal>(type: "numeric", nullable: false),
                    WeatherCode = table.Column<int>(type: "integer", nullable: false),
                    WeatherServiceCode = table.Column<string>(type: "text", nullable: false),
                    OriginalResponse = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherCondition_MatchDetails_MatchDetailsId",
                        column: x => x.MatchDetailsId,
                        principalTable: "MatchDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherCondition_MatchDetailsId",
                table: "WeatherCondition",
                column: "MatchDetailsId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherCondition");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Stadiums",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Stadiums",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
