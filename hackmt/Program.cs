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

        var cmd = new MySqlCommand("SELECT * FROM player;", conn);
        var reader = cmd.ExecuteReader();

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
