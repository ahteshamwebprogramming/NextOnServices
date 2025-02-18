using NextOnServices.Core.Entities;
using NextOnServices.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Core.Repository;

public interface IGProjectRepository : IGenericRepository<Project>
{

    Task<PagedListReturnValues<IEnumerable<Project>>> ProjectsListPaged(PagedListParams pagedListParams);
}
