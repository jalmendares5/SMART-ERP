using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class InvoiceService
{
    public static async Task EnsureInvoicesTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS invoices
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    InvoiceNumber VARCHAR(50) NOT NULL UNIQUE,
    
    InvoiceDate DATETIME NOT NULL,
    
    CustomerId INT NOT NULL,
    
    CustomerName VARCHAR(200) NOT NULL,
    
    Salesperson VARCHAR(100) NULL,
    
    PaymentTerms VARCHAR(50) NOT NULL DEFAULT 'CONTADO',
    
    CreditDays INT NOT NULL DEFAULT 0,
    
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
    
    INDEX IX_invoices_InvoiceNumber (InvoiceNumber),
    INDEX IX_invoices_CustomerId (CustomerId),
    INDEX IX_invoices_InvoiceDate (InvoiceDate),
    INDEX IX_invoices_Status (Status)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task EnsureInvoiceItemsTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS invoice_items
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    InvoiceId INT NOT NULL,
    
    ProductId INT NOT NULL,
    
    ProductCode VARCHAR(50) NOT NULL,
    
    ProductName VARCHAR(200) NOT NULL,
    
    Quantity DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    PRIMARY KEY (Id),
    
    INDEX IX_invoice_items_InvoiceId (InvoiceId),
    INDEX IX_invoice_items_ProductId (ProductId),
    
    FOREIGN KEY (InvoiceId) REFERENCES invoices(Id) ON DELETE CASCADE
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<Invoice>> GetAllAsync()
    {
        var invoices = new List<Invoice>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return invoices;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    InvoiceNumber,
    InvoiceDate,
    CustomerId,
    CustomerName,
    Salesperson,
    PaymentTerms,
    CreditDays,
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
FROM invoices
ORDER BY InvoiceDate DESC;
";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            invoices.Add(new Invoice
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                InvoiceNumber = reader.GetString(reader.GetOrdinal("InvoiceNumber")),
                InvoiceDate = reader.GetDateTime(reader.GetOrdinal("InvoiceDate")),
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                Salesperson = reader.IsDBNull(reader.GetOrdinal("Salesperson")) ? string.Empty : reader.GetString(reader.GetOrdinal("Salesperson")),
                PaymentTerms = reader.GetString(reader.GetOrdinal("PaymentTerms")),
                CreditDays = reader.GetInt32(reader.GetOrdinal("CreditDays")),
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

        return invoices;
    }

    public static async Task<bool> CreateAsync(Invoice invoice)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
INSERT INTO invoices
(
    InvoiceNumber,
    InvoiceDate,
    CustomerId,
    CustomerName,
    Salesperson,
    PaymentTerms,
    CreditDays,
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
    @InvoiceNumber,
    @InvoiceDate,
    @CustomerId,
    @CustomerName,
    @Salesperson,
    @PaymentTerms,
    @CreditDays,
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
        command.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
        command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
        command.Parameters.AddWithValue("@CustomerId", invoice.CustomerId);
        command.Parameters.AddWithValue("@CustomerName", invoice.CustomerName);
        command.Parameters.AddWithValue("@Salesperson", invoice.Salesperson);
        command.Parameters.AddWithValue("@PaymentTerms", invoice.PaymentTerms);
        command.Parameters.AddWithValue("@CreditDays", invoice.CreditDays);
        command.Parameters.AddWithValue("@Subtotal", invoice.Subtotal);
        command.Parameters.AddWithValue("@Tax", invoice.Tax);
        command.Parameters.AddWithValue("@Discount", invoice.Discount);
        command.Parameters.AddWithValue("@Total", invoice.Total);
        command.Parameters.AddWithValue("@PaidAmount", invoice.PaidAmount);
        command.Parameters.AddWithValue("@Balance", invoice.Balance);
        command.Parameters.AddWithValue("@Status", invoice.Status);
        command.Parameters.AddWithValue("@Notes", invoice.Notes);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        command.Parameters.AddWithValue("@CreatedBy", invoice.CreatedBy);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static string GenerateNextInvoiceNumber()
    {
        var invoices = GetAllAsync().Result;
        if (!invoices.Any())
            return "FAC-0001";

        var lastNumber = invoices.OrderByDescending(i => i.InvoiceNumber).First().InvoiceNumber;
        if (lastNumber.StartsWith("FAC-"))
        {
            var number = int.Parse(lastNumber.Substring(4));
            return $"FAC-{number + 1:D4}";
        }

        return "FAC-0001";
    }
}
