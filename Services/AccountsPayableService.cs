using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class AccountsPayableService
{
    public static async Task EnsureAccountsPayableTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS accounts_payable
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    PurchaseId INT NOT NULL UNIQUE,
    
    PurchaseNumber VARCHAR(50) NOT NULL,
    
    VendorId INT NOT NULL,
    
    VendorName VARCHAR(200) NOT NULL,
    
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    DueDate DATETIME NULL,
    
    DaysOverdue INT NOT NULL DEFAULT 0,
    
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    
    CreatedAt DATETIME NOT NULL,
    
    PRIMARY KEY (Id),
    
    INDEX IX_accounts_payable_PurchaseId (PurchaseId),
    INDEX IX_accounts_payable_VendorId (VendorId),
    INDEX IX_accounts_payable_Status (Status),
    INDEX IX_accounts_payable_DueDate (DueDate)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<AccountsPayable>> GetAllAsync()
    {
        var accounts = new List<AccountsPayable>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return accounts;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    PurchaseId,
    PurchaseNumber,
    VendorId,
    VendorName,
    TotalAmount,
    PaidAmount,
    Balance,
    DueDate,
    DaysOverdue,
    Status,
    CreatedAt
FROM accounts_payable
ORDER BY DueDate ASC;
";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            accounts.Add(new AccountsPayable
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PurchaseId = reader.GetInt32(reader.GetOrdinal("PurchaseId")),
                PurchaseNumber = reader.GetString(reader.GetOrdinal("PurchaseNumber")),
                VendorId = reader.GetInt32(reader.GetOrdinal("VendorId")),
                VendorName = reader.GetString(reader.GetOrdinal("VendorName")),
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

    public static async Task<bool> CreateAsync(AccountsPayable account)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
INSERT INTO accounts_payable
(
    PurchaseId,
    PurchaseNumber,
    VendorId,
    VendorName,
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
    @PurchaseId,
    @PurchaseNumber,
    @VendorId,
    @VendorName,
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
        command.Parameters.AddWithValue("@PurchaseId", account.PurchaseId);
        command.Parameters.AddWithValue("@PurchaseNumber", account.PurchaseNumber);
        command.Parameters.AddWithValue("@VendorId", account.VendorId);
        command.Parameters.AddWithValue("@VendorName", account.VendorName);
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

    public static async Task<bool> UpdatePaymentAsync(int purchaseId, decimal paymentAmount)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
UPDATE accounts_payable
SET
    PaidAmount = PaidAmount + @PaymentAmount,
    Balance = Balance - @PaymentAmount,
    Status = CASE 
        WHEN (Balance - @PaymentAmount) <= 0 THEN 'PAID'
        WHEN (Balance - @PaymentAmount) < TotalAmount THEN 'PARTIAL'
        ELSE 'PENDING'
    END
WHERE PurchaseId = @PurchaseId;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
        command.Parameters.AddWithValue("@PurchaseId", purchaseId);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
