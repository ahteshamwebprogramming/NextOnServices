using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Questionnaire;
using System.Collections.Generic;

namespace NextOnServices.Infrastructure.ViewModels.ProjectQuestionsMapping;

public class ProjectQuestionsMappingViewModel
{
    public ProjectDTO? Project { get; set; }
    public List<CountryMasterDTO>? Countries { get; set; }
    public List<QuestionsMasterDTO>? Questions { get; set; }
    public ProjectQuestionsMappingDTO? ProjectQuestionsMapping { get; set; }
    public List<ProjectQuestionsMappingDTO>? ProjectQuestionsMappingList { get; set; }
}

