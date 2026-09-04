using SMART_ERP.Services;
using System.Windows.Controls;

namespace SMART_ERP.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        DataContext = CompanyInfoService.Current;
    }
}
