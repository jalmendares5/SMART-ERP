namespace SMART_ERP.Models;

public class OperationalAreaReport
{
    public int AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal TotalSold { get; set; }
    public decimal Commissions { get; set; }
}
