using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class AccountsPayableView : Window
    {
        private AccountsPayableViewModel? _viewModel;

        public AccountsPayableView()
        {
            InitializeComponent();
            _viewModel = DataContext as AccountsPayableViewModel;
        }
    }
}
