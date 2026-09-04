using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class ProductService
{
    public static async Task EnsureProductsTableAsync(MySqlConnection connection)
    {
        const string sql = @"
CREATE TABLE IF NOT EXISTS products
(
    Id INT NOT NULL AUTO_INCREMENT,
    
    Code VARCHAR(50) NOT NULL UNIQUE,
    
    Name VARCHAR(200) NOT NULL,
    
    Description TEXT NULL,
    
    Category VARCHAR(100) NULL,
    
    Cost DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Price1 DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Price2 DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Price3 DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Price4 DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Stock DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    MinStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    MaxStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    Unit VARCHAR(50) NOT NULL DEFAULT 'UNIDAD',
    
    BarCode VARCHAR(50) NULL,
    
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    
    CreatedAt DATETIME NOT NULL,
    
    CreatedBy VARCHAR(100) NULL,
    
    PRIMARY KEY (Id),
    
    INDEX IX_products_Code (Code),
    INDEX IX_products_Category (Category),
    INDEX IX_products_IsActive (IsActive),
    INDEX IX_products_Stock (Stock)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

        await using var command = new MySqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public static async Task<List<Product>> GetAllAsync()
    {
        var products = new List<Product>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return products;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    Code,
    Name,
    Description,
    Category,
    Cost,
    Price1,
    Price2,
    Price3,
    Price4,
    Stock,
    MinStock,
    MaxStock,
    Unit,
    BarCode,
    IsActive,
    CreatedAt,
    CreatedBy
FROM products
ORDER BY Name;
";

        await using var command = new MySqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? string.Empty : reader.GetString(reader.GetOrdinal("Category")),
                Cost = reader.GetDecimal(reader.GetOrdinal("Cost")),
                Price1 = reader.GetDecimal(reader.GetOrdinal("Price1")),
                Price2 = reader.GetDecimal(reader.GetOrdinal("Price2")),
                Price3 = reader.GetDecimal(reader.GetOrdinal("Price3")),
                Price4 = reader.GetDecimal(reader.GetOrdinal("Price4")),
                Stock = reader.GetDecimal(reader.GetOrdinal("Stock")),
                MinStock = reader.GetDecimal(reader.GetOrdinal("MinStock")),
                MaxStock = reader.GetDecimal(reader.GetOrdinal("MaxStock")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                BarCode = reader.IsDBNull(reader.GetOrdinal("BarCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("BarCode")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
            });
        }

        return products;
    }

    public static async Task<Product?> GetByIdAsync(int id)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return null;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
SELECT
    Id,
    Code,
    Name,
    Description,
    Category,
    Cost,
    Price1,
    Price2,
    Price3,
    Price4,
    Stock,
    MinStock,
    MaxStock,
    Unit,
    BarCode,
    IsActive,
    CreatedAt,
    CreatedBy
FROM products
WHERE Id = @Id
LIMIT 1;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new Product
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Code = reader.GetString(reader.GetOrdinal("Code")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
            Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? string.Empty : reader.GetString(reader.GetOrdinal("Category")),
            Cost = reader.GetDecimal(reader.GetOrdinal("Cost")),
            Price1 = reader.GetDecimal(reader.GetOrdinal("Price1")),
            Price2 = reader.GetDecimal(reader.GetOrdinal("Price2")),
            Price3 = reader.GetDecimal(reader.GetOrdinal("Price3")),
            Price4 = reader.GetDecimal(reader.GetOrdinal("Price4")),
            Stock = reader.GetDecimal(reader.GetOrdinal("Stock")),
            MinStock = reader.GetDecimal(reader.GetOrdinal("MinStock")),
            MaxStock = reader.GetDecimal(reader.GetOrdinal("MaxStock")),
            Unit = reader.GetString(reader.GetOrdinal("Unit")),
            BarCode = reader.IsDBNull(reader.GetOrdinal("BarCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("BarCode")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
        };
    }

    public static async Task<bool> CreateAsync(Product product)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
INSERT INTO products
(
    Code,
    Name,
    Description,
    Category,
    Cost,
    Price1,
    Price2,
    Price3,
    Price4,
    Stock,
    MinStock,
    MaxStock,
    Unit,
    BarCode,
    IsActive,
    CreatedAt,
    CreatedBy
)
VALUES
(
    @Code,
    @Name,
    @Description,
    @Category,
    @Cost,
    @Price1,
    @Price2,
    @Price3,
    @Price4,
    @Stock,
    @MinStock,
    @MaxStock,
    @Unit,
    @BarCode,
    @IsActive,
    @CreatedAt,
    @CreatedBy
);
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Code", product.Code);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Description", product.Description);
        command.Parameters.AddWithValue("@Category", product.Category);
        command.Parameters.AddWithValue("@Cost", product.Cost);
        command.Parameters.AddWithValue("@Price1", product.Price1);
        command.Parameters.AddWithValue("@Price2", product.Price2);
        command.Parameters.AddWithValue("@Price3", product.Price3);
        command.Parameters.AddWithValue("@Price4", product.Price4);
        command.Parameters.AddWithValue("@Stock", product.Stock);
        command.Parameters.AddWithValue("@MinStock", product.MinStock);
        command.Parameters.AddWithValue("@MaxStock", product.MaxStock);
        command.Parameters.AddWithValue("@Unit", product.Unit);
        command.Parameters.AddWithValue("@BarCode", product.BarCode);
        command.Parameters.AddWithValue("@IsActive", product.IsActive);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        command.Parameters.AddWithValue("@CreatedBy", product.CreatedBy);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static async Task<bool> UpdateAsync(Product product)
    {
        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
UPDATE products
SET
    Name = @Name,
    Description = @Description,
    Category = @Category,
    Cost = @Cost,
    Price1 = @Price1,
    Price2 = @Price2,
    Price3 = @Price3,
    Price4 = @Price4,
    Stock = @Stock,
    MinStock = @MinStock,
    MaxStock = @MaxStock,
    Unit = @Unit,
    BarCode = @BarCode,
    IsActive = @IsActive
WHERE Id = @Id;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Description", product.Description);
        command.Parameters.AddWithValue("@Category", product.Category);
        command.Parameters.AddWithValue("@Cost", product.Cost);
        command.Parameters.AddWithValue("@Price1", product.Price1);
        command.Parameters.AddWithValue("@Price2", product.Price2);
        command.Parameters.AddWithValue("@Price3", product.Price3);
        command.Parameters.AddWithValue("@Price4", product.Price4);
        command.Parameters.AddWithValue("@Stock", product.Stock);
        command.Parameters.AddWithValue("@MinStock", product.MinStock);
        command.Parameters.AddWithValue("@MaxStock", product.MaxStock);
        command.Parameters.AddWithValue("@Unit", product.Unit);
        command.Parameters.AddWithValue("@BarCode", product.BarCode);
        command.Parameters.AddWithValue("@IsActive", product.IsActive);
        command.Parameters.AddWithValue("@Id", product.Id);

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
DELETE FROM products
WHERE Id = @Id;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public static string GenerateNextCode()
    {
        var products = GetAllAsync().Result;
        if (!products.Any())
            return "PROD0001";

        var lastCode = products.OrderByDescending(p => p.Code).First().Code;
        if (lastCode.StartsWith("PROD"))
        {
            var number = int.Parse(lastCode.Substring(4));
            return $"PROD{number + 1:D4}";
        }

        return "PROD0001";
    }
}
