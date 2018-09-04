create table Processing
(
  Id uniqueidentifier NOT NULL PRIMARY KEY,
  Name nvarchar(200) NOT NULL,
  ProcessedTime datetime2 NOT NULL default('01.01.1900'),
  ProcessedKeys nvarchar(max) NULL,

  UpdateTime datetime2 NOT NULL default('01.01.1900'),

  --INDEX Processing_Name_Index (Name)
);
