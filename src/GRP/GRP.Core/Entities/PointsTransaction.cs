using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Entities;

[Dapper.Contrib.Extensions.Table("PointsTransaction")]
public class PointsTransaction
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PointsTransactionId { get; set; }

    public int UserId { get; set; }

    public double? EarnedPoints { get; set; }

    public double? RedeemPoints { get; set; }

    public double BalancePoints { get; set; }
}
