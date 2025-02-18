using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Masters;

public class ProfileInfoSurveyDTO
{
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
    public string? EncryptedProfileInfoSurveyId { get; set; }
    public string? EncryptedProfileInfoCategoryId { get; set; }
}
