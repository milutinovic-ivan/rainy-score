using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class teams_country_id_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "country_id",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_teams_country_id",
                table: "teams",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "fk_teams_countries_country_id",
                table: "teams",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_countries_country_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "ix_teams_country_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "country_id",
                table: "teams");
        }
    }
}
