using System.Windows;
using System.Windows.Input;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ExitCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
            ExitCommand = new RelayCommand(ExecuteExit);
        }

        private async void ExecuteLogin(object? parameter)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Ingrese usuario y contraseña.";
                return;
            }

            var connectionString = CompanyConnectionService.GetActiveConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                ErrorMessage = "Seleccione una empresa activa para iniciar sesión.";
                return;
            }

            try
            {
                await using var testConnection = new MySqlConnector.MySqlConnection(connectionString);
                await testConnection.OpenAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"No se pudo conectar con la base de datos de la empresa:\n{ex.Message}";
                return;
            }

            if (await AuthenticationService.LoginAsync(Username, Password))
            {
                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();

                Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(w => w is Views.LoginWindow)?.Close();
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos";
                Password = string.Empty;
            }
        }

        private void ExecuteExit(object? parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
