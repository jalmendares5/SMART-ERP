using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class InvoiceView : Window
    {
        private InvoiceViewModel? _viewModel;

        public InvoiceView()
        {
            InitializeComponent();
            _viewModel = DataContext as InvoiceViewModel;
        }
    }
}
