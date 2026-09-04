using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Windows;
using System.Windows.Controls;

namespace SMART_ERP.Views;

public partial class CustomersView : UserControl
{
    private int _selectedCustomerId;

    public CustomersView()
    {
        InitializeComponent();
        LoadCustomers();
    }

    public void CreateNewCustomer()
    {
        var window = new NewCustomerWindow
        {
            Owner = Window.GetWindow(this)
        };

        if (window.ShowDialog() == true)
        {
            LoadCustomers(TxtBuscar.Text);
        }
    }

    public void SaveCurrentCustomer()
    {
        if (DgClientes.SelectedItem is not Customer selected)
        {
            MessageBox.Show(
                "Selecciona un cliente para editar.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        EditCustomer(selected);
    }

    private void EditCustomer(Customer original)
    {
        var customer = new Customer
        {
            Id = original.Id,
            Code = original.Code,
            Name = original.Name,
            PaymentTerms = original.PaymentTerms,
            CreditDays = original.CreditDays,
            CreditLimit = original.CreditLimit,
            CurrentBalance = original.CurrentBalance,
            PendingBalance = original.PendingBalance,
            PriceLevel = original.PriceLevel,
            Salesperson = original.Salesperson,
            Phone = original.Phone,
            Email = original.Email,
            Address = original.Address,
            Rtn = original.Rtn,
            ContactName = original.ContactName,
            ContactPhone = original.ContactPhone,
            ContactEmail = original.ContactEmail,
            City = original.City,
            Country = original.Country,
            Department = original.Department,
            Note = original.Note,
            IsActive = original.IsActive
        };

        var window = new NewCustomerWindow(customer)
        {
            Owner = Window.GetWindow(this)
        };

        if (window.ShowDialog() != true)
        {
            return;
        }

        if (window.CreatedCustomer is null)
        {
            return;
        }

        CustomerService.Save(window.CreatedCustomer);

        LoadCustomers(TxtBuscar.Text);

        MessageBox.Show(
            "Cliente actualizado correctamente.",
            "Éxito",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
    public void EditSelectedCustomer()
    {
        SaveCurrentCustomer();
    }
    public void DeleteSelectedCustomer()
    {
        if (_selectedCustomerId == 0)
        {
            MessageBox.Show(
                "Selecciona un cliente para eliminar.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var result = MessageBox.Show(
            "¿Deseas eliminar el cliente seleccionado?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        CustomerService.Delete(_selectedCustomerId);

        _selectedCustomerId = 0;

        LoadCustomers(TxtBuscar.Text);

        MessageBox.Show(
            "Cliente eliminado.",
            "Éxito",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void LoadCustomers(string term = "")
    {
        DgClientes.ItemsSource = CustomerService.Search(term);
    }

    private void TxtBuscar_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        LoadCustomers(TxtBuscar.Text);
    }

    private void DgClientes_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DgClientes.SelectedItem is Customer customer)
        {
            _selectedCustomerId = customer.Id;
        }
        else
        {
            _selectedCustomerId = 0;
        }
    }
}


