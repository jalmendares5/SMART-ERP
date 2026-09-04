using ClosedXML.Excel;
using SMART_ERP.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SMART_ERP.Services;

public static class ExcelExportService
{
    public static void ExportSalesSummaryToExcel(ObservableCollection<SalesSummaryReport> data, SalesSummaryTotals totals)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Ventas");

            // Headers
            worksheet.Cell("B1").Value = "SMART ERP - Reporte General de Ventas";
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Font.FontSize = 14;
            worksheet.Range("B1:K1").Merge();

            worksheet.Cell("B2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("B2:K2").Merge();

            // Column Headers
            var headers = new[] { "#", "Factura", "Fecha", "Cliente", "Empresa", "Área", "Vendedor", "Forma Pago", "Días Crédito", "Estado", "Total", "Base Comisión", "%", "Comisión" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            int row = 5;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.SaleId;
                worksheet.Cell(row, 3).Value = item.InvoiceNumber;
                worksheet.Cell(row, 4).Value = item.SaleDate.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 5).Value = item.CustomerName;
                worksheet.Cell(row, 6).Value = item.BillingCompanyName;
                worksheet.Cell(row, 7).Value = item.OperationalAreaName;
                worksheet.Cell(row, 8).Value = item.PrimaryVendorName;
                worksheet.Cell(row, 9).Value = item.PaymentMethod;
                worksheet.Cell(row, 10).Value = item.CreditDays;
                worksheet.Cell(row, 11).Value = item.Status;
                worksheet.Cell(row, 12).Value = item.Total;
                worksheet.Cell(row, 12).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 13).Value = item.CommissionBase;
                worksheet.Cell(row, 13).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 14).Value = item.CommissionPercentage;
                worksheet.Cell(row, 14).Style.NumberFormat.Format = "0.00%";
                worksheet.Cell(row, 15).Value = item.CommissionAmount;
                worksheet.Cell(row, 15).Style.NumberFormat.Format = "L. #,##0.00";
                row++;
            }

            // Totals
            row += 2;
            worksheet.Cell(row, 2).Value = "TOTALES";
            worksheet.Cell(row, 2).Style.Font.Bold = true;
            worksheet.Range($"B{row}:C{row}").Merge();

            worksheet.Cell(row + 1, 2).Value = "Cantidad de Ventas:";
            worksheet.Cell(row + 1, 3).Value = totals.SalesCount;
            worksheet.Cell(row + 1, 3).Style.Font.Bold = true;

            worksheet.Cell(row + 2, 2).Value = "Total Vendido:";
            worksheet.Cell(row + 2, 3).Value = totals.TotalSold;
            worksheet.Cell(row + 2, 3).Style.NumberFormat.Format = "L. #,##0.00";
            worksheet.Cell(row + 2, 3).Style.Font.Bold = true;

            worksheet.Cell(row + 3, 2).Value = "Base Comisión:";
            worksheet.Cell(row + 3, 3).Value = totals.TotalCommissionBase;
            worksheet.Cell(row + 3, 3).Style.NumberFormat.Format = "L. #,##0.00";
            worksheet.Cell(row + 3, 3).Style.Font.Bold = true;

            worksheet.Cell(row + 4, 2).Value = "Total Comisiones:";
            worksheet.Cell(row + 4, 3).Value = totals.TotalCommissions;
            worksheet.Cell(row + 4, 3).Style.NumberFormat.Format = "L. #,##0.00";
            worksheet.Cell(row + 4, 3).Style.Font.Bold = true;

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar Reporte de Ventas",
                FileName = $"ReporteVentas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Reporte exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportVendorReportToExcel(ObservableCollection<VendorReport> data)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Vendedores");

            // Headers
            worksheet.Cell("B1").Value = "SMART ERP - Reporte por Vendedor";
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Font.FontSize = 14;
            worksheet.Range("B1:G1").Merge();

            worksheet.Cell("B2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("B2:G2").Merge();

            // Column Headers
            var headers = new[] { "Vendedor", "Ventas", "Total Vendido", "Base Comisión", "Comisión Generada", "Promedio Venta" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            int row = 5;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.VendorName;
                worksheet.Cell(row, 3).Value = item.SalesCount;
                worksheet.Cell(row, 4).Value = item.TotalSold;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 5).Value = item.CommissionBase;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 6).Value = item.CommissionGenerated;
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 7).Value = item.AverageSale;
                worksheet.Cell(row, 7).Style.NumberFormat.Format = "L. #,##0.00";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar Reporte por Vendedor",
                FileName = $"ReporteVendedores_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Reporte exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportCustomerReportToExcel(ObservableCollection<CustomerReport> data)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Clientes");

            // Headers
            worksheet.Cell("B1").Value = "SMART ERP - Reporte por Cliente";
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Font.FontSize = 14;
            worksheet.Range("B1:F1").Merge();

            worksheet.Cell("B2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("B2:F2").Merge();

            // Column Headers
            var headers = new[] { "Cliente", "Compras", "Total Comprado", "Promedio Compra", "Última Compra" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            int row = 5;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.CustomerName;
                worksheet.Cell(row, 3).Value = item.PurchaseCount;
                worksheet.Cell(row, 4).Value = item.TotalPurchased;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 5).Value = item.AveragePurchase;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 6).Value = item.LastPurchaseDate?.ToString("dd/MM/yyyy") ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar Reporte por Cliente",
                FileName = $"ReporteClientes_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Reporte exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportBillingCompanyReportToExcel(ObservableCollection<BillingCompanyReport> data)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Empresas");

            // Headers
            worksheet.Cell("B1").Value = "SMART ERP - Reporte por Empresa Facturadora";
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Font.FontSize = 14;
            worksheet.Range("B1:F1").Merge();

            worksheet.Cell("B2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("B2:F2").Merge();

            // Column Headers
            var headers = new[] { "Empresa", "Ventas", "Total Vendido", "Base Comisión", "Comisiones" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            int row = 5;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.CompanyName;
                worksheet.Cell(row, 3).Value = item.SalesCount;
                worksheet.Cell(row, 4).Value = item.TotalSold;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 5).Value = item.CommissionBase;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 6).Value = item.Commissions;
                worksheet.Cell(row, 6).Style.NumberFormat.Format = "L. #,##0.00";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar Reporte por Empresa",
                FileName = $"ReporteEmpresas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Reporte exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportOperationalAreaReportToExcel(ObservableCollection<OperationalAreaReport> data)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Áreas");

            // Headers
            worksheet.Cell("B1").Value = "SMART ERP - Reporte por Área Operativa";
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Font.FontSize = 14;
            worksheet.Range("B1:E1").Merge();

            worksheet.Cell("B2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("B2:E2").Merge();

            // Column Headers
            var headers = new[] { "Área", "Ventas", "Total Vendido", "Comisiones" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            int row = 5;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.AreaName;
                worksheet.Cell(row, 3).Value = item.SalesCount;
                worksheet.Cell(row, 4).Value = item.TotalSold;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 5).Value = item.Commissions;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "L. #,##0.00";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar Reporte por Área",
                FileName = $"ReporteAreas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Reporte exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportPaymentMethodReportToExcel(ObservableCollection<PaymentMethodReport> data)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte Formas Pago");

            // Headers
            worksheet.Cell("B1").Value = "SMART ERP - Reporte por Forma de Pago";
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Font.FontSize = 14;
            worksheet.Range("B1:E1").Merge();

            worksheet.Cell("B2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            worksheet.Range("B2:E2").Merge();

            // Column Headers
            var headers = new[] { "Forma de Pago", "Operaciones", "Total", "%" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 2);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            int row = 5;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.PaymentMethod;
                worksheet.Cell(row, 3).Value = item.OperationCount;
                worksheet.Cell(row, 4).Value = item.Total;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "L. #,##0.00";
                worksheet.Cell(row, 5).Value = item.PercentageOfTotal;
                worksheet.Cell(row, 5).Style.NumberFormat.Format = "0.00%";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Save
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar Reporte por Forma de Pago",
                FileName = $"ReporteFormasPago_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Reporte exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void ExportDashboardToExcel(DashboardSummary summary, ObservableCollection<TopVendor> topVendors, ObservableCollection<TopCustomer> topCustomers, ObservableCollection<SalesTrend> trends, DashboardComparison comparison)
    {
        try
        {
            using var workbook = new XLWorkbook();
            
            // Dashboard Summary Sheet
            var summarySheet = workbook.Worksheets.Add("Dashboard Resumen");
            
            summarySheet.Cell("B1").Value = "SMART ERP - Dashboard Resumen";
            summarySheet.Cell("B1").Style.Font.Bold = true;
            summarySheet.Cell("B1").Style.Font.FontSize = 16;
            summarySheet.Range("B1:K1").Merge();

            summarySheet.Cell("B2").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            summarySheet.Range("B2:K2").Merge();

            summarySheet.Cell("B3").Value = $"Período: {summary.ReportDate:dd/MM/yyyy} a {DateTime.Now:dd/MM/yyyy}";
            summarySheet.Range("B3:K3").Merge();

            // KPIs
            int row = 5;
            var kpis = new[] { summary.TotalSales, summary.TotalCommission, summary.SalesCount, summary.AverageSale, summary.ActiveVendors, summary.ActiveCustomers };
            foreach (var kpi in kpis)
            {
                summarySheet.Cell(row, 2).Value = kpi.Title;
                summarySheet.Cell(row, 3).Value = kpi.Value;
                summarySheet.Cell(row, 3).Style.NumberFormat.Format = kpi.Format == "C2" ? "L. #,##0.00" : "#,##0";
                summarySheet.Cell(row, 3).Style.Font.Bold = true;
                row++;
            }

            // Comparison
            row += 2;
            summarySheet.Cell(row, 2).Value = "COMPARATIVO CON PERÍODO ANTERIOR";
            summarySheet.Cell(row, 2).Style.Font.Bold = true;
            row += 2;
            
            summarySheet.Cell(row, 2).Value = "Ventas Actuales:";
            summarySheet.Cell(row, 3).Value = comparison.CurrentTotalSales;
            summarySheet.Cell(row, 3).Style.NumberFormat.Format = "L. #,##0.00";
            row++;
            
            summarySheet.Cell(row, 2).Value = "Ventas Anteriores:";
            summarySheet.Cell(row, 3).Value = comparison.PreviousTotalSales;
            summarySheet.Cell(row, 3).Style.NumberFormat.Format = "L. #,##0.00";
            row++;
            
            summarySheet.Cell(row, 2).Value = "Crecimiento:";
            summarySheet.Cell(row, 3).Value = comparison.SalesGrowthPercentage;
            summarySheet.Cell(row, 3).Style.NumberFormat.Format = "0.00%";
            row += 2;

            // Top Vendors Sheet
            var vendorsSheet = workbook.Worksheets.Add("Top Vendedores");
            vendorsSheet.Cell("B1").Value = "Top 5 Vendedores";
            vendorsSheet.Cell("B1").Style.Font.Bold = true;
            vendorsSheet.Cell("B1").Style.Font.FontSize = 14;
            vendorsSheet.Range("B1:F1").Merge();

            var vendorHeaders = new[] { "Vendedor", "Ventas", "Total Vendido", "Comisión Generada" };
            for (int i = 0; i < vendorHeaders.Length; i++)
            {
                var cell = vendorsSheet.Cell(3, i + 2);
                cell.Value = vendorHeaders[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            row = 4;
            foreach (var vendor in topVendors)
            {
                vendorsSheet.Cell(row, 2).Value = vendor.VendorName;
                vendorsSheet.Cell(row, 3).Value = vendor.SalesCount;
                vendorsSheet.Cell(row, 4).Value = vendor.TotalSales;
                vendorsSheet.Cell(row, 4).Style.NumberFormat.Format = "L. #,##0.00";
                vendorsSheet.Cell(row, 5).Value = vendor.CommissionGenerated;
                vendorsSheet.Cell(row, 5).Style.NumberFormat.Format = "L. #,##0.00";
                row++;
            }

            vendorsSheet.Columns().AdjustToContents();

            // Top Customers Sheet
            var customersSheet = workbook.Worksheets.Add("Top Clientes");
            customersSheet.Cell("B1").Value = "Top 5 Clientes";
            customersSheet.Cell("B1").Style.Font.Bold = true;
            customersSheet.Cell("B1").Style.Font.FontSize = 14;
            customersSheet.Range("B1:F1").Merge();

            var customerHeaders = new[] { "Cliente", "Compras", "Total Comprado", "Última Compra" };
            for (int i = 0; i < customerHeaders.Length; i++)
            {
                var cell = customersSheet.Cell(3, i + 2);
                cell.Value = customerHeaders[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            row = 4;
            foreach (var customer in topCustomers)
            {
                customersSheet.Cell(row, 2).Value = customer.CustomerName;
                customersSheet.Cell(row, 3).Value = customer.PurchaseCount;
                customersSheet.Cell(row, 4).Value = customer.TotalPurchased;
                customersSheet.Cell(row, 4).Style.NumberFormat.Format = "L. #,##0.00";
                customersSheet.Cell(row, 5).Value = customer.LastPurchaseDate?.ToString("dd/MM/yyyy") ?? "";
                row++;
            }

            customersSheet.Columns().AdjustToContents();

            // Sales Trends Sheet
            var trendsSheet = workbook.Worksheets.Add("Tendencia Ventas");
            trendsSheet.Cell("B1").Value = "Tendencia de Ventas (Últimos 30 días)";
            trendsSheet.Cell("B1").Style.Font.Bold = true;
            trendsSheet.Cell("B1").Style.Font.FontSize = 14;
            trendsSheet.Range("B1:E1").Merge();

            var trendHeaders = new[] { "Fecha", "Ventas", "Cantidad", "Comisiones" };
            for (int i = 0; i < trendHeaders.Length; i++)
            {
                var cell = trendsSheet.Cell(3, i + 2);
                cell.Value = trendHeaders[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            row = 4;
            foreach (var trend in trends)
            {
                trendsSheet.Cell(row, 2).Value = trend.Period;
                trendsSheet.Cell(row, 3).Value = trend.TotalSales;
                trendsSheet.Cell(row, 3).Style.NumberFormat.Format = "L. #,##0.00";
                trendsSheet.Cell(row, 4).Value = trend.SalesCount;
                trendsSheet.Cell(row, 5).Value = trend.TotalCommission;
                trendsSheet.Cell(row, 5).Style.NumberFormat.Format = "L. #,##0.00";
                row++;
            }

            trendsSheet.Columns().AdjustToContents();

            // Save
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                Title = "Guardar Dashboard",
                FileName = $"Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Dashboard exportado exitosamente", "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
