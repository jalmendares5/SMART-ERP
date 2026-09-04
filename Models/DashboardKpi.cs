namespace SMART_ERP.Models;

public class DashboardKpi
{
    public string Title { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Format { get; set; } = "N2";
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = "#0F172A";
    public decimal? ChangePercentage { get; set; }
    public string? Trend { get; set; } // "up", "down", "neutral"
}

public class DashboardSummary
{
    public DateTime ReportDate { get; set; }
    public DashboardKpi TotalSales { get; set; } = new();
    public DashboardKpi TotalCommission { get; set; } = new();
    public DashboardKpi SalesCount { get; set; } = new();
    public DashboardKpi AverageSale { get; set; } = new();
    public DashboardKpi ActiveVendors { get; set; } = new();
    public DashboardKpi ActiveCustomers { get; set; } = new();
}

public class DashboardComparison
{
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime PreviousPeriodStart { get; set; }
    public DateTime PreviousPeriodEnd { get; set; }
    
    public decimal CurrentTotalSales { get; set; }
    public decimal PreviousTotalSales { get; set; }
    public decimal SalesGrowthPercentage { get; set; }
    
    public int CurrentSalesCount { get; set; }
    public int PreviousSalesCount { get; set; }
    public double CountGrowthPercentage { get; set; }
    
    public decimal CurrentTotalCommission { get; set; }
    public decimal PreviousTotalCommission { get; set; }
    public decimal CommissionGrowthPercentage { get; set; }
}

public class TopVendor
{
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int SalesCount { get; set; }
    public decimal CommissionGenerated { get; set; }
}

public class TopCustomer
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalPurchased { get; set; }
    public int PurchaseCount { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
}

public class SalesTrend
{
    public string Period { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int SalesCount { get; set; }
    public decimal TotalCommission { get; set; }
}
