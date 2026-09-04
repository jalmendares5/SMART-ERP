using System.Collections.ObjectModel;
using System.Windows.Input;
using SMART_ERP.Models;
using SMART_ERP.Services;

namespace SMART_ERP.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private ObservableCollection<User> _users = new();
        private User? _selectedUser;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _fullName = string.Empty;
        private string _role = "Vendedor";
        private bool _isActive = true;
        private bool _isEditing = false;
        private string _errorMessage = string.Empty;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public User? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

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

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand CreateNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RefreshCommand { get; }

        public UsersViewModel()
        {
            CreateNewCommand = new RelayCommand(ExecuteCreateNew);
            SaveCommand = new RelayCommand(ExecuteSave);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            CancelCommand = new RelayCommand(ExecuteCancel);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            Task.Run(async () => await LoadUsersAsync());
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var users = await UserService.GetAllAsync();
                Users = new ObservableCollection<User>(users);
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar usuarios: {ex.Message}";
            }
        }

        private void ExecuteCreateNew(object? parameter)
        {
            ClearForm();
            IsEditing = false;
            ErrorMessage = string.Empty;
        }

        private async void ExecuteSave(object? parameter)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Debe completar todos los campos obligatorios.";
                return;
            }

            try
            {
                var user = new User
                {
                    Username = Username,
                    Password = Password,
                    FullName = FullName,
                    Role = Role,
                    IsActive = IsActive
                };

                if (IsEditing && SelectedUser != null)
                {
                    user.Id = SelectedUser.Id;
                    user.CreatedAt = SelectedUser.CreatedAt;
                    await UserService.UpdateAsync(user);
                }
                else
                {
                    await UserService.CreateAsync(user);
                }

                await LoadUsersAsync();
                ClearForm();
                IsEditing = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al guardar usuario: {ex.Message}";
            }
        }

        private async void ExecuteDelete(object? parameter)
        {
            if (SelectedUser == null)
            {
                ErrorMessage = "Debe seleccionar un usuario para eliminar.";
                return;
            }

            if (SelectedUser.Username.ToLower() == "admin")
            {
                ErrorMessage = "No se puede eliminar el usuario administrador.";
                return;
            }

            var result = System.Windows.MessageBox.Show(
                $"¿Está seguro de eliminar el usuario '{SelectedUser.FullName}'?",
                "Confirmar eliminación",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    await UserService.DeleteAsync(SelectedUser.Id);
                    await LoadUsersAsync();
                    ClearForm();
                    IsEditing = false;
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al eliminar usuario: {ex.Message}";
                }
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            ClearForm();
            IsEditing = false;
            ErrorMessage = string.Empty;
        }

        private void ExecuteRefresh(object? parameter)
        {
            Task.Run(async () => await LoadUsersAsync());
        }

        private void ClearForm()
        {
            Username = string.Empty;
            Password = string.Empty;
            FullName = string.Empty;
            Role = "Vendedor";
            IsActive = true;
            SelectedUser = null;
        }

        public void EditUser(User user)
        {
            SelectedUser = user;
            Username = user.Username;
            Password = user.Password;
            FullName = user.FullName;
            Role = user.Role;
            IsActive = user.IsActive;
            IsEditing = true;
            ErrorMessage = string.Empty;
        }
    }
}
