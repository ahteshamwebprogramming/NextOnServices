using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextOnServices.Services.DBContext;

namespace NextOnServices.Services;

public class ProjectsUrlRepository : DapperGenericRepository<ProjectsUrl>, IProjectsUrlRepository
{
    public ProjectsUrlRepository(DapperDBSetting dbSetting) : base(dbSetting)
    {

    }
}
