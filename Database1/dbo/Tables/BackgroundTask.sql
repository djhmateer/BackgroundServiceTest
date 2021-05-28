CREATE TABLE [dbo].[BackgroundTask] (
    [BackgroundTaskId]   INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [LastRun]            DATETIME2 (7)  NULL,
    [NextRun]            DATETIME2 (7)  NULL,
    [FrequencyInMinutes] INT            NOT NULL,
    CONSTRAINT [PK_BackgroundTask] PRIMARY KEY CLUSTERED ([BackgroundTaskId] ASC)
);

