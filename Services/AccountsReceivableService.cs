using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class AccountsReceivableService
{
    public static async Task EnsureAccountsReceivableTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS accounts_receivable
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    InvoiceId INT NOT NULL UNIQUE,
    
    InvoiceNumber VARCHAR(50) NOT NULL,
    
    CustomerId INT NOT NULL,
    
    CustomerName VARCHAR(200) NOT NULL,
    
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    DueDate DATETIME NULL,
    
    DaysOverdue INT NOT NULL DEFAULT 0,
    
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    
    CreatedAt DATETIME NOT NULL,
    
    PRIMARY KEY (Id),
    
    INDEX IX_accounts_receivable_InvoiceId (InvoiceId),
    INDEX IX_accounts_receivable_CustomerId (CustomerId),
    INDEX IX_accounts_receivable_Status (Status),
    INDEX IX_accounts_receivable_DueDate (DueDate)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<AccountsReceivable>> GetAllAsync()
    {
        var accounts = new List<AccountsReceivable>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return accounts;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    InvoiceId,
    InvoiceNumber,
    CustomerId,
    CustomerName,
    TotalAmount,
    PaidAmount,
    Balance,
    DueDate,
    DaysOverdue,
    Status,
    CreatedAt
FROM accounts_receivable
ORDER BY DueDate ASC;
";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            accounts.Add(new AccountsReceivable
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                InvoiceId = reader.GetInt32(reader.GetOrdinal("InvoiceId")),
                InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
                DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                DaysOverdue = reader.GetInt32(reader.GetOrdinal("DaysOverdue")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            });
        }

        return accounts;
    }

    public static async Task<bool> CreateAsync(AccountsReceivable account)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
INSERT INTO accounts_receivable
(
    InvoiceId,
    InvoiceNumber,
    CustomerId,
    CustomerName,
    TotalAmount,
    PaidAmount,
    Balance,
    DueDate,
    DaysOverdue,
    Status,
    CreatedAt
)
VALUES
(
    @InvoiceId,
    @InvoiceNumber,
    @CustomerId,
    @CustomerName,
    @TotalAmount,
    @PaidAmount,
    @Balance,
    @DueDate,
    @DaysOverdue,
    @Status,
    @CreatedAt
);
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@InvoiceId", account.InvoiceId);
        command.Parameters.AddWithValue("@InvoiceNumber", account.InvoiceNumber);
        command.Parameters.AddWithValue("@CustomerId", account.CustomerId);
        command.Parameters.AddWithValue("@CustomerName", account.CustomerName);
        command.Parameters.AddWithValue("@TotalAmount", account.TotalAmount);
        command.Parameters.AddWithValue("@PaidAmount", account.PaidAmount);
        command.Parameters.AddWithValue("@Balance", account.Balance);
        command.Parameters.AddWithValue("@DueDate", account.DueDate == DateTime.MinValue ? (object)DBNull.Value : account.DueDate);
        command.Parameters.AddWithValue("@DaysOverdue", account.DaysOverdue);
        command.Parameters.AddWithValue("@Status", account.Status);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static async Task<bool> UpdatePaymentAsync(int invoiceId, decimal paymentAmount)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
UPDATE accounts_receivable
SET
    PaidAmount = PaidAmount + @PaymentAmount,
    Balance = Balance - @PaymentAmount,
    Status = CASE 
        WHEN (Balance - @PaymentAmount) <= 0 THEN 'PAID'
        WHEN (Balance - @PaymentAmount) < TotalAmount THEN 'PARTIAL'
        ELSE 'PENDING'
    END
WHERE InvoiceId = @InvoiceId;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
        command.Parameters.AddWithValue("@InvoiceId", invoiceId);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
