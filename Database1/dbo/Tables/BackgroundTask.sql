CREATE TABLE [dbo].[BackgroundTask] (
    [BackgroundTaskId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255) NOT NULL,
    [LastRunStart]     DATETIME2 (7)  NOT NULL,
    [LastRunEnd]       DATETIME2 (7)  NOT NULL,
    [LastStatusId]     INT            NOT NULL,
    [RunEveryDayAfter] TIME (7)       NULL,
    CONSTRAINT [PK_BackgroundTask] PRIMARY KEY CLUSTERED ([BackgroundTaskId] ASC)
);



