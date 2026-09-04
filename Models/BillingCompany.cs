namespace SMART_ERP.Models;

public class BillingCompany
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LegalName { get; set; } = string.Empty;

    public string TaxId { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
