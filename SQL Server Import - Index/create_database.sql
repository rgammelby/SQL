-- checks if database exists, creates database if not
IF db_id('Performance') IS NULL
	USE [master]
	CREATE DATABASE [Performance]

GO

-- instance table creation; allows user to see incidence of numbers (highest/lowest)
USE [Performance]
CREATE TABLE Instance (
	number VARCHAR(8) PRIMARY KEY NOT NULL,
	instances INT NOT NULL
);

-- random table omitted, as it's created automatically during flat file import

GO

-- stored procedure for MOST frequently occurring number output
CREATE PROCEDURE SelectHighestOccurrence
AS
SELECT TOP 1 * FROM Instance
ORDER BY instances DESC

GO

-- stored procedure for LEAST frequently occurring number output
CREATE PROCEDURE SelectLowestOccurrence
AS
SELECT TOP 1 * FROM Instance
ORDER BY instances ASC