using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Survey;

public class SurveyDTO
{
    public int SurveyId { get; set; }
    public string? encSurveyId { get; set; }

    public string? SurveyName { get; set; }

    public string? SurveyDescription { get; set; }

    public string? ImageUrl { get; set; }

    public string? SurveyUrl { get; set; }

    public int? Duration { get; set; }

    public string? ClientDetail { get; set; }

    public int? MaxCount { get; set; }

    public bool IsActive { get; set; }

    public string? City { get; set; }

    public string? Age { get; set; }

    public string? Gender { get; set; }

    public int? SurveyPoint { get; set; }

    public int? ClickCount { get; set; }

    public string? Criteria { get; set; }

    public string? SurveyIdHost { get; set; }

    public string? CompleteURL { get; set; }

    public string? TerminateURL { get; set; }

    public string? QuotafullURL { get; set; }

    public string? Country { get; set; }

    public string? AgeFrom { get; set; }

    public string? AgeTo { get; set; }
}
