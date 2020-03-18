﻿CREATE TABLE [dbo].[QueuRecord]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(300) NOT NULL, 
    [TopicPattern] NVARCHAR(MAX) NULL DEFAULT NULL,
	[NotificationAdress] NVARCHAR(MAX) NULL DEFAULT NULL,
    [Created] DATETIME2 NOT NULL
)

GO

CREATE UNIQUE INDEX [IX_QueuRecord_Name] ON [dbo].[QueuRecord] ([Name])
