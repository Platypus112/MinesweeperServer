﻿use Master;
Go
IF EXISTS (SELECT * FROM sys.databases WHERE name=N'MinesweeperDB')
BEGIN
	DROP DATABASE MinesweeperDB;
END
GO
CREATE DATABASE MinesweeperDB; 
GO
USE MinesweeperDB;
GO

CREATE TABLE Users(
	[id] int IDENTITY(1,1) PRIMARY KEY,--id for each player
	[Name] nvarchar(30) NOT NULL,
	[Email] nvarchar(70) NOT NULL,
	[Password] nvarchar(30) NOT NULL,
	[PicPath] nvarchar(5) NULL,
	[Description] nvarchar(300) NULL,
	[Admin] bit NOT NULL
)


CREATE TABLE Difficulties( --table for calssifying different difficulties--
	[id] int IDENTITY(1,1) PRIMARY KEY, --id for the difficulty
	[name] nvarchar(50) NOT NULL, --name of the difficulty
	[height] int NOT NULL, --height of the board
	[width] int NOT NULL, --width of the board
	[bombs] int NOT NULL --amount of bombs in the board
)
CREATE TABLE Statuses(
	[id] int IDENTITY(1,1) PRIMARY KEY, --id for idenitfying the report
	[name] nvarchar(50) NOT NULL, --the name of the status (approved,denied,pending)
)

CREATE TABLE FinishedGames(
	[id] int IDENTITY(1,1) PRIMARY KEY,
	[userId] int NOT NULL, --id of the player who played this game
	[difficultyId] int NOT NULL, --id indicating the difficulty of said game--
	[Date] DATETIME NULL, --the time the game started--
	[TimeInSeconds] int NOT NULL, --the time the game took to finish measured in seconds
	CONSTRAINT FK_GameToPlayerID FOREIGN KEY (userId) REFERENCES Users(id),
	CONSTRAINT FK_GameIDToDifficultyID FOREIGN KEY (difficultyId) REFERENCES Difficulties(id),
)

CREATE TABLE GameReports(
	[id] int  IDENTITY(1,1) PRIMARY KEY, --id for the report
	[gameId] int NOT NULL, --id of the game that the report is about
	[statusId] int NOT NULL, --the status of the report
	[Description] nvarchar(200) NULL, --the description of the problem. dosn't have to be filled
	CONSTRAINT FK_ReportToGameID FOREIGN KEY (gameId) REFERENCES FinishedGames(id),
	CONSTRAINT FK_GameReportToStatus FOREIGN KEY (statusId) REFERENCES Statuses(id), --connects the report to it's status
)

CREATE TABLE UserReports(
	[id] int  IDENTITY(1,1) PRIMARY KEY, --id for the report
	[userId] int NOT NULL, --id of the user that the report is about
	[statusId] int NOT NULL, --the status of the report
	[Description] nvarchar(200) NULL, --the description of the problem. dosn't have to be filled
	CONSTRAINT FK_ReportToUserID FOREIGN KEY (userId) REFERENCES Users(id),
	CONSTRAINT FK_UserReportToStatus FOREIGN KEY (statusId) REFERENCES Statuses(id), --connects the report to it's status
)

CREATE TABLE FriendRequests(
	[id] int IDENTITY(1,1) PRIMARY KEY, --id for the friend request
	[userSendingId] int NOT NULL,
	[userRecievingId] int NOT NULL,
	[statusid] int NOT NULL,--status for the friend request.
	--if it is approved then it adds both the users on the sending and recieving side to this table with the approved status
	--if the user is blocked then this will be declined and when trying to send the same friend request it wouldn't work
	--if the request is declined tha row describing this erquest will be removed from this table.
	--while the request is still pending then the status will be pending
	CONSTRAINT FK_FriendsToUser1ID FOREIGN KEY (userSendingId) REFERENCES Users(id),
	CONSTRAINT FK_FriendsToUser2ID FOREIGN KEY (userRecievingId) REFERENCES Users(id),
	CONSTRAINT FK_RequestToStatus FOREIGN KEY (statusId) REFERENCES Statuses(id),
)
go
INSERT INTO dbo.Users VALUES('joeAdmin','idancar7@gmail.com','Idan12345',null,'most esteamed leader',1);
INSERT INTO dbo.Users VALUES('joeUser','joe6987@gmail.com','joeMode5',null,'nuh uh',0);
INSERT INTO dbo.Users VALUES('badGuy','baddy77@gmail.com','Bad12',null,'tee hee',0);

INSERT INTO dbo.Statuses VALUES('pending');
INSERT INTO dbo.Statuses VALUES('approved');
INSERT INTO dbo.Statuses VALUES('declined');

INSERT INTO dbo.Difficulties VALUES('Begginer',22,12,12);
INSERT INTO dbo.Difficulties VALUES('Easy',10,7,10);
INSERT INTO dbo.Difficulties VALUES('Medium',22,12,40);
INSERT INTO dbo.Difficulties VALUES('Hard',32,18,100);
INSERT INTO dbo.Difficulties VALUES('Extreme',32,18,150);

go
INSERT INTO dbo.FriendRequests VALUES(2,1,1)
INSERT INTO dbo.FriendRequests VALUES(2,3,2)
INSERT INTO dbo.FriendRequests VALUES(3,2,2)

go
INSERT INTO dbo.UserReports VALUES(3,1,'too bad')

go
INSERT INTO dbo.FinishedGames VALUES(1,5,'4-june-2024',145)
Go

-- Create a login for the admin user
CREATE LOGIN [MinesweeperAdminLogin] WITH PASSWORD = 'joe123';
Go

-- Create a user in the TasksManagementDB database for the login
CREATE USER [MinesweeperAdminUser] FOR LOGIN [MinesweeperAdminLogin];
Go

-- Add the user to the db_owner role to grant admin privileges
ALTER ROLE db_owner ADD MEMBER [MinesweeperAdminUser];
Go

--EF Code

--scaffold-DbContext "Server = (localdb)\MSSQLLocalDB;Initial Catalog=MinesweeperDB;User ID=MinesweeperAdminLogin;Password=joe123;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models -Context MinesweeperDbContext -DataAnnotations -force

