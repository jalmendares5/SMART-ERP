using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Controls;
using SMART_ERP.Services;
using SMART_ERP.Views;

namespace SMART_ERP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
public partial class App : Application
{
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        try
        {
            var companies = CompanyConnectionService.GetAll();

            // Si no hay empresas configuradas o activas, abrir ventana para registrar la primera
            if (!companies.Any(c => c.IsActive))
            {
                var newCompanyWindow = new NewCompanyWindow();
                newCompanyWindow.ShowDialog();

                companies = CompanyConnectionService.GetAll();
                if (!companies.Any(c => c.IsActive))
                {
                    Shutdown();
                    return;
                }
            }

            // Intentar verificar la conexión con la empresa activa
            try
            {
                bool connected = await CompanyConnectionService.InitializeActiveCompanyAsync();
                if (!connected)
                {
                    MessageBox.Show(
                        "No fue posible conectar automáticamente con la base de datos de la empresa activa.\n\n" +
                        "Puede continuar al inicio de sesión y seleccionar o verificar la configuración de su empresa.",
                        "SMART ERP - Advertencia de Conexión",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ocurrió una advertencia al verificar la base de datos:\n{ex.Message}\n\nPuede continuar al inicio de sesión.",
                    "SMART ERP - Advertencia de Conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            var loginWindow = new LoginWindow();
            MainWindow = loginWindow;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            loginWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error durante el inicio del sistema:\n{ex.Message}",
                "SMART ERP - Error Crítico",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown();
        }
    }

    // ========================================================
    // SMART ERP - ESTANDARES GLOBALES DE NUMEROS Y DINERO
    // ========================================================

    public static readonly System.Windows.DependencyProperty
        SmartErpNumericProperty =
        System.Windows.DependencyProperty.RegisterAttached(
            "SmartErpNumeric",
            typeof(bool),
            typeof(App),
            new System.Windows.PropertyMetadata(false, OnSmartErpNumericChanged));

    public static void SetSmartErpNumeric(
        System.Windows.DependencyObject element,
        bool value)
    {
        element.SetValue(SmartErpNumericProperty, value);
    }

    public static bool GetSmartErpNumeric(
        System.Windows.DependencyObject element)
    {
        return (bool)element.GetValue(SmartErpNumericProperty);
    }

    public static readonly System.Windows.DependencyProperty
        SmartErpMoneyProperty =
        System.Windows.DependencyProperty.RegisterAttached(
            "SmartErpMoney",
            typeof(bool),
            typeof(App),
            new System.Windows.PropertyMetadata(false, OnSmartErpMoneyChanged));

    public static void SetSmartErpMoney(
        System.Windows.DependencyObject element,
        bool value)
    {
        element.SetValue(SmartErpMoneyProperty, value);
    }

    public static bool GetSmartErpMoney(
        System.Windows.DependencyObject element)
    {
        return (bool)element.GetValue(SmartErpMoneyProperty);
    }

    private static void OnSmartErpNumericChanged(
        System.Windows.DependencyObject d,
        System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
            return;

        if (e.NewValue is true)
        {
            textBox.GotFocus += SmartErpSelectAllOnFocus;
        }
        else
        {
            textBox.GotFocus -= SmartErpSelectAllOnFocus;
        }
    }

    private static void OnSmartErpMoneyChanged(
        System.Windows.DependencyObject d,
        System.Windows.DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
            return;

        if (e.NewValue is true)
        {
            textBox.GotFocus += SmartErpMoneyGotFocus;
            textBox.LostFocus += SmartErpMoneyLostFocus;
        }
        else
        {
            textBox.GotFocus -= SmartErpMoneyGotFocus;
            textBox.LostFocus -= SmartErpMoneyLostFocus;
        }
    }

    private static void SmartErpSelectAllOnFocus(
        object sender,
        RoutedEventArgs e)
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

    private static void SmartErpMoneyGotFocus(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        var text = textBox.Text?.Trim() ?? string.Empty;

        if (text.StartsWith("L", StringComparison.OrdinalIgnoreCase))
        {
            text = text.Substring(1).Trim();
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

    private static void SmartErpMoneyLostFocus(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        var text = textBox.Text?.Trim() ?? string.Empty;

        if (text.StartsWith("L", StringComparison.OrdinalIgnoreCase))
        {
            text = text.Substring(1).Trim();
        }

        if (decimal.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out var value))
        {
            textBox.Text = $"L {value.ToString("N2", CultureInfo.CurrentCulture)}";
        }
        else
        {
            textBox.Text = "L 0.00";
        }
    }

    public App()
    {
        RegisterSmartErpTextBoxHandler();
    }

    // ========================================================
    // SMART ERP - ESTANDAR GLOBAL DE CAPTURA DE TEXTO
    // ========================================================

    private static bool _smartErpTextBoxHandlerRegistered;

    private static void RegisterSmartErpTextBoxHandler()
    {
        if (_smartErpTextBoxHandlerRegistered)
            return;

        _smartErpTextBoxHandlerRegistered = true;

        EventManager.RegisterClassHandler(
            typeof(TextBox),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler(SmartErpGlobalTextBoxHandler));
    }

    private static void SmartErpGlobalTextBoxHandler(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // Todo texto operativo del ERP se captura en MAYUSCULAS
            // directamente, sin conversión posterior visible.
            textBox.CharacterCasing = CharacterCasing.Upper;
        }
    }

    }

}


