using MySql.Data.MySqlClient;
using HackMT2026.Models;

namespace HackMT2026.Endpoints
{
    public static class GameEndpoints
    {
        public static void MapGameEndpoints(this WebApplication app)
        {
            app.MapGet("/games", (int? gameId, int? homeTeamId, int? awayTeamId, int? season) =>
            {
                var games = new List<Game>();

                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand();
                cmd.Connection = conn;

                string sql = @"
                    SELECT 
                        g.gameId,
                        g.gameDate,
                        

                        -- Home team info
                        home.teamId AS HomeTeamId,
                        home.season AS HomeTeamSeason,
                        home.teamName AS HomeTeamName,
                        home.ageGroup AS HomeAgeGroup,

                        -- Away team info
                        away.teamId AS AwayTeamId,
                        away.season AS AwayTeamSeason,
                        away.teamName AS AwayTeamName,
                        away.ageGroup AS AwayAgeGroup
                    FROM game g
                    JOIN team home 
                        ON g.homeTeamId = home.teamId 
                    AND g.homeTeamSeason = home.season
                    JOIN team away
                        ON g.awayTeamId = away.teamId 
                    AND g.awayTeamSeason = away.season
                    WHERE 1=1
                ";

                if (gameId.HasValue)
                {
                    sql += " AND g.gameId = @gameId";
                    cmd.Parameters.AddWithValue("@gameId", gameId.Value);
                }
                if (homeTeamId.HasValue)
                {
                    sql += " AND g.homeTeamId = @homeTeamId";
                    cmd.Parameters.AddWithValue("@homeTeamId", homeTeamId.Value);
                }

                if (awayTeamId.HasValue)
                {
                    sql += " AND g.awayTeamId = @awayTeamId";
                    cmd.Parameters.AddWithValue("@awayTeamId", awayTeamId.Value);
                }

                if (season.HasValue)
                {
                    sql += " AND (g.homeTeamSeason = @season OR g.awayTeamSeason = @season)";
                    cmd.Parameters.AddWithValue("@season", season.Value);
                }

                cmd.CommandText = sql;

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    games.Add(new Game
                    {
                        GameId = reader.GetInt32("gameId"),
                        GameDate = reader.GetString("gameDate"),

                        // Home team
                        HomeTeamId = reader.GetInt32("HomeTeamId"),
                        HomeTeamSeason = reader.GetInt32("HomeTeamSeason"),
                        HomeTeamName = reader.GetString("HomeTeamName"),
                        HomeAgeGroup = reader.GetString("HomeAgeGroup"),

                        // Away team
                        AwayTeamId = reader.GetInt32("AwayTeamId"),
                        AwayTeamSeason = reader.GetInt32("AwayTeamSeason"),
                        AwayTeamName = reader.GetString("AwayTeamName"),
                        AwayAgeGroup = reader.GetString("AwayAgeGroup")
                    });
                }

                return Results.Ok(games);
            });
        }
    }
}