namespace SMART_ERP.Models;

public class AccountsReceivable
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING, PARTIAL, PAID, OVERDUE
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
