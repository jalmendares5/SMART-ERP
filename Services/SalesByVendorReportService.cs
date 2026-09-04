using MySqlConnector;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class SalesByVendorReportService
{
    public static async Task<List<SalesByVendorReport>> GenerateReportAsync(
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var reports = new List<SalesByVendorReport>();

        string? connectionString = CompanyConnectionService.GetActiveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            return reports;

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        string sql = @"
SELECT 
    s.salesperson as VendorName,
    s.salesperson as VendorCode,
    COUNT(*) as SalesCount,
    SUM(s.total_amount) as TotalSales,
    SUM(s.quantity) as TotalQuantity,
    SUM(s.commission) as TotalCommission,
    AVG(s.total_amount) as AverageSale,
    MIN(s.sale_date) as FirstSaleDate,
    MAX(s.sale_date) as LastSaleDate
FROM sales s
WHERE (@StartDate IS NULL OR s.sale_date >= @StartDate)
  AND (@EndDate IS NULL OR s.sale_date <= @EndDate)
GROUP BY s.salesperson
ORDER BY TotalSales DESC;
";

        await using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@StartDate", startDate ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@EndDate", endDate ?? (object)DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            reports.Add(new SalesByVendorReport
            {
                VendorName = reader.IsDBNull(reader.GetOrdinal("VendorName")) ? "Sin Vendedor" : reader.GetString(reader.GetOrdinal("VendorName")),
                VendorCode = reader.IsDBNull(reader.GetOrdinal("VendorCode")) ? "N/A" : reader.GetString(reader.GetOrdinal("VendorCode")),
                SalesCount = reader.GetInt32(reader.GetOrdinal("SalesCount")),
                TotalSales = reader.GetDecimal(reader.GetOrdinal("TotalSales")),
                TotalQuantity = reader.GetDecimal(reader.GetOrdinal("TotalQuantity")),
                TotalCommission = reader.GetDecimal(reader.GetOrdinal("TotalCommission")),
                AverageSale = reader.GetDecimal(reader.GetOrdinal("AverageSale")),
                FirstSaleDate = reader.IsDBNull(reader.GetOrdinal("FirstSaleDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("FirstSaleDate")),
                LastSaleDate = reader.IsDBNull(reader.GetOrdinal("LastSaleDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LastSaleDate"))
            });
        }

        return reports;
    }
}
