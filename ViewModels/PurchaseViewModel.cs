using System.Collections.ObjectModel;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class PurchaseViewModel : BaseViewModel
    {
        private ObservableCollection<Purchase> _purchases = new();
        private ObservableCollection<PurchaseItem> _purchaseItems = new();
        private ObservableCollection<Vendor> _vendors = new();
        private ObservableCollection<Product> _products = new();
        private Purchase? _selectedPurchase;
        private PurchaseItem? _selectedPurchaseItem;
        private string _purchaseNumber = string.Empty;
        private DateTime _purchaseDate = DateTime.Now;
        private Vendor? _selectedVendor;
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
        private decimal _itemCost;
        private decimal _itemDiscount;

        public ObservableCollection<Purchase> Purchases
        {
            get => _purchases;
            set => SetProperty(ref _purchases, value);
        }

        public ObservableCollection<PurchaseItem> PurchaseItems
        {
            get => _purchaseItems;
            set => SetProperty(ref _purchaseItems, value);
        }

        public ObservableCollection<Vendor> Vendors
        {
            get => _vendors;
            set => SetProperty(ref _vendors, value);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Purchase? SelectedPurchase
        {
            get => _selectedPurchase;
            set => SetProperty(ref _selectedPurchase, value);
        }

        public PurchaseItem? SelectedPurchaseItem
        {
            get => _selectedPurchaseItem;
            set => SetProperty(ref _selectedPurchaseItem, value);
        }

        public string PurchaseNumber
        {
            get => _purchaseNumber;
            set => SetProperty(ref _purchaseNumber, value);
        }

        public DateTime PurchaseDate
        {
            get => _purchaseDate;
            set => SetProperty(ref _purchaseDate, value);
        }

        public Vendor? SelectedVendor
        {
            get => _selectedVendor;
            set => SetProperty(ref _selectedVendor, value);
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

        public decimal ItemCost
        {
            get => _itemCost;
            set => SetProperty(ref _itemCost, value);
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

        public PurchaseViewModel()
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
                var purchases = await PurchaseService.GetAllAsync();
                var vendors = Task.Run(() => VendorService.GetAll()).Result;
                var products = await ProductService.GetAllAsync();

                Purchases = new ObservableCollection<Purchase>(purchases);
                Vendors = new ObservableCollection<Vendor>(vendors);
                Products = new ObservableCollection<Product>(products);
                
                PurchaseNumber = PurchaseService.GenerateNextPurchaseNumber();
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

            if (SelectedVendor == null)
            {
                ErrorMessage = "Debe seleccionar un proveedor.";
                return;
            }

            if (!PurchaseItems.Any())
            {
                ErrorMessage = "Debe agregar al menos un producto a la compra.";
                return;
            }

            try
            {
                var purchase = new Purchase
                {
                    PurchaseNumber = PurchaseNumber,
                    PurchaseDate = PurchaseDate,
                    VendorId = SelectedVendor.Id,
                    VendorName = SelectedVendor.Name,
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

                await PurchaseService.CreateAsync(purchase);
                _ = LoadDataAsync();
                ClearForm();
                
                System.Windows.MessageBox.Show(
                    $"Compra {PurchaseNumber} creada exitosamente.",
                    "Éxito",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al guardar compra: {ex.Message}";
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

            var itemTotal = (ItemQuantity * ItemCost) - ItemDiscount;
            
            var purchaseItem = new PurchaseItem
            {
                ProductId = SelectedProduct.Id,
                ProductCode = SelectedProduct.Code,
                ProductName = SelectedProduct.Name,
                Quantity = ItemQuantity,
                Cost = ItemCost,
                Discount = ItemDiscount,
                Total = itemTotal
            };

            PurchaseItems.Add(purchaseItem);
            CalculateTotals();
            
            ItemQuantity = 0;
            ItemCost = 0;
            ItemDiscount = 0;
            SelectedProduct = null;
        }

        private void ExecuteRemoveItem(object? parameter)
        {
            if (SelectedPurchaseItem != null)
            {
                PurchaseItems.Remove(SelectedPurchaseItem);
                CalculateTotals();
            }
        }

        private void CalculateTotals()
        {
            Subtotal = PurchaseItems.Sum(i => i.Total);
            Tax = Subtotal * 0.15m; // 15% ISV en Honduras
            Discount = 0;
            Total = Subtotal + Tax - Discount;
            PaidAmount = 0;
            Balance = Total;
        }

        private void ExecuteRefresh(object? parameter)
        {
            Task.Run(async () => await LoadDataAsync());
        }

        private void ClearForm()
        {
            PurchaseNumber = PurchaseService.GenerateNextPurchaseNumber();
            PurchaseDate = DateTime.Now;
            SelectedVendor = null;
            Subtotal = 0;
            Tax = 0;
            Discount = 0;
            Total = 0;
            PaidAmount = 0;
            Balance = 0;
            Status = "PENDING";
            Notes = string.Empty;
            PurchaseItems.Clear();
            SelectedProduct = null;
            ItemQuantity = 0;
            ItemCost = 0;
            ItemDiscount = 0;
        }
    }
}
