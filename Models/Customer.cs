namespace SMART_ERP.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string PaymentTerms { get; set; } = "CONTADO"; // CONTADO, CREDITO
    public int CreditDays { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public string PriceLevel { get; set; } = "PRECIO DE VENTA 1";
    public string Salesperson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Rtn { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = "HONDURAS";
    public string Department { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

