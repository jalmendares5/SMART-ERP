using System.Collections.ObjectModel;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class InvoiceViewModel : BaseViewModel
    {
        private ObservableCollection<Invoice> _invoices = new();
        private ObservableCollection<InvoiceItem> _invoiceItems = new();
        private ObservableCollection<Customer> _customers = new();
        private ObservableCollection<Product> _products = new();
        private Invoice? _selectedInvoice;
        private InvoiceItem? _selectedInvoiceItem;
        private string _invoiceNumber = string.Empty;
        private DateTime _invoiceDate = DateTime.Now;
        private Customer? _selectedCustomer;
        private string _salesperson = string.Empty;
        private string _paymentTerms = "CONTADO";
        private int _creditDays;
        private decimal _subtotal;
        private decimal _tax;
        private decimal _discount;
        private decimal _total;
        private decimal _paidAmount;
        private decimal _balance;
        private string _status = "PENDING";
        private string _notes = string.Empty;
        private string _errorMessage = string.Empty;
        private Product? _selectedProduct;
        private decimal _itemQuantity;
        private decimal _itemPrice;
        private decimal _itemDiscount;

        public ObservableCollection<Invoice> Invoices
        {
            get => _invoices;
            set => SetProperty(ref _invoices, value);
        }

        public ObservableCollection<InvoiceItem> InvoiceItems
        {
            get => _invoiceItems;
            set => SetProperty(ref _invoiceItems, value);
        }

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Invoice? SelectedInvoice
        {
            get => _selectedInvoice;
            set => SetProperty(ref _selectedInvoice, value);
        }

        public InvoiceItem? SelectedInvoiceItem
        {
            get => _selectedInvoiceItem;
            set => SetProperty(ref _selectedInvoiceItem, value);
        }

        public string InvoiceNumber
        {
            get => _invoiceNumber;
            set => SetProperty(ref _invoiceNumber, value);
        }

        public DateTime InvoiceDate
        {
            get => _invoiceDate;
            set => SetProperty(ref _invoiceDate, value);
        }

        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public string Salesperson
        {
            get => _salesperson;
            set => SetProperty(ref _salesperson, value);
        }

        public string PaymentTerms
        {
            get => _paymentTerms;
            set => SetProperty(ref _paymentTerms, value);
        }

        public int CreditDays
        {
            get => _creditDays;
            set => SetProperty(ref _creditDays, value);
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set => SetProperty(ref _subtotal, value);
        }

        public decimal Tax
        {
            get => _tax;
            set => SetProperty(ref _tax, value);
        }

        public decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        public decimal Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public decimal PaidAmount
        {
            get => _paidAmount;
            set => SetProperty(ref _paidAmount, value);
        }

        public decimal Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
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

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public decimal ItemQuantity
        {
            get => _itemQuantity;
            set => SetProperty(ref _itemQuantity, value);
        }

        public decimal ItemPrice
        {
            get => _itemPrice;
            set => SetProperty(ref _itemPrice, value);
        }

        public decimal ItemDiscount
        {
            get => _itemDiscount;
            set => SetProperty(ref _itemDiscount, value);
        }

        public ICommand CreateNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand RefreshCommand { get; }

        public InvoiceViewModel()
        {
            CreateNewCommand = new RelayCommand(ExecuteCreateNew);
            SaveCommand = new RelayCommand(ExecuteSave);
            AddItemCommand = new RelayCommand(ExecuteAddItem);
            RemoveItemCommand = new RelayCommand(ExecuteRemoveItem);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            Task.Run(async () => await LoadDataAsync());
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var invoices = await InvoiceService.GetAllAsync();
                var customers = Task.Run(() => CustomerService.GetAll()).Result;
                var products = await ProductService.GetAllAsync();

                Invoices = new ObservableCollection<Invoice>(invoices);
                Customers = new ObservableCollection<Customer>(customers);
                Products = new ObservableCollection<Product>(products);
                
                InvoiceNumber = InvoiceService.GenerateNextInvoiceNumber();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar datos: {ex.Message}";
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

            if (SelectedCustomer == null)
            {
                ErrorMessage = "Debe seleccionar un cliente.";
                return;
            }

            if (!InvoiceItems.Any())
            {
                ErrorMessage = "Debe agregar al menos un producto a la factura.";
                return;
            }

            try
            {
                var invoice = new Invoice
                {
                    InvoiceNumber = InvoiceNumber,
                    InvoiceDate = InvoiceDate,
                    CustomerId = SelectedCustomer.Id,
                    CustomerName = SelectedCustomer.Name,
                    Salesperson = Salesperson,
                    PaymentTerms = PaymentTerms,
                    CreditDays = CreditDays,
                    Subtotal = Subtotal,
                    Tax = Tax,
                    Discount = Discount,
                    Total = Total,
                    PaidAmount = PaidAmount,
                    Balance = Balance,
                    Status = Status,
                    Notes = Notes,
                    CreatedBy = AuthenticationService.CurrentUser?.FullName ?? "Sistema"
                };

                await InvoiceService.CreateAsync(invoice);
                _ = LoadDataAsync();
                ClearForm();
                
                System.Windows.MessageBox.Show(
                    $"Factura {InvoiceNumber} creada exitosamente.",
                    "Éxito",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al guardar factura: {ex.Message}";
            }
        }

        private void ExecuteAddItem(object? parameter)
        {
            if (SelectedProduct == null)
            {
                ErrorMessage = "Debe seleccionar un producto.";
                return;
            }

            if (ItemQuantity <= 0)
            {
                ErrorMessage = "La cantidad debe ser mayor a 0.";
                return;
            }

            var itemTotal = (ItemQuantity * ItemPrice) - ItemDiscount;
            
            var invoiceItem = new InvoiceItem
            {
                ProductId = SelectedProduct.Id,
                ProductCode = SelectedProduct.Code,
                ProductName = SelectedProduct.Name,
                Quantity = ItemQuantity,
                Price = ItemPrice,
                Discount = ItemDiscount,
                Total = itemTotal
            };

            InvoiceItems.Add(invoiceItem);
            CalculateTotals();
            
            ItemQuantity = 0;
            ItemPrice = 0;
            ItemDiscount = 0;
            SelectedProduct = null;
        }

        private void ExecuteRemoveItem(object? parameter)
        {
            if (SelectedInvoiceItem != null)
            {
                InvoiceItems.Remove(SelectedInvoiceItem);
                CalculateTotals();
            }
        }

        private void CalculateTotals()
        {
            Subtotal = InvoiceItems.Sum(i => i.Total);
            Tax = Subtotal * 0.15m; // 15% ISV en Honduras
            Discount = 0;
            Total = Subtotal + Tax - Discount;
            PaidAmount = PaymentTerms == "CONTADO" ? Total : 0;
            Balance = Total - PaidAmount;
        }

        private void ExecuteRefresh(object? parameter)
        {
            Task.Run(async () => await LoadDataAsync());
        }

        private void ClearForm()
        {
            InvoiceNumber = InvoiceService.GenerateNextInvoiceNumber();
            InvoiceDate = DateTime.Now;
            SelectedCustomer = null;
            Salesperson = string.Empty;
            PaymentTerms = "CONTADO";
            CreditDays = 0;
            Subtotal = 0;
            Tax = 0;
            Discount = 0;
            Total = 0;
            PaidAmount = 0;
            Balance = 0;
            Status = "PENDING";
            Notes = string.Empty;
            InvoiceItems.Clear();
            SelectedProduct = null;
            ItemQuantity = 0;
            ItemPrice = 0;
            ItemDiscount = 0;
        }
    }
}
