using MySql.Data.MySqlClient;
using HackMT2026.Models;

namespace HackMT2026.Endpoints 
{
    public static class PitchEndpoints
    {
        public static void MapPitchEndpoints(this WebApplication app)
        {
            app.MapGet("/pitches", (
                int? pitcherId,
                string? outcome,
                int? pitchNumber,
                int? gameID,
                string? pitchType,
                int? coachCall,
                int? actualCall,
                int? batterID
            ) =>
            {
                var pitches = new List<Pitch>();

                using var conn = Database.GetConnection();
                conn.Open();

                var cmd = new MySqlCommand();
                cmd.Connection = conn;

                // Base SQL
                string cmdString = @"
                    SELECT 
                        pi.pitchNumber,
                        pi.gameID,
                        pi.pitchType,
                        pi.coachCall,
                        pi.actualCall,
                        pi.outcome,
                        pl_pitcher.playerID AS pitcherID,
                        pl_pitcher.fName AS pitcherFirstName,
                        pl_pitcher.lName AS pitcherLastName,
                        pl_pitcher.playerNumber AS pitcherNumber,
                        pl_batter.playerID AS batterID,
                        pl_batter.fName AS batterFirstName,
                        pl_batter.lName AS batterLastName,
                        pl_batter.playerNumber AS batterNumber
                    FROM pitch pi
                    JOIN player pl_pitcher ON pi.pitcherID = pl_pitcher.playerID
                    JOIN player pl_batter ON pi.batterID = pl_batter.playerID
                    WHERE 1=1
                ";

                // Optional filters
                if (pitcherId.HasValue)
                {
                    cmdString += " AND pi.pitcherID = @pitcherId";
                    cmd.Parameters.AddWithValue("@pitcherId", pitcherId.Value);
                }
                if (!string.IsNullOrEmpty(outcome))
                {
                    cmdString += " AND pi.outcome = @outcome";
                    cmd.Parameters.AddWithValue("@outcome", outcome);
                }
                if (pitchNumber.HasValue)
                {
                    cmdString += " AND pi.pitchNumber = @pitchNumber";
                    cmd.Parameters.AddWithValue("@pitchNumber", pitchNumber.Value);
                }
                if (gameID.HasValue)
                {
                    cmdString += " AND pi.gameID = @gameID";
                    cmd.Parameters.AddWithValue("@gameID", gameID.Value);
                }
                if (!string.IsNullOrEmpty(pitchType))
                {
                    cmdString += " AND pi.pitchType = @pitchType";
                    cmd.Parameters.AddWithValue("@pitchType", pitchType);
                }
                if (coachCall.HasValue)
                {
                    cmdString += " AND pi.coachCall = @coachCall";
                    cmd.Parameters.AddWithValue("@coachCall", coachCall.Value);
                }
                if (actualCall.HasValue)
                {
                    cmdString += " AND pi.actualCall = @actualCall";
                    cmd.Parameters.AddWithValue("@actualCall", actualCall.Value);
                }
                if (batterID.HasValue)
                {
                    cmdString += " AND pi.batterID = @batterID";
                    cmd.Parameters.AddWithValue("@batterID", batterID.Value);
                }

                cmd.CommandText = cmdString;
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    pitches.Add(new Pitch
                    {
                        // Pitch info
                        PitchNumber = reader.GetInt32("pitchNumber"),
                        GameID = reader.GetInt32("gameID"),
                        PitchType = reader.GetString("pitchType"),
                        CoachCall = reader.GetInt32("coachCall"),
                        ActualCall = reader.GetInt32("actualCall"),
                        Outcome = reader.GetString("outcome"),

                        // Pitcher info
                        PitcherID = reader.GetInt32("pitcherID"),
                        PitcherFirstName = reader.GetString("pitcherFirstName"),
                        PitcherLastName = reader.GetString("pitcherLastName"),
                        PitcherNumber = reader.GetInt32("pitcherNumber"),

                        // Batter info
                        BatterID = reader.GetInt32("batterID"),
                        BatterFirstName = reader.GetString("batterFirstName"),
                        BatterLastName = reader.GetString("batterLastName"),
                        BatterNumber = reader.GetInt32("batterNumber")
                    });
                }

                return Results.Ok(pitches);
            });

            app.MapGet("/pitches/pitchers", (int? pitcherId) =>
            {
                var pitchers = new List<Pitcher>();

                using var conn = Database.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand();
                cmd.Connection = conn;

                string cmdString = @"
                    SELECT DISTINCT
                        p.playerID,
                        p.fName,
                        p.lName,
                        p.playerNumber,
                        t.teamName,
                        t.season,
                        t.ageGroup
                    FROM player p
                    JOIN pitch pi ON pi.pitcherID = p.playerID
                    JOIN playerTeam pt
                        ON p.playerID = pt.playerID
                    JOIN team t
                        ON t.teamID = pt.teamID
                    AND t.season = pt.season
                    JOIN (
                        SELECT
                            playerID,
                            MAX(season) AS latestSeason
                        FROM playerTeam
                        GROUP BY playerID
                    ) latest
                        ON latest.playerID = pt.playerID
                    AND latest.latestSeason = pt.season
                    ";
                    
                if (pitcherId.HasValue)
                {
                    cmdString += " AND p.playerId = @pitcherId";
                    cmd.Parameters.AddWithValue("@pitcherId", pitcherId.Value);
                }

                cmdString += " ORDER BY p.lName, p.fName";

                cmd.CommandText = cmdString;

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    pitchers.Add(new Pitcher
                    {
                        PlayerId = reader.GetInt32("playerID"),
                        FName = reader.GetString("fName"),
                        LName = reader.GetString("lName"),
                        PlayerNumber = reader.GetInt32("playerNumber"),
                        TeamName = reader.GetString("teamName"),
                        Season = reader.GetInt32("season"),
                        AgeGroup = reader.GetString("ageGroup")
                    });
                }

                return Results.Ok(pitchers);
            });

            app.MapGet("/pitches/pitchers/{id:int}", (int id, int? gameId) =>
            {
                var pitcherStats = new PitcherStats();

                List<PitchInstance> pitchInstances = new List<PitchInstance>();
                
                using var conn = Database.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand();
                cmd.Connection = conn;

                string cmdString = @"
                        SELECT
                            p.pitcherId,
                            pl.fName,
                            pl.lName,
                            p.pitchType,
                            p.outcome,
                            p.coachCall,
                            p.actualCall
                        FROM pitch p
                        JOIN player pl ON p.pitcherId = pl.playerId
                        JOIN game g ON p.gameId = g.gameId
                        WHERE p.pitcherId = @pitcherId
                ";

                if (gameId.HasValue)
                {
                    cmdString += " AND g.gameId = @gameId";
                    cmd.Parameters.AddWithValue("@gameId", gameId.Value);
                }

                cmd.CommandText = cmdString;
                cmd.Parameters.AddWithValue("@pitcherId", id);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int coachCall = reader.GetInt32("coachCall");
                    int actualCall = reader.GetInt32("actualCall");

                    pitcherStats.FirstName = reader.GetString("fName");
                    pitcherStats.LastName = reader.GetString("lName");
                    
                    pitchInstances.Add(new PitchInstance
                    {
                        PitchType = reader.GetString("pitchType"),
                        Outcome = reader.GetString("outcome"),
                        IsAccurate = coachCall == actualCall
                    });
                }

                foreach (var pi in pitchInstances)
                {
                    if (!pitcherStats.PitchTypes.ContainsKey(pi.PitchType))
                    {
                        pitcherStats.PitchTypes[pi.PitchType] = new PitchTypeStats();
                    }

                    switch (pi.Outcome.ToLower())
                    {
                        case "strike":
                            pitcherStats.PitchTypes[pi.PitchType].Strikes++;
                            break;
                        case "ball":
                            pitcherStats.PitchTypes[pi.PitchType].Balls++;
                            break;
                        case "foul":
                            pitcherStats.PitchTypes[pi.PitchType].Fouls++;
                            break;
                        case "inplay":
                            pitcherStats.PitchTypes[pi.PitchType].InPlay++;
                            break;
                    }

                    if (pi.IsAccurate)
                    {
                        pitcherStats.TotalAccurate++;
                    }

                    pitcherStats.TotalPitches++;
                }

                return Results.Ok(pitcherStats);
            });

            app.MapPost("/pitches", async (PitchRequest request) =>
            {
                using var conn = Database.GetConnection();
                await conn.OpenAsync();

                var sql = @"
                    INSERT INTO pitch (pitcherId, gameId, pitchType, coachCall, actualCall, outcome, batterId)
                    VALUES (@PitcherId, @GameId, @PitchType, @CoachCall, @ActualCall, @Outcome, @BatterId);
                ";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PitcherId", request.PitcherId);
                cmd.Parameters.AddWithValue("@GameId", request.GameId);
                cmd.Parameters.AddWithValue("@PitchType", request.PitchType);
                cmd.Parameters.AddWithValue("@CoachCall", request.CoachCall);
                cmd.Parameters.AddWithValue("@ActualCall", request.ActualCall);
                cmd.Parameters.AddWithValue("@Outcome", request.Outcome);
                cmd.Parameters.AddWithValue("@BatterId", request.BatterId);

                var rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                    return Results.Created($"/pitches/{request.GameId}-{request.PitcherId}", request);
                else
                    return Results.BadRequest("Failed to create pitch.");
            });
        }
    }
}