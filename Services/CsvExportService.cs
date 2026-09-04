using SMART_ERP.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace SMART_ERP.Services;

public static class CsvExportService
{
    public static void ExportSalesSummaryToCsv(ObservableCollection<SalesSummaryReport> data, SalesSummaryTotals totals)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("SMART ERP - Reporte General de Ventas");
            csv.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
            csv.AppendLine();

            // Column Headers
            csv.AppendLine("#,Factura,Fecha,Cliente,Empresa,Área,Vendedor,Forma Pago,Días Crédito,Estado,Total,Base Comisión,%,Comisión");

            // Data
            foreach (var item in data)
            {
                csv.AppendLine($"{item.SaleId}," +
                             $"\"{item.InvoiceNumber}\"," +
                             $"\"{item.SaleDate:dd/MM/yyyy}\"," +
                             $"\"{EscapeCsv(item.CustomerName)}\"," +
                             $"\"{EscapeCsv(item.BillingCompanyName)}\"," +
                             $"\"{EscapeCsv(item.OperationalAreaName)}\"," +
                             $"\"{EscapeCsv(item.PrimaryVendorName)}\"," +
                             $"\"{item.PaymentMethod}\"," +
                             $"{item.CreditDays}," +
                             $"\"{item.Status}\"," +
                             $"{item.Total:F2}," +
                             $"{item.CommissionBase:F2}," +
                             $"{item.CommissionPercentage:F2}," +
                             $"{item.CommissionAmount:F2}");
            }

            // Totals
            csv.AppendLine();
            csv.AppendLine("TOTALES");
            csv.AppendLine($"Cantidad de Ventas,{totals.SalesCount}");
            csv.AppendLine($"Total Vendido,{totals.TotalSold:F2}");
            csv.AppendLine($"Base Comisión,{totals.TotalCommissionBase:F2}");
            csv.AppendLine($"Total Comisiones,{totals.TotalCommissions:F2}");

            SaveCsvFile(csv, "ReporteVentas");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportVendorReportToCsv(ObservableCollection<VendorReport> data)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("SMART ERP - Reporte por Vendedor");
            csv.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
            csv.AppendLine();

            // Column Headers
            csv.AppendLine("Vendedor,Ventas,Total Vendido,Base Comisión,Comisión Generada,Promedio Venta");

            // Data
            foreach (var item in data)
            {
                csv.AppendLine($"\"{EscapeCsv(item.VendorName)}\"," +
                             $"{item.SalesCount}," +
                             $"{item.TotalSold:F2}," +
                             $"{item.CommissionBase:F2}," +
                             $"{item.CommissionGenerated:F2}," +
                             $"{item.AverageSale:F2}");
            }

            SaveCsvFile(csv, "ReporteVendedores");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportCustomerReportToCsv(ObservableCollection<CustomerReport> data)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("SMART ERP - Reporte por Cliente");
            csv.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
            csv.AppendLine();

            // Column Headers
            csv.AppendLine("Cliente,Compras,Total Comprado,Promedio Compra,Última Compra");

            // Data
            foreach (var item in data)
            {
                csv.AppendLine($"\"{EscapeCsv(item.CustomerName)}\"," +
                             $"{item.PurchaseCount}," +
                             $"{item.TotalPurchased:F2}," +
                             $"{item.AveragePurchase:F2}," +
                             $"\"{item.LastPurchaseDate?.ToString("dd/MM/yyyy") ?? ""}\"");
            }

            SaveCsvFile(csv, "ReporteClientes");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportBillingCompanyReportToCsv(ObservableCollection<BillingCompanyReport> data)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("SMART ERP - Reporte por Empresa Facturadora");
            csv.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
            csv.AppendLine();

            // Column Headers
            csv.AppendLine("Empresa,Ventas,Total Vendido,Base Comisión,Comisiones");

            // Data
            foreach (var item in data)
            {
                csv.AppendLine($"\"{EscapeCsv(item.CompanyName)}\"," +
                             $"{item.SalesCount}," +
                             $"{item.TotalSold:F2}," +
                             $"{item.CommissionBase:F2}," +
                             $"{item.Commissions:F2}");
            }

            SaveCsvFile(csv, "ReporteEmpresas");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportOperationalAreaReportToCsv(ObservableCollection<OperationalAreaReport> data)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("SMART ERP - Reporte por Área Operativa");
            csv.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
            csv.AppendLine();

            // Column Headers
            csv.AppendLine("Área,Ventas,Total Vendido,Comisiones");

            // Data
            foreach (var item in data)
            {
                csv.AppendLine($"\"{EscapeCsv(item.AreaName)}\"," +
                             $"{item.SalesCount}," +
                             $"{item.TotalSold:F2}," +
                             $"{item.Commissions:F2}");
            }

            SaveCsvFile(csv, "ReporteAreas");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportPaymentMethodReportToCsv(ObservableCollection<PaymentMethodReport> data)
    {
        try
        {
            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("SMART ERP - Reporte por Forma de Pago");
            csv.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
            csv.AppendLine();

            // Column Headers
            csv.AppendLine("Forma de Pago,Operaciones,Total,%");

            // Data
            foreach (var item in data)
            {
                csv.AppendLine($"\"{item.PaymentMethod}\"," +
                             $"{item.OperationCount}," +
                             $"{item.Total:F2}," +
                             $"{item.PercentageOfTotal:F2}");
            }

            SaveCsvFile(csv, "ReporteFormasPago");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void SaveCsvFile(StringBuilder csv, string baseFileName)
    {
        var saveDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            Title = "Guardar Reporte CSV",
            FileName = $"{baseFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllText(saveDialog.FileName, csv.ToString(), Encoding.UTF8);
            MessageBox.Show("Reporte exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private static string EscapeCsv(string field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        // Escape quotes and wrap in quotes if contains comma, quote or newline
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            return field.Replace("\"", "\"\"");
        }

        return field;
    }
}
