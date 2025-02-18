using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class ProjectOptionMapping
{
    public int Id { get; set; }

    public int? ProjectQuestionsMappingId { get; set; }

    public string? OptionsId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? Pid { get; set; }

    public int? Cid { get; set; }

    public int? Qid { get; set; }
}
