using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Masters;

public class StatusMasterDTO
{
    public int Id { get; set; }

    public string? Pstatus { get; set; }

    public int? Pvalue { get; set; }
}
