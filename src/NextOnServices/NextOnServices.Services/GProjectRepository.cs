using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NextOnServices.Services;

public class GProjectRepository : GenericRepository<Project>, IGProjectRepository
{
    private readonly NextOnServicesDbContext _dataContext;
    public GProjectRepository(NextOnServicesDbContext context) : base(context)
    {
        DbContext = _dataContext = context;
    }

    public async Task<PagedListReturnValues<IEnumerable<Project>>> ProjectsListPaged(PagedListParams pagedListParams)
    {
        int totalRecord = 0;
        int filterRecord = 0;

        var data = _dataContext.Set<Project>().AsQueryable<Project>();
        totalRecord = data.Count();
        if (!string.IsNullOrEmpty(pagedListParams.searchValue))
        {
            data = data.Where(x => x.Pid.ToLower().Contains(pagedListParams.searchValue.ToLower()) || x.Pname.ToLower().Contains(pagedListParams.searchValue.ToLower()));
        }
        filterRecord = data.Count();

        if (!string.IsNullOrEmpty(pagedListParams.sortColumn) && !string.IsNullOrEmpty(pagedListParams.sortColumnDirection))
        {
            Func<Project, string> orderingFunction = e => e.Pname;
            var dataOrdered = data.OrderBy(orderingFunction);
            var L = dataOrdered.Skip(pagedListParams.skip).Take(pagedListParams.pageSize);
            var List = L.ToList();

            PagedListReturnValues<IEnumerable<Project>> pagedListReturnValues = new PagedListReturnValues<IEnumerable<Project>>();
            pagedListReturnValues.data = List;
            pagedListReturnValues.filterRecord = filterRecord;
            pagedListReturnValues.totalRecord = totalRecord;
            return pagedListReturnValues;
        }
        else
        {
            var List = data.Skip(pagedListParams.skip).Take(pagedListParams.pageSize);
            PagedListReturnValues<IEnumerable<Project>> pagedListReturnValues = new PagedListReturnValues<IEnumerable<Project>>();
            pagedListReturnValues.data = List;
            pagedListReturnValues.filterRecord = filterRecord;
            pagedListReturnValues.totalRecord = totalRecord;
            return pagedListReturnValues;
        }
        //pagination

    }
}
