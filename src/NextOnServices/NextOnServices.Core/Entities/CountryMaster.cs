using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class CountryMaster
{
    public int CountryId { get; set; }

    public string? Country { get; set; }

    public string? Alpha2 { get; set; }

    public string? Alpha3 { get; set; }

    public int? NumericCode { get; set; }

    public int? Ctype { get; set; }
}
