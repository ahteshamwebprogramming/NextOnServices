using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class ProjectQuestionTerminate
{
    public int Id { get; set; }

    public int? Cid { get; set; }

    public int? Pid { get; set; }

    public int? Qid { get; set; }

    public int? OptionId { get; set; }

    public string? Logic { get; set; }

    public int? QuotaCount { get; set; }

    public int? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
}
