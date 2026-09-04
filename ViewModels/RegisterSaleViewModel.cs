using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SMART_ERP.ViewModels;

public class RegisterSaleViewModel : BaseViewModel
{
    private string _invoiceNumber = string.Empty;
    private DateTime _saleDate = DateTime.Now;
    private string _status = "ACTIVA";
    private string _invoiceTaxType = "GRAVADA ISV";
    private decimal _exoneratedAmount;
    private decimal _exemptAmount;
    private Customer? _selectedCustomer;
    private string _customerSearchText = string.Empty;
    private Vendor? _selectedPrimaryVendor;
   private string _primaryVendorSearchText = string.Empty;
    private decimal _primaryCommissionPercentage;
    private bool _isSpecialSale;
    private Vendor? _selectedSecondaryVendor;
    private string _secondaryVendorSearchText = string.Empty;
    private decimal _secondaryCommissionPercentage;
    private decimal _total;
    private string _paymentMethod = "EFECTIVO";
    private int _creditDays;
    private string _notes = string.Empty;
    private string _errorMessage = string.Empty;

    public RegisterSaleViewModel()
    {
        GuardarCommand = new RelayCommand(ExecuteGuardar, CanExecuteGuardar);
        CancelarCommand = new RelayCommand(ExecuteCancelar);

        LoadCaptureSettings();
        LoadBillingCompaniesAndAreas();

        LoadCustomers();
        LoadVendors();
    }

    #region Properties

    public string InvoiceNumber
    {
        get => _invoiceNumber;
        set
        {
            SetProperty(ref _invoiceNumber, value);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            ErrorMessage = string.Empty;
        }
    }

    public DateTime SaleDate
    {
        get => _saleDate;
        set => SetProperty(ref _saleDate, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public ObservableCollection<string> AvailableStatuses { get; } = new();

    public string InvoiceTaxType
    {
        get => _invoiceTaxType;
        set
        {
            if (SetProperty(ref _invoiceTaxType, value))
            {
                OnPropertyChanged(nameof(ShowExoneratedAmount));
                OnPropertyChanged(nameof(ShowExemptAmount));
            }
        }
    }

    public ObservableCollection<string> AvailableInvoiceTaxTypes { get; } = new();

    public decimal ExoneratedAmount
{
    get => _exoneratedAmount;
    set
    {
        if (SetProperty(ref _exoneratedAmount, value))
        {
            OnPropertyChanged(nameof(TotalFiscal));
OnPropertyChanged(nameof(TaxableBase));
OnPropertyChanged(nameof(IsvAmount));
OnPropertyChanged(nameof(InvoiceTotal));
OnPropertyChanged(nameof(CommissionBase));
OnPropertyChanged(nameof(PrimaryCommissionAmount));
OnPropertyChanged(nameof(SecondaryCommissionAmount));
OnPropertyChanged(nameof(TotalCommissionAmount));
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }
    }
}

    public decimal ExemptAmount
{
    get => _exemptAmount;
    set
    {
        if (SetProperty(ref _exemptAmount, value))
        {
            OnPropertyChanged(nameof(TotalFiscal));
OnPropertyChanged(nameof(TaxableBase));
OnPropertyChanged(nameof(IsvAmount));
OnPropertyChanged(nameof(InvoiceTotal));
OnPropertyChanged(nameof(CommissionBase));
OnPropertyChanged(nameof(PrimaryCommissionAmount));
OnPropertyChanged(nameof(SecondaryCommissionAmount));
OnPropertyChanged(nameof(TotalCommissionAmount));
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }
    }
}

    public bool ShowExoneratedAmount =>
        InvoiceTaxType == "MIXTA" || InvoiceTaxType == "EXONERADA";

    public bool ShowExemptAmount =>
        InvoiceTaxType == "MIXTA" || InvoiceTaxType == "EXENTA";

    public void ReloadCaptureSettings()
    {
        LoadCaptureSettings();
        LoadBillingCompaniesAndAreas();
    }

    public void ReloadVendors()
    {
        LoadVendors();
    }

    private void LoadBillingCompaniesAndAreas()
    {
        BillingCompanies.Clear();

        foreach (var company in BillingCompanyService.GetAll())
        {
            BillingCompanies.Add(company);
        }

        OperationalAreas.Clear();

        foreach (var area in OperationalAreaService.GetAll())
        {
            OperationalAreas.Add(area);
        }

        if (SelectedBillingCompany == null && BillingCompanies.Count > 0)
        {
            SelectedBillingCompany = BillingCompanies[0];
        }

        if (SelectedOperationalArea == null && OperationalAreas.Count > 0)
        {
            SelectedOperationalArea = OperationalAreas[0];
        }
    }
    private void LoadCaptureSettings()
    {
        var settings = SalesCaptureSettingsService.Current;

        AvailableSalesConditions.Clear();

        foreach (var condition in SalesCaptureSettingsService.Current.SalesConditions
                     .Where(x => x.IsActive)
                     .OrderBy(x => x.Name))
        {
            AvailableSalesConditions.Add(condition);
        }

        AvailableReceivingAccounts.Clear();

        foreach (var account in SalesCaptureSettingsService.Current.ReceivingAccounts
                     .Where(x => x.IsActive)
                     .OrderBy(x => x.Name))
        {
            AvailableReceivingAccounts.Add(account);
        }
        AvailablePaymentMethods.Clear();

        foreach (var method in settings.PaymentMethods
                     .Where(x => x.IsActive)
                     .OrderBy(x => x.Name))
        {
            AvailablePaymentMethods.Add(method.Name.ToUpperInvariant());
        }

        InvoiceTaxType = NormalizeFromList(InvoiceTaxType, AvailableInvoiceTaxTypes, "GRAVADA ISV");

        if (PrimaryCommissionPercentage == 0)
        {
            PrimaryCommissionPercentage = settings.DefaultPrimaryCommission;
        }

        if (SecondaryCommissionPercentage == 0)
        {
            SecondaryCommissionPercentage = settings.DefaultSecondaryCommission;
        }
    }

    private static IEnumerable<string> SplitCsv(string csv)
    {
        return (csv ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(v => !string.IsNullOrWhiteSpace(v));
    }

    private static string NormalizeFromList(string? value, IEnumerable<string> options, string fallback)
    {
        var normalized = (value ?? string.Empty).Trim().ToUpperInvariant().Replace("CRÃ‰DITO", "CREDITO");
        var optionList = options.ToList();

        if (optionList.Contains(normalized))
        {
            return normalized;
        }

        var normalizedFallback = fallback.ToUpperInvariant();
        if (optionList.Contains(normalizedFallback))
        {
            return normalizedFallback;
        }

        return optionList.FirstOrDefault() ?? normalizedFallback;
    }

    public string CustomerSearchText
    {
        get => _customerSearchText;
        set
        {
            SetProperty(ref _customerSearchText, value);
            FilterCustomers();
        }
    }

    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            SetProperty(ref _selectedCustomer, value);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            LoadCustomerDefaults();
            ErrorMessage = string.Empty;
        }
    }

    public ObservableCollection<Customer> FilteredCustomers { get; } = new();
    public ObservableCollection<Customer> AllCustomers { get; } = new();

    public string PrimaryVendorSearchText
    {
        get => _primaryVendorSearchText;
        set
        {
            SetProperty(ref _primaryVendorSearchText, value);
            FilterPrimaryVendors();
        }
    }

    public Vendor? SelectedPrimaryVendor
    {
        get => _selectedPrimaryVendor;
        set
        {
            SetProperty(ref _selectedPrimaryVendor, value);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            LoadPrimaryVendorDefaults();
            ErrorMessage = string.Empty;
        }
    }

    public ObservableCollection<Vendor> FilteredPrimaryVendors { get; } = new();

    public decimal PrimaryCommissionPercentage
    {
        get => _primaryCommissionPercentage;
        set
        {
            SetProperty(ref _primaryCommissionPercentage, value);
            UpdateCommissionCalculations();
        }
    }

    public bool IsSpecialSale
    {
        get => _isSpecialSale;
        set
        {
            SetProperty(ref _isSpecialSale, value);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            if (!value)
            {
                SelectedSecondaryVendor = null;
                SecondaryCommissionPercentage = 0;
            }
            OnPropertyChanged(nameof(IsSecondaryVendorEnabled));
            UpdateCommissionCalculations();
        }
    }

    public bool IsSecondaryVendorEnabled => IsSpecialSale;

    public string SecondaryVendorSearchText
    {
        get => _secondaryVendorSearchText;
        set
        {
            SetProperty(ref _secondaryVendorSearchText, value);
            FilterSecondaryVendors();
        }
    }

    public Vendor? SelectedSecondaryVendor
    {
        get => _selectedSecondaryVendor;
        set
        {
            SetProperty(ref _selectedSecondaryVendor, value);
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            LoadSecondaryVendorDefaults();
            ErrorMessage = string.Empty;
        }
    }

    public ObservableCollection<Vendor> FilteredSecondaryVendors { get; } = new();
    public ObservableCollection<Vendor> AllVendors { get; } = new();

    public decimal SecondaryCommissionPercentage
    {
        get => _secondaryCommissionPercentage;
        set
        {
            SetProperty(ref _secondaryCommissionPercentage, value);
            UpdateCommissionCalculations();
        }
    }

    public decimal Total
{
    get => _total;
    set
    {
        if (SetProperty(ref _total, value))
        {
            OnPropertyChanged(nameof(TotalFiscal));
OnPropertyChanged(nameof(TaxableBase));
OnPropertyChanged(nameof(IsvAmount));
OnPropertyChanged(nameof(InvoiceTotal));
OnPropertyChanged(nameof(CommissionBase));
OnPropertyChanged(nameof(PrimaryCommissionAmount));
OnPropertyChanged(nameof(SecondaryCommissionAmount));
OnPropertyChanged(nameof(TotalCommissionAmount));
            UpdateCommissionCalculations();
            ErrorMessage = string.Empty;
        }
    }
}

    public string PaymentMethod
    {
        get => _paymentMethod;
        set
        {
            // Se permite sobrescribir libremente la forma de pago y dÃ­as de crÃ©dito
            // aunque vengan preconfigurados desde el cliente.
            var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();
            if (normalized == "CRÃ‰DITO")
            {
                normalized = "CREDITO";
            }

            SetProperty(ref _paymentMethod, normalized);
        }
    }

    public bool IsSalesConditionEditable =>
        !string.Equals(PaymentMethod, "EFECTIVO", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(PaymentMethod, "CONTADO", StringComparison.OrdinalIgnoreCase);
    public bool IsPaymentMethodEnabled =>
        SelectedSalesCondition != null &&
        SelectedSalesCondition.CreditDays == 0;

    public bool IsReceivingAccountEnabled =>
        IsPaymentMethodEnabled;
    public ObservableCollection<SalesConditionOption> AvailableSalesConditions { get; } = new();

    private SalesConditionOption? _selectedSalesCondition;
    public SalesConditionOption? SelectedSalesCondition
{
    get => _selectedSalesCondition;
    set
    {
        _selectedSalesCondition = value;

        CreditDays = value?.CreditDays ?? 0;

        OnPropertyChanged();
        OnPropertyChanged(nameof(CreditDays));
        OnPropertyChanged(nameof(IsPaymentMethodEnabled));
        OnPropertyChanged(nameof(IsReceivingAccountEnabled));

        // Si la condición es crédito, no debe quedar
        // seleccionada una forma de pago de contado.
        if (value != null && value.CreditDays > 0)
        {
            PaymentMethod = "CREDITO";
        }
        else if (value != null && value.CreditDays == 0)
        {
            if (string.Equals(PaymentMethod, "CREDITO", StringComparison.OrdinalIgnoreCase))
            {
                PaymentMethod = "EFECTIVO";
            }
        }
    }
}

    public ObservableCollection<ReceivingAccountOption> AvailableReceivingAccounts { get; } = new();

    private ReceivingAccountOption? _selectedReceivingAccount;
    public ReceivingAccountOption? SelectedReceivingAccount
    {
        get => _selectedReceivingAccount;
        set
        {
            _selectedReceivingAccount = value;
            OnPropertyChanged();
        }
    }
    public ObservableCollection<string> AvailablePaymentMethods { get; } = new();

    public int CreditDays
    {
        get => _creditDays;
        set => SetProperty(ref _creditDays, value);
    }

    public string Notes
    {
        get => _notes;
        set
        {
            if (value.Length <= 200)
            {
                SetProperty(ref _notes, value);
                OnPropertyChanged(nameof(NotesRemaining));
            }
        }
    }

    public string NotesRemaining => $"{Notes.Length} / 200";

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // Total fiscal mostrado en el resumen.
// Incluye el total base registrado y los montos fiscales adicionales
// que estén capturados como exonerados o exentos.
public decimal TaxableBase =>
    Total > 0 ? Math.Round(Total / 1.15m, 2) : 0m;

public decimal IsvAmount =>
    Total > 0 ? Math.Round(Total - TaxableBase, 2) : 0m;

public decimal InvoiceTotal =>
    Math.Round(Total + ExoneratedAmount + ExemptAmount, 2);

public decimal CommissionBase =>
    Math.Round(TaxableBase + ExoneratedAmount + ExemptAmount, 2);

public decimal TotalFiscal =>
    InvoiceTotal;
// Computed Properties para el resumen
    public decimal TotalCommissionPercentage => PrimaryCommissionPercentage + 
        (IsSpecialSale ? SecondaryCommissionPercentage : 0);

    public decimal PrimaryCommissionAmount => CommissionBase * (PrimaryCommissionPercentage / 100);

    public decimal SecondaryCommissionAmount => CommissionBase * (SecondaryCommissionPercentage / 100);

    public decimal TotalCommissionAmount => PrimaryCommissionAmount + 
        (IsSpecialSale ? SecondaryCommissionAmount : 0);

    // InformaciÃ³n del cliente
    public string CustomerCondition => SelectedCustomer?.PaymentTerms ?? "N/A";
    public string CustomerCreditDays => SelectedCustomer?.PaymentTerms == "CREDITO" 
        ? "30" 
        : "0";
    public string CustomerCreditLimit => SelectedCustomer != null 
        ? $"L. {SelectedCustomer.CreditLimit:N2}" 
        : "L. 0.00";
    public string CustomerPendingBalance => SelectedCustomer != null 
        ? $"L. {SelectedCustomer.PendingBalance:N2}" 
        : "L. 0.00";

    #endregion

    // ============================================================
    // EMPRESA FACTURADORA / AREA EJECUTORA
    // ============================================================

    private BillingCompany? _selectedBillingCompany;
    private OperationalArea? _selectedOperationalArea;

    public ObservableCollection<BillingCompany> BillingCompanies { get; } = new();

    public ObservableCollection<OperationalArea> OperationalAreas { get; } = new();

    public BillingCompany? SelectedBillingCompany
    {
        get => _selectedBillingCompany;
        set
        {
            if (!SetProperty(ref _selectedBillingCompany, value))
                return;

            ErrorMessage = string.Empty;

            if (value != null)
            {
                SetDefaultOperationalArea(value);
            }
        }
    }

    private void SetDefaultOperationalArea(BillingCompany company)
    {
        if (OperationalAreas.Count == 0)
            return;

        string? defaultAreaName = null;

        if (company.Name.Equals("TALLER AUTOMOTRIZ DARWIN", StringComparison.OrdinalIgnoreCase))
        {
            defaultAreaName = "TALLER AUTOMOTRIZ DARWIN";
        }
        else if (company.Name.Equals("RESERMA", StringComparison.OrdinalIgnoreCase))
        {
            defaultAreaName = "RESERMA";
        }

        if (defaultAreaName == null)
            return;

        var defaultArea = OperationalAreas.FirstOrDefault(
            a => a.Name.Equals(defaultAreaName, StringComparison.OrdinalIgnoreCase));

        if (defaultArea != null)
        {
            SelectedOperationalArea = defaultArea;
        }
    }

    public OperationalArea? SelectedOperationalArea
    {
        get => _selectedOperationalArea;
        set
        {
            SetProperty(ref _selectedOperationalArea, value);
            ErrorMessage = string.Empty;
        }
    }
    #region Commands

    public ICommand GuardarCommand { get; }
    public event System.EventHandler? SaleSaved;
    public ICommand CancelarCommand { get; }

    private bool CanExecuteGuardar(object? parameter)
    {
        return !string.IsNullOrWhiteSpace(InvoiceNumber) &&
               SelectedCustomer != null &&
               InvoiceTotal > 0 &&
               SelectedPrimaryVendor != null &&
               (!IsSpecialSale || SelectedSecondaryVendor != null);
    }

    private void ExecuteGuardar(object? parameter)
    {
        var sale = new Sale
        {
            InvoiceNumber = InvoiceNumber.Trim(),
            SaleDate = SaleDate,
            Status = Status,
            // Empresa facturadora seleccionada.
            BillingCompanyId = SelectedBillingCompany?.Id ?? 0,
            BillingCompanyName = SelectedBillingCompany?.Name ?? string.Empty,

            // Area ejecutora seleccionada.
            OperationalAreaId = SelectedOperationalArea?.Id ?? 0,
            OperationalAreaName = SelectedOperationalArea?.Name ?? string.Empty,

            CustomerId = SelectedCustomer!.Id,
            CustomerName = SelectedCustomer.Name,
            PrimaryVendorId = SelectedPrimaryVendor!.Id,
            PrimaryVendorName = SelectedPrimaryVendor.Name,
            PrimaryCommissionPercentage = PrimaryCommissionPercentage,
            IsSpecialSale = IsSpecialSale,
            SecondaryVendorId = SelectedSecondaryVendor?.Id,
            SecondaryVendorName = SelectedSecondaryVendor?.Name ?? string.Empty,
            SecondaryCommissionPercentage = SecondaryCommissionPercentage,
            Total = Total,
            CommissionBase = CommissionBase,
            PaymentMethod = PaymentMethod,
            CreditDays = CreditDays,
            Notes = Notes.Trim(),
            CreatedBy = AuthenticationService.CurrentUser?.Username ?? "Sistema"
        };

        var result = SaleService.Save(sale);

        if (result.success)
        {
            MessageBox.Show(
                "âœ“ Venta guardada exitosamente", 
                "Ã‰xito", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);

                        SaleSaved?.Invoke(this, System.EventArgs.Empty);
Clear();
        }
        else
        {
            ErrorMessage = result.message;
            MessageBox.Show(
                result.message, 
                "Error de validaciÃ³n", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
        }
    }

    private void ExecuteCancelar(object? parameter)
    {
        var result = MessageBox.Show(
            "Â¿Desea cancelar el registro de esta venta?",
            "Confirmar cancelaciÃ³n",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Clear();
        }
    }

    #endregion

    #region Methods

    public void RefreshCustomersAndSelect(int customerId)
    {
        LoadCustomers();

        var customer = AllCustomers.FirstOrDefault(c => c.Id == customerId);

        if (customer is null)
        {
            return;
        }

        SelectedCustomer = customer;
        CustomerSearchText = customer.Name;
        ErrorMessage = string.Empty;

        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
    }
    private void LoadCustomers()
    {
        var customers = CustomerService.GetAll();
        AllCustomers.Clear();
        FilteredCustomers.Clear();

        foreach (var customer in customers)
        {
            AllCustomers.Add(customer);
            FilteredCustomers.Add(customer);
        }
    }

    private void FilterCustomers()
    {
        FilteredCustomers.Clear();

        var filtered = string.IsNullOrWhiteSpace(CustomerSearchText)
            ? AllCustomers
            : AllCustomers.Where(c => 
                c.Name.Contains(CustomerSearchText, StringComparison.OrdinalIgnoreCase) ||
                c.Code.Contains(CustomerSearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var customer in filtered)
        {
            FilteredCustomers.Add(customer);
        }
    }

    private void LoadCustomerDefaults()
    {
        if (SelectedBillingCompany == null)
        {
            ErrorMessage = "Debe seleccionar la empresa facturadora.";
            return;
        }

        if (SelectedOperationalArea == null)
        {
            ErrorMessage = "Debe seleccionar el área ejecutora.";
            return;
        }

        if (SelectedOperationalArea.Name.Equals("SETMEC", StringComparison.OrdinalIgnoreCase)
            && SelectedBillingCompany.Name.Equals("SETMEC", StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "SETMEC no puede utilizarse como empresa facturadora.";
            return;
        }
        if (SelectedCustomer == null) return;

        // Cargar valores predeterminados del cliente
        if (SelectedCustomer.PaymentTerms == "CREDITO")
        {
            PaymentMethod = "CREDITO";

            var customerCondition = AvailableSalesConditions
                .FirstOrDefault(x =>
                    x.CreditDays == SelectedCustomer.CreditDays);

            if (customerCondition != null)
            {
                SelectedSalesCondition = customerCondition;
            }
            else
            {
                CreditDays = SelectedCustomer.CreditDays;
            }
        }
        else
        {
            PaymentMethod = "EFECTIVO";

            var contado = AvailableSalesConditions
                .FirstOrDefault(x =>
                    x.CreditDays == 0 ||
                    x.Name.Equals("CONTADO", StringComparison.OrdinalIgnoreCase));

            if (contado != null)
            {
                SelectedSalesCondition = contado;
            }
            else
            {
                CreditDays = 0;
            }
        }

        OnPropertyChanged(nameof(CustomerCondition));
        OnPropertyChanged(nameof(CustomerCreditDays));
        OnPropertyChanged(nameof(CustomerCreditLimit));
        OnPropertyChanged(nameof(CustomerPendingBalance));
    }

    private void LoadVendors()
    {
        var vendors = VendorService.GetAll();
        AllVendors.Clear();
        FilteredPrimaryVendors.Clear();
        FilteredSecondaryVendors.Clear();

        foreach (var vendor in vendors)
        {
            AllVendors.Add(vendor);
            FilteredPrimaryVendors.Add(vendor);
            FilteredSecondaryVendors.Add(vendor);
        }
    }

    private void FilterPrimaryVendors()
    {
        FilteredPrimaryVendors.Clear();

        var filtered = string.IsNullOrWhiteSpace(PrimaryVendorSearchText)
            ? AllVendors
            : AllVendors.Where(v => 
                v.Name.Contains(PrimaryVendorSearchText, StringComparison.OrdinalIgnoreCase) ||
                v.Code.Contains(PrimaryVendorSearchText, StringComparison.OrdinalIgnoreCase));

        foreach (var vendor in filtered)
        {
            FilteredPrimaryVendors.Add(vendor);
        }
    }

    private void FilterSecondaryVendors()
    {
        FilteredSecondaryVendors.Clear();

        var filtered = string.IsNullOrWhiteSpace(SecondaryVendorSearchText)
            ? AllVendors.Where(v => v.Id != SelectedPrimaryVendor?.Id)
            : AllVendors.Where(v => 
                v.Id != SelectedPrimaryVendor?.Id &&
                (v.Name.Contains(SecondaryVendorSearchText, StringComparison.OrdinalIgnoreCase) ||
                v.Code.Contains(SecondaryVendorSearchText, StringComparison.OrdinalIgnoreCase)));

        foreach (var vendor in filtered)
        {
            FilteredSecondaryVendors.Add(vendor);
        }
    }

    private void LoadPrimaryVendorDefaults()
    {
        if (SelectedPrimaryVendor == null) return;
        PrimaryCommissionPercentage = SelectedPrimaryVendor.CommissionPercentage;
    }

    private void LoadSecondaryVendorDefaults()
    {
        if (SelectedSecondaryVendor == null) return;
        SecondaryCommissionPercentage = SelectedSecondaryVendor.CommissionPercentage;
    }

    private void UpdateCommissionCalculations()
    {
        OnPropertyChanged(nameof(TotalCommissionPercentage));
        OnPropertyChanged(nameof(PrimaryCommissionAmount));
        OnPropertyChanged(nameof(SecondaryCommissionAmount));
        OnPropertyChanged(nameof(TotalCommissionAmount));
    }

    private void Clear()
    {
        InvoiceNumber = string.Empty;
        SaleDate = DateTime.Now;
        Status = "ACTIVA";

        InvoiceTaxType = "GRAVADA ISV";
        ExoneratedAmount = 0;
        ExemptAmount = 0;

        SelectedCustomer = null;
        CustomerSearchText = string.Empty;

        SelectedPrimaryVendor = null;
        PrimaryVendorSearchText = string.Empty;
        PrimaryCommissionPercentage = 0;

        IsSpecialSale = false;

        SelectedSecondaryVendor = null;
        SecondaryVendorSearchText = string.Empty;
        SecondaryCommissionPercentage = 0;

        Total = 0;

        PaymentMethod = "EFECTIVO";
        CreditDays = 0;
        Notes = string.Empty;
        ErrorMessage = string.Empty;

        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
    }

    #endregion
}


























