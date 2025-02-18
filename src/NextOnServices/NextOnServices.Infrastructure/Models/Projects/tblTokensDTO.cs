using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Projects;

public class tblTokensDTO
{
    public int ID { get; set; }
    public int? ProjectURLId { get; set; }
    public string? Token { get; set; }
    public int? IsUsed { get; set; }
    public int? TStatus { get; set; }
    public string? SID { get; set; }
    public string? UID { get; set; }
}
