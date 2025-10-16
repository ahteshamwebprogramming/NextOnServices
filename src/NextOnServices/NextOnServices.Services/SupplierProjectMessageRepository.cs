using System;
using System.Threading.Tasks;
using Dapper;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class SupplierProjectMessageRepository : DapperGenericRepository<SupplierProjectMessage>, ISupplierProjectMessageRepository
{
    public SupplierProjectMessageRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {
    }

    public async Task<int> MarkSupplierMessagesAsReadAsync(int projectMappingId, DateTime readUtc)
    {
        const string sql = @"UPDATE SupplierProjectMessages
                             SET IsRead = 1,
                                 ReadUtc = @ReadUtc
                             WHERE ProjectMappingId = @ProjectMappingId
                               AND FromSupplier = 1
                               AND IsRead = 0;";

        if (DbConnection.State != System.Data.ConnectionState.Open)
        {
            DbConnection.Open();
        }

        try
        {
            return await DbConnection.ExecuteAsync(sql, new { ProjectMappingId = projectMappingId, ReadUtc = readUtc });
        }
        finally
        {
            if (DbConnection.State == System.Data.ConnectionState.Open)
            {
                DbConnection.Close();
            }
        }
    }
}
