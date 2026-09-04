using SMART_ERP.Models;
using System;
using System.IO;
using System.Text.Json;

namespace SMART_ERP.Services;

public static class SalesCaptureSettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SMART_ERP");

    private static readonly string SettingsPath = Path.Combine(SettingsDirectory, "sales-capture-settings.json");

    public static SalesCaptureSettings Current { get; private set; } = Load();

    public static SalesCaptureSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                return new SalesCaptureSettings();
            }

            var json = File.ReadAllText(SettingsPath);
            var data = JsonSerializer.Deserialize<SalesCaptureSettings>(json, JsonOptions);
            return data ?? new SalesCaptureSettings();
        }
        catch
        {
            return new SalesCaptureSettings();
        }
    }

    public static void Save(SalesCaptureSettings settings)
    {
        Current = settings;
        Directory.CreateDirectory(SettingsDirectory);
        var json = JsonSerializer.Serialize(Current, JsonOptions);
        File.WriteAllText(SettingsPath, json);
    }
}
