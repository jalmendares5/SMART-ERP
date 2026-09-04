using System.Windows;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class MonthlyCloseView : Window
    {
        private MonthlyCloseViewModel? _viewModel;

        public MonthlyCloseView()
        {
            InitializeComponent();
            _viewModel = DataContext as MonthlyCloseViewModel;
        }
    }
}
