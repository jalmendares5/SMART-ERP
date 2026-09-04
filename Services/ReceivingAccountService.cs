using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class ReceivingAccountService
{
    public static List<ReceivingAccountOption> GetAll()
    {
        return SalesCaptureSettingsService.Current.ReceivingAccounts
            .OrderBy(x => x.Name)
            .ToList();
    }

    public static List<ReceivingAccountOption> GetActive()
    {
        return SalesCaptureSettingsService.Current.ReceivingAccounts
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToList();
    }

    public static ReceivingAccountOption? GetById(int id)
    {
        return SalesCaptureSettingsService.Current.ReceivingAccounts
            .FirstOrDefault(x => x.Id == id);
    }

    public static ReceivingAccountOption Add(
        string name,
        string description = "")
    {
        var settings = SalesCaptureSettingsService.Current;

        int nextId = settings.ReceivingAccounts.Count == 0
            ? 1
            : settings.ReceivingAccounts.Max(x => x.Id) + 1;

        var item = new ReceivingAccountOption
        {
            Id = nextId,
            Name = name.Trim(),
            Description = description.Trim(),
            IsActive = true
        };

        settings.ReceivingAccounts.Add(item);
        SalesCaptureSettingsService.Save(settings);

        return item;
    }

    public static bool Update(
        int id,
        string name,
        string description)
    {
        var item = GetById(id);

        if (item == null)
            return false;

        item.Name = name.Trim();
        item.Description = description.Trim();

        SalesCaptureSettingsService.Save(
            SalesCaptureSettingsService.Current);

        return true;
    }

    public static bool SetActive(
        int id,
        bool isActive)
    {
        var item = GetById(id);

        if (item == null)
            return false;

        item.IsActive = isActive;

        SalesCaptureSettingsService.Save(
            SalesCaptureSettingsService.Current);

        return true;
    }
}
