using GRP.Infrastructure.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Profile;

public class ProfileSurveysViewModel
{
    public IEnumerable<ProfileInfoSurveyViewModel>? ProfileInfoSurveys { get; set; }
    public string? EncryptedProfileInfoCategoryId { get; set; }
    public int? ProfileInfoCategoryId { get; set; }

}

public class ProfileInfoSurveyViewModel
{
    public IEnumerable<ProfileInfoCategoryDTO>? Categories { get; set; }
    public ProfileInfoSurveyDTO? QuestionLabel { get; set; }
    public IEnumerable<QuestionTypeSelectFrameworkDTO>? AttributesSelect { get; set; }
    public IEnumerable<QuestionTypeMasterDTO>? QuestionTypeList { get; set; }

    public string? EncryptedProfileInfoSurveyId { get; set; }
    public string? EncryptedProfileInfoCategoryId { get; set; }
    public int? ProfileInfoCategoryId { get; set; }
    public string? RespondentResponse { get; set; }
}


