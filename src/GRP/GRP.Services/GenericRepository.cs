using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using GRP.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GRP.Services.DBContext;
using X.PagedList;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Data.Entity.Core;

using System.Data.Entity.Core.Objects;
using GRP.Core.Helper;

namespace GRP.Services;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly GRPDbContext _dataContext;
    protected DbContext DbContext { get; set; }
    protected DbSet<T> DbSet { get; set; }
    public GenericRepository(GRPDbContext dataContext)
    {
        DbContext = _dataContext = dataContext;
        // _dataContext.Database.EnsureCreated();
    }
    #region IGenericRepository<T> Members
    //public void AddAsync(T entity) => _dataContext.Set<T>().AddAsync(entity);

    public T Create(T entity)
    {
        _dataContext.Add(entity);
        _dataContext.SaveChanges();
        return entity;
    }

    public void AddAsync(T entity)
    {
        try
        {
            _dataContext.Set<T>().AddAsync(entity);
        }
        catch (Exception e)
        {
            int a = 1;
        }

        //_dataContext.Set<T>().AsNoTracking();
    }

    //public dynamic AddAsync(T entity, string propertyName)
    //{

    //    _dataContext.Set<T>().AddAsync(entity);


    //    //return _dataContext.Set<T>().LastOrDefault();


    //}

    private static EntityKey GetEntityKey<T>(ObjectSet<T> objectSet, object keyValue) where T : class
    {
        var entitySetName = objectSet.Context.DefaultContainerName + "." + objectSet.EntitySet.Name;
        var keyPropertyName = objectSet.EntitySet.ElementType.KeyMembers[0].ToString();
        var entityKey = new EntityKey(entitySetName, new[] { new EntityKeyMember(keyPropertyName, keyValue) });



        return entityKey;
    }
    public void AddRangeAsync(T entity) => _dataContext.Set<T>().AddRangeAsync(entity);
    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _dataContext.Set<T>().Where(predicate).AsNoTracking();
    }
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dataContext.Set<T>().ToListAsync();
    }
    public async Task<T> GetByIdAsync(int id)
    {
        return await _dataContext.Set<T>().FindAsync(id);
    }
    public async Task<T> GetByIdAsync(long id)
    {
        return await _dataContext.Set<T>().FindAsync(id);
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dataContext.Set<T>().FindAsync(id);
    }
    public void Remove(T entity) => _dataContext.Set<T>().Remove(entity);
    public void RemoveRange(IEnumerable<T> entities) => _dataContext.Set<T>().RemoveRange(entities);

    public void UpdateRange(IEnumerable<T> entities) => _dataContext.Set<T>().UpdateRange(entities);

    public void Update(T entity) => _dataContext.Set<T>().Update(entity);
    public bool Exists(Expression<Func<T, bool>> predicate)
    {
        if (_dataContext.Set<T>().AsNoTracking().Where(predicate).ToList().Count != 0)
        {
            return true;
        }
        return false;
    }
    public List<T>? FindAllByExpression(Expression<Func<T, bool>> predicate)
    {
        return _dataContext.Set<T>().AsNoTracking().Where(predicate).ToList();
    }

    public T? FindFirstByExpression(Expression<Func<T, bool>> predicate)
    {
        return _dataContext.Set<T>().AsNoTracking().Where(predicate).ToList().FirstOrDefault();
    }

    public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
    {

        IQueryable<T> query = _dataContext.Set<T>();
        if (expression != null)
        {
            query = query.Where(expression);
        }
        Func<IQueryable<T>, IQueryable<T>> include = GetNavigations<T>();
        if (include != null)
        {
            query = include(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.AsNoTracking().ToListAsync();

    }




    public async Task<IPagedList<T>> GetPagedListWithExpression(Core.Helper.RequestParams requestParams, Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
    {
        IQueryable<T> query = _dataContext.Set<T>();
        Func<IQueryable<T>, IQueryable<T>> includes = GetNavigations<T>();
        if (expression != null)
        {
            query = query.Where(expression);
        }
        if (includes != null)
        {
            query = includes(query);
        }

        if (requestParams.PageNumber == 0 || requestParams.PageSize == 0)
        {
            requestParams.PageSize = await GetDataCount();
            requestParams.PageNumber = 1;
        }

        return await query.AsNoTracking()
            .ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
    }

    public async Task<IPagedList<T>> GetPagedList(Core.Helper.RequestParams requestParams, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
    {
        IQueryable<T> query = _dataContext.Set<T>();
        Func<IQueryable<T>, IQueryable<T>> includes = GetNavigations<T>();
        if (includes != null)
        {
            query = includes(query);
        }

        if (requestParams.PageNumber == 0 || requestParams.PageSize == 0)
        {
            requestParams.PageSize = await GetDataCount();
            requestParams.PageNumber = 1;
        }

        return await query.AsNoTracking()
            .ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
    }

    public async Task<IPagedList<T>> GetAll(Core.Helper.RequestParams? requestParams, Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {

        IQueryable<T> query = _dataContext.Set<T>();

        if (expression != null)
        {
            query = query.Where(expression);
        }

        Func<IQueryable<T>, IQueryable<T>> include = GetNavigations<T>();
        if (include != null)
            query = include(query);


        if (orderBy != null)
            query = orderBy(query);

        if (requestParams == null)
        {
            requestParams = new Core.Helper.RequestParams();
            requestParams.PageNumber = 0;
            requestParams.PageSize = 0;
        }
        if (requestParams.PageNumber == 0 || requestParams.PageSize == 0)
        {
            requestParams.PageSize = await GetDataCount();
            requestParams.PageNumber = 1;
        }

        return await query.AsNoTracking().ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
    }
    public Task<int> GetDataCount()
    {
        return _dataContext.Set<T>().CountAsync();
    }

    public static Func<IQueryable<T>, IQueryable<T>> GetNavigations<T>() where T : class
    {
        var type = typeof(T);
        var navigationProperties = new List<string>();

        //get navigation properties
        GetNavigationProperties(type, type, string.Empty, navigationProperties);

        Func<IQueryable<T>, IQueryable<T>> includes = (query =>
        {
            return navigationProperties.Aggregate(query, (current, inc) => current.Include(inc));
        });

        return includes;
    }

    private static void GetNavigationProperties(Type baseType, Type type, string parentPropertyName, IList<string> accumulator)
    {
        //get navigation properties
        var properties = type.GetProperties();
        var navigationPropertyInfoList = properties.Where(prop => prop.IsDefined(typeof(NavigationPropertyAttribute)));

        foreach (PropertyInfo prop in navigationPropertyInfoList)
        {
            var propertyType = prop.PropertyType;
            var elementType = propertyType.GetTypeInfo().IsGenericType ? propertyType.GetGenericArguments()[0] : propertyType;

            //Prepare navigation property in {parentPropertyName}.{propertyName} format and push into accumulator
            var properyName = string.Format("{0}{1}{2}", parentPropertyName, string.IsNullOrEmpty(parentPropertyName) ? string.Empty : ".", prop.Name);
            accumulator.Add(properyName);

            //Skip recursion of propert has JsonIgnore attribute or current property type is the same as baseType
            var isJsonIgnored = prop.IsDefined(typeof(JsonIgnoreAttribute));
            if (!isJsonIgnored && elementType != baseType)
            {
                GetNavigationProperties(baseType, elementType, properyName, accumulator);
            }
        }
    }

    public async Task<T> GetByIdWithChildrenAsync(int id)
    {
        var entity = await _dataContext.FindAsync<T>(id);

        var type = typeof(T);
        var navigationProperties = new List<string>();

        //get navigation properties
        GetNavigationProperties(type, type, string.Empty, navigationProperties);

        foreach (var navigation in _dataContext.Entry(entity).Navigations)
        {

            if (navigationProperties.Contains(navigation.Metadata.PropertyInfo.Name))
            {
                await navigation.LoadAsync();
                //await LoadRelativeChildrenAsync(navigation.Metadata.ClrType);
            }

        }
        return entity;
    }
    public bool UpdateDbEntry(T entity, string properties)
    {
        this._dataContext.Set<T>().Attach(entity);
        var entry = this._dataContext.Entry(entity);
        // entry.State = EntityState.Modified;
        foreach (var property in properties.Split(",").ToList())
        {
            entry.Property(property).IsModified = true;

        }
        this._dataContext.SaveChanges();
        return true;
    }
    public IEnumerable<T> GetWithRawSql(string query, params object[] parameters)
    {
        return this._dataContext.Set<T>().FromSqlRaw(query, parameters).ToList();
    }

    //private async Task LoadRelativeChildrenAsync(Type entityTypeValue)
    //{
    //    var setMethod = typeof(Microsoft.EntityFrameworkCore.DbContext).GetMethod("Set");

    //    var entityType = _dataContext.Model.FindEntityType(entityTypeValue);
    //    foreach (var navigation in entityType.GetNavigations())
    //    {
    //        await ((IQueryable)setMethod.MakeGenericMethod(navigation.ClrType)
    //            .Invoke(_dataContext, null))
    //            .OfType<object>()
    //            .LoadAsync();
    //    }
    //}
    #endregion
}
