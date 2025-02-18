using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class Client
{
    [ExplicitKey]
    public int ClientId { get; set; }

    public string? Company { get; set; }

    public string? Cperson { get; set; }

    public string? Cnumber { get; set; }

    public string? Cemail { get; set; }

    public string? Country { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreationDate { get; set; }

    public int? Cstatus { get; set; }

    public int? IsActive { get; set; }
}
