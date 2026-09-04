namespace SMART_ERP.Models;

public class Sale
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "ACTIVA"; // ACTIVA, ANULADA, PENDIENTE

    // Empresa facturadora
    public int BillingCompanyId { get; set; }

    public string BillingCompanyName { get; set; } = string.Empty;

    // Área / unidad operativa que realiza el trabajo
    public int OperationalAreaId { get; set; }

    public string OperationalAreaName { get; set; } = string.Empty;
    // Cliente
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    // Vendedor Principal
    public int PrimaryVendorId { get; set; }
    public string PrimaryVendorName { get; set; } = string.Empty;
    public decimal PrimaryCommissionPercentage { get; set; }
    public decimal PrimaryCommissionAmount { get; set; }

    // Venta Especial (2 vendedores)
    public bool IsSpecialSale { get; set; }
    public int? SecondaryVendorId { get; set; }
    public string SecondaryVendorName { get; set; } = string.Empty;
    public decimal SecondaryCommissionPercentage { get; set; }
    public decimal SecondaryCommissionAmount { get; set; }

    // Totales
    public decimal Total { get; set; }
    public decimal CommissionBase { get; set; }
    public decimal TotalCommissionPercentage { get; set; }
    public decimal TotalCommissionAmount { get; set; }

    // Condiciones de venta
    public string PaymentMethod { get; set; } = "EFECTIVO"; // EFECTIVO, CONTADO, CRÉDITO
    public int CreditDays { get; set; }

    // Observaciones
    public string Notes { get; set; } = string.Empty;

    // Auditoría
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = string.Empty;
}


