create table Processing
(
  Id uniqueidentifier NOT NULL PRIMARY KEY,
  Name nvarchar(200) NOT NULL,
  ProcessedTime datetime2 NOT NULL default('01.01.1900'),
  ProcessedKeys nvarchar(max) NULL,

  UpdateTime datetime2 NOT NULL default('01.01.1900'),

  --INDEX Processing_Name_Index (Name)
);

go
create table ShotCategory
(
  Id uniqueidentifier NOT NULL PRIMARY KEY,
  Shot nvarchar(200) NOT NULL,
  Category nvarchar(200) NOT NULL,

  ChangeTick datetime2 NOT NULL default(GEtUTCDATE()),

--  INDEX ShotCategory_Category_Index NONCLUSTERED (Category),
--  INDEX ShotCategory_Shot_Index NONCLUSTERED (Shot)

);
go
create table Shot
(
  Name nvarchar(200) NOT NULL PRIMARY KEY,
  Game nvarchar(200) NOT NULL,

  ChangeTick datetime2 NOT NULL default(GEtUTCDATE()),

--  INDEX Shot_Name_Index NONCLUSTERED (Name),

);
go