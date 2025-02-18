using NextOnServices.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Services.DBContext;

public class DapperSqlServerStrategy : IDapperDbStrategy
{
    public IDbConnection GetConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }
}
