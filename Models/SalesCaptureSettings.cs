using System.Collections.Generic;

namespace SMART_ERP.Models;

public class SalesCaptureSettings
{
    public string PaymentMethodsCsv { get; set; } = "EFECTIVO,CONTADO,TARJETA,CHEQUE,TRANSFERENCIA,CREDITO";
    public string DefaultPaymentMethod { get; set; } = "EFECTIVO";

    public bool AllowTaxGravadaIsv { get; set; } = true;
    public bool AllowTaxMixta { get; set; } = true;
    public bool AllowTaxExonerada { get; set; } = true;
    public bool AllowTaxExenta { get; set; } = true;

    public string StatusesCsv { get; set; } = "ACTIVA,ANULADA,PENDIENTE";

    public decimal DefaultPrimaryCommission { get; set; } = 3m;
    public decimal DefaultSecondaryCommission { get; set; } = 0m;

    public bool RequireCancellationReasonWhenVoided { get; set; } = true;

    public List<PaymentMethodOption> PaymentMethods { get; set; } = new()
    {
        new PaymentMethodOption { Id = 1, Name = "EFECTIVO", IsActive = true },
        new PaymentMethodOption { Id = 2, Name = "TRANSFERENCIA", IsActive = true },
        new PaymentMethodOption { Id = 3, Name = "TARJETA", IsActive = true },
        new PaymentMethodOption { Id = 4, Name = "CHEQUE", IsActive = true }
    };

    public List<SalesConditionOption> SalesConditions { get; set; } = new()
    {
        new SalesConditionOption { Id = 1, Name = "CONTADO", CreditDays = 0, IsActive = true },
        new SalesConditionOption { Id = 2, Name = "CRÉDITO 15 DÍAS", CreditDays = 15, IsActive = true },
        new SalesConditionOption { Id = 3, Name = "CRÉDITO 30 DÍAS", CreditDays = 30, IsActive = true },
        new SalesConditionOption { Id = 4, Name = "CRÉDITO 60 DÍAS", CreditDays = 60, IsActive = true }
    };

    public List<ReceivingAccountOption> ReceivingAccounts { get; set; } = new()
    {
        new ReceivingAccountOption
        {
            Id = 1,
            Name = "CAJA GENERAL",
            Description = "Caja general",
            IsActive = true
        }
    };
    public List<BillingCompany> BillingCompanies { get; set; } = new()
    {
        new BillingCompany
        {
            Id = 1,
            Name = "RESERMA",
            LegalName = "RESERMA S. DE R.L.",
            IsActive = true
        },
        new BillingCompany
        {
            Id = 2,
            Name = "TALLER AUTOMOTRIZ DARWIN",
            LegalName = "TALLER AUTOMOTRIZ DARWIN",
            IsActive = true
        }
    };

    public List<OperationalArea> OperationalAreas { get; set; } = new()
    {
        new OperationalArea
        {
            Id = 1,
            Name = "RESERMA",
            Description = "OPERACIONES Y VENTAS RESERMA",
            IsActive = true
        },
        new OperationalArea
        {
            Id = 2,
            Name = "TALLER AUTOMOTRIZ DARWIN",
            Description = "OPERACIONES Y VENTAS TALLER AUTOMOTRIZ DARWIN",
            IsActive = true
        },
        new OperationalArea
        {
            Id = 3,
            Name = "SETMEC",
            Description = "MECANIZADO Y TORNO",
            IsActive = true
        }
    };
}

