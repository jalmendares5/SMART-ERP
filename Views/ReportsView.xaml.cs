using SMART_ERP.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;

namespace SMART_ERP.Views;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();
        DataContext = new ReportsViewModel();
        
        // Setup error message visibility binding
        var errorBinding = new Binding("ErrorMessage")
        {
            Converter = new StringToVisibilityConverter()
        };
        ErrorMessageBlock.SetBinding(VisibilityProperty, errorBinding);
    }

    public void Reload()
    {
        if (DataContext is ReportsViewModel viewModel)
        {
            viewModel.ExecuteRefresh(null);
        }
    }

    public void ClearFilters()
    {
        if (DataContext is ReportsViewModel viewModel)
        {
            viewModel.ClearFilterCommand.Execute(null);
        }
    }

    public void ExportToExcel()
    {
        if (DataContext is ReportsViewModel viewModel)
        {
            viewModel.ExportToExcelCommand.Execute(null);
        }
    }

    public void ExportToCsv()
    {
        if (DataContext is ReportsViewModel viewModel)
        {
            viewModel.ExportToCsvCommand.Execute(null);
        }
    }

    private class StringToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string) ? 
                System.Windows.Visibility.Collapsed : 
                System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
