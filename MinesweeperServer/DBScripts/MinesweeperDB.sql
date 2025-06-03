use Master;
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
INSERT INTO dbo.Users VALUES('friendGuy','Friends5ever@gmail.com','Happy1',null,'top 1000 freindlisest peoeple',0);
INSERT INTO dbo.Users VALUES('extremlyViolentDude','VIolenece69@gmail.com','Anger9',null,'mmmmmm yes i love violoenece and breaking the law hmmmmmm yes',0);


INSERT INTO dbo.Statuses VALUES('pending');
INSERT INTO dbo.Statuses VALUES('approved');
INSERT INTO dbo.Statuses VALUES('declined');

INSERT INTO dbo.Difficulties VALUES('Begginer',22,12,12);
INSERT INTO dbo.Difficulties VALUES('Easy',10,7,10);
INSERT INTO dbo.Difficulties VALUES('Medium',22,12,40);
INSERT INTO dbo.Difficulties VALUES('Hard',32,18,100);
INSERT INTO dbo.Difficulties VALUES('Extreme',32,18,150);

go
--joeUser to joeAdmin Request
INSERT INTO dbo.FriendRequests VALUES(2,1,1)

--badGuy to joeAdmin Request
INSERT INTO dbo.FriendRequests VALUES(3,1,1)

--joeUser to FriendGuy Request
INSERT INTO dbo.FriendRequests VALUES(2,4,1)

-- joeUser, badGuy friends
INSERT INTO dbo.FriendRequests VALUES(2,3,2)
INSERT INTO dbo.FriendRequests VALUES(3,2,2)

-- friendGuy, joeAdmin friends
INSERT INTO dbo.FriendRequests VALUES(4,1,2)
INSERT INTO dbo.FriendRequests VALUES(1,4,2)

-- friendGuy, badGuy friends
INSERT INTO dbo.FriendRequests VALUES(4,3,2)
INSERT INTO dbo.FriendRequests VALUES(3,4,2)

-- badGuy, extremlyViolentDude friends
INSERT INTO dbo.FriendRequests VALUES(5,3,2)
INSERT INTO dbo.FriendRequests VALUES(3,5,2)

-- joeUser to extremlyViolentdude block Request
INSERT INTO dbo.FriendRequests VALUES(2,5,3)

--friendGuy to extremlyViolentdude block Request
INSERT INTO dbo.FriendRequests VALUES(4,5,3)

--extremlyViolentdude to joeAdmin block Request
INSERT INTO dbo.FriendRequests VALUES(5,1,3)


go
-- badGuy pending report
INSERT INTO dbo.UserReports VALUES(3,1,'too bad')

-- extremlyViolentdude approved report
INSERT INTO dbo.UserReports VALUES(5,2,'man this guy was violent to me irl')

-- extremlyViolentdude declined report
INSERT INTO dbo.UserReports VALUES(5,3,'i dunno mmmmm')

go
-- extreme difficulty game to report by badGuy
INSERT INTO dbo.FinishedGames VALUES(3,5,'4-june-2025',20) --badguy
-- report for ^^ game
INSERT INTO dbo.GameReports VALUES(1,2,'too fast')
INSERT INTO dbo.GameReports VALUES(1,3,'hate this guy')
INSERT INTO dbo.GameReports VALUES(1,1,'broke my record sdadge')


-- begginer difficulty games
INSERT INTO dbo.FinishedGames VALUES(2,1,'12-june-2025',15) --joeUser
INSERT INTO dbo.FinishedGames VALUES(4,1,'12-june-2025',18) --friendGuy
INSERT INTO dbo.FinishedGames VALUES(3,1,'8-june-2025',25) --badGuy
INSERT INTO dbo.FinishedGames VALUES(1,1,'12-may-2025',21) --joeAdmin

-- easy difficulty games
INSERT INTO dbo.FinishedGames VALUES(2,2,'12-june-2025',45) --joeUser
INSERT INTO dbo.FinishedGames VALUES(4,2,'5-may-2025',90) --friendGuy
INSERT INTO dbo.FinishedGames VALUES(2,2,'8-june-2025',50) --joeUser

-- medium difficulty games
INSERT INTO dbo.FinishedGames VALUES(2,3,'9-june-2025',89) --joeUser
INSERT INTO dbo.FinishedGames VALUES(4,3,'7-june-2025',153) --friendGuy

-- hard difficulty games
INSERT INTO dbo.FinishedGames VALUES(2,4,'12-june-2025',101) --joeUser
INSERT INTO dbo.FinishedGames VALUES(4,4,'8-june-2025',187) --friendGuy

-- extreme difficulty games
INSERT INTO dbo.FinishedGames VALUES(1,5,'4-june-2024',145)--jeoAdmin
INSERT INTO dbo.FinishedGames VALUES(2,5,'11-june-2025',190)--joeUser
INSERT INTO dbo.FinishedGames VALUES(2,5,'10-june-2025',250)--joeUser
INSERT INTO dbo.FinishedGames VALUES(2,5,'10-june-2025',260)--joeUser
INSERT INTO dbo.FinishedGames VALUES(5,5,'9-june-2025',280)--extremlyViolentdude
INSERT INTO dbo.FinishedGames VALUES(5,5,'6-june-2025',370)--extremlyViolentdude
INSERT INTO dbo.FinishedGames VALUES(3,5,'16-may-2024',415)--badGuy
INSERT INTO dbo.FinishedGames VALUES(4,5,'8-june-2025',430)--friendGuy
INSERT INTO dbo.FinishedGames VALUES(4,5,'9-june-2025',447)--friendGuy
INSERT INTO dbo.FinishedGames VALUES(4,5,'10-june-2025',616)--friendGuy

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

