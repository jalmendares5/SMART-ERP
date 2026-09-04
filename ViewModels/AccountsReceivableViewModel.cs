using System.Collections.ObjectModel;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class AccountsReceivableViewModel : BaseViewModel
    {
        private ObservableCollection<AccountsReceivable> _accounts = new();
        private AccountsReceivable? _selectedAccount;
        private decimal _paymentAmount;
        private string _errorMessage = string.Empty;
        private decimal _totalBalance;
        private decimal _totalOverdue;

        public ObservableCollection<AccountsReceivable> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        public AccountsReceivable? SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value);
        }

        public decimal PaymentAmount
        {
            get => _paymentAmount;
            set => SetProperty(ref _paymentAmount, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public decimal TotalBalance
        {
            get => _totalBalance;
            set => SetProperty(ref _totalBalance, value);
        }

        public decimal TotalOverdue
        {
            get => _totalOverdue;
            set => SetProperty(ref _totalOverdue, value);
        }

        public ICommand RecordPaymentCommand { get; }
        public ICommand RefreshCommand { get; }

        public AccountsReceivableViewModel()
        {
            RecordPaymentCommand = new RelayCommand(ExecuteRecordPayment);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            Task.Run(async () => await LoadAccountsAsync());
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                var accounts = await AccountsReceivableService.GetAllAsync();
                Accounts = new ObservableCollection<AccountsReceivable>(accounts);
                
                TotalBalance = accounts.Sum(a => a.Balance);
                TotalOverdue = accounts.Where(a => a.Status == "OVERDUE").Sum(a => a.Balance);
                
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar cuentas por cobrar: {ex.Message}";
            }
        }

        private async void ExecuteRecordPayment(object? parameter)
        {
            ErrorMessage = string.Empty;

            if (SelectedAccount == null)
            {
                ErrorMessage = "Debe seleccionar una cuenta por cobrar.";
                return;
            }

            if (PaymentAmount <= 0)
            {
                ErrorMessage = "El monto del pago debe ser mayor a 0.";
                return;
            }

            if (PaymentAmount > SelectedAccount.Balance)
            {
                ErrorMessage = "El monto del pago no puede exceder el saldo pendiente.";
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"¿Está seguro de registrar un pago de L {PaymentAmount:N2} a {SelectedAccount.CustomerName}?",
                "Confirmar Pago",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await AccountsReceivableService.UpdatePaymentAsync(SelectedAccount.InvoiceId, PaymentAmount);
                    _ = LoadAccountsAsync();
                    PaymentAmount = 0;
                    
                    System.Windows.MessageBox.Show(
                        "Pago registrado exitosamente.",
                        "Éxito",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al registrar pago: {ex.Message}";
                }
            }
        }

        private void ExecuteRefresh(object? parameter)
        {
            Task.Run(async () => await LoadAccountsAsync());
        }
    }
}
