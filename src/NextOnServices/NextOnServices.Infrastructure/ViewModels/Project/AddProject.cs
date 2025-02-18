using NextOnServices.Infrastructure.Models.Account;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Project;

public class AddProject
{
    public ProjectDTO Project { get; set; }
    public List<ClientDTO> ClientsList { get; set; }
    public List<UserDTO> UsersList { get; set; }
    public List<CountryMasterDTO> CountriesList { get; set; }
    public List<StatusMasterDTO> StatusList { get; set; }
}
