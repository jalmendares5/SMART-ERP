namespace SMART_ERP.Models;

public class AccountsPayable
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public string PurchaseNumber { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING, PARTIAL, PAID, OVERDUE
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
