--case 1. rainy conditions, away team is strong favourite
select 
md."HomeWinOdds", md."DrawWinOdds", md."AwayWinOdds", 
wc."Precipitation",  md."FullTimeWiner", md."FullTimeHomeGoals", 
md."FullTimeAwayGoals" 
from "MatchDetails" as md
inner join "WeatherConditions" as wc
on md."Id" = wc."MatchDetailsId"
where wc."Precipitation" > 5 and md."AwayWinOdds" < 1.5

--case 1. ROI
SELECT
    COUNT(*)                                      AS total_matches,
    SUM(
        CASE
            WHEN md."FullTimeWiner" = 'D'
                THEN md."DrawWinOdds" - 1
            ELSE -1
        END
    )                                             AS total_profit,
    COUNT(*) * 1.0                                AS total_stake,
    ROUND(
        (SUM(
            CASE
                WHEN md."FullTimeWiner" = 'D'
                    THEN md."DrawWinOdds" - 1
                ELSE -1
            END
        ) / (COUNT(*) * 1.0)) * 100
    , 2)                                          AS roi_percent
FROM "MatchDetails" md
INNER JOIN "WeatherConditions" wc
    ON md."Id" = wc."MatchDetailsId"
WHERE wc."Precipitation" > 5
  AND md."AwayWinOdds" < 1.5;

