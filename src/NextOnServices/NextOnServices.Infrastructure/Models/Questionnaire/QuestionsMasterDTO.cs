using System;
using System.Collections.Generic;

namespace NextOnServices.Infrastructure.Models.Questionnaire;

public class QuestionsMasterDTO
{
    public int Id { get; set; }
    public string? QuestionId { get; set; }
    public string? QuestionLabel { get; set; }
    public int? QuestionType { get; set; }
    public DateTime? CreationDate { get; set; }
    public List<QuestionOptionDTO>? Options { get; set; }
}

public class QuestionOptionDTO
{
    public int Id { get; set; }
    public int? Qid { get; set; }
    public int? OptionNumber { get; set; }
    public string? OptionLabel { get; set; }
    public string? OptionCode { get; set; }
    public bool? IsChecked { get; set; }
    public DateTime? CrDate { get; set; }
}

