using MySql.Data.MySqlClient;

public static class PitchEndpoints
{
    public static void MapPitchEndpoints(this WebApplication app)
    {
        app.MapGet("/pitches", () =>
        {
            var pitches = new List<Pitch>();

            using var conn = Database.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand(@"
                SELECT * FROM pitch
            ", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                
                pitches.Add(new Pitch
                {
                    PitchNumber = reader.GetInt32("pitchNumber"),
                    PitcherID = reader.GetInt32("pitcherID"),
                    GameID = reader.GetInt32("gameID"),
                    PitchType = reader.GetString("pitchType"),
                    CoachCall = reader.GetInt32("coachCall"),
                    ActualCall = reader.GetInt32("coachCall"),
                    Outcome = reader.GetString("outcome"),
                    BatterID = reader.GetInt32("batterID")
                });
            }

            return Results.Ok(pitches);
        });
    }
}
