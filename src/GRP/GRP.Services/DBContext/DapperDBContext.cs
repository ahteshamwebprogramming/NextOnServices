using GRP.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Services.DBContext;

public class DapperDBContext
{
    private IDapperDbStrategy _dbStrategy;
    public DapperDBContext SetStrategy()
    {
        _dbStrategy = new DapperSqlServerStrategy();
        return this;
    }

    public IDbConnection GetDbContext(string connectionString)
    {
        return _dbStrategy.GetConnection(connectionString);
    }
}
