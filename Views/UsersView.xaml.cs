using System.Windows;
using System.Windows.Controls;
using SMART_ERP.ViewModels;

namespace SMART_ERP.Views
{
    public partial class UsersView : Window
    {
        private UsersViewModel? _viewModel;

        public UsersView()
        {
            InitializeComponent();
            _viewModel = DataContext as UsersViewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null && sender is PasswordBox passwordBox)
            {
                _viewModel.Password = passwordBox.Password;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_viewModel != null && _viewModel.SelectedUser != null)
            {
                _viewModel.EditUser(_viewModel.SelectedUser);
            }
        }
    }
}
