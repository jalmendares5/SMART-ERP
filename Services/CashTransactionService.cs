using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class CashTransactionService
{
    public static async Task EnsureCashTransactionsTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS cash_transactions
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    TransactionNumber VARCHAR(50) NOT NULL UNIQUE,
    
    TransactionDate DATETIME NOT NULL,
    
    TransactionType VARCHAR(10) NOT NULL DEFAULT 'IN',
    
    Category VARCHAR(100) NULL,
    
    Description TEXT NULL,
    
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    ReferenceType VARCHAR(50) NULL,
    
    ReferenceId INT NULL,
    
    ReferenceNumber VARCHAR(50) NULL,
    
    Notes TEXT NULL,
    
    CreatedAt DATETIME NOT NULL,
    
    CreatedBy VARCHAR(100) NULL,
    
    PRIMARY KEY (Id),
    
    INDEX IX_cash_transactions_TransactionNumber (TransactionNumber),
    INDEX IX_cash_transactions_TransactionDate (TransactionDate),
    INDEX IX_cash_transactions_TransactionType (TransactionType),
    INDEX IX_cash_transactions_Category (Category)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<CashTransaction>> GetAllAsync()
    {
        var transactions = new List<CashTransaction>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return transactions;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    TransactionNumber,
    TransactionDate,
    TransactionType,
    Category,
    Description,
    Amount,
    ReferenceType,
    ReferenceId,
    ReferenceNumber,
    Notes,
    CreatedAt,
    CreatedBy
FROM cash_transactions
ORDER BY TransactionDate DESC;
";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            transactions.Add(new CashTransaction
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TransactionNumber = reader.GetString(reader.GetOrdinal("TransactionNumber")),
                TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                TransactionType = reader.GetString(reader.GetOrdinal("TransactionType")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? string.Empty : reader.GetString(reader.GetOrdinal("Category")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                ReferenceType = reader.IsDBNull(reader.GetOrdinal("ReferenceType")) ? string.Empty : reader.GetString(reader.GetOrdinal("ReferenceType")),
                ReferenceId = reader.IsDBNull(reader.GetOrdinal("ReferenceId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ReferenceId")),
                ReferenceNumber = reader.IsDBNull(reader.GetOrdinal("ReferenceNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("ReferenceNumber")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
            });
        }

        return transactions;
    }

    public static async Task<bool> CreateAsync(CashTransaction transaction)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
INSERT INTO cash_transactions
(
    TransactionNumber,
    TransactionDate,
    TransactionType,
    Category,
    Description,
    Amount,
    ReferenceType,
    ReferenceId,
    ReferenceNumber,
    Notes,
    CreatedAt,
    CreatedBy
)
VALUES
(
    @TransactionNumber,
    @TransactionDate,
    @TransactionType,
    @Category,
    @Description,
    @Amount,
    @ReferenceType,
    @ReferenceId,
    @ReferenceNumber,
    @Notes,
    @CreatedAt,
    @CreatedBy
);
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TransactionNumber", transaction.TransactionNumber);
        command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
        command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
        command.Parameters.AddWithValue("@Category", transaction.Category);
        command.Parameters.AddWithValue("@Description", transaction.Description);
        command.Parameters.AddWithValue("@Amount", transaction.Amount);
        command.Parameters.AddWithValue("@ReferenceType", transaction.ReferenceType);
        command.Parameters.AddWithValue("@ReferenceId", transaction.ReferenceId);
        command.Parameters.AddWithValue("@ReferenceNumber", transaction.ReferenceNumber);
        command.Parameters.AddWithValue("@Notes", transaction.Notes);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        command.Parameters.AddWithValue("@CreatedBy", transaction.CreatedBy);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static async Task<decimal> GetBalanceAsync()
    {
        var transactions = await GetAllAsync();
        var income = transactions.Where(t => t.TransactionType == "IN").Sum(t => t.Amount);
        var expense = transactions.Where(t => t.TransactionType == "OUT").Sum(t => t.Amount);
        return income - expense;
    }

    public static string GenerateNextTransactionNumber()
    {
        var transactions = GetAllAsync().Result;
        if (!transactions.Any())
            return "CAJA-0001";

        var lastNumber = transactions.OrderByDescending(t => t.TransactionNumber).First().TransactionNumber;
        if (lastNumber.StartsWith("CAJA-"))
        {
            var number = int.Parse(lastNumber.Substring(5));
            return $"CAJA-{number + 1:D4}";
        }

        return "CAJA-0001";
    }
}
