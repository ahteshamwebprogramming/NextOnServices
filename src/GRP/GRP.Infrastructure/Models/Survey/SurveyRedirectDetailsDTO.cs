using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Survey;

public class SurveyRedirectDetailsDTO
{
    public int SurveyRedirectDetailsId { get; set; }

    public string? RespondentId { get; set; }

    public int? SurveyId { get; set; }

    public int? UserId { get; set; }

    public string? Status { get; set; }

    public bool? IsSent { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ActualStatus { get; set; }
}
