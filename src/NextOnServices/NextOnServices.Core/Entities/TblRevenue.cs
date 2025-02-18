using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class TblRevenue
{
    public int Id { get; set; }

    public string? Sid { get; set; }

    public int? Projectid { get; set; }

    public int? Supplierid { get; set; }

    public int? Clientid { get; set; }

    public int? Countryid { get; set; }

    public int? Cpi { get; set; }

    public int? Completes { get; set; }

    public int? Revenue { get; set; }
}
