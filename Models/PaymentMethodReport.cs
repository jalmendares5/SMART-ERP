namespace SMART_ERP.Models;

public class PaymentMethodReport
{
    public string PaymentMethod { get; set; } = string.Empty;
    public int OperationCount { get; set; }
    public decimal Total { get; set; }
    public decimal PercentageOfTotal { get; set; }
}
