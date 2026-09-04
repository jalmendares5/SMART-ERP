using SMART_ERP.Models;

namespace SMART_ERP.Services;

public static class BillingCompanyService
{
    public static List<BillingCompany> GetAll()
    {
        return SalesCaptureSettingsService.Current.BillingCompanies
            .OrderBy(c => c.Name)
            .ToList();
    }

    public static List<BillingCompany> GetActive()
    {
        return SalesCaptureSettingsService.Current.BillingCompanies
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToList();
    }

    public static BillingCompany? GetById(int id)
    {
        return SalesCaptureSettingsService.Current.BillingCompanies
            .FirstOrDefault(c => c.Id == id);
    }

    public static BillingCompany Add(
        string name,
        string legalName,
        string taxId = "")
    {
        var settings = SalesCaptureSettingsService.Current;

        int nextId = settings.BillingCompanies.Count == 0
            ? 1
            : settings.BillingCompanies.Max(c => c.Id) + 1;

        var company = new BillingCompany
        {
            Id = nextId,
            Name = name.Trim(),
            LegalName = legalName.Trim(),
            TaxId = taxId.Trim(),
            IsActive = true
        };

        settings.BillingCompanies.Add(company);
        SalesCaptureSettingsService.Save(settings);

        return company;
    }

    public static bool Update(
        int id,
        string name,
        string legalName,
        string taxId)
    {
        var company = GetById(id);

        if (company == null)
            return false;

        company.Name = name.Trim();
        company.LegalName = legalName.Trim();
        company.TaxId = taxId.Trim();

        SalesCaptureSettingsService.Save(
            SalesCaptureSettingsService.Current);

        return true;
    }

    public static bool SetActive(int id, bool isActive)
    {
        var company = GetById(id);

        if (company == null)
            return false;

        company.IsActive = isActive;

        SalesCaptureSettingsService.Save(
            SalesCaptureSettingsService.Current);

        return true;
    }
}
