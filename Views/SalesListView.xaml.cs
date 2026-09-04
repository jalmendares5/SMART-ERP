using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Windows;
using System.Windows.Controls;

namespace SMART_ERP.Views;

public partial class SalesListView : UserControl
{
    private List<Sale> _allSales = new();

    public SalesListView()
    {
        InitializeComponent();
        Reload();
    }

    public void Reload()
    {
        _allSales = SaleService.GetAll();

        LoadFilterOptions();
        ApplyFilters();
    }

    private void LoadFilterOptions()
    {
        CmbBillingCompany.Items.Clear();
        CmbBillingCompany.Items.Add("Todas");

        foreach (var company in BillingCompanyService.GetAll())
        {
            CmbBillingCompany.Items.Add(company.Name);
        }

        CmbBillingCompany.SelectedIndex = 0;


        CmbOperationalArea.Items.Clear();
        CmbOperationalArea.Items.Add("Todas");

        foreach (var area in OperationalAreaService.GetAll())
        {
            CmbOperationalArea.Items.Add(area.Name);
        }

        CmbOperationalArea.SelectedIndex = 0;


        CmbCustomer.Items.Clear();
        CmbCustomer.Items.Add("Todos");

        foreach (var customer in CustomerService.GetAll())
        {
            CmbCustomer.Items.Add(customer.Name);
        }

        CmbCustomer.SelectedIndex = 0;


        CmbVendor.Items.Clear();
        CmbVendor.Items.Add("Todos");

        foreach (var vendor in VendorService.GetAll())
        {
            CmbVendor.Items.Add(vendor.Name);
        }

        CmbVendor.SelectedIndex = 0;
    }

    private void ApplyFilters()
    {
        IEnumerable<Sale> filtered = _allSales;

        if (CmbBillingCompany.SelectedIndex > 0)
        {
            string company = CmbBillingCompany.SelectedItem?.ToString() ?? string.Empty;

            filtered = filtered.Where(s =>
                s.BillingCompanyName.Equals(
                    company,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (CmbOperationalArea.SelectedIndex > 0)
        {
            string area = CmbOperationalArea.SelectedItem?.ToString() ?? string.Empty;

            filtered = filtered.Where(s =>
                s.OperationalAreaName.Equals(
                    area,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (CmbCustomer.SelectedIndex > 0)
        {
            string customer = CmbCustomer.SelectedItem?.ToString() ?? string.Empty;

            filtered = filtered.Where(s =>
                s.CustomerName.Equals(
                    customer,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (CmbVendor.SelectedIndex > 0)
        {
            string vendor = CmbVendor.SelectedItem?.ToString() ?? string.Empty;

            filtered = filtered.Where(s =>
                s.PrimaryVendorName.Equals(
                    vendor,
                    StringComparison.OrdinalIgnoreCase) ||
                s.SecondaryVendorName.Equals(
                    vendor,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (DpDateFrom.SelectedDate.HasValue)
        {
            DateTime from = DpDateFrom.SelectedDate.Value.Date;

            filtered = filtered.Where(s =>
                s.SaleDate.Date >= from);
        }

        if (DpDateTo.SelectedDate.HasValue)
        {
            DateTime to = DpDateTo.SelectedDate.Value.Date;

            filtered = filtered.Where(s =>
                s.SaleDate.Date <= to);
        }

        DgSales.ItemsSource = null;
        DgSales.ItemsSource = filtered
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized)
            return;

        ApplyFilters();
    }

    private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!IsInitialized)
            return;

        ApplyFilters();
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        Reload();
    }
}
