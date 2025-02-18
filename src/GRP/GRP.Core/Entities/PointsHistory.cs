using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Entities;

[Dapper.Contrib.Extensions.Table("PointsHistory")]
public class PointsHistory
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
