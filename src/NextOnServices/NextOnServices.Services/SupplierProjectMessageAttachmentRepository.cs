using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class SupplierProjectMessageAttachmentRepository : ISupplierProjectMessageAttachmentRepository
{
    private readonly string _connectionString;

    public SupplierProjectMessageAttachmentRepository(DapperDBSetting dbSetting)
    {
        _connectionString = string.IsNullOrWhiteSpace(dbSetting.ConnectionString)
            ? "Data Source=182.18.138.217;Initial Catalog=NextOnServicesCore_BK;User ID=sa;Password=CzWR6nbSsE44c$;Encrypt=False;"
            : dbSetting.ConnectionString;
    }

    public async Task AddRangeAsync(IEnumerable<SupplierProjectMessageAttachment> attachments)
    {
        var items = attachments?.ToList();
        if (items == null || items.Count == 0)
        {
            return;
        }

        const string sql = @"INSERT INTO SupplierProjectMessageAttachments (Id, MessageId, ClientId, FileName, ContentType, FileSize, FileData, UploadedUtc)
                             VALUES (@Id, @MessageId, @ClientId, @FileName, @ContentType, @FileSize, @FileData, @UploadedUtc);";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var attachment in items)
            {
                await connection.ExecuteAsync(sql, new
                {
                    attachment.Id,
                    attachment.MessageId,
                    attachment.ClientId,
                    attachment.FileName,
                    attachment.ContentType,
                    attachment.FileSize,
                    attachment.FileData,
                    attachment.UploadedUtc
                }, transaction).ConfigureAwait(false);
            }

            transaction.Commit();
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // ignored
            }

            throw;
        }
    }

    public async Task<SupplierProjectMessageAttachment?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT TOP 1 Id, MessageId, ClientId, FileName, ContentType, FileSize, FileData, UploadedUtc
                             FROM SupplierProjectMessageAttachments
                             WHERE Id = @Id;";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<SupplierProjectMessageAttachment>(sql, new { Id = id }).ConfigureAwait(false);
    }

    public async Task<SupplierProjectMessageAttachment?> GetByIdForProjectAsync(Guid id, int projectMappingId)
    {
        const string sql = @"SELECT TOP 1 spa.Id, spa.MessageId, spa.ClientId, spa.FileName, spa.ContentType, spa.FileSize, spa.FileData, spa.UploadedUtc
                             FROM SupplierProjectMessageAttachments spa
                             INNER JOIN SupplierProjectMessages spm ON spm.Id = spa.MessageId
                             WHERE spa.Id = @Id AND spm.ProjectMappingId = @ProjectMappingId;";

        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<SupplierProjectMessageAttachment>(sql, new { Id = id, ProjectMappingId = projectMappingId }).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<SupplierProjectMessageAttachment>> GetByMessageIdsAsync(IEnumerable<int> messageIds)
    {
        var ids = messageIds?.Distinct().ToArray();
        if (ids == null || ids.Length == 0)
        {
            return Array.Empty<SupplierProjectMessageAttachment>();
        }

        const string sql = @"SELECT Id, MessageId, ClientId, FileName, ContentType, FileSize, FileData, UploadedUtc
                             FROM SupplierProjectMessageAttachments
                             WHERE MessageId IN @MessageIds;";

        using var connection = new SqlConnection(_connectionString);
        var records = await connection.QueryAsync<SupplierProjectMessageAttachment>(sql, new { MessageIds = ids }).ConfigureAwait(false);
        return records.ToList();
    }
}
