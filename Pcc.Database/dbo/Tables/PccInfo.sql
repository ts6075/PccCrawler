CREATE TABLE [dbo].[PccInfo] (
    [CaseNo]          VARCHAR(100)            NOT NULL,
    [Category] NVARCHAR(20) NOT NULL, 
    [Name]        NVARCHAR (50)  NOT NULL,
    [HtmlContent] NVARCHAR (MAX) NULL,
    [CreateTime] DATETIME NOT NULL DEFAULT getdate(), 
    CONSTRAINT [PK_PccInfo] PRIMARY KEY CLUSTERED ([CaseNo], [Category], [Name])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = 'CaseNo';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = 'Category';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'欄位名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = N'Name';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'欄位Html資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = N'HtmlContent';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = N'CreateTime';
