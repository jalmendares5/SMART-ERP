using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class AccountsReceivableView : Window
    {
        private AccountsReceivableViewModel? _viewModel;

        public AccountsReceivableView()
        {
            InitializeComponent();
            _viewModel = DataContext as AccountsReceivableViewModel;
        }
    }
}
