using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Survey;

public class PointsTransactionDTO
{
    public int PointsTransactionId { get; set; }

    public int UserId { get; set; }

    public double? EarnedPoints { get; set; }

    public double? RedeemPoints { get; set; }

    public double BalancePoints { get; set; }
}
