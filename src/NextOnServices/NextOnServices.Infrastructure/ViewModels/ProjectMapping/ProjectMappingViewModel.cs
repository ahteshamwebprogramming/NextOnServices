using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.Infrastructure.ViewModels.ProjectDetails;
using NextOnServices.Infrastructure.ViewModels.ProjectsURL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectMapping;

public class ProjectMappingViewModel
{
    public List<CountryMasterDTO>? Countries { get; set; }
    public List<SupplierDTO>? Suppliers { get; set; }
    public List<ProjectMappingDTO>? ProjectMappingList { get; set; }
    public ProjectMappingDTO? ProjectMapping { get; set; }
    public ProjectDTO? Project { get; set; }
    public List<ProjectMappingWithChild>? ProjectMappingWithChildList { get; set; }
    public ProjectMappingWithChild? ProjectMappingWithChild { get; set; }
    
}
