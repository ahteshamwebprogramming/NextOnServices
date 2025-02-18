using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class StatusMaster
{
    public int Id { get; set; }

    public string? Pstatus { get; set; }

    public int? Pvalue { get; set; }
}
