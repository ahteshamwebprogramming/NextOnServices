using Dapper;
using Z.BulkOperations;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GRP.Core.Repository;
using System.Data.Entity;
using Microsoft.SqlServer.Types;
using System.Data.SqlClient;
using GRP.Services.DBContext;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Transactions;
using System.Data.Common;
using System.Reflection.Metadata;
using GRP.Core.Entities;
using GRP.Infrastructure.Helper;
using Z.Dapper.Plus;
using System.Collections;

//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

//using Microsoft.EntityFrameworkCore.Query;
//using System.Data.Common;
//using Z.Dapper.Plus;

namespace GRP.Services;

public class DapperGenericRepository<T> : IDapperRepository<T> where T : class, new()
{
    protected IDbConnection DbConnection { get; private set; }
    private readonly DapperDBSetting _dbSettings;
    private readonly SqlConnection _connection;
    public DapperGenericRepository(DapperDBSetting dbSettings)
    {
        _dbSettings = dbSettings;
        if (_dbSettings.ConnectionString.IsNullOrEmpty())
            //_dbSettings.ConnectionString = "Data Source=SHIVANSHJUYAL\\SQLEXPRESS;Initial Catalog=SimpliHRDB;Integrated Security=True;TrustServerCertificate=True";
            _dbSettings.ConnectionString = "Data Source=182.18.138.217;Initial Catalog=GRPCore;User ID=sa;Password=CzWR6nbSsE44c$;Encrypt=False;";
        DbConnection = new DapperDBContext().SetStrategy()
            .GetDbContext(_dbSettings.ConnectionString);
        _connection = new SqlConnection("Data Source=182.18.138.217;Initial Catalog=GRPCore;User ID=sa;Password=CzWR6nbSsE44c$;Encrypt=False;");
    }
    public object UpdateFields<TS>(T param, IDbConnection connection, IDbTransaction transaction = null, int? commandTimeOut = null)
    {


        var names = new List<string>();
        string tableID = string.Empty;
        object id = null;
        T t = new();
        tableID = GetKeyOfEntity(t);
        if (tableID.Equals(string.Empty))
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(param))
            {
                if (!tableID.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase))
                    names.Add(property.Name);
                else
                    id = property.GetValue(param);
            }

            if (id != null && names.Count > 0)
            {
                var sql = string.Format("UPDATE {1} SET {0} WHERE Id=@Id", string.Join(",", names.Select(t => { t = t + "=@" + t; return t; })), typeof(T).Name);
                if (Debugger.IsAttached)
                    Trace.WriteLine(string.Format("UpdateFields: {0}", sql));
                return connection.Execute(sql, param, transaction, commandTimeOut) > 0 ? id : null;
            }
        }
        return null;
    }
    public string GetKeyOfEntity(T entity)
    {
        string tableKey = string.Empty;
        PropertyInfo[] properties = typeof(T).GetProperties();
        //Find the tableID
        foreach (PropertyInfo propertyInfo in properties)
        {
            bool isIdentity = propertyInfo.GetCustomAttributes(inherit: true).Any((object a) => a is KeyAttribute);
            if (isIdentity)
            {
                tableKey = propertyInfo.Name;
                break;
            }
        }
        return tableKey;
    }

    public async Task<int> EexecuteAddAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null)
    {
        if (dbConnection.State == ConnectionState.Closed)
            DbConnection.Open();

        try
        {
            var inserted = (await DbConnection
            .InsertAsync<T>(entity, transaction, timeOut));

            return inserted;
        }
        catch (Exception ex)
        {
            return 0;
        }
        //finally { DbConnection.Close(); }
    }

    public async Task<bool> EexecuteUpdateAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null)
    {
        if (dbConnection.State == ConnectionState.Closed)
            DbConnection.Open();

        try
        {
            return await DbConnection
                .UpdateAsync<T>(entity, transaction, timeOut);

        }
        catch (Exception ex)
        {
            return false;
        }
        //finally { DbConnection.Close(); }
    }

    public async Task<bool> EexecuteDeleteAsync(int id, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null)
    {
        if (dbConnection.State == ConnectionState.Closed)
            DbConnection.Open();

        try
        {
            var entity = await DbConnection
               .GetAsync<T>(id);

            if (entity == null)
                return false;

            return await DbConnection
                .DeleteAsync<T>(entity, transaction, timeOut);

        }
        catch (Exception ex)
        {
            return false;
        }
        //finally { DbConnection.Close(); }
    }

    public async Task<DapperPlusActionSet<T>> AddRangeAsync(T entity)
    {
        _connection.Open();
        try
        {
            var inserted = (_connection.BulkInsert<T>(entity));
            return inserted;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally { _connection.Close(); }
    }

    public async Task<int> AddAsync(T entity)
    {
        DbConnection.Open();

        try
        {
            var inserted = (await DbConnection
            .InsertAsync<T>(entity));

            return inserted;
        }
        catch (Exception ex)
        {
            return 0;
        }
        finally { DbConnection.Close(); }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        DbConnection.Open();

        try
        {
            var entity = await DbConnection
                .GetAsync<T>(id);

            if (entity == null)
                return false;

            return await DbConnection
                .DeleteAsync<T>(entity);
        }
        finally { DbConnection.Close(); }
    }

    public async Task<List<T>> FindAllAsync()

    {
        DbConnection.Open();

        try
        {
            var results = await DbConnection
                .GetAllAsync<T>();

            return results
                .ToList();
        }
        finally { DbConnection.Close(); }
    }

    public async Task<T> FindByIdAsync(int id)
    {
        DbConnection.Open();

        try
        {
            return await DbConnection
                .GetAsync<T>(id);
        }
        finally { DbConnection.Close(); }
    }

    public async Task<bool> UpdateAsync(T entity)
    {

        DbConnection.Open();

        try
        {
            return await DbConnection
                .UpdateAsync<T>(entity);
        }
        catch (Exception ex)
        {
            return false;
        }
        finally { DbConnection.Close(); }
    }

    public async Task<bool> Exists(Expression<Func<T, bool>> filter)
    {
        DbConnection.Open();
        try
        {
            var data = await DbConnection.GetAllAsync<T>();
            var results = data.AsQueryable().SingleOrDefault(filter);
            if (results != null)
                return true;
            return false;
        }

        finally { DbConnection.Close(); }
    }
    public async Task<T> GetFilter(Expression<Func<T, bool>> filter)
    {
        DbConnection.Open();
        try
        {
            var data = await DbConnection.GetAllAsync<T>();
            var results = data.AsQueryable().SingleOrDefault(filter);
            return results;
        }

        finally { DbConnection.Close(); }
    }
    public async Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter, Expression<Func<T, bool>> orderBy)
    {
        DbConnection.Open();

        try
        {
            var data = await DbConnection.GetAllAsync<T>();
            var results = data.AsQueryable().Where(filter).OrderBy(orderBy).ToList();
            return results;
        }
        finally { DbConnection.Close(); }
    }
    public async Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter)
    {
        DbConnection.Open();

        try
        {

            var data = await DbConnection.GetAllAsync<T>();
            var results = data.AsQueryable().Where(filter).ToList();
            return results;
        }
        //catch(Exception ex)
        //{
        //    var x = 0;
        //    //var data = await DbConnection.GetAllAsync<T>();
        //    //var results = data.AsQueryable().Where(filter).ToList();
        //    //return results;

        //}
        finally { DbConnection.Close(); }
    }

    public async Task<List<T>> GetDynamicQuery(string query, object dynamicParameters)
    {
        DbConnection.Open();
        List<T> result = new List<T>();
        try
        {
            var results = (await DbConnection.QueryAsync<T>(query, dynamicParameters, commandType: CommandType.Text)).ToList();
            return results;
        }
        catch (Exception ex)
        {
            return result;
        }
        finally { DbConnection.Close(); }
    }

    public async Task<bool> IsExists(string query, object dynamicParameters)
    {
        DbConnection.Open();
        try
        {
            List<T> results = (await DbConnection.QueryAsync<T>(query, dynamicParameters, commandType: CommandType.Text)).ToList();
            return (results.Count == 0 ? false : true);
        }
        catch (Exception ex)
        {
            return false;
        }
        finally { DbConnection.Close(); }
    }

    public async Task<int> GetStoredProcedure(string storedProcedure, DynamicParameters dynamicParameters)
    {
        DbConnection.Open();
        try
        {
            var results = await DbConnection.ExecuteAsync(storedProcedure, dynamicParameters, commandType: CommandType.StoredProcedure);
            return results;
        }
        catch (Exception ex)
        {
            return 0;
        }
        finally { DbConnection.Close(); }
    }
    public async Task<List<T>> GetQueryAll(string query)
    {
        DbConnection.Open();
        try
        {
            var data = await DbConnection.QueryAsync<T>(query);
            return data.ToList();
        }
        finally { DbConnection.Close(); }
    }

    public async Task<List<T>> GetQueryAll<T>(string query, IDbConnection IDBConn, IDbTransaction trans)
    {
        //if (connection.State != ConnectionState.Open)
        //    connection.Open();
        //else
        if (IDBConn.State != ConnectionState.Open)
            IDBConn.Open();
        try
        {
            var data = await IDBConn.QueryAsync<T>(query, null, trans);
            return data.ToList();
        }
        catch (Exception ex)
        {
            return null;
        }
        //finally { IDBConn.Close(); }
    }

    public async Task<List<T>> GetTableData<T>(string sQuery)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;

        var query = sQuery;
        if (DbConnection.State != ConnectionState.Open)
            DbConnection.Open();

        try
        {
            var data = await DbConnection.QueryAsync<T>(query);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { DbConnection.Close(); }
    }

    public async Task<List<T>> GetTableData<T>(IDbConnection connection, IDbTransaction trans = null, string sWhere = "", string sOrderBy = "")
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        string sQryWhere = (sWhere != "" ? " Where " + sWhere : "");
        string sQryOrderBy = (sOrderBy != "" ? " ORDER BY " + sOrderBy : "");
        var query = $"SELECT * FROM {tableName} {sQryWhere} {sQryOrderBy}";
        if (connection.State != ConnectionState.Open)
            connection.Open();

        try
        {
            var data = await connection.QueryAsync<T>(query, null, trans);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { DbConnection.Close(); }
    }
    public async Task<List<T>> GetTableData<T>(string sQuery, object parameters = null)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;

        var query = sQuery;
        if (DbConnection.State != ConnectionState.Open)
            DbConnection.Open();

        try
        {
            var data = await DbConnection.QueryAsync<T>(query, parameters);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { DbConnection.Close(); }
    }
    public async Task<T> GetEntityData<T>(string sQuery, object parameters = null)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;

        if (DbConnection.State != ConnectionState.Open)
            DbConnection.Open();

        try
        {
            var data = await DbConnection.QueryFirstOrDefaultAsync<T>(sQuery, parameters);
            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (DbConnection.State == ConnectionState.Open)
                DbConnection.Close();
        }
    }
    public async Task<List<T>> GetTableData<T>(string sQuery, IDbConnection connection, IDbTransaction trans = null)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        var query = sQuery;
        if (DbConnection.State != ConnectionState.Open)
            DbConnection.Open();
        try
        {
            var data = await DbConnection.QueryAsync<T>(query, null, trans);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
    }
    public async Task<List<T>> GetTableDataExec<T>(string sQuery)
    {
        var tableName = typeof(T).Name;

        var query = sQuery;
        if (DbConnection.State != ConnectionState.Open)
            DbConnection.Open();

        try
        {
            var data = await DbConnection.QueryAsync<T>(query);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { DbConnection.Close(); }

        ////var DbConnection = trans?.Connection ?? connection;
        //var tableName = typeof(T).Name;
        //var query = sQuery;
        //if (DbConnection.State != ConnectionState.Open)
        //    DbConnection.Open();
        //try
        //{
        //    var data = await DbConnection.QueryAsync<T>(query, null, null);
        //    return data.ToList();
        //}
        //catch (Exception ex) { throw ex; }
        //finally { DbConnection.Close(); }
    }

    public async Task<bool> DeleteTableData<T>(IDbConnection connection, IDbTransaction trans = null, string sWhere = "")
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        string sQryWhere = (sWhere != "" ? " Where " + sWhere : "");
        var query = $"DELETE FROM {tableName} {sQryWhere}";
        if (DbConnection.State != ConnectionState.Open)
            DbConnection.Open();

        try
        {
            await DbConnection.QueryAsync<T>(query, null, trans);
            return true;
        }
        catch (Exception ex) { return false; }
    }

    public async Task<bool> RunSQLCommand(string sQuery)
    {
        //var DbConnection = trans?.Connection ?? connection;

        if (DbConnection.State != ConnectionState.Open)
            DbConnection.Open();

        try
        {
            await DbConnection.QueryAsync(sQuery);
            return true;
            //return true;
        }
        catch (Exception ex) { return false; }
    }

    public async Task<List<T>> GetAllPagedAsync(int limit, int offset, string sWhere = "", string sOrderBy = "")
    {
        var tableName = typeof(T).Name;
        string sQryWhere = (sWhere != "" ? " Where " + sWhere : "");
        string sQryOrderBy = (sOrderBy != "" ? " ORDER BY " + sOrderBy : "");
        var query = $"SELECT * FROM {tableName} {sQryWhere} {sQryOrderBy} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
        DbConnection.Open();

        try
        {
            var data = await DbConnection.QueryAsync<T>(query);
            return data.ToList();
        }
        finally { DbConnection.Close(); }
    }

    //our function can accept an array of property or field expressions for the output object, which you can then map to the DynamicParameters object.

    private DynamicParameters GetDynamicParameters<TInput, TOutput>(
        TInput inputObject,
        TOutput outputObject,
        params Expression<Func<TOutput, object>>[] outputExpressions
        )
    {
        var dp = new DynamicParameters(inputObject);
        foreach (var expr in outputExpressions)
            dp.Output(outputObject, expr);

        return dp;
    }

    public void CallProcedure<TInput, TOutput>(
        string storedProcedure,
        TInput inputObject,
        TOutput outputObject,
        string connectionId,
        params Expression<Func<TOutput, object>>[] outputExpressions
        )
    {
        using var connection = DbConnection;

        var dynamicParameters = GetDynamicParameters<TInput, TOutput>(inputObject, outputObject, outputExpressions);

        connection.Execute(storedProcedure, dynamicParameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<T>> GetSPData<T>(IDbConnection connection, IDbTransaction trans = null, string spName = "", DynamicParameters spInput = null)
    {
        var data = await SqlMapper.QueryAsync<T>(connection, spName, spInput, commandType: CommandType.StoredProcedure);
        return data.ToList();
    }

    public async Task<List<T>> GetSPData(string spName = "", DynamicParameters spInput = null)
    {
        try
        {
            var data = await SqlMapper.QueryAsync<T>(DbConnection, spName, spInput, commandType: CommandType.StoredProcedure);
            return data.ToList();
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task CallProcedureAsync<TInput, TOutput>(
        string storedProcedure,
        TInput inputObject,
        TOutput outputObject,
        string connectionId = "Default",
        params Expression<Func<TOutput, object>>[] outputExpressions
        )
    {
        using var connection = DbConnection;

        var dynamicParameters = GetDynamicParameters<TInput, TOutput>(inputObject, outputObject, outputExpressions);

        await connection.ExecuteAsync(storedProcedure, dynamicParameters,
            commandType: CommandType.StoredProcedure);
    }



    public async Task<bool> ExecuteListData<T>(List<T> listData, string sQuery)
    {
        IDbConnection IDBConn = DbConnection;
        string sWhere = string.Empty, sReturnMsg = "Success", status;

        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {

                if (listData.Count > 0)
                {
                    IDBConn.Execute(sQuery, listData, trans);

                }

                if (listData.Count == 0 || listData.Count == 0)
                    trans.Rollback();
                else
                    trans.Commit();
                return true;

            }

            //ProcessAttendance
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return false;
        }
        finally { IDBConn.Close(); }
    }



}
