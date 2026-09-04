using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class CompanyDatabaseService
{
    public static async Task<bool> EnsureDatabaseAsync(
        CompanyConnection company)
    {
        try
        {
            string databaseName =
                CompanyConnectionService.GenerateDatabaseName(
                    company.CompanyName);

            string serverConnection =
                $"Server={company.Server};" +
                $"Port={company.Port};" +
                $"User ID={company.Username};" +
                $"Password={company.Password};" +
                "SslMode=None;" +
                "AllowPublicKeyRetrieval=True;" +
                "ConnectionTimeout=5;";

            await using var connection =
                new MySqlConnection(serverConnection);

            await connection.OpenAsync();

            string safeDatabaseName =
                databaseName.Replace("`", "");

            string createDatabaseSql =
                $"CREATE DATABASE IF NOT EXISTS `{safeDatabaseName}` " +
                "CHARACTER SET utf8mb4 " +
                "COLLATE utf8mb4_unicode_ci;";

            await using (var command =
                new MySqlCommand(createDatabaseSql, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            company.DatabaseName = databaseName;

            return await EnsureTablesAsync(company);
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> EnsureTablesAsync(
        CompanyConnection company)
    {
        try
        {
            string databaseName =
                string.IsNullOrWhiteSpace(company.DatabaseName)
                    ? CompanyConnectionService.GenerateDatabaseName(
                        company.CompanyName)
                    : company.DatabaseName;

            var builder = new MySqlConnectionStringBuilder
            {
                Server = company.Server,
                Port = (uint)company.Port,
                Database = databaseName,
                UserID = company.Username,
                Password = company.Password,
                SslMode = MySqlSslMode.None,
                ConnectionTimeout = 5
            };

            await using var connection =
                new MySqlConnection(builder.ConnectionString);

            await connection.OpenAsync();

            const string sql = @"
CREATE TABLE IF NOT EXISTS sales
(
    Id INT NOT NULL AUTO_INCREMENT,
    InvoiceNumber VARCHAR(100) NOT NULL,
    SaleDate DATETIME NOT NULL,
    Status VARCHAR(30) NOT NULL DEFAULT 'ACTIVA',

    BillingCompanyId INT NOT NULL DEFAULT 0,
    BillingCompanyName VARCHAR(255) NOT NULL DEFAULT '',

    OperationalAreaId INT NOT NULL DEFAULT 0,
    OperationalAreaName VARCHAR(255) NOT NULL DEFAULT '',

    CustomerId INT NOT NULL DEFAULT 0,
    CustomerName VARCHAR(255) NOT NULL DEFAULT '',

    PrimaryVendorId INT NOT NULL DEFAULT 0,
    PrimaryVendorName VARCHAR(255) NOT NULL DEFAULT '',
    PrimaryCommissionPercentage DECIMAL(18,4) NOT NULL DEFAULT 0,
    PrimaryCommissionAmount DECIMAL(18,2) NOT NULL DEFAULT 0,

    IsSpecialSale BOOLEAN NOT NULL DEFAULT FALSE,

    SecondaryVendorId INT NULL,
    SecondaryVendorName VARCHAR(255) NOT NULL DEFAULT '',
    SecondaryCommissionPercentage DECIMAL(18,4) NOT NULL DEFAULT 0,
    SecondaryCommissionAmount DECIMAL(18,2) NOT NULL DEFAULT 0,

    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    CommissionBase DECIMAL(18,2) NOT NULL DEFAULT 0,
    TotalCommissionPercentage DECIMAL(18,4) NOT NULL DEFAULT 0,
    TotalCommissionAmount DECIMAL(18,2) NOT NULL DEFAULT 0,

    PaymentMethod VARCHAR(50) NOT NULL DEFAULT 'EFECTIVO',
    CreditDays INT NOT NULL DEFAULT 0,

    Notes VARCHAR(500) NOT NULL DEFAULT '',

    CreatedAt DATETIME NOT NULL,

    CreatedBy VARCHAR(255) NOT NULL DEFAULT '',

    PRIMARY KEY (Id),
    UNIQUE KEY UX_sales_InvoiceNumber (InvoiceNumber),
    INDEX IX_sales_SaleDate (SaleDate),
    INDEX IX_sales_CustomerId (CustomerId),
    INDEX IX_sales_PrimaryVendorId (PrimaryVendorId)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

            await using var command =
                new MySqlCommand(sql, connection);

            await command.ExecuteNonQueryAsync();

            // ------------------------------------------------
            // CUSTOMERS
            // ------------------------------------------------

            const string customersSql = @"
CREATE TABLE IF NOT EXISTS customers
(
    Id INT NOT NULL AUTO_INCREMENT,

    Name VARCHAR(255) NOT NULL DEFAULT '',
    Code VARCHAR(100) NOT NULL DEFAULT '',

    PaymentTerms VARCHAR(50) NOT NULL DEFAULT 'CONTADO',
    CreditDays INT NOT NULL DEFAULT 0,

    CreditLimit DECIMAL(18,2) NOT NULL DEFAULT 0,
    CurrentBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    PendingBalance DECIMAL(18,2) NOT NULL DEFAULT 0,

    PriceLevel VARCHAR(100) NOT NULL DEFAULT 'PRECIO DE VENTA 1',
    Salesperson VARCHAR(255) NOT NULL DEFAULT '',

    Phone VARCHAR(100) NOT NULL DEFAULT '',
    Email VARCHAR(255) NOT NULL DEFAULT '',
    Address VARCHAR(500) NOT NULL DEFAULT '',

    Rtn VARCHAR(100) NOT NULL DEFAULT '',

    ContactName VARCHAR(255) NOT NULL DEFAULT '',
    ContactPhone VARCHAR(100) NOT NULL DEFAULT '',
    ContactEmail VARCHAR(255) NOT NULL DEFAULT '',

    City VARCHAR(150) NOT NULL DEFAULT '',
    Country VARCHAR(150) NOT NULL DEFAULT 'HONDURAS',
    Department VARCHAR(150) NOT NULL DEFAULT '',

    Note VARCHAR(1000) NOT NULL DEFAULT '',

    IsActive BOOLEAN NOT NULL DEFAULT TRUE,

    PRIMARY KEY (Id),
    UNIQUE KEY UX_customers_Code (Code),
    INDEX IX_customers_Name (Name),
    INDEX IX_customers_Rtn (Rtn)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

            await using (var customersCommand =
                new MySqlCommand(customersSql, connection))
            {
                await customersCommand.ExecuteNonQueryAsync();
            }

            // ------------------------------------------------
            // VENDORS
            // ------------------------------------------------

            const string vendorsSql = @"
CREATE TABLE IF NOT EXISTS vendors
(
    Id INT NOT NULL AUTO_INCREMENT,

    Name VARCHAR(255) NOT NULL DEFAULT '',
    Code VARCHAR(100) NOT NULL DEFAULT '',

    CommissionPercentage DECIMAL(18,4) NOT NULL DEFAULT 3,

    Phone VARCHAR(100) NOT NULL DEFAULT '',
    Email VARCHAR(255) NOT NULL DEFAULT '',

    IdentityNumber VARCHAR(100) NOT NULL DEFAULT '',
    Address VARCHAR(500) NOT NULL DEFAULT '',

    EntryDate DATETIME NOT NULL,

    Note VARCHAR(1000) NOT NULL DEFAULT '',
    PhotoPath VARCHAR(1000) NOT NULL DEFAULT '',

    IsActive BOOLEAN NOT NULL DEFAULT TRUE,

    PRIMARY KEY (Id),
    UNIQUE KEY UX_vendors_Code (Code),
    INDEX IX_vendors_Name (Name),
    INDEX IX_vendors_IdentityNumber (IdentityNumber)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;
";

            await using (var vendorsCommand =
                new MySqlCommand(vendorsSql, connection))
            {
                await vendorsCommand.ExecuteNonQueryAsync();
            }

            // ------------------------------------------------
            // USERS
            // ------------------------------------------------

            await UserService.EnsureUsersTableAsync(connection);

            // Usuario inicial de cada empresa:
            // Usuario: admin
            // Contraseña: admin

            await UserService.EnsureDefaultAdminAsync(connection);

            return true;
        }
        catch
        {
            return false;
        }
    }
}


