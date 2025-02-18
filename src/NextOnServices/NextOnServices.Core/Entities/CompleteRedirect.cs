using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class CompleteRedirect
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public int? Status { get; set; }

    public int? Enable { get; set; }

    public int? IsActive { get; set; }
}
