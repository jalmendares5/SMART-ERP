using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class SalesConditionService
{
    public static List<SalesConditionOption> GetAll()
    {
        return SalesCaptureSettingsService.Current.SalesConditions
            .OrderBy(x => x.CreditDays)
            .ThenBy(x => x.Name)
            .ToList();
    }

    public static List<SalesConditionOption> GetActive()
    {
        return SalesCaptureSettingsService.Current.SalesConditions
            .Where(x => x.IsActive)
            .OrderBy(x => x.CreditDays)
            .ThenBy(x => x.Name)
            .ToList();
    }

    public static SalesConditionOption? GetById(int id)
    {
        return SalesCaptureSettingsService.Current.SalesConditions
            .FirstOrDefault(x => x.Id == id);
    }

    public static SalesConditionOption Add(
        string name,
        int creditDays)
    {
        var settings = SalesCaptureSettingsService.Current;

        int nextId = settings.SalesConditions.Count == 0
            ? 1
            : settings.SalesConditions.Max(x => x.Id) + 1;

        var item = new SalesConditionOption
        {
            Id = nextId,
            Name = name.Trim(),
            CreditDays = creditDays,
            IsActive = true
        };

        settings.SalesConditions.Add(item);
        SalesCaptureSettingsService.Save(settings);

        return item;
    }

    public static bool Update(
        int id,
        string name,
        int creditDays)
    {
        var item = GetById(id);

        if (item == null)
            return false;

        item.Name = name.Trim();
        item.CreditDays = creditDays;

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
