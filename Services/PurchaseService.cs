using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class PurchaseService
{
    public static async Task EnsurePurchasesTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS purchases
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    PurchaseNumber VARCHAR(50) NOT NULL UNIQUE,
    
    PurchaseDate DATETIME NOT NULL,
    
    VendorId INT NOT NULL,
    
    VendorName VARCHAR(200) NOT NULL,
    
    Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Tax DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    
    Notes TEXT NULL,
    
    CreatedAt DATETIME NOT NULL,
    
    CreatedBy VARCHAR(100) NULL,
    
    PRIMARY KEY (Id),
    
    INDEX IX_purchases_PurchaseNumber (PurchaseNumber),
    INDEX IX_purchases_VendorId (VendorId),
    INDEX IX_purchases_PurchaseDate (PurchaseDate),
    INDEX IX_purchases_Status (Status)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task EnsurePurchaseItemsTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS purchase_items
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    PurchaseId INT NOT NULL,
    
    ProductId INT NOT NULL,
    
    ProductCode VARCHAR(50) NOT NULL,
    
    ProductName VARCHAR(200) NOT NULL,
    
    Quantity DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Cost DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    PRIMARY KEY (Id),
    
    INDEX IX_purchase_items_PurchaseId (PurchaseId),
    INDEX IX_purchase_items_ProductId (ProductId),
    
    FOREIGN KEY (PurchaseId) REFERENCES purchases(Id) ON DELETE CASCADE
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<Purchase>> GetAllAsync()
    {
        var purchases = new List<Purchase>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return purchases;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    PurchaseNumber,
    PurchaseDate,
    VendorId,
    VendorName,
    Subtotal,
    Tax,
    Discount,
    Total,
    PaidAmount,
    Balance,
    Status,
    Notes,
    CreatedAt,
    CreatedBy
FROM purchases
ORDER BY PurchaseDate DESC;
";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            purchases.Add(new Purchase
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PurchaseNumber = reader.GetString(reader.GetOrdinal("PurchaseNumber")),
                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                VendorId = reader.GetInt32(reader.GetOrdinal("VendorId")),
                VendorName = reader.GetString(reader.GetOrdinal("VendorName")),
                Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                Tax = reader.GetDecimal(reader.GetOrdinal("Tax")),
                Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                PaidAmount = reader.GetDecimal(reader.GetOrdinal("PaidAmount")),
                Balance = reader.GetDecimal(reader.GetOrdinal("Balance")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? string.Empty : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
            });
        }

        return purchases;
    }

    public static async Task<bool> CreateAsync(Purchase purchase)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
INSERT INTO purchases
(
    PurchaseNumber,
    PurchaseDate,
    VendorId,
    VendorName,
    Subtotal,
    Tax,
    Discount,
    Total,
    PaidAmount,
    Balance,
    Status,
    Notes,
    CreatedAt,
    CreatedBy
)
VALUES
(
    @PurchaseNumber,
    @PurchaseDate,
    @VendorId,
    @VendorName,
    @Subtotal,
    @Tax,
    @Discount,
    @Total,
    @PaidAmount,
    @Balance,
    @Status,
    @Notes,
    @CreatedAt,
    @CreatedBy
);
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@PurchaseNumber", purchase.PurchaseNumber);
        command.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
        command.Parameters.AddWithValue("@VendorId", purchase.VendorId);
        command.Parameters.AddWithValue("@VendorName", purchase.VendorName);
        command.Parameters.AddWithValue("@Subtotal", purchase.Subtotal);
        command.Parameters.AddWithValue("@Tax", purchase.Tax);
        command.Parameters.AddWithValue("@Discount", purchase.Discount);
        command.Parameters.AddWithValue("@Total", purchase.Total);
        command.Parameters.AddWithValue("@PaidAmount", purchase.PaidAmount);
        command.Parameters.AddWithValue("@Balance", purchase.Balance);
        command.Parameters.AddWithValue("@Status", purchase.Status);
        command.Parameters.AddWithValue("@Notes", purchase.Notes);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        command.Parameters.AddWithValue("@CreatedBy", purchase.CreatedBy);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static string GenerateNextPurchaseNumber()
    {
        var purchases = GetAllAsync().Result;
        if (!purchases.Any())
            return "COMP-0001";

        var lastNumber = purchases.OrderByDescending(p => p.PurchaseNumber).First().PurchaseNumber;
        if (lastNumber.StartsWith("COMP-"))
        {
            var number = int.Parse(lastNumber.Substring(5));
            return $"COMP-{number + 1:D4}";
        }

        return "COMP-0001";
    }
}
