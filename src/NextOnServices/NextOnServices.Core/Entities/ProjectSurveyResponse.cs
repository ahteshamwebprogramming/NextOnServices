using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class ProjectSurveyResponse
{
    public int Id { get; set; }

    public int? Pid { get; set; }

    public int? Qid { get; set; }

    public string? Uid { get; set; }

    public string? Qoptions { get; set; }

    public DateTime? Crdate { get; set; }
}
