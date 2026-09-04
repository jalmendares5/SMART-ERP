using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Globalization;
using System.Text;
using System.Windows;

namespace SMART_ERP.Views;

public partial class NewCompanyWindow : Window
{
    public NewCompanyWindow()
    {
        InitializeComponent();

        UpdateConnectionDefaults();
        UpdateDatabaseName();
    }

    // ============================================================
    // NOMBRE DE EMPRESA
    // ============================================================

    private void TxtCompanyName_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e)
    {
        UpdateDatabaseName();
    }

    private void UpdateDatabaseName()
    {
        if (TxtDatabase == null || TxtCompanyName == null)
            return;

        string databaseName =
            GenerateDatabaseName(TxtCompanyName.Text);

        // La BD real se conserva en minusculas.
        // La interfaz la muestra en mayusculas.
        TxtDatabase.Text =
            databaseName.ToUpperInvariant();
    }

    // ============================================================
    // GENERACION AUTOMATICA DE BASE DE DATOS
    // ============================================================

    private static string GenerateDatabaseName(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return string.Empty;

        string normalized = companyName
            .Trim()
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder();

        foreach (char character in normalized)
        {
            UnicodeCategory category =
                CharUnicodeInfo.GetUnicodeCategory(character);

            if (category == UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else
            {
                builder.Append('_');
            }
        }

        string result = builder.ToString();

        while (result.Contains("__"))
        {
            result = result.Replace("__", "_");
        }

        result = result.Trim('_');

        if (string.IsNullOrWhiteSpace(result))
            return string.Empty;

        // NOMBRE REAL DE LA BD
        // Ejemplo:
        // Taller Automotriz Darwin
        // -> bd_taller_automotriz_darwin
        return $"bd_{result}";
    }

    // ============================================================
    // RTN
    // FORMATO: ####-####-######
    // ============================================================

    private void TxtTaxId_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e)
    {
        FormatTaxId();
    }

    private void FormatTaxId()
    {
        if (TxtTaxId == null)
            return;

        string digits = new string(
            TxtTaxId.Text
                .Where(char.IsDigit)
                .ToArray());

        if (digits.Length > 14)
            digits = digits[..14];

        string formatted = digits;

        if (digits.Length > 8)
        {
            formatted =
                digits[..4] + "-" +
                digits.Substring(4, 4) + "-" +
                digits[8..];
        }
        else if (digits.Length > 4)
        {
            formatted =
                digits[..4] + "-" +
                digits[4..];
        }

        if (TxtTaxId.Text != formatted)
        {
            TxtTaxId.Text = formatted;
            TxtTaxId.CaretIndex = TxtTaxId.Text.Length;
        }
    }

    // ============================================================
    // TELEFONO
    // FORMATO: ####-####
    // ============================================================

    private void TxtPhone_TextChanged(
        object sender,
        System.Windows.Controls.TextChangedEventArgs e)
    {
        FormatPhone();
    }

    private void FormatPhone()
    {
        if (TxtPhone == null)
            return;

        string digits = new string(
            TxtPhone.Text
                .Where(char.IsDigit)
                .ToArray());

        if (digits.Length > 8)
            digits = digits[..8];

        string formatted = digits;

        if (digits.Length > 4)
        {
            formatted =
                digits[..4] + "-" +
                digits[4..];
        }

        if (TxtPhone.Text != formatted)
        {
            TxtPhone.Text = formatted;
            TxtPhone.CaretIndex = TxtPhone.Text.Length;
        }
    }

    // ============================================================
    // TIPO DE CONEXION
    // ============================================================

    private async void ConnectionType_Changed(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized)
            return;

        UpdateConnectionDefaults();
    }

    private void UpdateConnectionDefaults()
    {
        if (RbLocal?.IsChecked == true)
        {
            TxtServer.Text = "127.0.0.1";
            TxtPort.Text = "3307";
        }
        else if (RbLan?.IsChecked == true)
        {
            TxtServer.Text = "";
            TxtPort.Text = "3307";
        }
        else if (RbRemote?.IsChecked == true)
        {
            TxtServer.Text = "";
            TxtPort.Text = "3307";
        }
    }

    private string GetConnectionType()
    {
        if (RbLan.IsChecked == true)
            return "LAN";

        if (RbRemote.IsChecked == true)
            return "REMOTA";

        return "LOCAL";
    }

    // ============================================================
    // GUARDAR EMPRESA
    // ============================================================

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        string companyName =
            TxtCompanyName.Text.Trim();

        if (string.IsNullOrWhiteSpace(companyName))
        {
            MessageBox.Show(
                "Debe ingresar el nombre de la empresa.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtCompanyName.Focus();
            return;
        }

        string databaseName =
            GenerateDatabaseName(companyName);

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            MessageBox.Show(
                "No fue posible generar el nombre de la base de datos.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!int.TryParse(
                TxtPort.Text.Trim(),
                out int port) ||
            port <= 0 ||
            port > 65535)
        {
            MessageBox.Show(
                "El puerto de MariaDB no es válido.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtPort.Focus();
            return;
        }

        string connectionType =
            GetConnectionType();

        string server =
            TxtServer.Text.Trim();

        if (connectionType != "LOCAL" &&
            string.IsNullOrWhiteSpace(server))
        {
            MessageBox.Show(
                "Debe indicar el servidor o dirección IP para una conexión LAN o remota.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtServer.Focus();
            return;
        }

        var existing =
            CompanyConnectionService
                .GetByName(companyName);

        if (existing != null)
        {
            MessageBox.Show(
                "Ya existe una empresa con ese nombre.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtCompanyName.Focus();
            return;
        }

        var company = new CompanyConnection
        {
            CompanyName = companyName,

            ConnectionType = connectionType,

            Server =
                string.IsNullOrWhiteSpace(server)
                    ? "127.0.0.1"
                    : server,

            Port = port,

            // IMPORTANTE:
            // Aqui guardamos el nombre REAL
            // de MariaDB en minusculas.
            DatabaseName = databaseName,

            Username =
                TxtUsername.Text.Trim(),

            Password =
                TxtPassword.Password,

            IsActive = true
        };

        bool databaseCreated =
            await CompanyConnectionService.CreateDatabaseAsync(company);

        if (!databaseCreated)
        {
            MessageBox.Show(
                "No fue posible crear la base de datos en el servidor MariaDB.`n`n" +
                "Verifique el servidor, puerto, usuario y contraseña.",
                "SMART ERP - Base de datos",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        CompanyConnectionService.Save(company);
        CompanyConnectionService.SetActiveCompany(company);

        MessageBox.Show(
            $"Empresa guardada correctamente.\n\n" +
            $"Nombre de base de datos:\n" +
            $"{databaseName.ToUpperInvariant()}",
            "SMART ERP",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        DialogResult = true;
        Close();
    }

    // ============================================================
    // CANCELAR
    // ============================================================

    private void BtnCancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}


