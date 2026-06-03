using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class snake_case_naming_convention_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueExternalMaps_Leagues_LeagueId",
                table: "LeagueExternalMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_Leagues_Countries_CountryId",
                table: "Leagues");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchDetails_Leagues_LeagueId",
                table: "MatchDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchDetails_Teams_AwayTeamId",
                table: "MatchDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchDetails_Teams_HomeTeamId",
                table: "MatchDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Stadiums_StadiumId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_WeatherConditions_MatchDetails_MatchDetailsId",
                table: "WeatherConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Stadiums",
                table: "Stadiums");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Leagues",
                table: "Leagues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeatherConditions",
                table: "WeatherConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchDetails",
                table: "MatchDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueExternalMaps",
                table: "LeagueExternalMaps");

            migrationBuilder.RenameTable(
                name: "Teams",
                newName: "teams");

            migrationBuilder.RenameTable(
                name: "Stadiums",
                newName: "stadiums");

            migrationBuilder.RenameTable(
                name: "Leagues",
                newName: "leagues");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "countries");

            migrationBuilder.RenameTable(
                name: "WeatherConditions",
                newName: "weather_conditions");

            migrationBuilder.RenameTable(
                name: "MatchDetails",
                newName: "match_details");

            migrationBuilder.RenameTable(
                name: "LeagueExternalMaps",
                newName: "league_external_maps");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "teams",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "teams",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StadiumId",
                table: "teams",
                newName: "stadium_id");

            migrationBuilder.RenameColumn(
                name: "ShortCode",
                table: "teams",
                newName: "short_code");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "teams",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_StadiumId",
                table: "teams",
                newName: "ix_teams_stadium_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "stadiums",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "stadiums",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "stadiums",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "stadiums",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "stadiums",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "leagues",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "leagues",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ShortCode",
                table: "leagues",
                newName: "short_code");

            migrationBuilder.RenameColumn(
                name: "IsCup",
                table: "leagues",
                newName: "is_cup");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "leagues",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "leagues",
                newName: "country_id");

            migrationBuilder.RenameIndex(
                name: "IX_Leagues_CountryId",
                table: "leagues",
                newName: "ix_leagues_country_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "countries",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "countries",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ShortCode",
                table: "countries",
                newName: "short_code");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "countries",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_Name",
                table: "countries",
                newName: "ix_countries_name");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_ShortCode",
                table: "countries",
                newName: "ix_countries_short_code");

            migrationBuilder.RenameColumn(
                name: "Precipitation",
                table: "weather_conditions",
                newName: "precipitation");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "weather_conditions",
                newName: "longitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "weather_conditions",
                newName: "latitude");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "weather_conditions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WindSpeed10m",
                table: "weather_conditions",
                newName: "wind_speed_10m");

            migrationBuilder.RenameColumn(
                name: "WeatherServiceCode",
                table: "weather_conditions",
                newName: "weather_service_code");

            migrationBuilder.RenameColumn(
                name: "WeatherCode",
                table: "weather_conditions",
                newName: "weather_code");

            migrationBuilder.RenameColumn(
                name: "Temperature2m",
                table: "weather_conditions",
                newName: "temperature_2m");

            migrationBuilder.RenameColumn(
                name: "SunshineDuration",
                table: "weather_conditions",
                newName: "sunshine_duration");

            migrationBuilder.RenameColumn(
                name: "OriginalResponse",
                table: "weather_conditions",
                newName: "original_response");

            migrationBuilder.RenameColumn(
                name: "MatchDetailsId",
                table: "weather_conditions",
                newName: "match_details_id");

            migrationBuilder.RenameColumn(
                name: "DewPoint2m",
                table: "weather_conditions",
                newName: "dew_point_2m");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "weather_conditions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CloudCoverLow",
                table: "weather_conditions",
                newName: "cloud_cover_low");

            migrationBuilder.RenameColumn(
                name: "CloudCover",
                table: "weather_conditions",
                newName: "cloud_cover");

            migrationBuilder.RenameIndex(
                name: "IX_WeatherConditions_MatchDetailsId",
                table: "weather_conditions",
                newName: "ix_weather_conditions_match_details_id");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "match_details",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Season",
                table: "match_details",
                newName: "season");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "match_details",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "OriginalResponseOdds",
                table: "match_details",
                newName: "original_response_odds");

            migrationBuilder.RenameColumn(
                name: "MatchTime",
                table: "match_details",
                newName: "match_time");

            migrationBuilder.RenameColumn(
                name: "MatchDate",
                table: "match_details",
                newName: "match_date");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "match_details",
                newName: "league_id");

            migrationBuilder.RenameColumn(
                name: "IsHistory",
                table: "match_details",
                newName: "is_history");

            migrationBuilder.RenameColumn(
                name: "HomeWinOdds",
                table: "match_details",
                newName: "home_win_odds");

            migrationBuilder.RenameColumn(
                name: "HomeTeamId",
                table: "match_details",
                newName: "home_team_id");

            migrationBuilder.RenameColumn(
                name: "HalfTimeWiner",
                table: "match_details",
                newName: "half_time_winer");

            migrationBuilder.RenameColumn(
                name: "HalfTimeHomeGoals",
                table: "match_details",
                newName: "half_time_home_goals");

            migrationBuilder.RenameColumn(
                name: "HalfTimeAwayGoals",
                table: "match_details",
                newName: "half_time_away_goals");

            migrationBuilder.RenameColumn(
                name: "GoalsUnder25Odds",
                table: "match_details",
                newName: "goals_under_2_5_odds");

            migrationBuilder.RenameColumn(
                name: "GoalsOver25Odds",
                table: "match_details",
                newName: "goals_over_2_5_odds");

            migrationBuilder.RenameColumn(
                name: "FullTimeWiner",
                table: "match_details",
                newName: "full_time_winer");

            migrationBuilder.RenameColumn(
                name: "FullTimeHomeGoals",
                table: "match_details",
                newName: "full_time_home_goals");

            migrationBuilder.RenameColumn(
                name: "FullTimeAwayGoals",
                table: "match_details",
                newName: "full_time_away_goals");

            migrationBuilder.RenameColumn(
                name: "FixtureId",
                table: "match_details",
                newName: "fixture_id");

            migrationBuilder.RenameColumn(
                name: "DrawWinOdds",
                table: "match_details",
                newName: "draw_win_odds");

            migrationBuilder.RenameColumn(
                name: "DataSource",
                table: "match_details",
                newName: "data_source");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "match_details",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BookmakerName",
                table: "match_details",
                newName: "bookmaker_name");

            migrationBuilder.RenameColumn(
                name: "BookmakerId",
                table: "match_details",
                newName: "bookmaker_id");

            migrationBuilder.RenameColumn(
                name: "AwayWinOdds",
                table: "match_details",
                newName: "away_win_odds");

            migrationBuilder.RenameColumn(
                name: "AwayTeamId",
                table: "match_details",
                newName: "away_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_MatchDetails_LeagueId",
                table: "match_details",
                newName: "ix_match_details_league_id");

            migrationBuilder.RenameIndex(
                name: "IX_MatchDetails_HomeTeamId",
                table: "match_details",
                newName: "ix_match_details_home_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_MatchDetails_DataSource_FixtureId",
                table: "match_details",
                newName: "ix_match_details_data_source_fixture_id");

            migrationBuilder.RenameIndex(
                name: "IX_MatchDetails_AwayTeamId",
                table: "match_details",
                newName: "ix_match_details_away_team_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "league_external_maps",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "league_external_maps",
                newName: "league_id");

            migrationBuilder.RenameColumn(
                name: "ExternalLeagueId",
                table: "league_external_maps",
                newName: "external_league_id");

            migrationBuilder.RenameColumn(
                name: "DataSource",
                table: "league_external_maps",
                newName: "data_source");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "league_external_maps",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueExternalMaps_LeagueId_DataSource",
                table: "league_external_maps",
                newName: "ix_league_external_maps_league_id_data_source");

            migrationBuilder.RenameIndex(
                name: "IX_LeagueExternalMaps_ExternalLeagueId_DataSource",
                table: "league_external_maps",
                newName: "ix_league_external_maps_external_league_id_data_source");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "stadiums",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "stadiums",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "terrain_type",
                table: "stadiums",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_teams",
                table: "teams",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_stadiums",
                table: "stadiums",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_leagues",
                table: "leagues",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_countries",
                table: "countries",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_weather_conditions",
                table: "weather_conditions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_match_details",
                table: "match_details",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_league_external_maps",
                table: "league_external_maps",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_league_external_maps_leagues_league_id",
                table: "league_external_maps",
                column: "league_id",
                principalTable: "leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_leagues_countries_country_id",
                table: "leagues",
                column: "country_id",
                principalTable: "countries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_match_details_leagues_league_id",
                table: "match_details",
                column: "league_id",
                principalTable: "leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_match_details_teams_away_team_id",
                table: "match_details",
                column: "away_team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_match_details_teams_home_team_id",
                table: "match_details",
                column: "home_team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_stadiums_stadium_id",
                table: "teams",
                column: "stadium_id",
                principalTable: "stadiums",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_weather_conditions_match_details_match_details_id",
                table: "weather_conditions",
                column: "match_details_id",
                principalTable: "match_details",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_league_external_maps_leagues_league_id",
                table: "league_external_maps");

            migrationBuilder.DropForeignKey(
                name: "fk_leagues_countries_country_id",
                table: "leagues");

            migrationBuilder.DropForeignKey(
                name: "fk_match_details_leagues_league_id",
                table: "match_details");

            migrationBuilder.DropForeignKey(
                name: "fk_match_details_teams_away_team_id",
                table: "match_details");

            migrationBuilder.DropForeignKey(
                name: "fk_match_details_teams_home_team_id",
                table: "match_details");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_stadiums_stadium_id",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "fk_weather_conditions_match_details_match_details_id",
                table: "weather_conditions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_teams",
                table: "teams");

            migrationBuilder.DropPrimaryKey(
                name: "pk_stadiums",
                table: "stadiums");

            migrationBuilder.DropPrimaryKey(
                name: "pk_leagues",
                table: "leagues");

            migrationBuilder.DropPrimaryKey(
                name: "pk_countries",
                table: "countries");

            migrationBuilder.DropPrimaryKey(
                name: "pk_weather_conditions",
                table: "weather_conditions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_match_details",
                table: "match_details");

            migrationBuilder.DropPrimaryKey(
                name: "pk_league_external_maps",
                table: "league_external_maps");

            migrationBuilder.DropColumn(
                name: "address",
                table: "stadiums");

            migrationBuilder.DropColumn(
                name: "city",
                table: "stadiums");

            migrationBuilder.DropColumn(
                name: "terrain_type",
                table: "stadiums");

            migrationBuilder.RenameTable(
                name: "teams",
                newName: "Teams");

            migrationBuilder.RenameTable(
                name: "stadiums",
                newName: "Stadiums");

            migrationBuilder.RenameTable(
                name: "leagues",
                newName: "Leagues");

            migrationBuilder.RenameTable(
                name: "countries",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "weather_conditions",
                newName: "WeatherConditions");

            migrationBuilder.RenameTable(
                name: "match_details",
                newName: "MatchDetails");

            migrationBuilder.RenameTable(
                name: "league_external_maps",
                newName: "LeagueExternalMaps");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Teams",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Teams",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "stadium_id",
                table: "Teams",
                newName: "StadiumId");

            migrationBuilder.RenameColumn(
                name: "short_code",
                table: "Teams",
                newName: "ShortCode");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Teams",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_teams_stadium_id",
                table: "Teams",
                newName: "IX_Teams_StadiumId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Stadiums",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "Stadiums",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Stadiums",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Stadiums",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Stadiums",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Leagues",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Leagues",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "short_code",
                table: "Leagues",
                newName: "ShortCode");

            migrationBuilder.RenameColumn(
                name: "is_cup",
                table: "Leagues",
                newName: "IsCup");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Leagues",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "Leagues",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "ix_leagues_country_id",
                table: "Leagues",
                newName: "IX_Leagues_CountryId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Countries",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Countries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "short_code",
                table: "Countries",
                newName: "ShortCode");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Countries",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_countries_name",
                table: "Countries",
                newName: "IX_Countries_Name");

            migrationBuilder.RenameIndex(
                name: "ix_countries_short_code",
                table: "Countries",
                newName: "IX_Countries_ShortCode");

            migrationBuilder.RenameColumn(
                name: "precipitation",
                table: "WeatherConditions",
                newName: "Precipitation");

            migrationBuilder.RenameColumn(
                name: "longitude",
                table: "WeatherConditions",
                newName: "Longitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "WeatherConditions",
                newName: "Latitude");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "WeatherConditions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "wind_speed_10m",
                table: "WeatherConditions",
                newName: "WindSpeed10m");

            migrationBuilder.RenameColumn(
                name: "weather_service_code",
                table: "WeatherConditions",
                newName: "WeatherServiceCode");

            migrationBuilder.RenameColumn(
                name: "weather_code",
                table: "WeatherConditions",
                newName: "WeatherCode");

            migrationBuilder.RenameColumn(
                name: "temperature_2m",
                table: "WeatherConditions",
                newName: "Temperature2m");

            migrationBuilder.RenameColumn(
                name: "sunshine_duration",
                table: "WeatherConditions",
                newName: "SunshineDuration");

            migrationBuilder.RenameColumn(
                name: "original_response",
                table: "WeatherConditions",
                newName: "OriginalResponse");

            migrationBuilder.RenameColumn(
                name: "match_details_id",
                table: "WeatherConditions",
                newName: "MatchDetailsId");

            migrationBuilder.RenameColumn(
                name: "dew_point_2m",
                table: "WeatherConditions",
                newName: "DewPoint2m");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "WeatherConditions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "cloud_cover_low",
                table: "WeatherConditions",
                newName: "CloudCoverLow");

            migrationBuilder.RenameColumn(
                name: "cloud_cover",
                table: "WeatherConditions",
                newName: "CloudCover");

            migrationBuilder.RenameIndex(
                name: "ix_weather_conditions_match_details_id",
                table: "WeatherConditions",
                newName: "IX_WeatherConditions_MatchDetailsId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "MatchDetails",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "season",
                table: "MatchDetails",
                newName: "Season");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "MatchDetails",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "original_response_odds",
                table: "MatchDetails",
                newName: "OriginalResponseOdds");

            migrationBuilder.RenameColumn(
                name: "match_time",
                table: "MatchDetails",
                newName: "MatchTime");

            migrationBuilder.RenameColumn(
                name: "match_date",
                table: "MatchDetails",
                newName: "MatchDate");

            migrationBuilder.RenameColumn(
                name: "league_id",
                table: "MatchDetails",
                newName: "LeagueId");

            migrationBuilder.RenameColumn(
                name: "is_history",
                table: "MatchDetails",
                newName: "IsHistory");

            migrationBuilder.RenameColumn(
                name: "home_win_odds",
                table: "MatchDetails",
                newName: "HomeWinOdds");

            migrationBuilder.RenameColumn(
                name: "home_team_id",
                table: "MatchDetails",
                newName: "HomeTeamId");

            migrationBuilder.RenameColumn(
                name: "half_time_winer",
                table: "MatchDetails",
                newName: "HalfTimeWiner");

            migrationBuilder.RenameColumn(
                name: "half_time_home_goals",
                table: "MatchDetails",
                newName: "HalfTimeHomeGoals");

            migrationBuilder.RenameColumn(
                name: "half_time_away_goals",
                table: "MatchDetails",
                newName: "HalfTimeAwayGoals");

            migrationBuilder.RenameColumn(
                name: "goals_under_2_5_odds",
                table: "MatchDetails",
                newName: "GoalsUnder25Odds");

            migrationBuilder.RenameColumn(
                name: "goals_over_2_5_odds",
                table: "MatchDetails",
                newName: "GoalsOver25Odds");

            migrationBuilder.RenameColumn(
                name: "full_time_winer",
                table: "MatchDetails",
                newName: "FullTimeWiner");

            migrationBuilder.RenameColumn(
                name: "full_time_home_goals",
                table: "MatchDetails",
                newName: "FullTimeHomeGoals");

            migrationBuilder.RenameColumn(
                name: "full_time_away_goals",
                table: "MatchDetails",
                newName: "FullTimeAwayGoals");

            migrationBuilder.RenameColumn(
                name: "fixture_id",
                table: "MatchDetails",
                newName: "FixtureId");

            migrationBuilder.RenameColumn(
                name: "draw_win_odds",
                table: "MatchDetails",
                newName: "DrawWinOdds");

            migrationBuilder.RenameColumn(
                name: "data_source",
                table: "MatchDetails",
                newName: "DataSource");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "MatchDetails",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "bookmaker_name",
                table: "MatchDetails",
                newName: "BookmakerName");

            migrationBuilder.RenameColumn(
                name: "bookmaker_id",
                table: "MatchDetails",
                newName: "BookmakerId");

            migrationBuilder.RenameColumn(
                name: "away_win_odds",
                table: "MatchDetails",
                newName: "AwayWinOdds");

            migrationBuilder.RenameColumn(
                name: "away_team_id",
                table: "MatchDetails",
                newName: "AwayTeamId");

            migrationBuilder.RenameIndex(
                name: "ix_match_details_league_id",
                table: "MatchDetails",
                newName: "IX_MatchDetails_LeagueId");

            migrationBuilder.RenameIndex(
                name: "ix_match_details_home_team_id",
                table: "MatchDetails",
                newName: "IX_MatchDetails_HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "ix_match_details_data_source_fixture_id",
                table: "MatchDetails",
                newName: "IX_MatchDetails_DataSource_FixtureId");

            migrationBuilder.RenameIndex(
                name: "ix_match_details_away_team_id",
                table: "MatchDetails",
                newName: "IX_MatchDetails_AwayTeamId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "LeagueExternalMaps",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "league_id",
                table: "LeagueExternalMaps",
                newName: "LeagueId");

            migrationBuilder.RenameColumn(
                name: "external_league_id",
                table: "LeagueExternalMaps",
                newName: "ExternalLeagueId");

            migrationBuilder.RenameColumn(
                name: "data_source",
                table: "LeagueExternalMaps",
                newName: "DataSource");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "LeagueExternalMaps",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_league_external_maps_league_id_data_source",
                table: "LeagueExternalMaps",
                newName: "IX_LeagueExternalMaps_LeagueId_DataSource");

            migrationBuilder.RenameIndex(
                name: "ix_league_external_maps_external_league_id_data_source",
                table: "LeagueExternalMaps",
                newName: "IX_LeagueExternalMaps_ExternalLeagueId_DataSource");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stadiums",
                table: "Stadiums",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Leagues",
                table: "Leagues",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeatherConditions",
                table: "WeatherConditions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchDetails",
                table: "MatchDetails",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Leagues_Countries_CountryId",
                table: "Leagues",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchDetails_Leagues_LeagueId",
                table: "MatchDetails",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchDetails_Teams_AwayTeamId",
                table: "MatchDetails",
                column: "AwayTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchDetails_Teams_HomeTeamId",
                table: "MatchDetails",
                column: "HomeTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Stadiums_StadiumId",
                table: "Teams",
                column: "StadiumId",
                principalTable: "Stadiums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeatherConditions_MatchDetails_MatchDetailsId",
                table: "WeatherConditions",
                column: "MatchDetailsId",
                principalTable: "MatchDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
