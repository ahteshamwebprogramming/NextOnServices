using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.VT.Infrastructure.ViewModels.Client;

public class ClientViewModel
{
    public ClientDTO? Client { get; set; }
    public List<ClientDTO>? Clients { get; set; }
    public List<ClientWithChild>? ClientWithChildren { get; set; }
    public ClientWithChild? ClientWithChild { get; set; }
    public List<CountryMasterDTO>? Countries { get; set; }
}
