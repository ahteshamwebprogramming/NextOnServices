using NextOnServices.Infrastructure.Models.Questionnaire;

namespace NextOnServices.Infrastructure.ViewModels.Questionnaire;

public class AddQuestionnaire
{
    public QuestionsMasterDTO Question { get; set; } = new QuestionsMasterDTO();
}

public class ListQuestionnaire
{
    public int Id { get; set; }
    public string? QuestionId { get; set; }
    public string? QuestionLabel { get; set; }
    public int? QuestionType { get; set; }
    public string? QuestionTypeName { get; set; }
    public DateTime? CreationDate { get; set; }
    public int OptionsCount { get; set; }
    public string? IdEnc { get; set; }
}

