using System.Collections.ObjectModel;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private ObservableCollection<Product> _products = new();
        private Product? _selectedProduct;
        private string _code = string.Empty;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _category = string.Empty;
        private decimal _cost;
        private decimal _price1;
        private decimal _price2;
        private decimal _price3;
        private decimal _price4;
        private decimal _stock;
        private decimal _minStock;
        private decimal _maxStock;
        private string _unit = "UNIDAD";
        private string _barCode = string.Empty;
        private bool _isActive = true;
        private bool _isEditing;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public decimal Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }

        public decimal Price1
        {
            get => _price1;
            set => SetProperty(ref _price1, value);
        }

        public decimal Price2
        {
            get => _price2;
            set => SetProperty(ref _price2, value);
        }

        public decimal Price3
        {
            get => _price3;
            set => SetProperty(ref _price3, value);
        }

        public decimal Price4
        {
            get => _price4;
            set => SetProperty(ref _price4, value);
        }

        public decimal Stock
        {
            get => _stock;
            set => SetProperty(ref _stock, value);
        }

        public decimal MinStock
        {
            get => _minStock;
            set => SetProperty(ref _minStock, value);
        }

        public decimal MaxStock
        {
            get => _maxStock;
            set => SetProperty(ref _maxStock, value);
        }

        public string Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        public string BarCode
        {
            get => _barCode;
            set => SetProperty(ref _barCode, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand CreateNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RefreshCommand { get; }

        public ProductsViewModel()
        {
            CreateNewCommand = new RelayCommand(ExecuteCreateNew);
            SaveCommand = new RelayCommand(ExecuteSave);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            CancelCommand = new RelayCommand(ExecuteCancel);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            Task.Run(async () => await LoadProductsAsync());
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await ProductService.GetAllAsync();
                Products = new ObservableCollection<Product>(products);
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar productos: {ex.Message}";
            }
        }

        private void ExecuteCreateNew(object? parameter)
        {
            ClearForm();
            IsEditing = false;
            ErrorMessage = string.Empty;
        }

        private async void ExecuteSave(object? parameter)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Code) ||
                string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Debe completar código y nombre del producto.";
                return;
            }

            try
            {
                var product = new Product
                {
                    Code = Code,
                    Name = Name,
                    Description = Description,
                    Category = Category,
                    Cost = Cost,
                    Price1 = Price1,
                    Price2 = Price2,
                    Price3 = Price3,
                    Price4 = Price4,
                    Stock = Stock,
                    MinStock = MinStock,
                    MaxStock = MaxStock,
                    Unit = Unit,
                    BarCode = BarCode,
                    IsActive = IsActive,
                    CreatedBy = AuthenticationService.CurrentUser?.FullName ?? "Sistema"
                };

                if (IsEditing && SelectedProduct != null)
                {
                    product.Id = SelectedProduct.Id;
                    product.CreatedAt = SelectedProduct.CreatedAt;
                    await ProductService.UpdateAsync(product);
                }
                else
                {
                    await ProductService.CreateAsync(product);
                }

                _ = LoadProductsAsync();
                ClearForm();
                IsEditing = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al guardar producto: {ex.Message}";
            }
        }

        private async void ExecuteDelete(object? parameter)
        {
            if (SelectedProduct == null)
            {
                ErrorMessage = "Debe seleccionar un producto para eliminar.";
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"¿Está seguro de eliminar el producto '{SelectedProduct.Name}'?",
                "Confirmar eliminación",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await ProductService.DeleteAsync(SelectedProduct.Id);
                    _ = LoadProductsAsync();
                    ClearForm();
                    IsEditing = false;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al eliminar producto: {ex.Message}";
                }
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            ClearForm();
            IsEditing = false;
            ErrorMessage = string.Empty;
        }

        private void ExecuteRefresh(object? parameter)
        {
            Task.Run(async () => await LoadProductsAsync());
        }

        private void ClearForm()
        {
            Code = ProductService.GenerateNextCode();
            Name = string.Empty;
            Description = string.Empty;
            Category = string.Empty;
            Cost = 0;
            Price1 = 0;
            Price2 = 0;
            Price3 = 0;
            Price4 = 0;
            Stock = 0;
            MinStock = 0;
            MaxStock = 0;
            Unit = "UNIDAD";
            BarCode = string.Empty;
            IsActive = true;
        }
    }
}
