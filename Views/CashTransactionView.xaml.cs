using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class CashTransactionView : Window
    {
        private CashTransactionViewModel? _viewModel;

        public CashTransactionView()
        {
            InitializeComponent();
            _viewModel = DataContext as CashTransactionViewModel;
        }
    }
}
