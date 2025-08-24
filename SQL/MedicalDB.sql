-- Create database
IF DB_ID('MedicalDB') IS NULL
BEGIN
    CREATE DATABASE MedicalDB;
END
GO

USE MedicalDB;
GO

-- Drop tables if they exist (for repeatability)
IF OBJECT_ID('dbo.Appointments', 'U') IS NOT NULL DROP TABLE dbo.Appointments;
IF OBJECT_ID('dbo.Doctors', 'U') IS NOT NULL DROP TABLE dbo.Doctors;
IF OBJECT_ID('dbo.Patients', 'U') IS NOT NULL DROP TABLE dbo.Patients;
GO

-- Tables
CREATE TABLE dbo.Doctors(
    DoctorID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Specialty VARCHAR(100) NOT NULL,
    Availability BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.Patients(
    PatientID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL
);

CREATE TABLE dbo.Appointments(
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorID INT NOT NULL FOREIGN KEY REFERENCES dbo.Doctors(DoctorID),
    PatientID INT NOT NULL FOREIGN KEY REFERENCES dbo.Patients(PatientID),
    AppointmentDate DATETIME NOT NULL,
    Notes VARCHAR(500) NULL
);

-- Sample data
INSERT INTO dbo.Doctors(FullName, Specialty, Availability) VALUES
('Dr. Ama Mensah', 'Pediatrics', 1),
('Dr. Kwame Boateng', 'Cardiology', 1),
('Dr. Efua Owusu', 'Dermatology', 0);

INSERT INTO dbo.Patients(FullName, Email) VALUES
('Akosua Agyeman', 'akosua@example.com'),
('Yaw Asare', 'yaw@example.com'),
('Kofi Adu', 'kofi@example.com');

-- Helpful indexes
CREATE INDEX IX_Appointments_DoctorDate ON dbo.Appointments(DoctorID, AppointmentDate);
