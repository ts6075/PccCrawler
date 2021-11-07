﻿CREATE TABLE [dbo].[PccInfo] (
    [Id]          INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [HtmlContent] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PccInfo] PRIMARY KEY CLUSTERED ([Id], [Name])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = N'Id';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'欄位名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = N'Name';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'欄位Html資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccInfo', @level2type = N'COLUMN', @level2name = N'HtmlContent';