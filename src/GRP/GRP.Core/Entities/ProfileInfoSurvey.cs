using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Entities;
[Dapper.Contrib.Extensions.Table("ProfileInfoSurvey")]
public class ProfileInfoSurvey
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProfileInfoSurveyId { get; set; }

    public int ProfileInfoCategoryId { get; set; }

    public string? QuestionLabel { get; set; }

    public int? QuestionType { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
    public int SNo { get; set; }

    public int? QuestionCode { get; set; }

    public string? QuestionName { get; set; }
    public string? CulturalNotes { get; set; }
}
