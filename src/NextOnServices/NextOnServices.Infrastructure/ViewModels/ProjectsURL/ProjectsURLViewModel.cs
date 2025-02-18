using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectsURL;

public class ProjectsURLViewModel
{
    public List<CountryMasterDTO>? Countries { get; set; }
    public List<ProjectsUrlDTO>? ProjectsURLs { get; set; }
    public ProjectsUrlDTO? ProjectsURL { get; set; }
    public ProjectDTO? Project { get; set; }
    public List<ProjectURLWithChild>? ProjectURLWithChildList { get; set; }
}
