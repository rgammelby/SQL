USE [master]
--GO

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Performance')
BEGIN
	CREATE DATABASE [Performance]
	END

	GO
		USE [Performance]
	GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Random')
BEGIN
	CREATE TABLE Random (
		id int primary key not null,
		random_number int not null,
		check (random_number < 10000)
	)
END