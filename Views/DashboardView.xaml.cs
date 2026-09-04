using SMART_ERP.ViewModels;
using System.Windows.Controls;

namespace SMART_ERP.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        DataContext = new DashboardViewModel();
    }

    public void Reload()
    {
        if (DataContext is DashboardViewModel viewModel)
        {
            viewModel.ExecuteRefresh(null);
        }
    }
}
