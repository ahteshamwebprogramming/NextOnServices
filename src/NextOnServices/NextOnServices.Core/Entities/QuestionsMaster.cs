using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class QuestionsMaster
{
    public int Id { get; set; }

    public string? QuestionId { get; set; }

    public string? QuestionLabel { get; set; }

    public int? QuestionType { get; set; }

    public DateTime? CreationDate { get; set; }
}
