using System.Collections.ObjectModel;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class CashTransactionViewModel : BaseViewModel
    {
        private ObservableCollection<CashTransaction> _transactions = new();
        private CashTransaction? _selectedTransaction;
        private string _transactionNumber = string.Empty;
        private DateTime _transactionDate = DateTime.Now;
        private string _transactionType = "IN";
        private string _category = string.Empty;
        private string _description = string.Empty;
        private decimal _amount;
        private string _referenceType = string.Empty;
        private string _referenceNumber = string.Empty;
        private string _notes = string.Empty;
        private string _errorMessage = string.Empty;
        private decimal _currentBalance;

        public ObservableCollection<CashTransaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        public CashTransaction? SelectedTransaction
        {
            get => _selectedTransaction;
            set => SetProperty(ref _selectedTransaction, value);
        }

        public string TransactionNumber
        {
            get => _transactionNumber;
            set => SetProperty(ref _transactionNumber, value);
        }

        public DateTime TransactionDate
        {
            get => _transactionDate;
            set => SetProperty(ref _transactionDate, value);
        }

        public string TransactionType
        {
            get => _transactionType;
            set => SetProperty(ref _transactionType, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public string ReferenceType
        {
            get => _referenceType;
            set => SetProperty(ref _referenceType, value);
        }

        public string ReferenceNumber
        {
            get => _referenceNumber;
            set => SetProperty(ref _referenceNumber, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public decimal CurrentBalance
        {
            get => _currentBalance;
            set => SetProperty(ref _currentBalance, value);
        }

        public ICommand CreateNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }

        public CashTransactionViewModel()
        {
            CreateNewCommand = new RelayCommand(ExecuteCreateNew);
            SaveCommand = new RelayCommand(ExecuteSave);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            Task.Run(async () => await LoadDataAsync());
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var transactions = await CashTransactionService.GetAllAsync();
                Transactions = new ObservableCollection<CashTransaction>(transactions);
                CurrentBalance = await CashTransactionService.GetBalanceAsync();
                
                TransactionNumber = CashTransactionService.GenerateNextTransactionNumber();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar transacciones: {ex.Message}";
            }
        }

        private void ExecuteCreateNew(object? parameter)
        {
            ClearForm();
            ErrorMessage = string.Empty;
        }

        private async void ExecuteSave(object? parameter)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Description))
            {
                ErrorMessage = "Debe ingresar una descripción.";
                return;
            }

            if (Amount <= 0)
            {
                ErrorMessage = "El monto debe ser mayor a 0.";
                return;
            }

            try
            {
                var transaction = new CashTransaction
                {
                    TransactionNumber = TransactionNumber,
                    TransactionDate = TransactionDate,
                    TransactionType = TransactionType,
                    Category = Category,
                    Description = Description,
                    Amount = Amount,
                    ReferenceType = ReferenceType,
                    ReferenceNumber = ReferenceNumber,
                    Notes = Notes,
                    CreatedBy = AuthenticationService.CurrentUser?.FullName ?? "Sistema"
                };

                await CashTransactionService.CreateAsync(transaction);
                _ = LoadDataAsync();
                ClearForm();
                
                System.Windows.MessageBox.Show(
                    "Transacción registrada exitosamente.",
                    "Éxito",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al guardar transacción: {ex.Message}";
            }
        }

        private void ExecuteRefresh(object? parameter)
        {
            Task.Run(async () => await LoadDataAsync());
        }

        private void ClearForm()
        {
            TransactionNumber = CashTransactionService.GenerateNextTransactionNumber();
            TransactionDate = DateTime.Now;
            TransactionType = "IN";
            Category = string.Empty;
            Description = string.Empty;
            Amount = 0;
            ReferenceType = string.Empty;
            ReferenceNumber = string.Empty;
            Notes = string.Empty;
        }
    }
}
