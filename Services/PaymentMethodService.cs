using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class PaymentMethodService
{
    public static List<PaymentMethodOption> GetAll()
    {
        return SalesCaptureSettingsService.Current.PaymentMethods
            .OrderBy(x => x.Name)
            .ToList();
    }

    public static List<PaymentMethodOption> GetActive()
    {
        return SalesCaptureSettingsService.Current.PaymentMethods
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToList();
    }

    public static PaymentMethodOption? GetById(int id)
    {
        return SalesCaptureSettingsService.Current.PaymentMethods
            .FirstOrDefault(x => x.Id == id);
    }

    public static PaymentMethodOption Add(string name)
    {
        var settings = SalesCaptureSettingsService.Current;

        int nextId = settings.PaymentMethods.Count == 0
            ? 1
            : settings.PaymentMethods.Max(x => x.Id) + 1;

        var item = new PaymentMethodOption
        {
            Id = nextId,
            Name = name.Trim(),
            IsActive = true
        };

        settings.PaymentMethods.Add(item);
        SalesCaptureSettingsService.Save(settings);

        return item;
    }

    public static bool Update(int id, string name)
    {
        var item = GetById(id);

        if (item == null)
            return false;

        item.Name = name.Trim();

        SalesCaptureSettingsService.Save(
            SalesCaptureSettingsService.Current);

        return true;
    }

    public static bool SetActive(int id, bool isActive)
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
