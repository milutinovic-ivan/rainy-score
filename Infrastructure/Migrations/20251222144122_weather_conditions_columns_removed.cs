using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class weather_conditions_columns_removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApparentTemperature",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "Et0FaoEvapotranspiration",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "Rain",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "Snowfall",
                table: "WeatherConditions");

            migrationBuilder.DropColumn(
                name: "SurfacePressure",
                table: "WeatherConditions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ApparentTemperature",
                table: "WeatherConditions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Et0FaoEvapotranspiration",
                table: "WeatherConditions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Rain",
                table: "WeatherConditions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Snowfall",
                table: "WeatherConditions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SurfacePressure",
                table: "WeatherConditions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
