Create PROCEDURE [dbo].[SP_CreateDataBase]
(
@dataBaseName nvarchar(50)
)

AS
BEGIN
DECLARE @SQLDataBase NVARCHAR(50);
SET @SQLDataBase = 'CREATE DATABASE [' + @dataBaseName +'];';	
EXEC(@SQLDataBase);
END