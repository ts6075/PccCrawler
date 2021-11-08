CREATE TABLE [dbo].[PccMaster] (
    [Id]         INT           NOT NULL,
    [Category] NVARCHAR(20) NOT NULL, 
    [Url]        VARCHAR (200) NULL,
    [Status]     INT           NULL,
    [CreateTime] DATETIME      CONSTRAINT [DF_PccMaster_CreateTime] DEFAULT (getdate()) NOT NULL,
    [UpdateTime] DATETIME      CONSTRAINT [DF_PccMaster_UpdateTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_PccMaster] PRIMARY KEY CLUSTERED ([Category], [Id])
);





GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccMaster', @level2type = N'COLUMN', @level2name = N'Id';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccMaster', @level2type = N'COLUMN', @level2name = 'Category';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Url', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccMaster', @level2type = N'COLUMN', @level2name = N'Url';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccMaster', @level2type = N'COLUMN', @level2name = N'Status';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccMaster', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PccMaster', @level2type = N'COLUMN', @level2name = N'UpdateTime';

