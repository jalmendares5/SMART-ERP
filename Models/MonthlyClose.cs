namespace SMART_ERP.Models
{
    public class MonthlyClose
    {
        public int Id { get; set; }
        
        public int Year { get; set; }
        
        public int Month { get; set; }
        
        public decimal TotalSales { get; set; }
        
        public decimal TotalQuantity { get; set; }
        
        public decimal TotalCommission { get; set; }
        
        public bool IsClosed { get; set; }
        
        public DateTime ClosedAt { get; set; }
        
        public string ClosedBy { get; set; } = string.Empty;
        
        public string Notes { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
