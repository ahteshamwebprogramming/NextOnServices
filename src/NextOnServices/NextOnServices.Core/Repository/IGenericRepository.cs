using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using NextOnServices.Core.Helper;
using Microsoft.EntityFrameworkCore.Query;

namespace NextOnServices.Core.Repository;

public interface IGenericRepository<T> where T : class
{
    void AddAsync(T entity);
    // dynamic AddAsync(T entity, string propertyName);
    void AddRangeAsync(T entity);
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    //  Task<IEnumerable<T>> GetAll();
    Task<T> GetByIdAsync(int id);
    Task<T> GetByIdAsync(Guid id);
    Task<T> GetByIdWithChildrenAsync(int id);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    void UpdateRange(IEnumerable<T> entities);
    void Update(T entity);
    bool Exists(Expression<Func<T, bool>> predicate);
    T? FindFirstByExpression(Expression<Func<T, bool>> predicate);

    List<T>? FindAllByExpression(Expression<Func<T, bool>> predicate);

    Task<IList<T>> GetAll(
           Expression<Func<T, bool>> expression = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);
    public Task<IPagedList<T>> GetAll(RequestParams requestParams = null,
        Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>,
            IOrderedQueryable<T>>? orderBy = null);
    Task<IPagedList<T>> GetPagedList(
        RequestParams requestParams,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
        );
    Task<IPagedList<T>> GetPagedListWithExpression(
        RequestParams requestParams, Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
        );
    // public Task<IPagedList<T>> GetIncludesPageList(RequestParams requestParams, Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[]? children);
    Task<int> GetDataCount();
    bool UpdateDbEntry(T entity, string properties);

   

    //bool UpdateDbEntryAsync(T entity, params Expression<Func<T, object>>[] properties);
    // Task<List<T>> ExecuteRawQuery(FormattableString sQuery);
    // Task<List<T>> ExecuteRawQuery(string sQuery, params string[] parameters);
}
