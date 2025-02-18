using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Survey;

public class PointsHistoryDTO
{
    public int PointsHistoryId { get; set; }

    public double Points { get; set; }

    public string TransType { get; set; } = null!;

    public string Source { get; set; } = null!;

    public int? SourceRefId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public int UserId { get; set; }
}
