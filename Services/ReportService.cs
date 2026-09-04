using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class ReportService
{
    public static List<SalesSummaryReport> GetSalesSummary(SalesReportFilter filter)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<SalesSummaryReport>();

        var reports = new List<SalesSummaryReport>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = BuildSalesSummaryQuery(filter);
            
            using var command = new MySqlCommand(sql, connection);
            AddSalesSummaryParameters(command, filter);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(MapSalesSummaryReport(reader));
            }
        }
        catch
        {
            return new List<SalesSummaryReport>();
        }

        return reports;
    }

    public static SalesSummaryTotals GetSalesSummaryTotals(List<SalesSummaryReport> reports)
    {
        return new SalesSummaryTotals
        {
            SalesCount = reports.Count,
            TotalSold = reports.Sum(r => r.Total),
            TotalCommissionBase = reports.Sum(r => r.CommissionBase),
            TotalCommissions = reports.Sum(r => r.CommissionAmount)
        };
    }

    public static List<VendorReport> GetVendorReport(SalesReportFilter filter)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<VendorReport>();

        var reports = new List<VendorReport>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = BuildVendorReportQuery(filter);
            
            using var command = new MySqlCommand(sql, connection);
            AddSalesSummaryParameters(command, filter);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(MapVendorReport(reader));
            }
        }
        catch
        {
            return new List<VendorReport>();
        }

        return reports;
    }

    public static List<CustomerReport> GetCustomerReport(SalesReportFilter filter)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<CustomerReport>();

        var reports = new List<CustomerReport>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = BuildCustomerReportQuery(filter);
            
            using var command = new MySqlCommand(sql, connection);
            AddSalesSummaryParameters(command, filter);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(MapCustomerReport(reader));
            }
        }
        catch
        {
            return new List<CustomerReport>();
        }

        return reports;
    }

    public static List<BillingCompanyReport> GetBillingCompanyReport(SalesReportFilter filter)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<BillingCompanyReport>();

        var reports = new List<BillingCompanyReport>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = BuildBillingCompanyReportQuery(filter);
            
            using var command = new MySqlCommand(sql, connection);
            AddSalesSummaryParameters(command, filter);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(MapBillingCompanyReport(reader));
            }
        }
        catch
        {
            return new List<BillingCompanyReport>();
        }

        return reports;
    }

    public static List<OperationalAreaReport> GetOperationalAreaReport(SalesReportFilter filter)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<OperationalAreaReport>();

        var reports = new List<OperationalAreaReport>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = BuildOperationalAreaReportQuery(filter);
            
            using var command = new MySqlCommand(sql, connection);
            AddSalesSummaryParameters(command, filter);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(MapOperationalAreaReport(reader));
            }
        }
        catch
        {
            return new List<OperationalAreaReport>();
        }

        return reports;
    }

    public static List<PaymentMethodReport> GetPaymentMethodReport(SalesReportFilter filter)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<PaymentMethodReport>();

        var reports = new List<PaymentMethodReport>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = BuildPaymentMethodReportQuery(filter);
            
            using var command = new MySqlCommand(sql, connection);
            AddSalesSummaryParameters(command, filter);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                reports.Add(MapPaymentMethodReport(reader));
            }

            // Calculate percentages
            if (reports.Any())
            {
                var grandTotal = reports.Sum(r => r.Total);
                foreach (var report in reports)
                {
                    report.PercentageOfTotal = grandTotal > 0 ? (report.Total / grandTotal) * 100 : 0;
                }
            }
        }
        catch
        {
            return new List<PaymentMethodReport>();
        }

        return reports;
    }

    private static string BuildFilterWhereClause(SalesReportFilter filter)
    {
        var sql = string.Empty;

        if (filter.DateFrom.HasValue)
            sql += " AND SaleDate >= @DateFrom";

        if (filter.DateTo.HasValue)
            sql += " AND SaleDate <= @DateTo";

        if (filter.BillingCompanyId.HasValue && filter.BillingCompanyId.Value > 0)
            sql += " AND BillingCompanyId = @BillingCompanyId";

        if (filter.OperationalAreaId.HasValue && filter.OperationalAreaId.Value > 0)
            sql += " AND OperationalAreaId = @OperationalAreaId";

        if (filter.CustomerId.HasValue && filter.CustomerId.Value > 0)
            sql += " AND CustomerId = @CustomerId";

        if (filter.VendorId.HasValue && filter.VendorId.Value > 0)
            sql += " AND (PrimaryVendorId = @VendorId OR SecondaryVendorId = @VendorId)";

        if (!string.IsNullOrWhiteSpace(filter.Status) && !filter.Status.Equals("TODOS", StringComparison.OrdinalIgnoreCase))
            sql += " AND Status = @Status";

        if (!string.IsNullOrWhiteSpace(filter.PaymentMethod) &&
            !filter.PaymentMethod.StartsWith("--") &&
            !filter.PaymentMethod.Equals("TODOS", StringComparison.OrdinalIgnoreCase) &&
            !filter.PaymentMethod.Equals("TODAS", StringComparison.OrdinalIgnoreCase))
            sql += " AND PaymentMethod = @PaymentMethod";

        return sql;
    }

    private static string BuildSalesSummaryQuery(SalesReportFilter filter)
    {
        return $@"
SELECT
    Id as SaleId,
    InvoiceNumber,
    SaleDate,
    CustomerName,
    BillingCompanyName,
    OperationalAreaName,
    PrimaryVendorName,
    PaymentMethod,
    CreditDays,
    Status,
    Total,
    CommissionBase,
    TotalCommissionPercentage as CommissionPercentage,
    TotalCommissionAmount as CommissionAmount
FROM sales
WHERE 1=1 {BuildFilterWhereClause(filter)}
ORDER BY SaleDate DESC, Id DESC";
    }

    private static string BuildVendorReportQuery(SalesReportFilter filter)
    {
        return $@"
SELECT
    PrimaryVendorId as VendorId,
    PrimaryVendorName as VendorName,
    COUNT(*) as SalesCount,
    COALESCE(SUM(Total), 0) as TotalSold,
    COALESCE(SUM(CommissionBase), 0) as CommissionBase,
    COALESCE(SUM(TotalCommissionAmount), 0) as CommissionGenerated,
    COALESCE(AVG(Total), 0) as AverageSale
FROM sales
WHERE 1=1 {BuildFilterWhereClause(filter)}
GROUP BY PrimaryVendorId, PrimaryVendorName
ORDER BY TotalSold DESC";
    }

    private static string BuildCustomerReportQuery(SalesReportFilter filter)
    {
        return $@"
SELECT
    CustomerId,
    CustomerName,
    COUNT(*) as PurchaseCount,
    COALESCE(SUM(Total), 0) as TotalPurchased,
    COALESCE(AVG(Total), 0) as AveragePurchase,
    MAX(SaleDate) as LastPurchaseDate
FROM sales
WHERE 1=1 {BuildFilterWhereClause(filter)}
GROUP BY CustomerId, CustomerName
ORDER BY TotalPurchased DESC";
    }

    private static string BuildBillingCompanyReportQuery(SalesReportFilter filter)
    {
        return $@"
SELECT
    BillingCompanyId as CompanyId,
    BillingCompanyName as CompanyName,
    COUNT(*) as SalesCount,
    COALESCE(SUM(Total), 0) as TotalSold,
    COALESCE(SUM(CommissionBase), 0) as CommissionBase,
    COALESCE(SUM(TotalCommissionAmount), 0) as Commissions
FROM sales
WHERE 1=1 {BuildFilterWhereClause(filter)}
GROUP BY BillingCompanyId, BillingCompanyName
ORDER BY TotalSold DESC";
    }

    private static string BuildOperationalAreaReportQuery(SalesReportFilter filter)
    {
        return $@"
SELECT
    OperationalAreaId as AreaId,
    OperationalAreaName as AreaName,
    COUNT(*) as SalesCount,
    COALESCE(SUM(Total), 0) as TotalSold,
    COALESCE(SUM(TotalCommissionAmount), 0) as Commissions
FROM sales
WHERE 1=1 {BuildFilterWhereClause(filter)}
GROUP BY OperationalAreaId, OperationalAreaName
ORDER BY TotalSold DESC";
    }

    private static string BuildPaymentMethodReportQuery(SalesReportFilter filter)
    {
        return $@"
SELECT
    PaymentMethod,
    COUNT(*) as OperationCount,
    COALESCE(SUM(Total), 0) as Total
FROM sales
WHERE 1=1 {BuildFilterWhereClause(filter)}
GROUP BY PaymentMethod
ORDER BY Total DESC";
    }

    private static void AddSalesSummaryParameters(MySqlCommand command, SalesReportFilter filter)
    {
        if (filter.DateFrom.HasValue)
            command.Parameters.AddWithValue("@DateFrom", filter.DateFrom.Value.Date);

        if (filter.DateTo.HasValue)
            command.Parameters.AddWithValue("@DateTo", filter.DateTo.Value.Date.AddDays(1).AddTicks(-1));

        if (filter.BillingCompanyId.HasValue && filter.BillingCompanyId.Value > 0)
            command.Parameters.AddWithValue("@BillingCompanyId", filter.BillingCompanyId.Value);

        if (filter.OperationalAreaId.HasValue && filter.OperationalAreaId.Value > 0)
            command.Parameters.AddWithValue("@OperationalAreaId", filter.OperationalAreaId.Value);

        if (filter.CustomerId.HasValue && filter.CustomerId.Value > 0)
            command.Parameters.AddWithValue("@CustomerId", filter.CustomerId.Value);

        if (filter.VendorId.HasValue && filter.VendorId.Value > 0)
            command.Parameters.AddWithValue("@VendorId", filter.VendorId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Status) && !filter.Status.Equals("TODOS", StringComparison.OrdinalIgnoreCase))
            command.Parameters.AddWithValue("@Status", filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.PaymentMethod) &&
            !filter.PaymentMethod.StartsWith("--") &&
            !filter.PaymentMethod.Equals("TODOS", StringComparison.OrdinalIgnoreCase) &&
            !filter.PaymentMethod.Equals("TODAS", StringComparison.OrdinalIgnoreCase))
            command.Parameters.AddWithValue("@PaymentMethod", filter.PaymentMethod);
    }

    private static SalesSummaryReport MapSalesSummaryReport(MySqlDataReader reader)
    {
        return new SalesSummaryReport
        {
            SaleId = Convert.ToInt32(reader["SaleId"]),
            InvoiceNumber = reader["InvoiceNumber"]?.ToString() ?? "",
            SaleDate = Convert.ToDateTime(reader["SaleDate"]),
            CustomerName = reader["CustomerName"]?.ToString() ?? "",
            BillingCompanyName = reader["BillingCompanyName"]?.ToString() ?? "",
            OperationalAreaName = reader["OperationalAreaName"]?.ToString() ?? "",
            PrimaryVendorName = reader["PrimaryVendorName"]?.ToString() ?? "",
            PaymentMethod = reader["PaymentMethod"]?.ToString() ?? "",
            CreditDays = reader["CreditDays"] != DBNull.Value ? Convert.ToInt32(reader["CreditDays"]) : 0,
            Status = reader["Status"]?.ToString() ?? "",
            Total = Convert.ToDecimal(reader["Total"]),
            CommissionBase = Convert.ToDecimal(reader["CommissionBase"]),
            CommissionPercentage = Convert.ToDecimal(reader["CommissionPercentage"]),
            CommissionAmount = Convert.ToDecimal(reader["CommissionAmount"])
        };
    }

    private static VendorReport MapVendorReport(MySqlDataReader reader)
    {
        return new VendorReport
        {
            VendorId = Convert.ToInt32(reader["VendorId"]),
            VendorName = reader["VendorName"]?.ToString() ?? "",
            SalesCount = Convert.ToInt32(reader["SalesCount"]),
            TotalSold = Convert.ToDecimal(reader["TotalSold"]),
            CommissionBase = Convert.ToDecimal(reader["CommissionBase"]),
            CommissionGenerated = Convert.ToDecimal(reader["CommissionGenerated"]),
            AverageSale = Convert.ToDecimal(reader["AverageSale"])
        };
    }

    private static CustomerReport MapCustomerReport(MySqlDataReader reader)
    {
        return new CustomerReport
        {
            CustomerId = Convert.ToInt32(reader["CustomerId"]),
            CustomerName = reader["CustomerName"]?.ToString() ?? "",
            PurchaseCount = Convert.ToInt32(reader["PurchaseCount"]),
            TotalPurchased = Convert.ToDecimal(reader["TotalPurchased"]),
            AveragePurchase = Convert.ToDecimal(reader["AveragePurchase"]),
            LastPurchaseDate = reader["LastPurchaseDate"] != DBNull.Value 
                ? Convert.ToDateTime(reader["LastPurchaseDate"]) 
                : null
        };
    }

    private static BillingCompanyReport MapBillingCompanyReport(MySqlDataReader reader)
    {
        return new BillingCompanyReport
        {
            CompanyId = Convert.ToInt32(reader["CompanyId"]),
            CompanyName = reader["CompanyName"]?.ToString() ?? "",
            SalesCount = Convert.ToInt32(reader["SalesCount"]),
            TotalSold = Convert.ToDecimal(reader["TotalSold"]),
            CommissionBase = Convert.ToDecimal(reader["CommissionBase"]),
            Commissions = Convert.ToDecimal(reader["Commissions"])
        };
    }

    private static OperationalAreaReport MapOperationalAreaReport(MySqlDataReader reader)
    {
        return new OperationalAreaReport
        {
            AreaId = Convert.ToInt32(reader["AreaId"]),
            AreaName = reader["AreaName"]?.ToString() ?? "",
            SalesCount = Convert.ToInt32(reader["SalesCount"]),
            TotalSold = Convert.ToDecimal(reader["TotalSold"]),
            Commissions = Convert.ToDecimal(reader["Commissions"])
        };
    }

    private static PaymentMethodReport MapPaymentMethodReport(MySqlDataReader reader)
    {
        return new PaymentMethodReport
        {
            PaymentMethod = reader["PaymentMethod"]?.ToString() ?? "",
            OperationCount = Convert.ToInt32(reader["OperationCount"]),
            Total = Convert.ToDecimal(reader["Total"]),
            PercentageOfTotal = 0
        };
    }
}
