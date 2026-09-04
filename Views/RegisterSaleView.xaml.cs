using System.Globalization;
using SMART_ERP.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SMART_ERP.Views;

public partial class RegisterSaleView : UserControl
{
    public RegisterSaleView()
    {
        InitializeComponent();
        DataContext = new RegisterSaleViewModel();
        if (DataContext is RegisterSaleViewModel vm)
        {
            vm.SaleSaved += OnSaleSaved;
        }
    }

    private void OnSaleSaved(object? sender, System.EventArgs e)
{
    if (Window.GetWindow(this) is SMART_ERP.MainWindow mainWindow)
    {
        mainWindow.RefreshSalesListTab();
    }
}

    private void OpenNewCustomer_Click(object sender, RoutedEventArgs e)
    {
        var newCustomerWindow = new NewCustomerWindow
        {
            Owner = Window.GetWindow(this)
        };

        if (newCustomerWindow.ShowDialog() == true &&
            newCustomerWindow.CreatedCustomer is not null &&
            DataContext is RegisterSaleViewModel vm)
        {
            vm.RefreshCustomersAndSelect(newCustomerWindow.CreatedCustomer.Id);
        }
    }
private void OpenSalesSettings_Click(object sender, RoutedEventArgs e)
    {
        OpenSalesSettings();
    }

    public void ExecuteSave()
    {
        if (DataContext is RegisterSaleViewModel vm)
        {
            ExecuteCommand(vm.GuardarCommand);
        }
    }

    public void ExecuteCancel()
    {
        if (DataContext is RegisterSaleViewModel vm)
        {
            ExecuteCommand(vm.CancelarCommand);
        }
    }

    public void StartNewSale()
    {
        DataContext = new RegisterSaleViewModel();
    }

    public void RefreshVendors()
    {
        if (DataContext is RegisterSaleViewModel vm)
        {
            vm.ReloadVendors();
        }
    }

    public void OpenSettings()
    {
        OpenSalesSettings();
    }

    private void OpenSalesSettings()
    {
        var settingsWindow = new SalesCaptureSettingsWindow
        {
            Owner = Window.GetWindow(this)
        };

        if (settingsWindow.ShowDialog() == true && DataContext is RegisterSaleViewModel vm)
        {
            vm.ReloadCaptureSettings();
        }
    }

    private static void ExecuteCommand(ICommand command)
    {
        if (command.CanExecute(null))
        {
            command.Execute(null);
        }
    }

    // ========================================================
        // ========================================================
    // SMART ERP - CAMPOS MONETARIOS Y NUMERICOS
    // ========================================================

    private void MoneyTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
{
    if (sender is not TextBox textBox)
        return;

    // El TextBox contiene solamente el numero.
    // El "L." se muestra mediante el TextBlock visual.
    string text = textBox.Text?.Trim() ?? string.Empty;

    if (text.StartsWith("L.", StringComparison.OrdinalIgnoreCase))
    {
        text = text.Substring(2).Trim();
        textBox.Text = text;
    }

    textBox.Dispatcher.BeginInvoke(
        new Action(() =>
        {
            textBox.Focus();
            textBox.SelectAll();
        }),
        System.Windows.Threading.DispatcherPriority.Input);
}

private void MoneyTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
{
    if (sender is not TextBox textBox)
        return;

    string text = textBox.Text?.Trim() ?? string.Empty;

    if (text.StartsWith("L.", StringComparison.OrdinalIgnoreCase))
        text = text.Substring(2).Trim();

    if (decimal.TryParse(
        text,
        System.Globalization.NumberStyles.Any,
        System.Globalization.CultureInfo.CurrentCulture,
        out decimal value))
    {
        textBox.Text = value.ToString(
            "N2",
            System.Globalization.CultureInfo.CurrentCulture);
    }
    else
    {
        textBox.Text = "0.00";
    }
}

private void NumericSelectAll_GotFocus(object sender, System.Windows.RoutedEventArgs e)
{
    if (sender is not TextBox textBox)
        return;

    textBox.Dispatcher.BeginInvoke(
        new Action(() =>
        {
            textBox.Focus();
            textBox.SelectAll();
        }),
        System.Windows.Threading.DispatcherPriority.Input);
}

}







