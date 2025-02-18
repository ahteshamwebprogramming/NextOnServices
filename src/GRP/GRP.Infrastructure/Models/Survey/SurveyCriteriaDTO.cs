using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Survey;

public class SurveyCriteriaDTO
{
    public int SurveyCriteriaId { get; set; }

    public int SurveyId { get; set; }

    public int ProfileSurveyId { get; set; }

    public string? Options { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
