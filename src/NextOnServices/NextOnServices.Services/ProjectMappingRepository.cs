using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class ProjectMappingRepository : DapperGenericRepository<ProjectMapping>, IProjectMappingRepository
{
    public ProjectMappingRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {

    }
}
