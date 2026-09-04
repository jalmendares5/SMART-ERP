using MySqlConnector;
using SMART_ERP.Models;
using System.Collections.ObjectModel;

namespace SMART_ERP.Services;

public static class DashboardService
{
    public static DashboardSummary GetDashboardSummary(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new DashboardSummary();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var summary = new DashboardSummary
            {
                ReportDate = DateTime.Now
            };

            // Total Sales
            summary.TotalSales = new DashboardKpi
            {
                Title = "Total Ventas",
                Value = GetTotalSales(connection, fromDate, toDate),
                Format = "C2",
                Icon = "\uE8C7",
                Color = "#10B981"
            };

            // Total Commission
            summary.TotalCommission = new DashboardKpi
            {
                Title = "Total Comisiones",
                Value = GetTotalCommission(connection, fromDate, toDate),
                Format = "C2",
                Icon = "\uE77B",
                Color = "#3B82F6"
            };

            // Sales Count
            summary.SalesCount = new DashboardKpi
            {
                Title = "Cantidad Ventas",
                Value = GetSalesCount(connection, fromDate, toDate),
                Format = "N0",
                Icon = "\uE9D2",
                Color = "#8B5CF6"
            };

            // Average Sale
            summary.AverageSale = new DashboardKpi
            {
                Title = "Promedio Venta",
                Value = GetAverageSale(connection, fromDate, toDate),
                Format = "C2",
                Icon = "\uE9D9",
                Color = "#F59E0B"
            };

            // Active Vendors
            summary.ActiveVendors = new DashboardKpi
            {
                Title = "Vendedores Activos",
                Value = GetActiveVendorsCount(connection),
                Format = "N0",
                Icon = "\uE77B",
                Color = "#6366F1"
            };

            // Active Customers
            summary.ActiveCustomers = new DashboardKpi
            {
                Title = "Clientes Activos",
                Value = GetActiveCustomersCount(connection),
                Format = "N0",
                Icon = "\uE716",
                Color = "#EC4899"
            };

            return summary;
        }
        catch
        {
            return new DashboardSummary();
        }
    }

    public static List<TopVendor> GetTopVendors(int topN = 5, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<TopVendor>();

        var vendors = new List<TopVendor>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
SELECT 
    PrimaryVendorId as VendorId,
    PrimaryVendorName as VendorName,
    SUM(Total) as TotalSales,
    COUNT(*) as SalesCount,
    SUM(TotalCommissionAmount) as CommissionGenerated
FROM sales
WHERE Status = 'ACTIVA'";

            if (fromDate.HasValue)
                sql += " AND SaleDate >= @FromDate";

            if (toDate.HasValue)
                sql += " AND SaleDate <= @ToDate";

            sql += @"
GROUP BY PrimaryVendorId, PrimaryVendorName
ORDER BY TotalSales DESC
LIMIT @TopN";

            using var command = new MySqlCommand(sql, connection);
            
            if (fromDate.HasValue)
                command.Parameters.AddWithValue("@FromDate", fromDate.Value.Date);

            if (toDate.HasValue)
                command.Parameters.AddWithValue("@ToDate", toDate.Value.Date.AddDays(1).AddTicks(-1));

            command.Parameters.AddWithValue("@TopN", topN);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                vendors.Add(new TopVendor
                {
                    VendorId = Convert.ToInt32(reader["VendorId"]),
                    VendorName = reader["VendorName"]?.ToString() ?? "",
                    TotalSales = Convert.ToDecimal(reader["TotalSales"]),
                    SalesCount = Convert.ToInt32(reader["SalesCount"]),
                    CommissionGenerated = Convert.ToDecimal(reader["CommissionGenerated"])
                });
            }
        }
        catch
        {
            return new List<TopVendor>();
        }

        return vendors;
    }

    public static List<TopCustomer> GetTopCustomers(int topN = 5, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<TopCustomer>();

        var customers = new List<TopCustomer>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
SELECT 
    CustomerId,
    CustomerName,
    SUM(Total) as TotalPurchased,
    COUNT(*) as PurchaseCount,
    MAX(SaleDate) as LastPurchaseDate
FROM sales
WHERE Status = 'ACTIVA'";

            if (fromDate.HasValue)
                sql += " AND SaleDate >= @FromDate";

            if (toDate.HasValue)
                sql += " AND SaleDate <= @ToDate";

            sql += @"
GROUP BY CustomerId, CustomerName
ORDER BY TotalPurchased DESC
LIMIT @TopN";

            using var command = new MySqlCommand(sql, connection);
            
            if (fromDate.HasValue)
                command.Parameters.AddWithValue("@FromDate", fromDate.Value.Date);

            if (toDate.HasValue)
                command.Parameters.AddWithValue("@ToDate", toDate.Value.Date.AddDays(1).AddTicks(-1));

            command.Parameters.AddWithValue("@TopN", topN);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                customers.Add(new TopCustomer
                {
                    CustomerId = Convert.ToInt32(reader["CustomerId"]),
                    CustomerName = reader["CustomerName"]?.ToString() ?? "",
                    TotalPurchased = Convert.ToDecimal(reader["TotalPurchased"]),
                    PurchaseCount = Convert.ToInt32(reader["PurchaseCount"]),
                    LastPurchaseDate = reader["LastPurchaseDate"] != DBNull.Value 
                        ? Convert.ToDateTime(reader["LastPurchaseDate"]) 
                        : null
                });
            }
        }
        catch
        {
            return new List<TopCustomer>();
        }

        return customers;
    }

    public static List<SalesTrend> GetSalesTrend(int days = 30)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new List<SalesTrend>();

        var trends = new List<SalesTrend>();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var sql = @"
SELECT 
    DATE(SaleDate) as Period,
    SUM(Total) as TotalSales,
    COUNT(*) as SalesCount,
    SUM(TotalCommissionAmount) as TotalCommission
FROM sales
WHERE Status = 'ACTIVA'
  AND SaleDate >= DATE_SUB(CURDATE(), INTERVAL @Days DAY)
GROUP BY DATE(SaleDate)
ORDER BY Period ASC";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Days", days);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                trends.Add(new SalesTrend
                {
                    Period = Convert.ToDateTime(reader["Period"]).ToString("dd/MM"),
                    TotalSales = Convert.ToDecimal(reader["TotalSales"]),
                    SalesCount = Convert.ToInt32(reader["SalesCount"]),
                    TotalCommission = Convert.ToDecimal(reader["TotalCommission"])
                });
            }
        }
        catch
        {
            return new List<SalesTrend>();
        }

        return trends;
    }

    public static DashboardComparison GetComparisonWithPreviousPeriod(DateTime fromDate, DateTime toDate)
    {
        var connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return new DashboardComparison();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            var comparison = new DashboardComparison
            {
                CurrentPeriodStart = fromDate,
                CurrentPeriodEnd = toDate,
                PreviousPeriodStart = fromDate.AddDays(-(toDate - fromDate).Days),
                PreviousPeriodEnd = fromDate.AddDays(-1)
            };

            // Current period metrics
            comparison.CurrentTotalSales = GetTotalSales(connection, fromDate, toDate);
            comparison.CurrentSalesCount = GetSalesCount(connection, fromDate, toDate);
            comparison.CurrentTotalCommission = GetTotalCommission(connection, fromDate, toDate);

            // Previous period metrics
            comparison.PreviousTotalSales = GetTotalSales(connection, comparison.PreviousPeriodStart, comparison.PreviousPeriodEnd);
            comparison.PreviousSalesCount = GetSalesCount(connection, comparison.PreviousPeriodStart, comparison.PreviousPeriodEnd);
            comparison.PreviousTotalCommission = GetTotalCommission(connection, comparison.PreviousPeriodStart, comparison.PreviousPeriodEnd);

            // Calculate percentages
            if (comparison.PreviousTotalSales > 0)
                comparison.SalesGrowthPercentage = ((comparison.CurrentTotalSales - comparison.PreviousTotalSales) / comparison.PreviousTotalSales) * 100;

            if (comparison.PreviousSalesCount > 0)
                comparison.CountGrowthPercentage = ((comparison.CurrentSalesCount - comparison.PreviousSalesCount) / (double)comparison.PreviousSalesCount) * 100;

            if (comparison.PreviousTotalCommission > 0)
                comparison.CommissionGrowthPercentage = ((comparison.CurrentTotalCommission - comparison.PreviousTotalCommission) / comparison.PreviousTotalCommission) * 100;

            return comparison;
        }
        catch
        {
            return new DashboardComparison();
        }
    }

    private static decimal GetTotalSales(MySqlConnection connection, DateTime fromDate, DateTime toDate)
    {
        var sql = @"
SELECT COALESCE(SUM(Total), 0)
FROM sales
WHERE Status = 'ACTIVA'
  AND SaleDate BETWEEN @FromDate AND @ToDate";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FromDate", fromDate);
        command.Parameters.AddWithValue("@ToDate", toDate);

        var result = command.ExecuteScalar();
        return result != null ? Convert.ToDecimal(result) : 0;
    }

    private static int GetSalesCount(MySqlConnection connection, DateTime fromDate, DateTime toDate)
    {
        var sql = @"
SELECT COUNT(*)
FROM sales
WHERE Status = 'ACTIVA'
  AND SaleDate BETWEEN @FromDate AND @ToDate";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FromDate", fromDate);
        command.Parameters.AddWithValue("@ToDate", toDate);

        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    private static decimal GetTotalCommission(MySqlConnection connection, DateTime fromDate, DateTime toDate)
    {
        var sql = @"
SELECT COALESCE(SUM(TotalCommissionAmount), 0)
FROM sales
WHERE Status = 'ACTIVA'
  AND SaleDate BETWEEN @FromDate AND @ToDate";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FromDate", fromDate);
        command.Parameters.AddWithValue("@ToDate", toDate);

        var result = command.ExecuteScalar();
        return result != null ? Convert.ToDecimal(result) : 0;
    }

    private static decimal GetTotalSales(MySqlConnection connection, DateTime? fromDate, DateTime? toDate)
    {
        var sql = "SELECT COALESCE(SUM(Total), 0) FROM sales WHERE Status = 'ACTIVA'";
        
        if (fromDate.HasValue)
            sql += " AND SaleDate >= @FromDate";

        if (toDate.HasValue)
            sql += " AND SaleDate <= @ToDate";

        using var command = new MySqlCommand(sql, connection);
        
        if (fromDate.HasValue)
            command.Parameters.AddWithValue("@FromDate", fromDate.Value.Date);

        if (toDate.HasValue)
            command.Parameters.AddWithValue("@ToDate", toDate.Value.Date.AddDays(1).AddTicks(-1));

        return Convert.ToDecimal(command.ExecuteScalar());
    }

    private static decimal GetTotalCommission(MySqlConnection connection, DateTime? fromDate, DateTime? toDate)
    {
        var sql = "SELECT COALESCE(SUM(TotalCommissionAmount), 0) FROM sales WHERE Status = 'ACTIVA'";
        
        if (fromDate.HasValue)
            sql += " AND SaleDate >= @FromDate";

        if (toDate.HasValue)
            sql += " AND SaleDate <= @ToDate";

        using var command = new MySqlCommand(sql, connection);
        
        if (fromDate.HasValue)
            command.Parameters.AddWithValue("@FromDate", fromDate.Value.Date);

        if (toDate.HasValue)
            command.Parameters.AddWithValue("@ToDate", toDate.Value.Date.AddDays(1).AddTicks(-1));

        return Convert.ToDecimal(command.ExecuteScalar());
    }

    private static int GetSalesCount(MySqlConnection connection, DateTime? fromDate, DateTime? toDate)
    {
        var sql = "SELECT COUNT(*) FROM sales WHERE Status = 'ACTIVA'";
        
        if (fromDate.HasValue)
            sql += " AND SaleDate >= @FromDate";

        if (toDate.HasValue)
            sql += " AND SaleDate <= @ToDate";

        using var command = new MySqlCommand(sql, connection);
        
        if (fromDate.HasValue)
            command.Parameters.AddWithValue("@FromDate", fromDate.Value.Date);

        if (toDate.HasValue)
            command.Parameters.AddWithValue("@ToDate", toDate.Value.Date.AddDays(1).AddTicks(-1));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static decimal GetAverageSale(MySqlConnection connection, DateTime? fromDate, DateTime? toDate)
    {
        var sql = "SELECT COALESCE(AVG(Total), 0) FROM sales WHERE Status = 'ACTIVA'";
        
        if (fromDate.HasValue)
            sql += " AND SaleDate >= @FromDate";

        if (toDate.HasValue)
            sql += " AND SaleDate <= @ToDate";

        using var command = new MySqlCommand(sql, connection);
        
        if (fromDate.HasValue)
            command.Parameters.AddWithValue("@FromDate", fromDate.Value.Date);

        if (toDate.HasValue)
            command.Parameters.AddWithValue("@ToDate", toDate.Value.Date.AddDays(1).AddTicks(-1));

        return Convert.ToDecimal(command.ExecuteScalar());
    }

    private static int GetActiveVendorsCount(MySqlConnection connection)
    {
        const string sql = "SELECT COUNT(*) FROM vendors WHERE IsActive = true";
        
        using var command = new MySqlCommand(sql, connection);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static int GetActiveCustomersCount(MySqlConnection connection)
    {
        const string sql = "SELECT COUNT(*) FROM customers WHERE IsActive = true";
        
        using var command = new MySqlCommand(sql, connection);
        return Convert.ToInt32(command.ExecuteScalar());
    }
}
