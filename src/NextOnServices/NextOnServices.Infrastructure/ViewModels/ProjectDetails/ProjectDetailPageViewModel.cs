using NextOnServices.Infrastructure.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectDetails;

public class ProjectDetailPageViewModel
{
    public ProjectDTO Project { get; set; }
    public List<ProjectDetail> ProjectDetailList { get; set; }
    public List<SurveySpecs> SurveySpecsList { get; set; }

    public List<Table3> Table3List { get; set; }
    public List<CountryDetails> CountryDetailsList { get; set; }
    public List<Table5> Table5List { get; set; }
    public List<SupplierDetails> SupplierDetailsList { get; set; }
    public List<SurveyLinks> SurveyLinksList { get; set; }
    public FractionComplete0 FractionComplete0 { get; set; }
    public FractionComplete1 FractionComplete1 { get; set; }
    public FractionComplete3 FractionComplete3 { get; set; }
}
