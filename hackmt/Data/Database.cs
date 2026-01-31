using MySql.Data.MySqlClient;

public static class Database
{
    public static MySqlConnection GetConnection()
    {
        var connStr =
            $"Server={Environment.GetEnvironmentVariable("ENDPOINT")};" +
            $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
            $"User ID={Environment.GetEnvironmentVariable("USER_ID")};" +
            $"Password={Environment.GetEnvironmentVariable("PASSWORD")};" +
            $"SslMode=Preferred;";

        return new MySqlConnection(connStr);
    }
}
