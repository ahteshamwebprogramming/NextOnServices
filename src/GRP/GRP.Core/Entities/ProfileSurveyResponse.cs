using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Entities;

public class ProfileSurveyResponse
{
    public int ProfileSurveyResponsesId { get; set; }

    public int UserId { get; set; }

    public int? QuestionCode { get; set; }
    public int? ProfileSurveyCategoryId { get; set; }
    public int QuestionId { get; set; }

    public string? AnswerId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }
}
