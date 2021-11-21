CREATE TABLE [dbo].[LogEvent] (
    [Seq]          INT            IDENTITY (1, 1) NOT NULL,
    [EventLevel]   VARCHAR (10)   NOT NULL,
    [EventType]    NVARCHAR (50)  NULL,
    [EventContent] NVARCHAR (MAX) NULL,
    [CaseNo]       VARCHAR(100)            NULL,
    [CreateTime]   DATETIME       NOT NULL DEFAULT getdate(),
    CONSTRAINT [PK__LogEvent__CA1E3C88D1881868] PRIMARY KEY CLUSTERED ([Seq] ASC)
);



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LogEvent', @level2type = N'COLUMN', @level2name = N'Seq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'事件種類', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LogEvent', @level2type = N'COLUMN', @level2name = N'EventType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'事件內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LogEvent', @level2type = N'COLUMN', @level2name = N'EventContent';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LogEvent', @level2type = N'COLUMN', @level2name = N'CreateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LogEvent', @level2type = N'COLUMN', @level2name = 'CaseNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'事件等級', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LogEvent', @level2type = N'COLUMN', @level2name = N'EventLevel';

