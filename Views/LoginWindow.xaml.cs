using System.Windows;
using System.Windows.Controls;
using SMART_ERP.Models;
using SMART_ERP.Services;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoadCompanies();
        }

        private void LoadCompanies()
        {
            try
            {
                var companies = CompanyConnectionService.GetAll()
                    .Where(company => company.IsActive)
                    .OrderBy(company => company.CompanyName)
                    .ToList();

                CmbCompany.ItemsSource = companies;
                
                if (companies.Any())
                {
                    CmbCompany.SelectedItem = companies
                        .OrderByDescending(c => c.LastConnectionAt ?? DateTime.MinValue)
                        .FirstOrDefault() ?? companies.FirstOrDefault();
                }
            }
            catch
            {
                // Si falla cargar empresas, continuar sin ellas
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void CreateCompany_Click(object sender, RoutedEventArgs e)
        {
            var newCompanyWindow = new NewCompanyWindow
            {
                Owner = this
            };

            newCompanyWindow.ShowDialog();

            LoadCompanies();
        }

        private void Company_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (CmbCompany.SelectedItem is not CompanyConnection company)
                return;

            company.LastConnectionAt = DateTime.Now;
            CompanyConnectionService.Save(company);
            CompanyConnectionService.SetActiveCompany(company);
        }
    }
}
