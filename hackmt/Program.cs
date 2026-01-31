using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        var connectionString =
            "Server=your-rds-endpoint.amazonaws.com;" +
            "Port=3306;" +
            "Database=hackmt;" +
            "User ID=admin;" +
            "Password=hackMT2026;" +
            "SslMode=Required;";

        using var conn = new MySqlConnection(connectionString);
        conn.Open();

        using var cmd = new MySqlCommand("SELECT * FROM player", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            // Example: print all columns dynamically
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.Write($"{reader.GetName(i)}={reader.GetValue(i)} ");
            }
            Console.WriteLine();
        }
    }
}
