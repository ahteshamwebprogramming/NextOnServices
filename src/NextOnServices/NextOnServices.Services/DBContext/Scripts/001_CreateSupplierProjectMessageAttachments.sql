CREATE TABLE dbo.SupplierProjectMessageAttachments (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    MessageId INT NOT NULL FOREIGN KEY REFERENCES dbo.SupplierProjectMessages(Id),
    ClientId NVARCHAR(100) NULL,
    FileName NVARCHAR(255) NOT NULL,
    ContentType NVARCHAR(100) NOT NULL,
    FileSize BIGINT NOT NULL,
    FileData VARBINARY(MAX) NOT NULL,
    UploadedUtc DATETIMEOFFSET(7) NOT NULL
);

CREATE INDEX IX_SupplierProjectMessageAttachments_MessageId
    ON dbo.SupplierProjectMessageAttachments(MessageId);
