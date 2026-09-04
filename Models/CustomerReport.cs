namespace SMART_ERP.Models;

public class CustomerReport
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
    public decimal TotalPurchased { get; set; }
    public decimal AveragePurchase { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
}
