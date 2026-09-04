namespace SMART_ERP.Models;

public class BillingCompanyReport
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int SalesCount { get; set; }
    public decimal TotalSold { get; set; }
    public decimal CommissionBase { get; set; }
    public decimal Commissions { get; set; }
}
