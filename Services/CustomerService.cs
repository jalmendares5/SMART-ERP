using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class CustomerService
{
    public static List<Customer> GetAll()
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<Customer>();

        var customers = new List<Customer>();

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            const string sql = @"
SELECT
    Id,
    Name,
    Code,
    PaymentTerms,
    CreditDays,
    CreditLimit,
    CurrentBalance,
    PendingBalance,
    PriceLevel,
    Salesperson,
    Phone,
    Email,
    Address,
    Rtn,
    ContactName,
    ContactPhone,
    ContactEmail,
    City,
    Country,
    Department,
    Note,
    IsActive
FROM customers
ORDER BY Name;";

            using var command =
                new MySqlCommand(sql, connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(MapCustomer(reader));
            }
        }
        catch
        {
            return new List<Customer>();
        }

        return customers;
    }

    public static List<Customer> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return GetAll();

        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<Customer>();

        var customers = new List<Customer>();

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            const string sql = @"
SELECT
    Id,
    Name,
    Code,
    PaymentTerms,
    CreditDays,
    CreditLimit,
    CurrentBalance,
    PendingBalance,
    PriceLevel,
    Salesperson,
    Phone,
    Email,
    Address,
    Rtn,
    ContactName,
    ContactPhone,
    ContactEmail,
    City,
    Country,
    Department,
    Note,
    IsActive
FROM customers
WHERE
    Name LIKE @Term
    OR Code LIKE @Term
    OR Phone LIKE @Term
    OR Email LIKE @Term
    OR Rtn LIKE @Term
ORDER BY Name;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@Term",
                "%" + term + "%");

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(MapCustomer(reader));
            }
        }
        catch
        {
            return new List<Customer>();
        }

        return customers;
    }

    public static string GenerateNextCode()
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return "C001";

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            const string sql = @"
SELECT COALESCE(
    MAX(
        CAST(
            NULLIF(
                REGEXP_REPLACE(Code, '[^0-9]', ''),
                ''
            ) AS UNSIGNED
        )
    ),
    0
)
FROM customers;";

            using var command =
                new MySqlCommand(sql, connection);

            var result =
                Convert.ToInt32(command.ExecuteScalar());

            return $"C{result + 1:000}";
        }
        catch
        {
            return "C001";
        }
    }

    public static void Save(Customer customer)
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            if (customer.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(customer.Code))
                    customer.Code = GenerateNextCode();

                const string sql = @"
INSERT INTO customers
(
    Name,
    Code,
    PaymentTerms,
    CreditDays,
    CreditLimit,
    CurrentBalance,
    PendingBalance,
    PriceLevel,
    Salesperson,
    Phone,
    Email,
    Address,
    Rtn,
    ContactName,
    ContactPhone,
    ContactEmail,
    City,
    Country,
    Department,
    Note,
    IsActive
)
VALUES
(
    @Name,
    @Code,
    @PaymentTerms,
    @CreditDays,
    @CreditLimit,
    @CurrentBalance,
    @PendingBalance,
    @PriceLevel,
    @Salesperson,
    @Phone,
    @Email,
    @Address,
    @Rtn,
    @ContactName,
    @ContactPhone,
    @ContactEmail,
    @City,
    @Country,
    @Department,
    @Note,
    @IsActive
);

SELECT LAST_INSERT_ID();";

                using var command =
                    new MySqlCommand(sql, connection);

                AddParameters(command, customer);

                customer.Id =
                    Convert.ToInt32(command.ExecuteScalar());

                return;
            }

            const string updateSql = @"
UPDATE customers SET
    Name = @Name,
    Code = @Code,
    PaymentTerms = @PaymentTerms,
    CreditDays = @CreditDays,
    CreditLimit = @CreditLimit,
    CurrentBalance = @CurrentBalance,
    PendingBalance = @PendingBalance,
    PriceLevel = @PriceLevel,
    Salesperson = @Salesperson,
    Phone = @Phone,
    Email = @Email,
    Address = @Address,
    Rtn = @Rtn,
    ContactName = @ContactName,
    ContactPhone = @ContactPhone,
    ContactEmail = @ContactEmail,
    City = @City,
    Country = @Country,
    Department = @Department,
    Note = @Note,
    IsActive = @IsActive
WHERE Id = @Id;";

            using var updateCommand =
                new MySqlCommand(updateSql, connection);

            AddParameters(updateCommand, customer);

            updateCommand.Parameters.AddWithValue(
                "@Id",
                customer.Id);

            updateCommand.ExecuteNonQuery();
        }
        catch
        {
        }
    }

    public static void Delete(int customerId)
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            const string sql =
                "DELETE FROM customers WHERE Id = @Id;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@Id",
                customerId);

            command.ExecuteNonQuery();
        }
        catch
        {
        }
    }

    private static void AddParameters(
        MySqlCommand command,
        Customer customer)
    {
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@Code", customer.Code);
        command.Parameters.AddWithValue("@PaymentTerms", customer.PaymentTerms);
        command.Parameters.AddWithValue("@CreditDays", customer.CreditDays);
        command.Parameters.AddWithValue("@CreditLimit", customer.CreditLimit);
        command.Parameters.AddWithValue("@CurrentBalance", customer.CurrentBalance);
        command.Parameters.AddWithValue("@PendingBalance", customer.PendingBalance);
        command.Parameters.AddWithValue("@PriceLevel", customer.PriceLevel);
        command.Parameters.AddWithValue("@Salesperson", customer.Salesperson);
        command.Parameters.AddWithValue("@Phone", customer.Phone);
        command.Parameters.AddWithValue("@Email", customer.Email);
        command.Parameters.AddWithValue("@Address", customer.Address);
        command.Parameters.AddWithValue("@Rtn", customer.Rtn);
        command.Parameters.AddWithValue("@ContactName", customer.ContactName);
        command.Parameters.AddWithValue("@ContactPhone", customer.ContactPhone);
        command.Parameters.AddWithValue("@ContactEmail", customer.ContactEmail);
        command.Parameters.AddWithValue("@City", customer.City);
        command.Parameters.AddWithValue("@Country", customer.Country);
        command.Parameters.AddWithValue("@Department", customer.Department);
        command.Parameters.AddWithValue("@Note", customer.Note);
        command.Parameters.AddWithValue("@IsActive", customer.IsActive);
    }

    private static Customer MapCustomer(
        MySqlDataReader reader)
    {
        return new Customer
        {
            Id = Convert.ToInt32(reader["Id"]),
            Name = reader["Name"]?.ToString() ?? "",
            Code = reader["Code"]?.ToString() ?? "",
            PaymentTerms = reader["PaymentTerms"]?.ToString() ?? "CONTADO",
            CreditDays = Convert.ToInt32(reader["CreditDays"]),
            CreditLimit = Convert.ToDecimal(reader["CreditLimit"]),
            CurrentBalance = Convert.ToDecimal(reader["CurrentBalance"]),
            PendingBalance = Convert.ToDecimal(reader["PendingBalance"]),
            PriceLevel = reader["PriceLevel"]?.ToString() ?? "PRECIO DE VENTA 1",
            Salesperson = reader["Salesperson"]?.ToString() ?? "",
            Phone = reader["Phone"]?.ToString() ?? "",
            Email = reader["Email"]?.ToString() ?? "",
            Address = reader["Address"]?.ToString() ?? "",
            Rtn = reader["Rtn"]?.ToString() ?? "",
            ContactName = reader["ContactName"]?.ToString() ?? "",
            ContactPhone = reader["ContactPhone"]?.ToString() ?? "",
            ContactEmail = reader["ContactEmail"]?.ToString() ?? "",
            City = reader["City"]?.ToString() ?? "",
            Country = reader["Country"]?.ToString() ?? "HONDURAS",
            Department = reader["Department"]?.ToString() ?? "",
            Note = reader["Note"]?.ToString() ?? "",
            IsActive = Convert.ToBoolean(reader["IsActive"])
        };
    }
}
