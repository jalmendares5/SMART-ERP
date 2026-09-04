namespace SMART_ERP.Models;

public class Vendor
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public decimal CommissionPercentage { get; set; } = 3.0m;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string IdentityNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; } = DateTime.Today;

    public string Note { get; set; } = string.Empty;

    public string PhotoPath { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

