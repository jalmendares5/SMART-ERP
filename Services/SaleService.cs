using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class SaleService
{
    public static List<Sale> GetAll()
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<Sale>();

        var sales = new List<Sale>();

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            const string sql = @"
SELECT
    Id,
    InvoiceNumber,
    SaleDate,
    Status,

    BillingCompanyId,
    BillingCompanyName,

    OperationalAreaId,
    OperationalAreaName,

    CustomerId,
    CustomerName,

    PrimaryVendorId,
    PrimaryVendorName,
    PrimaryCommissionPercentage,
    PrimaryCommissionAmount,

    IsSpecialSale,

    SecondaryVendorId,
    SecondaryVendorName,
    SecondaryCommissionPercentage,
    SecondaryCommissionAmount,

    Total,
    CommissionBase,
    TotalCommissionPercentage,
    TotalCommissionAmount,

    PaymentMethod,
    CreditDays,

    Notes,
    CreatedAt,
    CreatedBy
FROM sales
ORDER BY CreatedAt DESC, Id DESC;";

            using var command =
                new MySqlCommand(sql, connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sales.Add(MapSale(reader));
            }
        }
        catch
        {
            return new List<Sale>();
        }

        return sales;
    }

    public static bool InvoiceExists(
        string invoiceNumber,
        int? excludeId = null)
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

            const string sql = @"
SELECT COUNT(*)
FROM sales
WHERE InvoiceNumber = @InvoiceNumber
  AND (@ExcludeId = 0 OR Id <> @ExcludeId);";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@InvoiceNumber",
                invoiceNumber);

            command.Parameters.AddWithValue(
                "@ExcludeId",
                excludeId ?? 0);

            var count =
                Convert.ToInt32(command.ExecuteScalar());

            return count > 0;
        }
        catch
        {
            return false;
        }
    }

    public static (bool success, string message) ValidateSale(Sale sale)
    {
        if (string.IsNullOrWhiteSpace(sale.InvoiceNumber))
        {
            return (false, "El número de factura es obligatorio.");
        }

        if (InvoiceExists(
                sale.InvoiceNumber,
                sale.Id == 0 ? null : sale.Id))
        {
            return (
                false,
                $"Ya existe una factura con el número {sale.InvoiceNumber}.");
        }

        if (sale.CustomerId == 0)
        {
            return (false, "Debe seleccionar un cliente.");
        }

        if (sale.Total <= 0 && sale.CommissionBase <= 0)
        {
            return (false, "El total debe ser mayor que cero.");
        }

        if (sale.PrimaryVendorId == 0)
        {
            return (false, "Debe seleccionar un vendedor principal.");
        }

        if (sale.PrimaryCommissionPercentage < 0)
        {
            return (
                false,
                "La comisión del vendedor principal no puede ser negativa.");
        }

        if (sale.IsSpecialSale)
        {
            if (sale.SecondaryVendorId == null ||
                sale.SecondaryVendorId == 0)
            {
                return (
                    false,
                    "En venta especial debe seleccionar un vendedor secundario.");
            }

            if (sale.SecondaryCommissionPercentage < 0)
            {
                return (
                    false,
                    "La comisión del vendedor secundario no puede ser negativa.");
            }

            if (sale.PrimaryVendorId == sale.SecondaryVendorId)
            {
                return (
                    false,
                    "El vendedor principal y secundario deben ser diferentes.");
            }
        }

        if (sale.CreditDays < 0)
        {
            return (
                false,
                "Los días de crédito no pueden ser negativos.");
        }

        if (sale.Notes.Length > 200)
        {
            return (
                false,
                "Las observaciones no pueden exceder los 200 caracteres.");
        }

        if (sale.Status == "ANULADA" &&
            SalesCaptureSettingsService.Current.RequireCancellationReasonWhenVoided &&
            string.IsNullOrWhiteSpace(sale.Notes))
        {
            return (
                false,
                "Debe ingresar el motivo de anulación en observaciones.");
        }

        return (true, string.Empty);
    }

    public static (
        decimal primaryAmount,
        decimal secondaryAmount,
        decimal totalAmount) CalculateCommissions(
        decimal total,
        decimal primaryPercentage,
        decimal secondaryPercentage = 0)
    {
        var primaryAmount =
            total * (primaryPercentage / 100);

        var secondaryAmount =
            total * (secondaryPercentage / 100);

        var totalAmount =
            primaryAmount + secondaryAmount;

        return (
            primaryAmount,
            secondaryAmount,
            totalAmount);
    }

    public static (bool success, string message) Save(Sale sale)
    {
        var validation = ValidateSale(sale);

        if (!validation.success)
            return validation;

        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return (
                false,
                "No hay una empresa conectada.");
        }

        var commissions =
            CalculateCommissions(
                sale.CommissionBase,
                sale.PrimaryCommissionPercentage,
                sale.IsSpecialSale
                    ? sale.SecondaryCommissionPercentage
                    : 0);

        sale.PrimaryCommissionAmount =
            commissions.primaryAmount;

        sale.SecondaryCommissionAmount =
            commissions.secondaryAmount;

        sale.TotalCommissionPercentage =
            sale.PrimaryCommissionPercentage +
            (
                sale.IsSpecialSale
                    ? sale.SecondaryCommissionPercentage
                    : 0
            );

        sale.TotalCommissionAmount =
            commissions.totalAmount;

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            if (sale.Id == 0)
            {
                const string sql = @"
INSERT INTO sales
(
    InvoiceNumber,
    SaleDate,
    Status,

    BillingCompanyId,
    BillingCompanyName,

    OperationalAreaId,
    OperationalAreaName,

    CustomerId,
    CustomerName,

    PrimaryVendorId,
    PrimaryVendorName,
    PrimaryCommissionPercentage,
    PrimaryCommissionAmount,

    IsSpecialSale,

    SecondaryVendorId,
    SecondaryVendorName,
    SecondaryCommissionPercentage,
    SecondaryCommissionAmount,

    Total,
    CommissionBase,
    TotalCommissionPercentage,
    TotalCommissionAmount,

    PaymentMethod,
    CreditDays,

    Notes,
    CreatedAt,
    CreatedBy
)
VALUES
(
    @InvoiceNumber,
    @SaleDate,
    @Status,

    @BillingCompanyId,
    @BillingCompanyName,

    @OperationalAreaId,
    @OperationalAreaName,

    @CustomerId,
    @CustomerName,

    @PrimaryVendorId,
    @PrimaryVendorName,
    @PrimaryCommissionPercentage,
    @PrimaryCommissionAmount,

    @IsSpecialSale,

    @SecondaryVendorId,
    @SecondaryVendorName,
    @SecondaryCommissionPercentage,
    @SecondaryCommissionAmount,

    @Total,
    @CommissionBase,
    @TotalCommissionPercentage,
    @TotalCommissionAmount,

    @PaymentMethod,
    @CreditDays,

    @Notes,
    @CreatedAt,
    @CreatedBy
);

SELECT LAST_INSERT_ID();";

                using var command =
                    new MySqlCommand(sql, connection);

                AddSaleParameters(command, sale);

                sale.CreatedAt = DateTime.Now;

                command.Parameters["@CreatedAt"].Value =
                    sale.CreatedAt;

                sale.Id =
                    Convert.ToInt32(command.ExecuteScalar());

                return (
                    true,
                    "Venta guardada exitosamente.");
            }

            const string updateSql = @"
UPDATE sales SET
    InvoiceNumber = @InvoiceNumber,
    SaleDate = @SaleDate,
    Status = @Status,

    BillingCompanyId = @BillingCompanyId,
    BillingCompanyName = @BillingCompanyName,

    OperationalAreaId = @OperationalAreaId,
    OperationalAreaName = @OperationalAreaName,

    CustomerId = @CustomerId,
    CustomerName = @CustomerName,

    PrimaryVendorId = @PrimaryVendorId,
    PrimaryVendorName = @PrimaryVendorName,
    PrimaryCommissionPercentage = @PrimaryCommissionPercentage,
    PrimaryCommissionAmount = @PrimaryCommissionAmount,

    IsSpecialSale = @IsSpecialSale,

    SecondaryVendorId = @SecondaryVendorId,
    SecondaryVendorName = @SecondaryVendorName,
    SecondaryCommissionPercentage = @SecondaryCommissionPercentage,
    SecondaryCommissionAmount = @SecondaryCommissionAmount,

    Total = @Total,
    CommissionBase = @CommissionBase,
    TotalCommissionPercentage = @TotalCommissionPercentage,
    TotalCommissionAmount = @TotalCommissionAmount,

    PaymentMethod = @PaymentMethod,
    CreditDays = @CreditDays,

    Notes = @Notes,
    CreatedBy = @CreatedBy
WHERE Id = @Id;";

            using var updateCommand =
                new MySqlCommand(updateSql, connection);

            AddSaleParameters(updateCommand, sale);
            updateCommand.Parameters.AddWithValue("@Id", sale.Id);

            var affected =
                updateCommand.ExecuteNonQuery();

            if (affected == 0)
            {
                return (
                    false,
                    "No se encontró la venta para actualizar.");
            }

            return (
                true,
                "Venta actualizada exitosamente.");
        }
        catch (MySqlException ex)
        {
            return (
                false,
                "Error de base de datos al guardar la venta: " +
                ex.Message);
        }
        catch (Exception ex)
        {
            return (
                false,
                "Error al guardar la venta: " +
                ex.Message);
        }
    }

    public static void Delete(int saleId)
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
                "DELETE FROM sales WHERE Id = @Id;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", saleId);

            command.ExecuteNonQuery();
        }
        catch
        {
        }
    }

    public static Sale? GetById(int saleId)
    {
        var connectionString =
            CompanyConnectionService.GetActiveConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
            return null;

        try
        {
            using var connection =
                new MySqlConnection(connectionString);

            connection.Open();

            const string sql = @"
SELECT *
FROM sales
WHERE Id = @Id
LIMIT 1;";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", saleId);

            using var reader =
                command.ExecuteReader();

            if (!reader.Read())
                return null;

            return MapSale(reader);
        }
        catch
        {
            return null;
        }
    }

    public static List<Sale> GetByCustomer(int customerId)
    {
        return GetAll()
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    public static List<Sale> GetByVendor(int vendorId)
    {
        return GetAll()
            .Where(s =>
                s.PrimaryVendorId == vendorId ||
                s.SecondaryVendorId == vendorId)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    public static List<Sale> GetByDateRange(
        DateTime from,
        DateTime to)
    {
        return GetAll()
            .Where(s =>
                s.SaleDate >= from &&
                s.SaleDate <= to)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    private static void AddSaleParameters(
        MySqlCommand command,
        Sale sale)
    {
        command.Parameters.AddWithValue(
            "@InvoiceNumber",
            sale.InvoiceNumber);

        command.Parameters.AddWithValue(
            "@SaleDate",
            sale.SaleDate);

        command.Parameters.AddWithValue(
            "@Status",
            sale.Status);

        command.Parameters.AddWithValue(
            "@BillingCompanyId",
            sale.BillingCompanyId);

        command.Parameters.AddWithValue(
            "@BillingCompanyName",
            sale.BillingCompanyName);

        command.Parameters.AddWithValue(
            "@OperationalAreaId",
            sale.OperationalAreaId);

        command.Parameters.AddWithValue(
            "@OperationalAreaName",
            sale.OperationalAreaName);

        command.Parameters.AddWithValue(
            "@CustomerId",
            sale.CustomerId);

        command.Parameters.AddWithValue(
            "@CustomerName",
            sale.CustomerName);

        command.Parameters.AddWithValue(
            "@PrimaryVendorId",
            sale.PrimaryVendorId);

        command.Parameters.AddWithValue(
            "@PrimaryVendorName",
            sale.PrimaryVendorName);

        command.Parameters.AddWithValue(
            "@PrimaryCommissionPercentage",
            sale.PrimaryCommissionPercentage);

        command.Parameters.AddWithValue(
            "@PrimaryCommissionAmount",
            sale.PrimaryCommissionAmount);

        command.Parameters.AddWithValue(
            "@IsSpecialSale",
            sale.IsSpecialSale);

        command.Parameters.AddWithValue(
            "@SecondaryVendorId",
            sale.SecondaryVendorId.HasValue
                ? sale.SecondaryVendorId.Value
                : DBNull.Value);

        command.Parameters.AddWithValue(
            "@SecondaryVendorName",
            sale.SecondaryVendorName);

        command.Parameters.AddWithValue(
            "@SecondaryCommissionPercentage",
            sale.SecondaryCommissionPercentage);

        command.Parameters.AddWithValue(
            "@SecondaryCommissionAmount",
            sale.SecondaryCommissionAmount);

        command.Parameters.AddWithValue(
            "@Total",
            sale.Total);

        command.Parameters.AddWithValue(
            "@CommissionBase",
            sale.CommissionBase);

        command.Parameters.AddWithValue(
            "@TotalCommissionPercentage",
            sale.TotalCommissionPercentage);

        command.Parameters.AddWithValue(
            "@TotalCommissionAmount",
            sale.TotalCommissionAmount);

        command.Parameters.AddWithValue(
            "@PaymentMethod",
            sale.PaymentMethod);

        command.Parameters.AddWithValue(
            "@CreditDays",
            sale.CreditDays);

        command.Parameters.AddWithValue(
            "@Notes",
            sale.Notes);

        command.Parameters.AddWithValue(
            "@CreatedAt",
            sale.CreatedAt);

        command.Parameters.AddWithValue(
            "@CreatedBy",
            sale.CreatedBy);
    }

    private static Sale MapSale(
        MySqlDataReader reader)
    {
        return new Sale
        {
            Id = Convert.ToInt32(reader["Id"]),
            InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? "",
            SaleDate = Convert.ToDateTime(reader["SaleDate"]),
            Status = reader["Status"]?.ToString() ?? "ACTIVA",

            BillingCompanyId =
                Convert.ToInt32(reader["BillingCompanyId"]),

            BillingCompanyName =
                reader["BillingCompanyName"]?.ToString() ?? "",

            OperationalAreaId =
                Convert.ToInt32(reader["OperationalAreaId"]),

            OperationalAreaName =
                reader["OperationalAreaName"]?.ToString() ?? "",

            CustomerId =
                Convert.ToInt32(reader["CustomerId"]),

            CustomerName =
                reader["CustomerName"]?.ToString() ?? "",

            PrimaryVendorId =
                Convert.ToInt32(reader["PrimaryVendorId"]),

            PrimaryVendorName =
                reader["PrimaryVendorName"]?.ToString() ?? "",

            PrimaryCommissionPercentage =
                Convert.ToDecimal(
                    reader["PrimaryCommissionPercentage"]),

            PrimaryCommissionAmount =
                Convert.ToDecimal(
                    reader["PrimaryCommissionAmount"]),

            IsSpecialSale =
                Convert.ToBoolean(
                    reader["IsSpecialSale"]),

            SecondaryVendorId =
                reader["SecondaryVendorId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(
                        reader["SecondaryVendorId"]),

            SecondaryVendorName =
                reader["SecondaryVendorName"]?.ToString() ?? "",

            SecondaryCommissionPercentage =
                Convert.ToDecimal(
                    reader["SecondaryCommissionPercentage"]),

            SecondaryCommissionAmount =
                Convert.ToDecimal(
                    reader["SecondaryCommissionAmount"]),

            Total =
                Convert.ToDecimal(
                    reader["Total"]),

            CommissionBase =
                Convert.ToDecimal(
                    reader["CommissionBase"]),

            TotalCommissionPercentage =
                Convert.ToDecimal(
                    reader["TotalCommissionPercentage"]),

            TotalCommissionAmount =
                Convert.ToDecimal(
                    reader["TotalCommissionAmount"]),

            PaymentMethod =
                reader["PaymentMethod"]?.ToString() ?? "EFECTIVO",

            CreditDays =
                Convert.ToInt32(
                    reader["CreditDays"]),

            Notes =
                reader["Notes"]?.ToString() ?? "",

            CreatedAt =
                Convert.ToDateTime(
                    reader["CreatedAt"]),

            CreatedBy =
                reader["CreatedBy"]?.ToString() ?? ""
        };
    }
}
