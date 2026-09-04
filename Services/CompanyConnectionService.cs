using MySqlConnector;
using System.IO;
using System.Text.Json;
using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class CompanyConnectionService
{
    private static CompanyConnection? _activeCompany;

    public static CompanyConnection? ActiveCompany => _activeCompany;

    public static async Task<bool> InitializeActiveCompanyAsync()
    {
        var companies = GetAll()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.LastConnectionAt ?? DateTime.MinValue)
            .ThenBy(x => x.CompanyName)
            .ToList();

        if (companies.Count == 0)
        {
            _activeCompany = null;
            return true;
        }

        foreach (var company in companies)
        {
            try
            {
                bool databaseReady =
                    await CompanyDatabaseService.EnsureDatabaseAsync(company);

                if (!databaseReady)
                    continue;

                string connectionString =
                    BuildConnectionStringForCompany(company);

                await using var connection =
                    new MySqlConnection(connectionString);

                await connection.OpenAsync();

                _activeCompany = company;

                company.DatabaseName =
                    GenerateDatabaseName(company.CompanyName);

                company.LastConnectionAt = DateTime.Now;

                Save(company);

                return true;
            }
            catch
            {
                continue;
            }
        }

        _activeCompany = null;
        return false;
    }

    public static string BuildConnectionStringForCompany(
        CompanyConnection company)
    {

        string databaseName =
            GenerateDatabaseName(company.CompanyName);

        company.DatabaseName = databaseName;

        var builder = new MySqlConnectionStringBuilder
        {
            Server = string.IsNullOrWhiteSpace(company.Server)
                ? "127.0.0.1"
                : company.Server,

            Port = (uint)company.Port,

            Database = databaseName,

            UserID = company.Username,
            Password = company.Password,

            SslMode = MySqlSslMode.None,
            AllowPublicKeyRetrieval = true,
            ConnectionTimeout = 5
        };

        return builder.ConnectionString;
    }
    private static readonly object _lock = new();

    private static readonly string DataDirectory =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "SMART ERP",
            "Data");

    private static readonly string FilePath =
        Path.Combine(DataDirectory, "companies.json");

    public static List<CompanyConnection> GetAll()
    {
        lock (_lock)
        {
            EnsureStorage();

            if (!File.Exists(FilePath))
            {
                return new List<CompanyConnection>();
            }

            try
            {
                var json = File.ReadAllText(FilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<CompanyConnection>();
                }

                return JsonSerializer.Deserialize<List<CompanyConnection>>(json)
                       ?? new List<CompanyConnection>();
            }
            catch
            {
                return new List<CompanyConnection>();
            }
        }
    }

    public static string GenerateDatabaseName(string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return "BD_EMPRESA";

        string normalized = companyName.Trim().ToUpperInvariant();

        string normalizedFormD = normalized.Normalize(
            System.Text.NormalizationForm.FormD);

        var builder = new System.Text.StringBuilder();

        foreach (char c in normalizedFormD)
        {
            var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);

            if (category == System.Globalization.UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetterOrDigit(c))
                builder.Append(c);
        }

        string cleanName = builder.ToString();

        if (string.IsNullOrWhiteSpace(cleanName))
            cleanName = "EMPRESA";

        string databaseName = "BD_" + cleanName;

        if (databaseName.Length > 64)
            databaseName = databaseName[..64];

        return databaseName;
    }
    public static async Task<bool> CreateDatabaseAsync(
        CompanyConnection company)
    {
        try
        {
            string databaseName =
                GenerateDatabaseName(company.CompanyName);

            string connectionString =
                $"Server={company.Server};" +
                $"Port={company.Port};" +
                $"User ID={company.Username};" +
                $"Password={company.Password};" +
                $"SslMode=None;";

            using var connection =
                new MySqlConnector.MySqlConnection(connectionString);

            await connection.OpenAsync();

            string safeDatabaseName =
                databaseName.Replace("`", "");

            string sql =
                $"CREATE DATABASE IF NOT EXISTS `{safeDatabaseName}` " +
                "CHARACTER SET utf8mb4 " +
                "COLLATE utf8mb4_unicode_ci;";

            using var command =
                new MySqlConnector.MySqlCommand(sql, connection);

            await command.ExecuteNonQueryAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }
    public static CompanyConnection? GetById(int id)
    {
        return GetAll().FirstOrDefault(x => x.Id == id);
    }

    public static CompanyConnection? GetByName(string companyName)
    {
        return GetAll().FirstOrDefault(x =>
            x.CompanyName.Equals(
                companyName,
                StringComparison.OrdinalIgnoreCase));
    }

    public static void Save(CompanyConnection company)
    {
        lock (_lock)
        {
            EnsureStorage();

            var companies = GetAll();

            if (company.Id == 0)
            {
                company.Id = companies.Count == 0
                    ? 1
                    : companies.Max(x => x.Id) + 1;

                company.CreatedAt = DateTime.Now;

                companies.Add(company);
            }
            else
            {
                var existing = companies.FirstOrDefault(x => x.Id == company.Id);

                if (existing == null)
                {
                    companies.Add(company);
                }
                else
                {
                    var index = companies.IndexOf(existing);
                    companies[index] = company;
                }
            }

            WriteAll(companies);
        }
    }

    public static void Delete(int id)
    {
        lock (_lock)
        {
            EnsureStorage();

            var companies = GetAll();

            companies.RemoveAll(x => x.Id == id);

            WriteAll(companies);
        }
    }

    public static void UpdateLastConnection(int id)
    {
        lock (_lock)
        {
            var company = GetById(id);

            if (company == null)
            {
                return;
            }

            company.LastConnectionAt = DateTime.Now;

            Save(company);
        }
    }


    public static void SetActiveCompany(CompanyConnection company)
    {
        _activeCompany = company;
    }

    public static void ClearActiveCompany()
    {
        _activeCompany = null;
    }

    public static string? GetActiveConnectionString()
    {
        if (_activeCompany == null)
        {
            return null;
        }

        string databaseName =
            string.IsNullOrWhiteSpace(_activeCompany.DatabaseName)
                ? GenerateDatabaseName(_activeCompany.CompanyName)
                : _activeCompany.DatabaseName;

        var builder = new MySqlConnectionStringBuilder
        {
            Server = _activeCompany.Server,
            Port = (uint)_activeCompany.Port,
            Database = databaseName,
            UserID = _activeCompany.Username,
            Password = _activeCompany.Password,
            SslMode = MySqlSslMode.None,
            ConnectionTimeout = 5
        };

        return builder.ConnectionString;
    }
    private static void EnsureStorage()
    {
        Directory.CreateDirectory(DataDirectory);
    }

    private static void WriteAll(List<CompanyConnection> companies)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(companies, options);

        File.WriteAllText(FilePath, json);
    }
}










