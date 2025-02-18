using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Masters;

public class CountryMasterDTO
{
    public int CountryId { get; set; }

    public string? Country { get; set; }

    public string? Alpha2 { get; set; }

    public string? Alpha3 { get; set; }

    public int? NumericCode { get; set; }

    public int? Ctype { get; set; }
}
