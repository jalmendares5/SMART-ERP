using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class UserService
{
    public static async Task EnsureUsersTableAsync(
        MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS users
(
    Id INT NOT NULL AUTO_INCREMENT,

    Username VARCHAR(100) NOT NULL,

    Password VARCHAR(255) NOT NULL,

    FullName VARCHAR(255) NOT NULL DEFAULT '',

    Role VARCHAR(50) NOT NULL DEFAULT 'Vendedor',

    IsActive BOOLEAN NOT NULL DEFAULT TRUE,

    CreatedAt DATETIME NOT NULL,

    PRIMARY KEY (Id),

    UNIQUE KEY UX_users_Username (Username),

    INDEX IX_users_IsActive (IsActive)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command =
            new MySqlCommand(sql, connection);

        await command.ExecuteNonQueryAsync();
    }

    public static async Task EnsureDefaultAdminAsync(
        MySqlConnection connection)
    {
        const string checkSql = @"
SELECT COUNT(*)
FROM users
WHERE Username = 'admin';
";

        await using var checkCommand =
            new MySqlCommand(checkSql, connection);

        var result = await checkCommand.ExecuteScalarAsync();

        int count = Convert.ToInt32(result);

        if (count > 0)
            return;

        const string insertSql = @"
INSERT INTO users
(
    Username,
    Password,
    FullName,
    Role,
    IsActive,
    CreatedAt
)
VALUES
(
    'admin',
    'admin',
    'Administrador',
    'Admin',
    TRUE,
    NOW()
);
";

        await using var insertCommand =
            new MySqlCommand(insertSql, connection);

        await insertCommand.ExecuteNonQueryAsync();
    }

    public static async Task<User?> AuthenticateAsync(
        string username,
        string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        string? connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return null;

        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    Username,
    Password,
    FullName,
    Role,
    IsActive,
    CreatedAt
FROM users
WHERE LOWER(Username) = LOWER(@Username)
  AND LOWER(Password) = LOWER(@Password)
LIMIT 1;
";

        await using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@Username",
            username.Trim());

        command.Parameters.AddWithValue(
            "@Password",
            password);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new User
        {
            Id = reader.GetInt32("Id"),
            Username = reader.GetString("Username"),
            Password = reader.GetString("Password"),
            FullName = reader.GetString("FullName"),
            Role = reader.GetString("Role"),
            IsActive = reader.GetBoolean("IsActive"),
            CreatedAt = reader.GetDateTime("CreatedAt")
        };
    }

    public static async Task<List<User>> GetAllAsync()
    {
        var users = new List<User>();

        string? connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return users;

        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    Username,
    Password,
    FullName,
    Role,
    IsActive,
    CreatedAt
FROM users
ORDER BY Username;
";

        await using var command =
            new MySqlCommand(sql, connection);

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32("Id"),
                Username = reader.GetString("Username"),
                Password = reader.GetString("Password"),
                FullName = reader.GetString("FullName"),
                Role = reader.GetString("Role"),
                IsActive = reader.GetBoolean("IsActive"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            });
        }

        return users;
    }

    public static async Task<User?> GetByIdAsync(int id)
    {
        string? connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return null;

        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    Username,
    Password,
    FullName,
    Role,
    IsActive,
    CreatedAt
FROM users
WHERE Id = @Id
LIMIT 1;
";

        await using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new User
        {
            Id = reader.GetInt32("Id"),
            Username = reader.GetString("Username"),
            Password = reader.GetString("Password"),
            FullName = reader.GetString("FullName"),
            Role = reader.GetString("Role"),
            IsActive = reader.GetBoolean("IsActive"),
            CreatedAt = reader.GetDateTime("CreatedAt")
        };
    }

    public static async Task<bool> CreateAsync(User user)
    {
        string? connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql = @"
INSERT INTO users
(
    Username,
    Password,
    FullName,
    Role,
    IsActive,
    CreatedAt
)
VALUES
(
    @Username,
    @Password,
    @FullName,
    @Role,
    @IsActive,
    @CreatedAt
);
";

        await using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@Password", user.Password);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Role", user.Role);
        command.Parameters.AddWithValue("@IsActive", user.IsActive);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static async Task<bool> UpdateAsync(User user)
    {
        string? connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql = @"
UPDATE users
SET
    Username = @Username,
    Password = @Password,
    FullName = @FullName,
    Role = @Role,
    IsActive = @IsActive
WHERE Id = @Id;
";

        await using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Username", user.Username);
        command.Parameters.AddWithValue("@Password", user.Password);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Role", user.Role);
        command.Parameters.AddWithValue("@IsActive", user.IsActive);
        command.Parameters.AddWithValue("@Id", user.Id);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static async Task<bool> DeleteAsync(int id)
    {
        string? connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection =
            new MySqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql = @"
DELETE FROM users
WHERE Id = @Id;
";

        await using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", id);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
