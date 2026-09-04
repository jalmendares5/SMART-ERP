using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class SalesByVendorReportView : Window
    {
        private SalesByVendorReportViewModel? _viewModel;

        public SalesByVendorReportView()
        {
            InitializeComponent();
            _viewModel = DataContext as SalesByVendorReportViewModel;
        }
    }
}
