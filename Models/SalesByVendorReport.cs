namespace SMART_ERP.Models;

public class SalesByVendorReport
{
    public string VendorName { get; set; } = string.Empty;
    public string VendorCode { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal AverageSale { get; set; }
    public DateTime FirstSaleDate { get; set; }
    public DateTime LastSaleDate { get; set; }
}
