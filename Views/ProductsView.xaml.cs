using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class ProductsView : Window
    {
        private ProductsViewModel? _viewModel;

        public ProductsView()
        {
            InitializeComponent();
            _viewModel = DataContext as ProductsViewModel;
        }
    }
}
