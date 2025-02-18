using GRP.Infrastructure.Models.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Dashboard;

public class DashboardViewModel
{
    public List<SurveyDTO>? surveys { get; set; }

    public float? ProfileCompletePercent { get; set; }
    public PointsTransactionDTO? PointsTransaction { get; set; }
    public int? PointsPending{ get; set; }
}
