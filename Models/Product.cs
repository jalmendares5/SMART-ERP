namespace SMART_ERP.Models;

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public decimal Price1 { get; set; }
    public decimal Price2 { get; set; }
    public decimal Price3 { get; set; }
    public decimal Price4 { get; set; }
    public decimal Stock { get; set; }
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public string Unit { get; set; } = "UNIDAD";
    public string BarCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = string.Empty;
}
