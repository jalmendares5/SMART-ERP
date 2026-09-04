namespace SMART_ERP.Models;

public class SalesReportFilter
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? BillingCompanyId { get; set; }
    public int? OperationalAreaId { get; set; }
    public int? CustomerId { get; set; }
    public int? VendorId { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }

    public bool HasFilters => 
        DateFrom.HasValue || 
        DateTo.HasValue || 
        BillingCompanyId.HasValue || 
        OperationalAreaId.HasValue || 
        CustomerId.HasValue || 
        VendorId.HasValue || 
        !string.IsNullOrWhiteSpace(Status) || 
        !string.IsNullOrWhiteSpace(PaymentMethod);
}
