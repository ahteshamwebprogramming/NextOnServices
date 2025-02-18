using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Survey;

public class EarnedPointHistoryViewModel
{
    public int? PointsHistoryId { get; set; }
    public double Points { get; set; }
    public string? TransType { get; set; }
    public string? Source { get; set; }
    public string? SourceRef { get; set; }    
}
