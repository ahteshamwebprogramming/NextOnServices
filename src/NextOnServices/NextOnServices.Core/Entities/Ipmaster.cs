using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class Ipmaster
{
    public int Id { get; set; }

    public string? Ip { get; set; }

    public string? CountryCode { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? CreatedDate { get; set; }
}
