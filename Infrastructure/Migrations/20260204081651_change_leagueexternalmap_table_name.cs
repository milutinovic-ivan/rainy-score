using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class change_leagueexternalmap_table_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueExternalMap_Leagues_LeagueId",
                table: "LeagueExternalMap");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueExternalMap",
                table: "LeagueExternalMap");

            migrationBuilder.RenameTable(
                name: "LeagueExternalMap",
                newName: "LeagueExternalMaps");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueExternalMap_LeagueId_DataSource",
                table: "LeagueExternalMaps",
                newName: "IX_LeagueExternalMaps_LeagueId_DataSource");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueExternalMap_ExternalLeagueId_DataSource",
                table: "LeagueExternalMaps",
                newName: "IX_LeagueExternalMaps_ExternalLeagueId_DataSource");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueExternalMaps",
                table: "LeagueExternalMaps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueExternalMaps_Leagues_LeagueId",
                table: "LeagueExternalMaps",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueExternalMaps_Leagues_LeagueId",
                table: "LeagueExternalMaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueExternalMaps",
                table: "LeagueExternalMaps");

            migrationBuilder.RenameTable(
                name: "LeagueExternalMaps",
                newName: "LeagueExternalMap");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueExternalMaps_LeagueId_DataSource",
                table: "LeagueExternalMap",
                newName: "IX_LeagueExternalMap_LeagueId_DataSource");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueExternalMaps_ExternalLeagueId_DataSource",
                table: "LeagueExternalMap",
                newName: "IX_LeagueExternalMap_ExternalLeagueId_DataSource");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueExternalMap",
                table: "LeagueExternalMap",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueExternalMap_Leagues_LeagueId",
                table: "LeagueExternalMap",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
