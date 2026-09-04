using SMART_ERP.Data;
using SMART_ERP.Models;
using System.Windows;
using System.Windows.Controls;

namespace SMART_ERP.Views;

public partial class CompanyEditorView : UserControl
{
    private CompanyConnection? _editingCompany;

    public event EventHandler? CancelRequested;
    public event EventHandler<CompanyConnection>? SaveRequested;

    public CompanyEditorView()
    {
        InitializeComponent();

        CmbConnectionType.SelectedIndex = 0;

        TxtServer.Text = "127.0.0.1";
        TxtPort.Text = "3307";

        TxtConnectionStatus.Text = "Sin probar conexión";
    }

    public void LoadCompany(CompanyConnection? company)
    {
        _editingCompany = company;

        if (company == null)
        {
            TxtTitle.Text = "Nueva empresa";

            TxtCompanyName.Text = string.Empty;

            CmbConnectionType.SelectedIndex = 0;

            TxtServer.Text = "127.0.0.1";
            TxtPort.Text = "3307";
            TxtDatabase.Text = string.Empty;
            TxtUsername.Text = string.Empty;
            TxtPassword.Password = string.Empty;

            TxtConnectionStatus.Text = "Sin probar conexión";

            return;
        }

        TxtTitle.Text = "Editar empresa";

        TxtCompanyName.Text = company.CompanyName;

        SelectConnectionType(company.ConnectionType);

        TxtServer.Text = company.Server;
        TxtPort.Text = company.Port.ToString();
        TxtDatabase.Text = company.DatabaseName;
        TxtUsername.Text = company.Username;
        TxtPassword.Password = company.Password;

        TxtConnectionStatus.Text = "Configuración cargada";
    }

    private void SelectConnectionType(string type)
    {
        for (int i = 0; i < CmbConnectionType.Items.Count; i++)
        {
            if (CmbConnectionType.Items[i] is ComboBoxItem item &&
                string.Equals(
                    item.Tag?.ToString(),
                    type,
                    StringComparison.OrdinalIgnoreCase))
            {
                CmbConnectionType.SelectedIndex = i;
                return;
            }
        }

        CmbConnectionType.SelectedIndex = 0;
    }

    private string GetConnectionType()
    {
        if (CmbConnectionType.SelectedItem is ComboBoxItem item)
        {
            return item.Tag?.ToString() ?? "LOCAL";
        }

        return "LOCAL";
    }

    private bool ReadPort(out int port)
    {
        return int.TryParse(
            TxtPort.Text.Trim(),
            out port) &&
            port > 0 &&
            port <= 65535;
    }

    private bool ValidateFields(out int port)
    {
        port = 0;

        if (string.IsNullOrWhiteSpace(TxtCompanyName.Text))
        {
            MessageBox.Show(
                "Ingrese el nombre de la empresa.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtCompanyName.Focus();

            return false;
        }

        if (string.IsNullOrWhiteSpace(TxtServer.Text))
        {
            MessageBox.Show(
                "Ingrese el servidor o dirección IP.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtServer.Focus();

            return false;
        }

        if (!ReadPort(out port))
        {
            MessageBox.Show(
                "El puerto debe ser un número entre 1 y 65535.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtPort.Focus();

            return false;
        }

        if (string.IsNullOrWhiteSpace(TxtDatabase.Text))
        {
            MessageBox.Show(
                "Ingrese el nombre de la base de datos.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtDatabase.Focus();

            return false;
        }

        if (string.IsNullOrWhiteSpace(TxtUsername.Text))
        {
            MessageBox.Show(
                "Ingrese el usuario de la base de datos.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            TxtUsername.Focus();

            return false;
        }

        return true;
    }

    private CompanyConnection BuildCompany()
    {
        ReadPort(out int port);

        return new CompanyConnection
        {
            Id = _editingCompany?.Id ?? 0,

            CompanyName = TxtCompanyName.Text.Trim(),

            ConnectionType = GetConnectionType(),

            Server = TxtServer.Text.Trim(),

            Port = port,

            DatabaseName = TxtDatabase.Text.Trim(),

            Username = TxtUsername.Text.Trim(),

            Password = TxtPassword.Password,

            IsActive = _editingCompany?.IsActive ?? true,

            CreatedAt = _editingCompany?.CreatedAt ?? DateTime.Now,

            LastConnectionAt = _editingCompany?.LastConnectionAt
        };
    }

    private async void BtnTestConnection_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!ValidateFields(out int port))
            return;

        BtnTestConnection.IsEnabled = false;
        BtnSave.IsEnabled = false;

        TxtConnectionStatus.Text = "Probando conexión...";

        try
        {
            bool success = await DatabaseConnection.TestConnectionAsync(
                TxtServer.Text.Trim(),
                port,
                TxtDatabase.Text.Trim(),
                TxtUsername.Text.Trim(),
                TxtPassword.Password);

            if (success)
            {
                TxtConnectionStatus.Text = "Conexión exitosa";

                MessageBox.Show(
                    "La conexión con la base de datos fue exitosa.",
                    "SMART ERP",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                TxtConnectionStatus.Text = "No fue posible conectar";

                MessageBox.Show(
                    "No fue posible establecer conexión con la base de datos. Verifique servidor, puerto, base de datos, usuario y contraseña.",
                    "SMART ERP",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            TxtConnectionStatus.Text = "Error de conexión";

            MessageBox.Show(
                $"Ocurrió un error al probar la conexión.\n\n{ex.Message}",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            BtnTestConnection.IsEnabled = true;
            BtnSave.IsEnabled = true;
        }
    }

    private void BtnSave_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!ValidateFields(out _))
            return;

        var company = BuildCompany();

        SaveRequested?.Invoke(this, company);
    }

    private void BtnCancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }

    private void CmbConnectionType_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (!IsInitialized)
            return;

        string type = GetConnectionType();

        if (type == "LOCAL")
        {
            TxtServer.Text = "127.0.0.1";
        }
    }
}
