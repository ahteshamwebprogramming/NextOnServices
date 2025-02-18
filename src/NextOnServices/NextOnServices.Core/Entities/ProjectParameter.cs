using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class ProjectParameter
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public string? ParameterKey { get; set; }

    public string? ParameterValue { get; set; }

    public int? IncludeInSupplier { get; set; }

    public int? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
