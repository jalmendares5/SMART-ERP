using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class VendorService
{
    public static List<Vendor> GetAll()
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<Vendor>();

        var vendors = new List<Vendor>();

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
    CommissionPercentage,
    Phone,
    Email,
    IdentityNumber,
    Address,
    EntryDate,
    Note,
    PhotoPath,
    IsActive
FROM vendors
ORDER BY Name;";

            using var command =
                new MySqlCommand(sql, connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                vendors.Add(MapVendor(reader));
            }
        }
        catch
        {
            return new List<Vendor>();
        }

        return vendors;
    }

    public static List<Vendor> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return GetAll();

        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<Vendor>();

        var vendors = new List<Vendor>();

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
    CommissionPercentage,
    Phone,
    Email,
    IdentityNumber,
    Address,
    EntryDate,
    Note,
    PhotoPath,
    IsActive
FROM vendors
WHERE
    Name LIKE @Term
    OR Code LIKE @Term
    OR Phone LIKE @Term
    OR Email LIKE @Term
    OR IdentityNumber LIKE @Term
ORDER BY Name;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@Term",
                "%" + term + "%");

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                vendors.Add(MapVendor(reader));
            }
        }
        catch
        {
            return new List<Vendor>();
        }

        return vendors;
    }

    public static string GenerateNextCode()
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return "V001";

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
FROM vendors;";

            using var command =
                new MySqlCommand(sql, connection);

            var result =
                Convert.ToInt32(command.ExecuteScalar());

            return $"V{result + 1:000}";
        }
        catch
        {
            return "V001";
        }
    }

    public static bool Save(Vendor vendor)
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            if (vendor.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(vendor.Code))
                    vendor.Code = GenerateNextCode();

                const string sql = @"
INSERT INTO vendors
(
    Name,
    Code,
    CommissionPercentage,
    Phone,
    Email,
    IdentityNumber,
    Address,
    EntryDate,
    Note,
    PhotoPath,
    IsActive
)
VALUES
(
    @Name,
    @Code,
    @CommissionPercentage,
    @Phone,
    @Email,
    @IdentityNumber,
    @Address,
    @EntryDate,
    @Note,
    @PhotoPath,
    @IsActive
);

SELECT LAST_INSERT_ID();";

                using var command =
                    new MySqlCommand(sql, connection);

                AddParameters(command, vendor);

                vendor.Id =
                    Convert.ToInt32(command.ExecuteScalar());

                return vendor.Id > 0;
            }

            const string updateSql = @"
UPDATE vendors SET
    Name = @Name,
    CommissionPercentage = @CommissionPercentage,
    Phone = @Phone,
    Email = @Email,
    IdentityNumber = @IdentityNumber,
    Address = @Address,
    EntryDate = @EntryDate,
    Note = @Note,
    PhotoPath = @PhotoPath,
    IsActive = @IsActive
WHERE Id = @Id;";

            using var updateCommand =
                new MySqlCommand(updateSql, connection);

            updateCommand.Parameters.AddWithValue("@Name", vendor.Name);
            updateCommand.Parameters.AddWithValue("@CommissionPercentage", vendor.CommissionPercentage);
            updateCommand.Parameters.AddWithValue("@Phone", vendor.Phone);
            updateCommand.Parameters.AddWithValue("@Email", vendor.Email);
            updateCommand.Parameters.AddWithValue("@IdentityNumber", vendor.IdentityNumber);
            updateCommand.Parameters.AddWithValue("@Address", vendor.Address);
            updateCommand.Parameters.AddWithValue("@EntryDate", vendor.EntryDate);
            updateCommand.Parameters.AddWithValue("@Note", vendor.Note);
            updateCommand.Parameters.AddWithValue("@PhotoPath", vendor.PhotoPath);
            updateCommand.Parameters.AddWithValue("@IsActive", vendor.IsActive);
            updateCommand.Parameters.AddWithValue("@Id", vendor.Id);

            return updateCommand.ExecuteNonQuery() > 0;
        }
        catch
        {
            return false;
        }
    }

    public static void Delete(int vendorId)
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
                "DELETE FROM vendors WHERE Id = @Id;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@Id",
                vendorId);

            command.ExecuteNonQuery();
        }
        catch
        {
        }
    }

    private static void AddParameters(
        MySqlCommand command,
        Vendor vendor)
    {
        command.Parameters.AddWithValue("@Name", vendor.Name);
        command.Parameters.AddWithValue("@Code", vendor.Code);
        command.Parameters.AddWithValue("@CommissionPercentage", vendor.CommissionPercentage);
        command.Parameters.AddWithValue("@Phone", vendor.Phone);
        command.Parameters.AddWithValue("@Email", vendor.Email);
        command.Parameters.AddWithValue("@IdentityNumber", vendor.IdentityNumber);
        command.Parameters.AddWithValue("@Address", vendor.Address);
        command.Parameters.AddWithValue("@EntryDate", vendor.EntryDate);
        command.Parameters.AddWithValue("@Note", vendor.Note);
        command.Parameters.AddWithValue("@PhotoPath", vendor.PhotoPath);
        command.Parameters.AddWithValue("@IsActive", vendor.IsActive);
    }

    private static Vendor MapVendor(
        MySqlDataReader reader)
    {
        return new Vendor
        {
            Id = Convert.ToInt32(reader["Id"]),
            Name = reader["Name"]?.ToString() ?? "",
            Code = reader["Code"]?.ToString() ?? "",
            CommissionPercentage =
                Convert.ToDecimal(reader["CommissionPercentage"]),
            Phone = reader["Phone"]?.ToString() ?? "",
            Email = reader["Email"]?.ToString() ?? "",
            IdentityNumber =
                reader["IdentityNumber"]?.ToString() ?? "",
            Address = reader["Address"]?.ToString() ?? "",
            EntryDate =
                Convert.ToDateTime(reader["EntryDate"]),
            Note = reader["Note"]?.ToString() ?? "",
            PhotoPath =
                reader["PhotoPath"]?.ToString() ?? "",
            IsActive =
                Convert.ToBoolean(reader["IsActive"])
        };
    }
}
