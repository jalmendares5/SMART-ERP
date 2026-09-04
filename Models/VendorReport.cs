namespace SMART_ERP.Models;

public class VendorReport
{
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal TotalSold { get; set; }
    public decimal CommissionBase { get; set; }
    public decimal CommissionGenerated { get; set; }
    public decimal AverageSale { get; set; }
}
