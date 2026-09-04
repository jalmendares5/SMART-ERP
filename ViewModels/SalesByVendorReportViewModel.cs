using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class SalesByVendorReportViewModel : BaseViewModel
    {
        private ObservableCollection<SalesByVendorReport> _salesReports = new();
        private DateTime _startDate = DateTime.Now.AddMonths(-1);
        private DateTime _endDate = DateTime.Now;
        private string _errorMessage = string.Empty;
        private decimal _totalSalesAllVendors;
        private int _totalSalesCount;

        public ObservableCollection<SalesByVendorReport> SalesReports
        {
            get => _salesReports;
            set => SetProperty(ref _salesReports, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public decimal TotalSalesAllVendors
        {
            get => _totalSalesAllVendors;
            set => SetProperty(ref _totalSalesAllVendors, value);
        }

        public int TotalSalesCount
        {
            get => _totalSalesCount;
            set => SetProperty(ref _totalSalesCount, value);
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportExcelCommand { get; }
        public ICommand ExportPdfCommand { get; }

        public SalesByVendorReportViewModel()
        {
            GenerateReportCommand = new RelayCommand(ExecuteGenerateReport);
            ExportExcelCommand = new RelayCommand(ExecuteExportExcel);
            ExportPdfCommand = new RelayCommand(ExecuteExportPdf);

            Task.Run(async () => await LoadReportAsync());
        }

        private async Task LoadReportAsync()
        {
            try
            {
                var reports = await SalesByVendorReportService.GenerateReportAsync(StartDate, EndDate);
                SalesReports = new ObservableCollection<SalesByVendorReport>(reports);
                
                TotalSalesAllVendors = reports.Sum(r => r.TotalSales);
                TotalSalesCount = reports.Sum(r => r.SalesCount);
                
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reporte: {ex.Message}";
            }
        }

        private async void ExecuteGenerateReport(object? parameter)
        {
            ErrorMessage = string.Empty;
            await LoadReportAsync();
        }

        private async void ExecuteExportExcel(object? parameter)
        {
            try
            {
                // Por ahora mostramos un mensaje de información
                System.Windows.MessageBox.Show(
                    "Funcionalidad de exportación a Excel en desarrollo.",
                    "Información",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al exportar a Excel: {ex.Message}";
            }
        }

        private async void ExecuteExportPdf(object? parameter)
        {
            try
            {
                // Por ahora mostramos un mensaje de información
                System.Windows.MessageBox.Show(
                    "Funcionalidad de exportación a PDF en desarrollo.",
                    "Información",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al exportar a PDF: {ex.Message}";
            }
        }
    }
}
