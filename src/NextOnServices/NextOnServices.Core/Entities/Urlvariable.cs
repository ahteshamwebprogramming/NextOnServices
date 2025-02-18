using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class Urlvariable
{
    public int Id { get; set; }

    public int? Rpid { get; set; }

    public string? VariableName { get; set; }

    public string? VariableValue { get; set; }

    public int? Status { get; set; }
}
