-- Create database
IF DB_ID('PharmacyDB') IS NULL
BEGIN
    CREATE DATABASE PharmacyDB;
END
GO

USE PharmacyDB;
GO

-- Drop tables if they exist
IF OBJECT_ID('dbo.Sales', 'U') IS NOT NULL DROP TABLE dbo.Sales;
IF OBJECT_ID('dbo.Medicines', 'U') IS NOT NULL DROP TABLE dbo.Medicines;
GO

-- Tables
CREATE TABLE dbo.Medicines(
    MedicineID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(120) NOT NULL,
    Category VARCHAR(80) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Quantity INT NOT NULL
);

CREATE TABLE dbo.Sales(
    SaleID INT IDENTITY(1,1) PRIMARY KEY,
    MedicineID INT NOT NULL FOREIGN KEY REFERENCES dbo.Medicines(MedicineID),
    QuantitySold INT NOT NULL,
    SaleDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- Stored Procedures
IF OBJECT_ID('dbo.AddMedicine', 'P') IS NOT NULL DROP PROCEDURE dbo.AddMedicine;
GO
CREATE PROCEDURE dbo.AddMedicine
    @Name VARCHAR(120),
    @Category VARCHAR(80),
    @Price DECIMAL(18,2),
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Medicines(Name, Category, Price, Quantity)
    VALUES(@Name, @Category, @Price, @Quantity);
END
GO

IF OBJECT_ID('dbo.SearchMedicine', 'P') IS NOT NULL DROP PROCEDURE dbo.SearchMedicine;
GO
CREATE PROCEDURE dbo.SearchMedicine
    @SearchTerm VARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MedicineID, Name, Category, Price, Quantity
    FROM dbo.Medicines
    WHERE Name LIKE '%' + @SearchTerm + '%' OR Category LIKE '%' + @SearchTerm + '%'
    ORDER BY Name;
END
GO

IF OBJECT_ID('dbo.UpdateStock', 'P') IS NOT NULL DROP PROCEDURE dbo.UpdateStock;
GO
CREATE PROCEDURE dbo.UpdateStock
    @MedicineID INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Medicines
    SET Quantity = @Quantity
    WHERE MedicineID = @MedicineID;
END
GO

IF OBJECT_ID('dbo.RecordSale', 'P') IS NOT NULL DROP PROCEDURE dbo.RecordSale;
GO
CREATE PROCEDURE dbo.RecordSale
    @MedicineID INT,
    @QuantitySold INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS(SELECT 1 FROM dbo.Medicines WHERE MedicineID = @MedicineID)
    BEGIN
        RAISERROR('Medicine not found.', 16, 1);
        RETURN;
    END

    DECLARE @currentQty INT;
    SELECT @currentQty = Quantity FROM dbo.Medicines WHERE MedicineID = @MedicineID;

    IF @currentQty < @QuantitySold
    BEGIN
        RAISERROR('Not enough stock.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Sales(MedicineID, QuantitySold)
    VALUES(@MedicineID, @QuantitySold);

    UPDATE dbo.Medicines SET Quantity = Quantity - @QuantitySold WHERE MedicineID = @MedicineID;
END
GO

IF OBJECT_ID('dbo.GetAllMedicines', 'P') IS NOT NULL DROP PROCEDURE dbo.GetAllMedicines;
GO
CREATE PROCEDURE dbo.GetAllMedicines
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MedicineID, Name, Category, Price, Quantity FROM dbo.Medicines ORDER BY Name;
END
GO
