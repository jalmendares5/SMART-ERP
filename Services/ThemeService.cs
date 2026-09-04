using System;
using System.Windows;

namespace SMART_ERP.Services;

public static class ThemeService
{
    public enum AppTheme
    {
        Light,
        Dark
    }

    public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    public static void Initialize()
    {
        // Por ahora solo inicializamos, no aplicamos temas dinámicos
        // Esto requiere más configuración en XAML
    }

    public static void SetTheme(AppTheme theme)
    {
        CurrentTheme = theme;
        // TODO: Implementar cambio de tema dinámico en el futuro
        System.Windows.MessageBox.Show(
            $"Cambio a tema {theme} - Funcionalidad en desarrollo",
            "Información",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    public static void ToggleTheme()
    {
        SetTheme(CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);
    }
}
