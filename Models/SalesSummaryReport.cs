namespace SMART_ERP.Models;

public class SalesSummaryReport
{
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string BillingCompanyName { get; set; } = string.Empty;
    public string OperationalAreaName { get; set; } = string.Empty;
    public string PrimaryVendorName { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public int CreditDays { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal CommissionBase { get; set; }
    public decimal CommissionPercentage { get; set; }
    public decimal CommissionAmount { get; set; }
}

public class SalesSummaryTotals
{
    public int SalesCount { get; set; }
    public decimal TotalSold { get; set; }
    public decimal TotalCommissionBase { get; set; }
    public decimal TotalCommissions { get; set; }
}
