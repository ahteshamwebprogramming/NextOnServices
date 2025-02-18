using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class TblToken
{
    public int Id { get; set; }

    public int? ProjectUrlid { get; set; }

    public string? Token { get; set; }

    public int? IsUsed { get; set; }

    public int? Tstatus { get; set; }

    public string? Sid { get; set; }

    public string? Uid { get; set; }
}
