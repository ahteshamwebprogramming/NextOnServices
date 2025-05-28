using Dapper;
using System.Data;
using System.Linq.Expressions;
using NextOnServices.Infrastructure.ViewModels.ProjectDetails;
namespace NextOnServices.Core.Repository;

public interface IDapperRepository<T> where T : class
{
    Task<List<T>> FindAllAsync();
    Task<T> FindByIdAsync(int id);
    object UpdateFields<TS>(T param, IDbConnection connection, IDbTransaction transaction = null, int? commandTimeOut = null);
    Task<List<T>> GetTableData<T>(IDbConnection connection, IDbTransaction trns, string sWhere = "", string sOrderBy = "");
    Task<List<T>> GetTableData<T>(string sQuery, IDbConnection connection, IDbTransaction trans = null);
    Task<List<T>> GetTableData<T>(string sQuery, object parameters = null);
    Task<bool> IsExists(string query, object dynamicParameters = null);
    Task<bool> Exists(Expression<Func<T, bool>> filter);
    Task<T> GetEntityData<T>(string sQuery, object parameters = null);
    Task<int> GetEntityCount(string sQuery, object parameters = null);
    Task<bool> ExecuteQueryAsync(string sQuery, object parameters = null);
    Task<int> EexecuteAddAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<bool> EexecuteUpdateAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<bool> EexecuteDeleteAsync(int id, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null);
    Task<int> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter);
    Task<T> GetFilter(Expression<Func<T, bool>> filter);
    Task<List<T>> GetQueryAll(string query);
    Task<List<T>> GetQueryAll<T>(string query);
    Task<int> GetStoredProcedure(string storedProcedure, DynamicParameters dynamicParameters);
    Task<List<T>> GetAllPagedAsync(int limit, int offset, string sWhere = "", string sOrderBy = "");

    Task<ProjectDetailPageViewModel> GetProjectDetailPageMultipleAsync(string query);
    Task<T> GetOutputFromStoredProcedure<T>(string storedProcedure, DynamicParameters dynamicParameters, string outputParamName);
}
