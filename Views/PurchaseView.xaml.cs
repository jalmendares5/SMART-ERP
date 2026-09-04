using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class PurchaseView : Window
    {
        private PurchaseViewModel? _viewModel;

        public PurchaseView()
        {
            InitializeComponent();
            _viewModel = DataContext as PurchaseViewModel;
        }
    }
}
