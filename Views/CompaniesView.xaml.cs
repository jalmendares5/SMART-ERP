using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Windows;
using System.Windows.Controls;

namespace SMART_ERP.Views;

public partial class CompaniesView : UserControl
{
    private CompanyConnection? _selectedCompany;

    public CompaniesView()
    {
        InitializeComponent();
        Reload();
    }

    public void Reload()
    {
        DgCompanies.ItemsSource = null;

        var companies = CompanyConnectionService
            .GetAll()
            .Where(x => x.IsActive)
            .OrderBy(x => x.CompanyName)
            .ToList();

        DgCompanies.ItemsSource = companies;

        ClearSelection();
    }

    private void DgCompanies_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        _selectedCompany = DgCompanies.SelectedItem as CompanyConnection;

        if (_selectedCompany == null)
        {
            ClearSelection();
            return;
        }

        TxtSelectedCompany.Text = _selectedCompany.CompanyName;
    }

    private void ClearSelection()
    {
        _selectedCompany = null;
        TxtSelectedCompany.Text = "Ninguna";
    }

    // ============================================================
    // ACCIONES DE LA BARRA SUPERIOR
    // ============================================================

    public void CreateNewCompany()
    {
        var window = new NewCompanyWindow
        {
            Owner = Window.GetWindow(this)
        };

        var result = window.ShowDialog();

        if (result == true)
        {
            Reload();
        }
    }

    public void EditSelectedCompany()
    {
        if (_selectedCompany == null)
        {
            MessageBox.Show(
                "Seleccione una empresa antes de editar.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        MessageBox.Show(
            $"La edición de '{_selectedCompany.CompanyName}' se implementará en el siguiente paso.",
            "SMART ERP",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public void DeleteSelectedCompany()
    {
        if (_selectedCompany == null)
        {
            MessageBox.Show(
                "Seleccione una empresa antes de eliminar.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var result = MessageBox.Show(
            $"¿Desea eliminar la configuración de '{_selectedCompany.CompanyName}'?",
            "Eliminar empresa",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        CompanyConnectionService.Delete(_selectedCompany.Id);

        Reload();
    }
}




