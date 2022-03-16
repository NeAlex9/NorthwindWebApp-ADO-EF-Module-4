CREATE PROCEDURE [dbo].DeleteCategory
    @categoryId INT
AS
    DELETE FROM dbo.Categories 
    WHERE @categoryId = dbo.Categories.CategoryID
    SELECT @@RowC


CREATE PROCEDURE [dbo].DeleteEmployee
    @employeeId INT
AS
    DELETE FROM dbo.Employees
    WHERE EmployeeID = @employeeId


CREATE PROCEDURE [dbo].DeleteProduct
    @productId int
AS
    DELETE FROM dbo.Products 
    WHERE ProductID = @productId
    SELECT @@ROWCOUNT


CREATE PROCEDURE [dbo].FindCategoryById
    @categoryId INT
AS
    SELECT CategoryID ,CategoryName, Description, Picture
    FROM dbo.Categories
    WHERE @categoryId = CategoryID


CREATE PROCEDURE [dbo].FindEmployeeById
    @employeeId INT
AS
    SELECT EmployeeID, LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo, PhotoPath
    FROM dbo.Employees
    WHERE EmployeeID = @employeeId


CREATE PROCEDURE [dbo].FindProductById
    @productId int
AS
    SELECT Products.ProductID, Products.ProductName, Products.SupplierID, Products.CategoryID, Products.QuantityPerUnit, Products.UnitPrice, Products.UnitsInStock, Products.UnitsOnOrder, Products.ReorderLevel, Products.Discontinued
    FROM Products
    WHERE Products.ProductID = @productId


CREATE PROCEDURE [dbo].InsertCategory
    @categoryName NVARCHAR (15), 
    @description NTEXT, 
    @picture IMAGE
AS
    INSERT INTO dbo.Categories (CategoryName, Description, Picture) OUTPUT Inserted.CategoryID
    VALUES (@categoryName, @description, @picture)


CREATE PROCEDURE [dbo].InsertEmployee
    @lastName NVARCHAR(20),
    @firstName NVARCHAR(10),
    @title NVARCHAR(30),
    @titleOfCourtesy NVARCHAR(25),
    @birthDate DATETIME,
    @hireDate DATETIME,
    @address NVARCHAR(60),
    @city NVARCHAR(15),
    @region NVARCHAR(15),
    @postalCode NVARCHAR(10),
    @country NVARCHAR(15),
    @homePhone NVARCHAR(24),
    @extension NVARCHAR(4),
    @photo Image,
    @notes NText,
    @reportsTo INT,
    @photoPath NVARCHAR(255)
AS
    INSERT INTO dbo.Employees (LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo, PhotoPath) OUTPUT Inserted.EmployeeId
    VALUES (@lastName, @firstName, @title, @titleOfCourtesy, @birthDate, @hireDate, @address, @city, @region, @postalCode, @country, @homePhone, @extension, @photo, @notes, @reportsTo, @photoPath)


CREATE PROCEDURE [dbo].InsertProduct
    @productName NVARCHAR (40),
    @supplierID INT,
    @categoryID INT,
    @quantityPerUnit NVARCHAR (20),
    @unitPrice MONEY,
    @unitsInStock SMALLINT, 
    @unitsOnOrder SMALLINT,
    @reorderLevel SMALLINT,
    @discontinued BIT
AS
    INSERT INTO dbo.Products (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued) OUTPUT Inserted.ProductID
    VALUES (@productName, @supplierId, @categoryId, @quantityPerUnit, @unitPrice, @unitsInStock, @unitsOnOrder, @reorderLevel, @discontinued)


CREATE PROCEDURE [dbo].SelectCategories
    @offset INT, 
    @limit INT
AS
    SELECT CategoryID, CategoryName, Description, Picture
    FROM dbo.Categories
    ORDER BY CategoryID
    OFFSET @offset ROWS
    FETCH FIRST @limit ROWS ONLY


CREATE PROCEDURE [dbo].SelectCategoriesByName
    @names StringCollection READONLY
AS
    SELECT CategoryID, CategoryName, Description, Picture
    FROM dbo.Categories
    WHERE CategoryName in (SELECT Name FROM @names)


CREATE PROCEDURE [dbo].SelectEmployees
    @offset INT, 
    @limit INT
AS
    SELECT EmployeeID, LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, Address, City, Region, PostalCode, Country, HomePhone, Extension, photo, notes, ReportsTo, PhotoPath
    FROM dbo.Employees
    ORDER BY EmployeeID
    OFFSET @offset ROWS
    FETCH FIRST @limit ROWS ONLY


CREATE PROCEDURE [dbo].SelectProducts
    @limit INT, @offset INT
AS
    SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued 
    FROM dbo.Products as p
    ORDER BY p.ProductID
    OFFSET @offset ROWS
    FETCH FIRST @limit ROWS ONLY


CREATE PROCEDURE [dbo].SelectProductsByCategory
    @categories IntCollection READONLY
AS
    SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued 
    FROM dbo.Products as p
    WHERE p.CategoryID in (SELECT Id FROM @categories)
    ORDER BY p.ProductID


CREATE PROCEDURE [dbo].SelectProductsByName
    @names StringCollection READONLY
AS
    SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued 
    FROM dbo.Products as p
    WHERE p.ProductName in (SELECT Name FROM @names)
    ORDER BY p.ProductID


CREATE PROCEDURE [dbo].UpdateCategory
    @categoryId INT,
    @categoryName NVARCHAR (15),
    @description NTEXT, 
    @picture IMAGE
AS
    UPDATE dbo.Categories
    SET CategoryName = @categoryName, Description = @description, Picture = @picture
    WHERE @categoryId = CategoryID
    SELECT @@ROWCOUNT


CREATE PROCEDURE [dbo].UpdateEmployee
    @employeeId INT, 
    @lastName NVARCHAR(20),
    @firstName NVARCHAR(10),
    @title NVARCHAR(30),
    @titleOfCourtesy NVARCHAR(25),
    @birthDate DATETIME,
    @hireDate DATETIME,
    @address NVARCHAR(60),
    @city NVARCHAR(15),
    @region NVARCHAR(15),
    @postalCode NVARCHAR(10),
    @country NVARCHAR(15),
    @homePhone NVARCHAR(24),
    @extension NVARCHAR(4),
    @photo Image,
    @notes NText,
    @reportsTo INT,
    @photoPath NVARCHAR(255)
AS
    UPDATE dbo.Employees
    SET LastName = @employeeId, FirstName = @firstName, Title = @title, TitleOfCourtesy = @titleOfCourtesy, BirthDate = @birthDate, HireDate = @hireDate, Address = @address, City = @city, Region = @region, PostalCode = postalCode, Country = country, HomePhone = @homePhone, Extension = @extension, Photo = @photo, Notes = @notes, ReportsTo = @reportsTo, PhotoPath = @photoPath
    WHERE @employeeId = EmployeeID
    SELECT @@ROWCOUNT


CREATE PROCEDURE [dbo].UpdateProduct
    @productId INT,
    @productName NVARCHAR (40),
    @supplierID INT,
    @categoryID INT,
    @quantityPerUnit NVARCHAR (20),
    @unitPrice MONEY,
    @unitsInStock SMALLINT, 
    @unitsOnOrder SMALLINT,
    @reorderLevel SMALLINT,
    @discontinued BIT
AS
    UPDATE dbo.Products
    SET ProductName = @productName, SupplierID = @supplierId, CategoryID = @categoryId, QuantityPerUnit = @quantityPerUnit, UnitPrice = @unitPrice, UnitsInStock = @unitsInStock, UnitsOnOrder = @unitsOnOrder, ReorderLevel = @reorderLevel, Discontinued = @discontinued
    WHERE ProductID = @productId
    SELECT @@ROWCOUNT
