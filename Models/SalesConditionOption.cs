namespace SMART_ERP.Models;

public class SalesConditionOption
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int CreditDays { get; set; }

    public bool IsActive { get; set; } = true;
}
