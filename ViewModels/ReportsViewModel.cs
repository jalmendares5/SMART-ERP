using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SMART_ERP.ViewModels;

public class ReportsViewModel : BaseViewModel
{
    private SalesReportFilter _filter = new();
    private string _selectedReportType = "General";
    private int _selectedTabIndex = 0;
    private string _errorMessage = string.Empty;

    public ReportsViewModel()
    {
        ApplyFilterCommand = new RelayCommand(ExecuteApplyFilter);
        ClearFilterCommand = new RelayCommand(ExecuteClearFilter);
        RefreshCommand = new RelayCommand(ExecuteRefresh);
        ExportToExcelCommand = new RelayCommand(ExecuteExportToExcel);
        ExportToCsvCommand = new RelayCommand(ExecuteExportToCsv);

        InitDefaultFilter();
        LoadFilterOptions();
        LoadReportData();
    }

    private void InitDefaultFilter()
    {
        _filter = new SalesReportFilter
        {
            BillingCompanyId = 0,
            OperationalAreaId = 0,
            CustomerId = 0,
            VendorId = 0,
            Status = "TODOS",
            PaymentMethod = "-- Todas las formas de pago --"
        };
        OnPropertyChanged(nameof(Filter));
    }

    #region Properties

    public SalesReportFilter Filter
    {
        get => _filter;
        set => SetProperty(ref _filter, value);
    }

    public string SelectedReportType
    {
        get => _selectedReportType;
        set
        {
            if (SetProperty(ref _selectedReportType, value))
            {
                int newIndex = ReportTypes.IndexOf(value);
                if (newIndex >= 0 && newIndex != _selectedTabIndex)
                {
                    _selectedTabIndex = newIndex;
                    OnPropertyChanged(nameof(SelectedTabIndex));
                }
                LoadReportData();
            }
        }
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (SetProperty(ref _selectedTabIndex, value))
            {
                if (value >= 0 && value < ReportTypes.Count)
                {
                    _selectedReportType = ReportTypes[value];
                    OnPropertyChanged(nameof(SelectedReportType));
                }
                LoadReportData();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // Filter Options
    public ObservableCollection<string> ReportTypes { get; } = new()
    {
        "General",
        "Por Vendedor",
        "Por Cliente",
        "Por Empresa Facturadora",
        "Por Área Operativa",
        "Por Forma de Pago"
    };

    public ObservableCollection<BillingCompany> BillingCompanies { get; } = new();
    public ObservableCollection<OperationalArea> OperationalAreas { get; } = new();
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Vendor> Vendors { get; } = new();
    public ObservableCollection<string> Statuses { get; } = new() { "TODOS", "ACTIVA", "ANULADA", "PENDIENTE" };
    public ObservableCollection<string> PaymentMethods { get; } = new();

    // Report Data
    public ObservableCollection<SalesSummaryReport> SalesSummaryData { get; } = new();
    public ObservableCollection<VendorReport> VendorReportData { get; } = new();
    public ObservableCollection<CustomerReport> CustomerReportData { get; } = new();
    public ObservableCollection<BillingCompanyReport> BillingCompanyReportData { get; } = new();
    public ObservableCollection<OperationalAreaReport> OperationalAreaReportData { get; } = new();
    public ObservableCollection<PaymentMethodReport> PaymentMethodReportData { get; } = new();

    // Totals
    private SalesSummaryTotals _salesSummaryTotals = new();
    public SalesSummaryTotals SalesSummaryTotals
    {
        get => _salesSummaryTotals;
        set => SetProperty(ref _salesSummaryTotals, value);
    }

    #endregion

    #region Commands

    public ICommand ApplyFilterCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ExportToExcelCommand { get; }
    public ICommand ExportToCsvCommand { get; }

    #endregion

    #region Methods

    private void LoadFilterOptions()
    {
        try
        {
            BillingCompanies.Clear();
            BillingCompanies.Add(new BillingCompany { Id = 0, Name = "-- Todas las empresas --" });
            foreach (var company in BillingCompanyService.GetAll())
                BillingCompanies.Add(company);

            OperationalAreas.Clear();
            OperationalAreas.Add(new OperationalArea { Id = 0, Name = "-- Todas las áreas --" });
            foreach (var area in OperationalAreaService.GetAll())
                OperationalAreas.Add(area);

            Customers.Clear();
            Customers.Add(new Customer { Id = 0, Name = "-- Todos los clientes --" });
            foreach (var customer in CustomerService.GetAll())
                Customers.Add(customer);

            Vendors.Clear();
            Vendors.Add(new Vendor { Id = 0, Name = "-- Todos los vendedores --" });
            foreach (var vendor in VendorService.GetAll())
                Vendors.Add(vendor);

            PaymentMethods.Clear();
            PaymentMethods.Add("-- Todas las formas de pago --");
            foreach (var method in PaymentMethodService.GetAll())
                PaymentMethods.Add(method.Name);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar opciones de filtro: {ex.Message}";
        }
    }

    private void ExecuteApplyFilter(object? parameter)
    {
        LoadReportData();
    }

    private void ExecuteClearFilter(object? parameter)
    {
        InitDefaultFilter();
        LoadReportData();
    }

    public void ExecuteRefresh(object? parameter)
    {
        LoadFilterOptions();
        LoadReportData();
    }

    private void ExecuteExportToExcel(object? parameter)
    {
        try
        {
            switch (SelectedReportType)
            {
                case "General":
                    ExcelExportService.ExportSalesSummaryToExcel(SalesSummaryData, SalesSummaryTotals);
                    break;
                case "Por Vendedor":
                    ExcelExportService.ExportVendorReportToExcel(VendorReportData);
                    break;
                case "Por Cliente":
                    ExcelExportService.ExportCustomerReportToExcel(CustomerReportData);
                    break;
                case "Por Empresa Facturadora":
                    ExcelExportService.ExportBillingCompanyReportToExcel(BillingCompanyReportData);
                    break;
                case "Por Área Operativa":
                    ExcelExportService.ExportOperationalAreaReportToExcel(OperationalAreaReportData);
                    break;
                case "Por Forma de Pago":
                    ExcelExportService.ExportPaymentMethodReportToExcel(PaymentMethodReportData);
                    break;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al exportar a Excel: {ex.Message}";
        }
    }

    private void ExecuteExportToCsv(object? parameter)
    {
        try
        {
            switch (SelectedReportType)
            {
                case "General":
                    CsvExportService.ExportSalesSummaryToCsv(SalesSummaryData, SalesSummaryTotals);
                    break;
                case "Por Vendedor":
                    CsvExportService.ExportVendorReportToCsv(VendorReportData);
                    break;
                case "Por Cliente":
                    CsvExportService.ExportCustomerReportToCsv(CustomerReportData);
                    break;
                case "Por Empresa Facturadora":
                    CsvExportService.ExportBillingCompanyReportToCsv(BillingCompanyReportData);
                    break;
                case "Por Área Operativa":
                    CsvExportService.ExportOperationalAreaReportToCsv(OperationalAreaReportData);
                    break;
                case "Por Forma de Pago":
                    CsvExportService.ExportPaymentMethodReportToCsv(PaymentMethodReportData);
                    break;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al exportar a CSV: {ex.Message}";
        }
    }

    private void LoadReportData()
    {
        try
        {
            ErrorMessage = string.Empty;

            switch (SelectedReportType)
            {
                case "General":
                    LoadSalesSummaryReport();
                    break;
                case "Por Vendedor":
                    LoadVendorReport();
                    break;
                case "Por Cliente":
                    LoadCustomerReport();
                    break;
                case "Por Empresa Facturadora":
                    LoadBillingCompanyReport();
                    break;
                case "Por Área Operativa":
                    LoadOperationalAreaReport();
                    break;
                case "Por Forma de Pago":
                    LoadPaymentMethodReport();
                    break;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar reporte: {ex.Message}";
        }
    }

    private void LoadSalesSummaryReport()
    {
        var data = ReportService.GetSalesSummary(Filter);
        SalesSummaryData.Clear();
        foreach (var item in data)
            SalesSummaryData.Add(item);

        SalesSummaryTotals = ReportService.GetSalesSummaryTotals(data.ToList());
    }

    private void LoadVendorReport()
    {
        var data = ReportService.GetVendorReport(Filter);
        VendorReportData.Clear();
        foreach (var item in data)
            VendorReportData.Add(item);
    }

    private void LoadCustomerReport()
    {
        var data = ReportService.GetCustomerReport(Filter);
        CustomerReportData.Clear();
        foreach (var item in data)
            CustomerReportData.Add(item);
    }

    private void LoadBillingCompanyReport()
    {
        var data = ReportService.GetBillingCompanyReport(Filter);
        BillingCompanyReportData.Clear();
        foreach (var item in data)
            BillingCompanyReportData.Add(item);
    }

    private void LoadOperationalAreaReport()
    {
        var data = ReportService.GetOperationalAreaReport(Filter);
        OperationalAreaReportData.Clear();
        foreach (var item in data)
            OperationalAreaReportData.Add(item);
    }

    private void LoadPaymentMethodReport()
    {
        var data = ReportService.GetPaymentMethodReport(Filter);
        PaymentMethodReportData.Clear();
        foreach (var item in data)
            PaymentMethodReportData.Add(item);
    }

    #endregion
}
