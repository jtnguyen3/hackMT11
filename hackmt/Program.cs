using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        DotNetEnv.Env.Load();

        string endpoint = Environment.GetEnvironmentVariable("ENDPOINT");
        string dbName = Environment.GetEnvironmentVariable("DB_NAME");
        string userID = Environment.GetEnvironmentVariable("USER_ID");
        string password = Environment.GetEnvironmentVariable("PASSWORD");

        string connStr = $"Server={endpoint};Database={dbName};User ID={userID};Password={password};SslMode=Preferred;";
        var conn = new MySqlConnection(connStr);
        conn.Open();

        var cmd = new MySqlCommand(@"SELECT g.gameID, p1.fName AS pitcherFirst,
    p1.lName AS pitcherLast,
    p2.fName AS batterFirst,
    p2.lName AS batterLast,
    pi.pitchType,
    pi.outcome
FROM pitch pi
JOIN game g ON pi.gameID = g.gameID
JOIN player p1 ON pi.pitcherID = p1.playerID
JOIN player p2 ON pi.batterID = p2.playerID
ORDER BY g.gameID, pi.pitchNumber;", conn);
        var reader = cmd.ExecuteReader();

        /// The reader stores all data of the query with the fields included.
        /// .Read() will give you one data row at a time.
        /// Access attribute name with reader.GetName(i) and actual data with reader[i].
        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
                Console.Write($"{reader.GetName(i)}: {reader[i]}  ");
            Console.WriteLine();
        }

        reader.Close();
        conn.Close();
    }
}
