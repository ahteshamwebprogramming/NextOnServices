using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Entities;
[Dapper.Contrib.Extensions.Table("SurveyRedirectDetails")]
public class SurveyRedirectDetails
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
