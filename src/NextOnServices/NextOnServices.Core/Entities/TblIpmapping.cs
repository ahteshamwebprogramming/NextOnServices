using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class TblIpmapping
{
    public int Id { get; set; }

    public int? ProUrlid { get; set; }

    public int? Countryid { get; set; }

    public int? Stat { get; set; }

    public int? Isactive { get; set; }

    public string? MappingMode { get; set; } // "Include" or "Exclude"
}
