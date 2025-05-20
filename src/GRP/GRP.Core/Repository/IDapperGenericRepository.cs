using Dapper;
using System.Data;

using System.Linq.Expressions;
using Z.Dapper.Plus;

namespace GRP.Core.Repository;

public interface IDapperRepository<T> where T : class
{
    Task<List<T>> FindAllAsync();
    Task<T> FindByIdAsync(int id);
    object UpdateFields<TS>(T param, IDbConnection connection, IDbTransaction transaction = null, int? commandTimeOut = null);

    Task<List<T>> GetTableData<T>(string sQuery, object parameters = null);
    Task<List<T>> GetTableData<T>(IDbConnection connection, IDbTransaction trns, string sWhere = "", string sOrderBy = "");
    Task<T> GetEntityData<T>(string sQuery, object parameters = null);
    Task<List<T>> GetTableData<T>(string sQuery, IDbConnection connection, IDbTransaction trans = null);
    Task<List<T>> GetTableData<T>(string sQuery);
    Task<List<T>> GetTableDataExec<T>(string sQuery);
    Task<int> EexecuteAddAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<bool> EexecuteUpdateAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<bool> EexecuteDeleteAsync(int id, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<int> AddAsync(T entity);
    Task<DapperPlusActionSet<T>> AddRangeAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter);
    Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter, Expression<Func<T, bool>> orderBy);
    Task<T> GetFilter(Expression<Func<T, bool>> filter);
    Task<bool> Exists(Expression<Func<T, bool>> filter);
    Task<List<T>> GetDynamicQuery(string query, object dynamicParameters);
    Task<bool> IsExists(string query, object dynamicParameters);
    Task<List<T>> GetQueryAll(string query);
    Task<int> GetStoredProcedure(string storedProcedure, DynamicParameters dynamicParameters);
    Task<List<T>> GetAllPagedAsync(int limit, int offset, string sWhere = "", string sOrderBy = "");

    Task<List<T>> GetSPData<T>(IDbConnection connection, IDbTransaction trans = null, string spName = "", DynamicParameters spInput = null);

    Task<List<T>> GetSPData(string spName = "", DynamicParameters spInput = null);

    void CallProcedure<TInput, TOutput>(
       string storedProcedure,
       TInput inputObject,
       TOutput outputObject,
       string connectionId,
       params Expression<Func<TOutput, object>>[] outputExpressions
       );

    Task CallProcedureAsync<TInput, TOutput>(
        string storedProcedure,
        TInput inputObject,
        TOutput outputObject,
        string connectionId = "Default",
        params Expression<Func<TOutput, object>>[] outputExpressions
        );

    Task<bool> ExecuteListData<T>(List<T> listData, string sQuery);

    Task<bool> RunSQLCommand(string sQuery);


}
