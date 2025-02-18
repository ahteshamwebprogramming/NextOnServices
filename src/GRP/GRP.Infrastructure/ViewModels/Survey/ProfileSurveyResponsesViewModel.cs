using GRP.Infrastructure.Models.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Survey;

public class ProfileSurveyResponsesViewModel
{
    public List<ProfileSurveyResponseDTO>? profileSurveyResponses { get; set; }
    public bool Completed { get; set; }
}
