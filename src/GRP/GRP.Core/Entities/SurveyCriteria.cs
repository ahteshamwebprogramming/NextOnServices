using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Entities;

[Dapper.Contrib.Extensions.Table("SurveyCriteria")]
public class SurveyCriteria
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
