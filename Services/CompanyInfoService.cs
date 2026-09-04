using SMART_ERP.Models;
using System;
using System.IO;
using System.Text.Json;

namespace SMART_ERP.Services;

public static class CompanyInfoService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SMART_ERP");

    private static readonly string SettingsPath = Path.Combine(SettingsDirectory, "company-info.json");

    public static CompanyInfo Current { get; } = Load();

    private static CompanyInfo Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                return new CompanyInfo();
            }

            var json = File.ReadAllText(SettingsPath);
            var data = JsonSerializer.Deserialize<CompanyInfo>(json, JsonOptions);
            return data ?? new CompanyInfo();
        }
        catch
        {
            return new CompanyInfo();
        }
    }

    public static void Save()
    {
        Directory.CreateDirectory(SettingsDirectory);
        var json = JsonSerializer.Serialize(Current, JsonOptions);
        File.WriteAllText(SettingsPath, json);
    }
}
