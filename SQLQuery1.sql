use master;
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'app')
BEGIN
    CREATE DATABASE app_db;
END
-- GO
--USE app_db;
-- GO
CREATE TABLE players (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PlayerName NVARCHAR(100),
    PlayerColor NVARCHAR(10),
    PoliticalX FLOAT,
    PoliticalY FLOAT,
    LikeabilityLiberals INT,
    LikeabilityConservatives INT,
    LikeabilityLibertarians INT,
    LikeabilityAuthoritarians INT
);