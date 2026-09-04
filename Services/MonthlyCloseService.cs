using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class MonthlyCloseService
{
    public static async Task EnsureMonthlyClosesTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS monthly_closes
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    Year INT NOT NULL,
    
    Month INT NOT NULL,
    
    TotalSales DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    TotalQuantity DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    TotalCommission DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    IsClosed BOOLEAN NOT NULL DEFAULT FALSE,
    
    ClosedAt DATETIME NULL,
    
    ClosedBy VARCHAR(100) NULL,
    
    Notes TEXT NULL,
    
    CreatedAt DATETIME NOT NULL,
    
    PRIMARY KEY (Id),
    
    UNIQUE KEY UX_monthly_closes_Year_Month (Year, Month),
    
    INDEX IX_monthly_closes_IsClosed (IsClosed)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<MonthlyClose>> GetAllAsync()
    {
        var closes = new List<MonthlyClose>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return closes;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    Year,
    Month,
    TotalSales,
    TotalQuantity,
    TotalCommission,
    IsClosed,
    ClosedAt,
    ClosedBy,
    Notes,
    CreatedAt
FROM monthly_closes
ORDER BY Year DESC, Month DESC;
";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            closes.Add(new MonthlyClose
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                Month = reader.GetInt32(reader.GetOrdinal("Month")),
                TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalSales")),
                TotalQuantity = reader.GetDecimal(reader.GetOrdinal("TotalQuantity")),
                TotalCommission = reader.GetDecimal(reader.GetOrdinal("TotalCommission")),
                IsClosed = reader.GetBoolean(reader.GetOrdinal("IsClosed")),
                ClosedAt = reader.IsDBNull(reader.GetOrdinal("ClosedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ClosedAt")),
                ClosedBy = reader.IsDBNull(reader.GetOrdinal("ClosedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("ClosedBy")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return closes;
    }

    public static async Task<MonthlyClose?> GetByYearMonthAsync(int year, int month)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return null;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    Year,
    Month,
    TotalSales,
    TotalQuantity,
    TotalCommission,
    IsClosed,
    ClosedAt,
    ClosedBy,
    Notes,
    CreatedAt
FROM monthly_closes
WHERE Year = @Year AND Month = @Month
LIMIT 1;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Year", year);
        command.Parameters.AddWithValue("@Month", month);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new MonthlyClose
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Year = reader.GetInt32(reader.GetOrdinal("Year")),
            Month = reader.GetInt32(reader.GetOrdinal("Month")),
            TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalSales")),
            TotalQuantity = reader.GetDecimal(reader.GetOrdinal("TotalQuantity")),
            TotalCommission = reader.GetDecimal(reader.GetOrdinal("TotalCommission")),
            IsClosed = reader.GetBoolean(reader.GetOrdinal("IsClosed")),
            ClosedAt = reader.IsDBNull(reader.GetOrdinal("ClosedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ClosedAt")),
            ClosedBy = reader.IsDBNull(reader.GetOrdinal("ClosedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("ClosedBy")),
            Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }

    public static async Task<bool> CreateAsync(MonthlyClose monthlyClose)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
INSERT INTO monthly_closes
(
    Year,
    Month,
    TotalSales,
    TotalQuantity,
    TotalCommission,
    IsClosed,
    ClosedAt,
    ClosedBy,
    Notes,
    CreatedAt
)
VALUES
(
    @Year,
    @Month,
    @TotalSales,
    @TotalQuantity,
    @TotalCommission,
    @IsClosed,
    @ClosedAt,
    @ClosedBy,
    @Notes,
    @CreatedAt
);
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Year", monthlyClose.Year);
        command.Parameters.AddWithValue("@Month", monthlyClose.Month);
        command.Parameters.AddWithValue("@TotalSales", monthlyClose.TotalSales);
        command.Parameters.AddWithValue("@TotalQuantity", monthlyClose.TotalQuantity);
        command.Parameters.AddWithValue("@TotalCommission", monthlyClose.TotalCommission);
        command.Parameters.AddWithValue("@IsClosed", monthlyClose.IsClosed);
        command.Parameters.AddWithValue("@ClosedAt", monthlyClose.ClosedAt == DateTime.MinValue ? (object)DBNull.Value : monthlyClose.ClosedAt);
        command.Parameters.AddWithValue("@ClosedBy", monthlyClose.ClosedBy);
        command.Parameters.AddWithValue("@Notes", monthlyClose.Notes);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static async Task<bool> UpdateAsync(MonthlyClose monthlyClose)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
UPDATE monthly_closes
SET
    TotalSales = @TotalSales,
    TotalQuantity = @TotalQuantity,
    TotalCommission = @TotalCommission,
    IsClosed = @IsClosed,
    ClosedAt = @ClosedAt,
    ClosedBy = @ClosedBy,
    Notes = @Notes
WHERE Id = @Id;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TotalSales", monthlyClose.TotalSales);
        command.Parameters.AddWithValue("@TotalQuantity", monthlyClose.TotalQuantity);
        command.Parameters.AddWithValue("@TotalCommission", monthlyClose.TotalCommission);
        command.Parameters.AddWithValue("@IsClosed", monthlyClose.IsClosed);
        command.Parameters.AddWithValue("@ClosedAt", monthlyClose.ClosedAt == DateTime.MinValue ? (object)DBNull.Value : monthlyClose.ClosedAt);
        command.Parameters.AddWithValue("@ClosedBy", monthlyClose.ClosedBy);
        command.Parameters.AddWithValue("@Notes", monthlyClose.Notes);
        command.Parameters.AddWithValue("@Id", monthlyClose.Id);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static async Task<bool> DeleteAsync(int id)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
DELETE FROM monthly_closes
WHERE Id = @Id;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
