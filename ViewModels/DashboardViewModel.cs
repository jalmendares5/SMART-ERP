using LiveCharts;
using LiveCharts.Wpf;
using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace SMART_ERP.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private DateTime _filterFromDate = DateTime.Today.AddDays(-30);
    private DateTime _filterToDate = DateTime.Today;
    private string _errorMessage = string.Empty;

    public DashboardViewModel()
    {
        RefreshCommand = new RelayCommand(ExecuteRefresh);
        ResetFiltersCommand = new RelayCommand(ExecuteResetFilters);
        ExportDashboardCommand = new RelayCommand(ExecuteExportDashboard);

        LoadDashboardData();
    }

    #region Properties

    public DateTime FilterFromDate
    {
        get => _filterFromDate;
        set
        {
            if (SetProperty(ref _filterFromDate, value))
            {
                LoadDashboardData();
            }
        }
    }

    public DateTime FilterToDate
    {
        get => _filterToDate;
        set
        {
            if (SetProperty(ref _filterToDate, value))
            {
                LoadDashboardData();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DashboardSummary Summary { get; set; } = new();

    public ObservableCollection<TopVendor> TopVendors { get; } = new();
    public ObservableCollection<TopCustomer> TopCustomers { get; } = new();
    public ObservableCollection<SalesTrend> SalesTrends { get; } = new();
    public DashboardComparison Comparison { get; set; } = new();

    // Chart Properties
    public SeriesCollection SalesSeries { get; set; } = new SeriesCollection();
    public ObservableCollection<string> TrendLabels { get; set; } = new ObservableCollection<string>();

    #endregion

    #region Commands

    public ICommand RefreshCommand { get; }
    public ICommand ResetFiltersCommand { get; }
    public ICommand ExportDashboardCommand { get; }

    #endregion

    #region Methods

    public void ExecuteRefresh(object? parameter)
    {
        LoadDashboardData();
    }

    private void ExecuteResetFilters(object? parameter)
    {
        FilterFromDate = DateTime.Today.AddDays(-30);
        FilterToDate = DateTime.Today;
    }

    private void LoadDashboardData()
    {
        try
        {
            ErrorMessage = string.Empty;

            // Load Summary
            Summary = DashboardService.GetDashboardSummary(FilterFromDate, FilterToDate);
            OnPropertyChanged(nameof(Summary));

            // Load Top Vendors
            var topVendors = DashboardService.GetTopVendors(5, FilterFromDate, FilterToDate);
            TopVendors.Clear();
            foreach (var vendor in topVendors)
                TopVendors.Add(vendor);

            // Load Top Customers
            var topCustomers = DashboardService.GetTopCustomers(5, FilterFromDate, FilterToDate);
            TopCustomers.Clear();
            foreach (var customer in topCustomers)
                TopCustomers.Add(customer);

            // Load Sales Trends and Chart
            var salesTrends = DashboardService.GetSalesTrend(30);
            SalesTrends.Clear();
            TrendLabels.Clear();
            
            var salesValues = new ChartValues<decimal>();
            foreach (var trend in salesTrends)
            {
                SalesTrends.Add(trend);
                salesValues.Add(trend.TotalSales);
                TrendLabels.Add(trend.Period);
            }

            SalesSeries.Clear();
            SalesSeries.Add(new LineSeries
            {
                Title = "Ventas",
                Values = salesValues,
                PointGeometry = null,
                Fill = System.Windows.Media.Brushes.Transparent
            });

            OnPropertyChanged(nameof(SalesSeries));
            OnPropertyChanged(nameof(TrendLabels));

            // Load Comparison
            Comparison = DashboardService.GetComparisonWithPreviousPeriod(FilterFromDate, FilterToDate);
            OnPropertyChanged(nameof(Comparison));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar dashboard: {ex.Message}";
        }
    }

    public string CurrencyFormatter(double value)
    {
        return value.ToString("C2", CultureInfo.GetCultureInfo("es-HN"));
    }

    private void ExecuteExportDashboard(object? parameter)
    {
        try
        {
            ExcelExportService.ExportDashboardToExcel(Summary, TopVendors, TopCustomers, SalesTrends, Comparison);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al exportar dashboard: {ex.Message}";
        }
    }

    #endregion
}
