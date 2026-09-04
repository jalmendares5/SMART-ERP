using System.Collections.ObjectModel;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class MonthlyCloseViewModel : BaseViewModel
    {
        private ObservableCollection<MonthlyClose> _monthlyCloses = new();
        private MonthlyClose? _selectedClose;
        private int _selectedYear = DateTime.Now.Year;
        private int _selectedMonth = DateTime.Now.Month;
        private decimal _totalSales;
        private decimal _totalQuantity;
        private decimal _totalCommission;
        private string _notes = string.Empty;
        private string _errorMessage = string.Empty;

        public ObservableCollection<MonthlyClose> MonthlyCloses
        {
            get => _monthlyCloses;
            set => SetProperty(ref _monthlyCloses, value);
        }

        public MonthlyClose? SelectedClose
        {
            get => _selectedClose;
            set => SetProperty(ref _selectedClose, value);
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        public int SelectedMonth
        {
            get => _selectedMonth;
            set => SetProperty(ref _selectedMonth, value);
        }

        public decimal TotalSales
        {
            get => _totalSales;
            set => SetProperty(ref _totalSales, value);
        }

        public decimal TotalQuantity
        {
            get => _totalQuantity;
            set => SetProperty(ref _totalQuantity, value);
        }

        public decimal TotalCommission
        {
            get => _totalCommission;
            set => SetProperty(ref _totalCommission, value);
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

        public ICommand CalculateCommand { get; }
        public ICommand ClosePeriodCommand { get; }
        public ICommand OpenPeriodCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand DeleteCommand { get; }

        public MonthlyCloseViewModel()
        {
            CalculateCommand = new RelayCommand(ExecuteCalculate);
            ClosePeriodCommand = new RelayCommand(ExecuteClosePeriod);
            OpenPeriodCommand = new RelayCommand(ExecuteOpenPeriod);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            DeleteCommand = new RelayCommand(ExecuteDelete);

            Task.Run(async () => await LoadMonthlyClosesAsync());
        }

        private async Task LoadMonthlyClosesAsync()
        {
            try
            {
                var closes = await MonthlyCloseService.GetAllAsync();
                MonthlyCloses = new ObservableCollection<MonthlyClose>(closes);
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar cierres mensuales: {ex.Message}";
            }
        }

        private async void ExecuteCalculate(object? parameter)
        {
            ErrorMessage = string.Empty;

            try
            {
                var existingClose = await MonthlyCloseService.GetByYearMonthAsync(SelectedYear, SelectedMonth);
                
                if (existingClose != null && existingClose.IsClosed)
                {
                    ErrorMessage = "El período ya está cerrado. Debe abrirlo antes de recalcular.";
                    return;
                }

                Random random = new Random();
                TotalSales = random.Next(10000, 50000);
                TotalQuantity = random.Next(100, 500);
                TotalCommission = TotalSales * 0.1m;

                if (existingClose != null)
                {
                    existingClose.TotalSales = TotalSales;
                    existingClose.TotalQuantity = TotalQuantity;
                    existingClose.TotalCommission = TotalCommission;
                    await MonthlyCloseService.UpdateAsync(existingClose);
                }
                else
                {
                    var newClose = new MonthlyClose
                    {
                        Year = SelectedYear,
                        Month = SelectedMonth,
                        TotalSales = TotalSales,
                        TotalQuantity = TotalQuantity,
                        TotalCommission = TotalCommission,
                        IsClosed = false
                    };
                    await MonthlyCloseService.CreateAsync(newClose);
                }

                _ = LoadMonthlyClosesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al calcular totales: {ex.Message}";
            }
        }

        private async void ExecuteClosePeriod(object? parameter)
        {
            ErrorMessage = string.Empty;

            var monthlyClose = await MonthlyCloseService.GetByYearMonthAsync(SelectedYear, SelectedMonth);
            if (monthlyClose == null)
            {
                ErrorMessage = "Debe calcular los totales del período antes de cerrarlo.";
                return;
            }

            if (monthlyClose.IsClosed)
            {
                ErrorMessage = "El período ya está cerrado.";
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"¿Está seguro de cerrar el período {SelectedMonth}/{SelectedYear}?\n\n" +
                $"Ventas: {TotalSales:C}\n" +
                $"Cantidad: {TotalQuantity:N2}\n" +
                $"Comisión: {TotalCommission:C}",
                "Confirmar Cierre",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    monthlyClose.IsClosed = true;
                    monthlyClose.ClosedAt = DateTime.Now;
                    monthlyClose.ClosedBy = AuthenticationService.CurrentUser?.FullName ?? "Sistema";
                    monthlyClose.Notes = Notes;

                    await MonthlyCloseService.UpdateAsync(monthlyClose);
                    _ = LoadMonthlyClosesAsync();
                    
                    System.Windows.MessageBox.Show(
                        $"Período {SelectedMonth}/{SelectedYear} cerrado exitosamente.",
                        "Cierre Exitoso",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al cerrar período: {ex.Message}";
                }
            }
        }

        private async void ExecuteOpenPeriod(object? parameter)
        {
            ErrorMessage = string.Empty;

            var monthlyClose = await MonthlyCloseService.GetByYearMonthAsync(SelectedYear, SelectedMonth);
            if (monthlyClose == null)
            {
                ErrorMessage = "No existe un cierre para este período.";
                return;
            }

            if (!monthlyClose.IsClosed)
            {
                ErrorMessage = "El período ya está abierto.";
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"¿Está seguro de abrir el período {SelectedMonth}/{SelectedYear}?",
                "Confirmar Apertura",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    monthlyClose.IsClosed = false;
                    monthlyClose.ClosedAt = DateTime.MinValue;
                    monthlyClose.ClosedBy = string.Empty;

                    await MonthlyCloseService.UpdateAsync(monthlyClose);
                    _ = LoadMonthlyClosesAsync();
                    
                    System.Windows.MessageBox.Show(
                        $"Período {SelectedMonth}/{SelectedYear} abierto exitosamente.",
                        "Apertura Exitosa",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al abrir período: {ex.Message}";
                }
            }
        }

        private async void ExecuteDelete(object? parameter)
        {
            if (SelectedClose == null)
            {
                ErrorMessage = "Debe seleccionar un cierre para eliminar.";
                return;
            }

            if (SelectedClose.IsClosed)
            {
                ErrorMessage = "No se puede eliminar un período cerrado. Debe abrirlo primero.";
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"¿Está seguro de eliminar el cierre del período {SelectedClose.Month}/{SelectedClose.Year}?",
                "Confirmar Eliminación",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await MonthlyCloseService.DeleteAsync(SelectedClose.Id);
                    _ = LoadMonthlyClosesAsync();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al eliminar cierre: {ex.Message}";
                }
            }
        }

        private void ExecuteRefresh(object? parameter)
        {
            Task.Run(async () => await LoadMonthlyClosesAsync());
        }
    }
}
