using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Survey;

public class PendingPointListViewModel
{
    public string? SurveyName { get; set; }
    public string? SurveyIdHost { get; set; }
    public int? SurveyId { get; set; }
    public int? UserId { get; set; }
    public int? PointsHistoryId { get; set; }
    public double Points { get; set; }
    public string? UserName { get; set; }
    public string? Status { get; set; }
}
