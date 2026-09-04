using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART_ERP.Views;

public partial class NewCustomerWindow : Window
{
    private readonly int _editingCustomerId;

    public Customer? CreatedCustomer { get; private set; }

    public NewCustomerWindow()
        : this(null)
    {
    }

    public NewCustomerWindow(Customer? customer)
    {
        InitializeComponent();
        Owner = Application.Current.MainWindow;

        TxtPhone.TextChanged += (_, _) => FormatPhone(TxtPhone);
        TxtContactPhone.TextChanged += (_, _) => FormatPhone(TxtContactPhone);

        // Cargar departamentos de Honduras
        LoadHondurasDepartments();

        if (customer is null)
        {
            _editingCustomerId = 0;
            ResetForm();
        }
        else
        {
            _editingCustomerId = customer.Id;
            LoadCustomer(customer);
        }
    }

    private void LoadHondurasDepartments()
    {
        var departments = HondurasLocationsService.GetDepartments();
        CmbDepartment.ItemsSource = departments;
        
        if (departments.Any())
        {
            CmbDepartment.SelectedIndex = 0;
        }
    }

    private void CmbDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbDepartment.SelectedItem is string selectedDepartment)
        {
            var cities = HondurasLocationsService.GetCitiesByDepartment(selectedDepartment);
            CmbCity.ItemsSource = cities;
            
            if (cities.Any())
            {
                CmbCity.SelectedIndex = 0;
            }
        }
    }

    private void ResetForm()
    {
        TxtCode.Text = CustomerService.GenerateNextCode();
        TxtName.Clear();
        TxtAddress.Clear();
        TxtPhone.Clear();
        TxtRtn.Clear();
        TxtEmail.Clear();
        TxtContactName.Clear();
        TxtCity.Clear();
        TxtCountry.Text = "HONDURAS";
        TxtContactPhone.Clear();
        CmbDepartment.SelectedIndex = -1;
        CmbCity.SelectedIndex = -1;
        TxtContactEmail.Clear();
        TxtCreditLimit.Text = "0";
        TxtCreditDays.Text = "0";
        TxtNote.Clear();
        CmbTerms.SelectedIndex = 0;
        CmbPriceLevel.SelectedIndex = 0;
        ChkActive.IsChecked = true;
        UpdateCreditDaysAvailability();
        TxtName.Focus();
    }

    private void LoadCustomer(Customer customer)
    {
        TxtCode.Text = customer.Code;
        TxtName.Text = customer.Name;
        TxtAddress.Text = customer.Address;
        TxtPhone.Text = customer.Phone;
        TxtRtn.Text = customer.Rtn;
        TxtEmail.Text = customer.Email;
        TxtContactName.Text = customer.ContactName;
        TxtCity.Text = customer.City;
        TxtCountry.Text = customer.Country;
        TxtContactPhone.Text = customer.ContactPhone;
        CmbDepartment.SelectedItem = customer.Department;
        TxtContactEmail.Text = customer.ContactEmail;
        TxtCreditLimit.Text = customer.CreditLimit.ToString("0.##", CultureInfo.CurrentCulture);
        TxtCreditDays.Text = customer.CreditDays.ToString();
        TxtNote.Text = customer.Note;

        var termsIndex = CmbTerms.Items
            .OfType<ComboBoxItem>()
            .Select((item, index) => new { item, index })
            .FirstOrDefault(x =>
                string.Equals(
                    x.item.Content?.ToString(),
                    customer.PaymentTerms,
                    StringComparison.OrdinalIgnoreCase));

        CmbTerms.SelectedIndex = termsIndex?.index ?? 0;

        var priceIndex = CmbPriceLevel.Items
            .OfType<ComboBoxItem>()
            .Select((item, index) => new { item, index })
            .FirstOrDefault(x =>
                string.Equals(
                    x.item.Content?.ToString(),
                    customer.PriceLevel,
                    StringComparison.OrdinalIgnoreCase));

        CmbPriceLevel.SelectedIndex = priceIndex?.index ?? 0;

        ChkActive.IsChecked = customer.IsActive;

        UpdateCreditDaysAvailability();

        TxtName.Focus();
    }
    private bool TryCreateCustomer(out Customer? customer)
    {
        customer = null;
        var code = TxtCode.Text.Trim();
        var name = TxtName.Text.Trim();

        if (string.IsNullOrWhiteSpace(code))
        {
            MessageBox.Show(this, "El código del cliente es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(this, "El nombre del cliente es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtName.Focus();
            return false;
        }

        if (CustomerService.GetAll().Any(existing =>
            existing.Code.Equals(code, StringComparison.OrdinalIgnoreCase) &&
            existing.Id != _editingCustomerId))
        {
            MessageBox.Show(this, "Ya existe un cliente con este código.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        var creditText = TxtCreditLimit.Text
            .Replace("L", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (!decimal.TryParse(creditText, NumberStyles.Number, CultureInfo.CurrentCulture, out var creditLimit) || creditLimit < 0)
        {
            MessageBox.Show(this, "El crédito máximo no es válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtCreditLimit.Focus();
            TxtCreditLimit.SelectAll();
            return false;
        }
        var rtnDigits = new string(
            TxtRtn.Text
                .Where(char.IsDigit)
                .ToArray());

        if (rtnDigits.Length != 14)
        {
            MessageBox.Show(
                this,
                "El RTN debe contener exactamente 14 dígitos.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtRtn.Focus();
            TxtRtn.SelectAll();
            return false;
        }

        // Validar RTN con el algoritmo de Honduras
        var validationResult = HondurasRtnValidator.ValidateRtn(rtnDigits);
        if (!validationResult.IsValid)
        {
            MessageBox.Show(
                this,
                validationResult.ErrorMessage,
                "Validación RTN",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtRtn.Focus();
            TxtRtn.SelectAll();
            return false;
        }
var paymentTerms = (CmbTerms.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "CONTADO";
        var creditDays = 0;

        if (paymentTerms == "CREDITO" &&
            (!int.TryParse(TxtCreditDays.Text.Trim(), out creditDays) || creditDays < 1))
        {
            MessageBox.Show(this, "Debe ingresar los días de crédito aprobados.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtCreditDays.Focus();
            TxtCreditDays.SelectAll();
            return false;
        }

        customer = new Customer
        {
            Id = _editingCustomerId,
            Code = code,
            Name = name,
            PaymentTerms = paymentTerms,
            CreditDays = creditDays,
            CreditLimit = creditLimit,
            CurrentBalance = 0m,
            PendingBalance = 0m,
            PriceLevel = (CmbPriceLevel.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "PRECIO DE VENTA 1",
            Phone = TxtPhone.Text.Trim(),
            Email = TxtEmail.Text.Trim(),
            Address = TxtAddress.Text.Trim(),
            Rtn = TxtRtn.Text.Trim(),
            ContactName = TxtContactName.Text.Trim(),
            ContactPhone = TxtContactPhone.Text.Trim(),
            ContactEmail = TxtContactEmail.Text.Trim(),
            City = CmbCity.SelectedItem?.ToString() ?? TxtCity.Text.Trim(),
            Country = TxtCountry.Text.Trim(),
            Department = CmbDepartment.SelectedItem?.ToString() ?? string.Empty,
            Note = TxtNote.Text.Trim(),
            IsActive = ChkActive.IsChecked ?? true
        };

        return true;
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (!TryCreateCustomer(out var customer) || customer is null)
        {
            return;
        }

        CustomerService.Save(customer);
        CreatedCustomer = customer;
        DialogResult = true;
    }

    private void BtnGuardarNuevo_Click(object sender, RoutedEventArgs e)
    {
        if (!TryCreateCustomer(out var customer) || customer is null)
        {
            return;
        }

        CustomerService.Save(customer);
        CreatedCustomer = customer;
        MessageBox.Show(this, $"Cliente {customer.Code} guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        ResetForm();
    }

    private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
    {
        ResetForm();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void UppercaseTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        var caret = textBox.CaretIndex;
        var uppercaseText = textBox.Text.ToUpperInvariant();

        if (textBox.Text != uppercaseText)
        {
            textBox.Text = uppercaseText;
            textBox.CaretIndex = Math.Min(caret, textBox.Text.Length);
        }
    }

    private static void FormatPhone(TextBox textBox)
    {
        var originalText = textBox.Text;
        var digits = new string(originalText.Where(char.IsDigit).Take(8).ToArray());
        var formattedText = digits.Length > 4
            ? $"{digits[..4]}-{digits[4..]}"
            : digits;

        if (originalText == formattedText)
        {
            return;
        }

        var digitsBeforeCursor = originalText
            .Take(Math.Min(textBox.SelectionStart, originalText.Length))
            .Count(char.IsDigit);

        textBox.Text = formattedText;
        textBox.SelectionStart = digitsBeforeCursor > 4
            ? Math.Min(digitsBeforeCursor + 1, formattedText.Length)
            : Math.Min(digitsBeforeCursor, formattedText.Length);
    }

    private void MoneyTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.Text = textBox.Text.Replace("L", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            textBox.SelectAll();
        }
    }

    private void MoneyTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (decimal.TryParse(textBox.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var value))
        {
            textBox.Text = $"L {value:N2}";
        }
        else
        {
            textBox.Text = "L 0.00";
        }
    }

    private void TxtRtn_TextChanged(object sender, TextChangedEventArgs e)
{
    if (sender is not TextBox textBox)
    {
        return;
    }

    var digits = new string(
        textBox.Text
            .Where(char.IsDigit)
            .Take(14)
            .ToArray());

    string formatted;

    if (digits.Length <= 4)
    {
        formatted = digits;
    }
    else if (digits.Length <= 8)
    {
        formatted =
            digits.Substring(0, 4) +
            "-" +
            digits.Substring(4);
    }
    else
    {
        formatted =
            digits.Substring(0, 4) +
            "-" +
            digits.Substring(4, 4) +
            "-" +
            digits.Substring(8);
    }

    if (textBox.Text != formatted)
    {
        textBox.Text = formatted;
        textBox.SelectionStart = textBox.Text.Length;
    }
}
private void CreditDaysTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }

    private void CmbTerms_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateCreditDaysAvailability();
    }

    private void UpdateCreditDaysAvailability()
    {
        // Durante InitializeComponent(), CmbTerms puede disparar
        // SelectionChanged antes de que TxtCreditDays exista.
        if (CmbTerms == null || TxtCreditDays == null)
        {
            return;
        }
var paymentTerms = (CmbTerms.SelectedItem as ComboBoxItem)?.Content?.ToString();
        var isCredit = paymentTerms == "CREDITO";

        TxtCreditDays.IsEnabled = isCredit;
        TxtCreditDays.Background = isCredit
            ? System.Windows.Media.Brushes.White
            : new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(243, 244, 246));

        if (!isCredit)
        {
            TxtCreditDays.Text = "0";
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            DialogResult = false;
        }
    }
}






