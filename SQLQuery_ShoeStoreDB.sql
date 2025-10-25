-- Страны
CREATE TABLE Countries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Изготовители
CREATE TABLE Manufacturers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL UNIQUE,
    CountryId INT,
    FOREIGN KEY (CountryId) REFERENCES Countries(Id)
);

-- Группы обуви (мужская, женская, детская и т.д.)
CREATE TABLE Groups (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Цвета
CREATE TABLE Colors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- Материалы
CREATE TABLE Materials (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

-- Модели
CREATE TABLE Models (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL
);

CREATE TABLE Shoes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    GroupId INT NOT NULL,
    Article NVARCHAR(50) NOT NULL UNIQUE,
    ManufacturerId INT NOT NULL,
    Size INT NOT NULL,
    ColorId INT NOT NULL,
    MaterialId INT NOT NULL,
    ModelId INT NOT NULL,
    ProductionDate DATE NOT NULL,
    WarrantyMonths INT NOT NULL DEFAULT 12,
    Price DECIMAL(10,2) NOT NULL CHECK (Price > 0),
    BonusPoints INT NOT NULL DEFAULT 0,
    IsOnSale BIT NOT NULL DEFAULT 0,
    
    FOREIGN KEY (GroupId) REFERENCES Groups(Id),
    FOREIGN KEY (ManufacturerId) REFERENCES Manufacturers(Id),
    FOREIGN KEY (ColorId) REFERENCES Colors(Id),
    FOREIGN KEY (MaterialId) REFERENCES Materials(Id),
    FOREIGN KEY (ModelId) REFERENCES Models(Id)
);