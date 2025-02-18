using System;
using System.Collections.Generic;

namespace NextOnServices.Core.Entities;

public partial class QuestionOption
{
    public int Id { get; set; }

    public int? Qid { get; set; }

    public int? OptionNumber { get; set; }

    public string? OptionLabel { get; set; }

    public string? OptionCode { get; set; }

    public bool? IsChecked { get; set; }

    public DateTime? CrDate { get; set; }
}
