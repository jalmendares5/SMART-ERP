using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class AuthenticationService
{
    public static User? CurrentUser { get; private set; }

    public static async Task<bool> LoginAsync(
        string username,
        string password)
    {
        try
        {
            var user =
                await UserService.AuthenticateAsync(
                    username,
                    password);

            if (user == null || !user.IsActive)
            {
                CurrentUser = null;
                return false;
            }

            CurrentUser = user;

            return true;
        }
        catch
        {
            CurrentUser = null;
            return false;
        }
    }

    public static void Logout()
    {
        CurrentUser = null;
    }
}
