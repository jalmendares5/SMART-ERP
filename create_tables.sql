-- Script para crear todas las tablas del sistema SMART ERP
-- Base de datos: BD_RESERMA

USE BD_RESERMA;

-- Tabla de Productos
CREATE TABLE IF NOT EXISTS products
(
    Id INT NOT NULL AUTO_INCREMENT,
    Code VARCHAR(50) NOT NULL UNIQUE,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    Category VARCHAR(100) NULL,
    Cost DECIMAL(18,2) NOT NULL DEFAULT 0,
    Price1 DECIMAL(18,2) NOT NULL DEFAULT 0,
    Price2 DECIMAL(18,2) NOT NULL DEFAULT 0,
    Price3 DECIMAL(18,2) NOT NULL DEFAULT 0,
    Price4 DECIMAL(18,2) NOT NULL DEFAULT 0,
    Stock DECIMAL(18,2) NOT NULL DEFAULT 0,
    MinStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    MaxStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    Unit VARCHAR(50) NOT NULL DEFAULT 'UNIDAD',
    BarCode VARCHAR(50) NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL,
    CreatedBy VARCHAR(100) NULL,
    PRIMARY KEY (Id),
    INDEX IX_products_Code (Code),
    INDEX IX_products_Category (Category),
    INDEX IX_products_IsActive (IsActive),
    INDEX IX_products_Stock (Stock)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Tabla de Facturas
CREATE TABLE IF NOT EXISTS invoices
(
    Id INT NOT NULL AUTO_INCREMENT,
    InvoiceNumber VARCHAR(50) NOT NULL UNIQUE,
    InvoiceDate DATETIME NOT NULL,
    CustomerId INT NOT NULL,
    CustomerName VARCHAR(200) NOT NULL,
    Salesperson VARCHAR(100) NULL,
    PaymentTerms VARCHAR(50) NOT NULL DEFAULT 'CONTADO',
    CreditDays INT NOT NULL DEFAULT 0,
    Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    Tax DECIMAL(18,2) NOT NULL DEFAULT 0,
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    Notes TEXT NULL,
    CreatedAt DATETIME NOT NULL,
    CreatedBy VARCHAR(100) NULL,
    PRIMARY KEY (Id),
    INDEX IX_invoices_InvoiceNumber (InvoiceNumber),
    INDEX IX_invoices_CustomerId (CustomerId),
    INDEX IX_invoices_InvoiceDate (InvoiceDate),
    INDEX IX_invoices_Status (Status)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Tabla de Items de Factura
CREATE TABLE IF NOT EXISTS invoice_items
(
    Id INT NOT NULL AUTO_INCREMENT,
    InvoiceId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductCode VARCHAR(50) NOT NULL,
    ProductName VARCHAR(200) NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL DEFAULT 0,
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (Id),
    INDEX IX_invoice_items_InvoiceId (InvoiceId),
    INDEX IX_invoice_items_ProductId (ProductId),
    FOREIGN KEY (InvoiceId) REFERENCES invoices(Id) ON DELETE CASCADE
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Tabla de Cuentas por Cobrar
CREATE TABLE IF NOT EXISTS accounts_receivable
(
    Id INT NOT NULL AUTO_INCREMENT,
    InvoiceId INT NOT NULL UNIQUE,
    InvoiceNumber VARCHAR(50) NOT NULL,
    CustomerId INT NOT NULL,
    CustomerName VARCHAR(200) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    DueDate DATETIME NULL,
    DaysOverdue INT NOT NULL DEFAULT 0,
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    CreatedAt DATETIME NOT NULL,
    PRIMARY KEY (Id),
    INDEX IX_accounts_receivable_InvoiceId (InvoiceId),
    INDEX IX_accounts_receivable_CustomerId (CustomerId),
    INDEX IX_accounts_receivable_Status (Status),
    INDEX IX_accounts_receivable_DueDate (DueDate)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Tabla de Compras
CREATE TABLE IF NOT EXISTS purchases
(
    Id INT NOT NULL AUTO_INCREMENT,
    PurchaseNumber VARCHAR(50) NOT NULL UNIQUE,
    PurchaseDate DATETIME NOT NULL,
    VendorId INT NOT NULL,
    VendorName VARCHAR(200) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
    Tax DECIMAL(18,2) NOT NULL DEFAULT 0,
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    Notes TEXT NULL,
    CreatedAt DATETIME NOT NULL,
    CreatedBy VARCHAR(100) NULL,
    PRIMARY KEY (Id),
    INDEX IX_purchases_PurchaseNumber (PurchaseNumber),
    INDEX IX_purchases_VendorId (VendorId),
    INDEX IX_purchases_PurchaseDate (PurchaseDate),
    INDEX IX_purchases_Status (Status)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Tabla de Items de Compra
CREATE TABLE IF NOT EXISTS purchase_items
(
    Id INT NOT NULL AUTO_INCREMENT,
    PurchaseId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductCode VARCHAR(50) NOT NULL,
    ProductName VARCHAR(200) NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL DEFAULT 0,
    Cost DECIMAL(18,2) NOT NULL DEFAULT 0,
    Discount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (Id),
    INDEX IX_purchase_items_PurchaseId (PurchaseId),
    INDEX IX_purchase_items_ProductId (ProductId),
    FOREIGN KEY (PurchaseId) REFERENCES purchases(Id) ON DELETE CASCADE
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Tabla de Cuentas por Pagar
CREATE TABLE IF NOT EXISTS accounts_payable
(
    Id INT NOT NULL AUTO_INCREMENT,
    PurchaseId INT NOT NULL UNIQUE,
    PurchaseNumber VARCHAR(50) NOT NULL,
    VendorId INT NOT NULL,
    VendorName VARCHAR(200) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    DueDate DATETIME NULL,
    DaysOverdue INT NOT NULL DEFAULT 0,
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING',
    CreatedAt DATETIME NOT NULL,
    PRIMARY KEY (Id),
    INDEX IX_accounts_payable_PurchaseId (PurchaseId),
    INDEX IX_accounts_payable_VendorId (VendorId),
    INDEX IX_accounts_payable_Status (Status),
    INDEX IX_accounts_payable_DueDate (DueDate)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Tabla de Transacciones de Caja
CREATE TABLE IF NOT EXISTS cash_transactions
(
    Id INT NOT NULL AUTO_INCREMENT,
    TransactionNumber VARCHAR(50) NOT NULL UNIQUE,
    TransactionDate DATETIME NOT NULL,
    TransactionType VARCHAR(10) NOT NULL DEFAULT 'IN',
    Category VARCHAR(100) NULL,
    Description TEXT NULL,
    Amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    ReferenceType VARCHAR(50) NULL,
    ReferenceId INT NULL,
    ReferenceNumber VARCHAR(50) NULL,
    Notes TEXT NULL,
    CreatedAt DATETIME NOT NULL,
    CreatedBy VARCHAR(100) NULL,
    PRIMARY KEY (Id),
    INDEX IX_cash_transactions_TransactionNumber (TransactionNumber),
    INDEX IX_cash_transactions_TransactionDate (TransactionDate),
    INDEX IX_cash_transactions_TransactionType (TransactionType),
    INDEX IX_cash_transactions_Category (Category)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

-- Mostrar mensaje de éxito
SELECT 'Todas las tablas creadas exitosamente' AS Mensaje;
