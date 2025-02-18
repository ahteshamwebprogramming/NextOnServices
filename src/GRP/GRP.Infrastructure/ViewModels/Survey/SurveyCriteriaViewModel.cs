using GRP.Infrastructure.Models.Masters;
using GRP.Infrastructure.Models.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Survey;

public class SurveyCriteriaViewModel
{
    public List<ProfileInfoSurveyDTO>? profileInfoSurveys { get; set; }
    public int? SurveyId { get; set; }
    public int? ProfileInfoSurveyId { get; set; }

    public List<QuestionTypeSelectFrameworkDTO>? QuestionTypeSelectFrameworkList { get; set; }
    public SurveyCriteriaDTO? surveyCriteriaDTO { get; set; }
}
