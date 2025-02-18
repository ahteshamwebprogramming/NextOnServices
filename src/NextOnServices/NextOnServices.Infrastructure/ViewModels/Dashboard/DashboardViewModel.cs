using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Dashboard;

public class DashboardViewModel
{
    public List<ProjectTableViewModel> ListOfProjects { get; set; }
    public List<UserDTO> Managers { get; set; }
    public DashboardProjectCountSummaryViewModel ProjectCountSummary { get; set; }

    public UserDTO CurrentUser { get; set; }

}
