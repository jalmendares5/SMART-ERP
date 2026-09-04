namespace SMART_ERP.Models;

public class PaymentMethodOption
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
