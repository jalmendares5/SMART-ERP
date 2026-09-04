using MySqlConnector;

namespace SMART_ERP.Data;

public static class DatabaseConnection
{
    public static string BuildConnectionString(
        string server,
        int port,
        string database,
        string username,
        string password)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = server,
            Port = (uint)port,
            Database = database,
            UserID = username,
            Password = password,
            SslMode = MySqlSslMode.None,
            ConnectionTimeout = 5
        };

        return builder.ConnectionString;
    }

    public static async Task<bool> TestConnectionAsync(
        string server,
        int port,
        string database,
        string username,
        string password)
    {
        try
        {
            var connectionString = BuildConnectionString(
                server,
                port,
                database,
                username,
                password);

            await using var connection = new MySqlConnection(connectionString);

            await connection.OpenAsync();

            await using var command = new MySqlCommand(
                "SELECT 1;",
                connection);

            var result = await command.ExecuteScalarAsync();

            return result is not null &&
                   Convert.ToInt32(result) == 1;
        }
        catch
        {
            return false;
        }
    }
}
