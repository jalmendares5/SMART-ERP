using System.Text.Json;
using System.IO;

namespace SMART_ERP.Services;

public static class UserSettingsService
{
    private static readonly string SettingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SMART_ERP",
        "user_settings.json");

    public class UserSettings
    {
        public DateTime? LastDateFrom { get; set; }
        public DateTime? LastDateTo { get; set; }
        public string? LastReportType { get; set; }
        public string? DefaultExportFormat { get; set; } = "Excel";
        public bool ShowNotifications { get; set; } = true;
        public int PageSize { get; set; } = 50;
    }

    public static UserSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                var settings = JsonSerializer.Deserialize<UserSettings>(json);
                return settings ?? new UserSettings();
            }
        }
        catch
        {
            // Return default settings if loading fails
        }

        return new UserSettings();
    }

    public static void SaveSettings(UserSettings settings)
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
            // Silently fail if saving doesn't work
        }
    }

    public static void UpdateSetting(Action<UserSettings> updateAction)
    {
        var settings = LoadSettings();
        updateAction(settings);
        SaveSettings(settings);
    }
}
