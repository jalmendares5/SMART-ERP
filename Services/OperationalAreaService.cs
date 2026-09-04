using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class OperationalAreaService
{
    public static List<OperationalArea> GetAll()
    {
        return SalesCaptureSettingsService.Current.OperationalAreas
            .OrderBy(a => a.Name)
            .ToList();
    }

    public static List<OperationalArea> GetActive()
    {
        return SalesCaptureSettingsService.Current.OperationalAreas
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToList();
    }

    public static OperationalArea? GetById(int id)
    {
        return SalesCaptureSettingsService.Current.OperationalAreas
            .FirstOrDefault(a => a.Id == id);
    }

    public static OperationalArea Add(
        string name,
        string description = "")
    {
        var settings = SalesCaptureSettingsService.Current;

        int nextId = settings.OperationalAreas.Count == 0
            ? 1
            : settings.OperationalAreas.Max(a => a.Id) + 1;

        var area = new OperationalArea
        {
            Id = nextId,
            Name = name.Trim(),
            Description = description.Trim(),
            IsActive = true
        };

        settings.OperationalAreas.Add(area);
        SalesCaptureSettingsService.Save(settings);

        return area;
    }

    public static bool Update(
        int id,
        string name,
        string description)
    {
        var area = GetById(id);

        if (area == null)
            return false;

        area.Name = name.Trim();
        area.Description = description.Trim();

        SalesCaptureSettingsService.Save(
            SalesCaptureSettingsService.Current);

        return true;
    }

    public static bool SetActive(int id, bool isActive)
    {
        var area = GetById(id);

        if (area == null)
            return false;

        area.IsActive = isActive;

        SalesCaptureSettingsService.Save(
            SalesCaptureSettingsService.Current);

        return true;
    }
}
