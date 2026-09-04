namespace SMART_ERP.Models;

public class CashTransaction
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public string TransactionType { get; set; } = "IN"; // IN (ingreso), OUT (egreso)
    public string Category { get; set; } = string.Empty; // VENTAS, PAGOS, COMPRAS, RETIRO, DEPOSITO
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ReferenceType { get; set; } = string.Empty; // FACTURA, COMPRA, PAGO_CLIENTE, PAGO_PROVEEDOR
    public int ReferenceId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = string.Empty;
}
