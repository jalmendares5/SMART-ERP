using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Globalization;
using System.Windows;

namespace SMART_ERP.Views;

public partial class SalesCaptureSettingsWindow : Window
{
    public SalesCaptureSettingsWindow()
    {
        Owner = Application.Current.MainWindow;
        InitializeComponent();
        LoadSettings();
        LoadCatalogs();
    }

    private void LoadSettings()
    {
        var s = SalesCaptureSettingsService.Current;


        ChkTaxGravada.IsChecked = s.AllowTaxGravadaIsv;
        ChkTaxMixta.IsChecked = s.AllowTaxMixta;
        ChkTaxExonerada.IsChecked = s.AllowTaxExonerada;
        ChkTaxExenta.IsChecked = s.AllowTaxExenta;

        TxtDefaultPrimaryCommission.Text =
            s.DefaultPrimaryCommission.ToString(CultureInfo.InvariantCulture);

        TxtDefaultSecondaryCommission.Text =
            s.DefaultSecondaryCommission.ToString(CultureInfo.InvariantCulture);

        TxtStatuses.Text = s.StatusesCsv;
        ChkRequireVoidReason.IsChecked =
            s.RequireCancellationReasonWhenVoided;
    }

    private void LoadCatalogs()
    {
        DgBillingCompanies.ItemsSource = null;
        DgBillingCompanies.ItemsSource =
            BillingCompanyService.GetAll();

        DgOperationalAreas.ItemsSource = null;
        DgOperationalAreas.ItemsSource =
            OperationalAreaService.GetAll();

        DgPaymentMethods.ItemsSource = null;
        DgPaymentMethods.ItemsSource =
            PaymentMethodService.GetAll();

        DgSalesConditions.ItemsSource = null;
        DgSalesConditions.ItemsSource =
            SalesConditionService.GetAll();

        DgReceivingAccounts.ItemsSource = null;
        DgReceivingAccounts.ItemsSource =
            ReceivingAccountService.GetAll();
    }

    private void BtnAddBillingCompany_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new CatalogTextInputWindow(
            "Nueva empresa facturadora",
            "Nombre de la empresa:",
            "",
            "Razón social:",
            "",
            "RTN:",
            "");

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        string name = window.Value.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return;

        BillingCompanyService.Add(
            name,
            window.SecondaryValue.Trim(),
            window.TertiaryValue.Trim());

        LoadCatalogs();
    }

    private void BtnEditBillingCompany_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgBillingCompanies.SelectedItem is not BillingCompany company)
        {
            MessageBox.Show(
                "Seleccione una empresa facturadora.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window = new CatalogTextInputWindow(
            "Editar empresa facturadora",
            "Nombre de la empresa:",
            company.Name,
            "Razón social:",
            company.LegalName,
            "RTN:",
            company.TaxId);

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        string name = window.Value.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return;

        BillingCompanyService.Update(
            company.Id,
            name,
            window.SecondaryValue.Trim(),
            window.TertiaryValue.Trim());

        LoadCatalogs();
    }

    private void BtnToggleBillingCompany_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgBillingCompanies.SelectedItem is not BillingCompany company)
        {
            MessageBox.Show(
                "Seleccione una empresa facturadora.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        BillingCompanyService.SetActive(
            company.Id,
            !company.IsActive);

        LoadCatalogs();
    }

    private void BtnAddOperationalArea_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new CatalogTextInputWindow(
            "Nueva área ejecutora",
            "Nombre del área:",
            "",
            "Descripción:",
            "");

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        string name = window.Value.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return;

        OperationalAreaService.Add(
            name,
            window.SecondaryValue.Trim());

        LoadCatalogs();
    }

    private void BtnEditOperationalArea_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgOperationalAreas.SelectedItem is not OperationalArea area)
        {
            MessageBox.Show(
                "Seleccione un área ejecutora.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window = new CatalogTextInputWindow(
            "Editar área ejecutora",
            "Nombre del área:",
            area.Name,
            "Descripción:",
            area.Description);

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        string name = window.Value.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return;

        OperationalAreaService.Update(
            area.Id,
            name,
            window.SecondaryValue.Trim());

        LoadCatalogs();
    }

    private void BtnToggleOperationalArea_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgOperationalAreas.SelectedItem is not OperationalArea area)
        {
            MessageBox.Show(
                "Seleccione un área ejecutora.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        OperationalAreaService.SetActive(
            area.Id,
            !area.IsActive);

        LoadCatalogs();
    }


    // ============================================================
    // FORMAS DE PAGO
    // ============================================================

    private void BtnAddPaymentMethod_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new CatalogTextInputWindow(
            "Nueva forma de pago",
            "Forma de pago:");

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        PaymentMethodService.Add(
            window.Value.Trim());

        LoadCatalogs();
    }

    private void BtnEditPaymentMethod_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgPaymentMethods.SelectedItem is not PaymentMethodOption item)
        {
            MessageBox.Show(
                "Seleccione una forma de pago.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window = new CatalogTextInputWindow(
            "Editar forma de pago",
            "Forma de pago:",
            item.Name);

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        PaymentMethodService.Update(
            item.Id,
            window.Value.Trim());

        LoadCatalogs();
    }

    private void BtnTogglePaymentMethod_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgPaymentMethods.SelectedItem is not PaymentMethodOption item)
        {
            MessageBox.Show(
                "Seleccione una forma de pago.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        PaymentMethodService.SetActive(
            item.Id,
            !item.IsActive);

        LoadCatalogs();
    }

    // ============================================================
    // CONDICIONES DE VENTA
    // ============================================================

    private void BtnAddSalesCondition_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new CatalogTextInputWindow(
            "Nueva condición de venta",
            "Condición:",
            "",
            "Días de crédito:",
            "0");

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        if (!int.TryParse(
                window.SecondaryValue.Trim(),
                out var creditDays) ||
            creditDays < 0)
        {
            MessageBox.Show(
                "Los días de crédito deben ser un número igual o mayor que cero.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        SalesConditionService.Add(
            window.Value.Trim(),
            creditDays);

        LoadCatalogs();
    }

    private void BtnEditSalesCondition_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgSalesConditions.SelectedItem is not SalesConditionOption item)
        {
            MessageBox.Show(
                "Seleccione una condición de venta.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window = new CatalogTextInputWindow(
            "Editar condición de venta",
            "Condición:",
            item.Name,
            "Días de crédito:",
            item.CreditDays.ToString());

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        if (!int.TryParse(
                window.SecondaryValue.Trim(),
                out var creditDays) ||
            creditDays < 0)
        {
            MessageBox.Show(
                "Los días de crédito deben ser un número igual o mayor que cero.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        SalesConditionService.Update(
            item.Id,
            window.Value.Trim(),
            creditDays);

        LoadCatalogs();
    }

    private void BtnToggleSalesCondition_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgSalesConditions.SelectedItem is not SalesConditionOption item)
        {
            MessageBox.Show(
                "Seleccione una condición de venta.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        SalesConditionService.SetActive(
            item.Id,
            !item.IsActive);

        LoadCatalogs();
    }

    // ============================================================
    // CUENTAS RECEPTORAS
    // ============================================================

    private void BtnAddReceivingAccount_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new CatalogTextInputWindow(
            "Nueva cuenta receptora",
            "Nombre de la cuenta:",
            "",
            "Descripción:",
            "");

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        ReceivingAccountService.Add(
            window.Value.Trim(),
            window.SecondaryValue.Trim());

        LoadCatalogs();
    }

    private void BtnEditReceivingAccount_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgReceivingAccounts.SelectedItem is not ReceivingAccountOption item)
        {
            MessageBox.Show(
                "Seleccione una cuenta receptora.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var window = new CatalogTextInputWindow(
            "Editar cuenta receptora",
            "Nombre de la cuenta:",
            item.Name,
            "Descripción:",
            item.Description);

        window.Owner = this;

        if (window.ShowDialog() != true)
            return;

        ReceivingAccountService.Update(
            item.Id,
            window.Value.Trim(),
            window.SecondaryValue.Trim());

        LoadCatalogs();
    }

    private void BtnToggleReceivingAccount_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgReceivingAccounts.SelectedItem is not ReceivingAccountOption item)
        {
            MessageBox.Show(
                "Seleccione una cuenta receptora.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        ReceivingAccountService.SetActive(
            item.Id,
            !item.IsActive);

        LoadCatalogs();
    }
    private void BtnSave_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!decimal.TryParse(
                TxtDefaultPrimaryCommission.Text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var defaultPrimary))
        {
            MessageBox.Show(
                "La comisión principal por defecto no es válida.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!decimal.TryParse(
                TxtDefaultSecondaryCommission.Text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var defaultSecondary))
        {
            MessageBox.Show(
                "La comisión secundaria por defecto no es válida.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var settings = SalesCaptureSettingsService.Current;


        settings.AllowTaxGravadaIsv = ChkTaxGravada.IsChecked == true;
        settings.AllowTaxMixta = ChkTaxMixta.IsChecked == true;
        settings.AllowTaxExonerada = ChkTaxExonerada.IsChecked == true;
        settings.AllowTaxExenta = ChkTaxExenta.IsChecked == true;

        settings.StatusesCsv = TxtStatuses.Text.Trim();

        settings.DefaultPrimaryCommission = defaultPrimary;
        settings.DefaultSecondaryCommission = defaultSecondary;

        settings.RequireCancellationReasonWhenVoided =
            ChkRequireVoidReason.IsChecked == true;

        SalesCaptureSettingsService.Save(settings);

        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
    }
}





