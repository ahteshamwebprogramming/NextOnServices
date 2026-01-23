using System;
using System.Collections.Generic;

namespace NextOnServices.Infrastructure.Models.Projects;

public class ProjectQuestionsMappingDTO
{
    public int Id { get; set; }
    public int? Cid { get; set; }
    public int? Pid { get; set; }
    public int? PreviousButton { get; set; }
    public int? QuestionQid { get; set; }
    public int? Logo { get; set; }
    public string? Qids { get; set; }
    public DateTime? Crdate { get; set; }
    public string? encProjectId { get; set; }
    public List<int>? SelectedQuestionIds { get; set; }
}

