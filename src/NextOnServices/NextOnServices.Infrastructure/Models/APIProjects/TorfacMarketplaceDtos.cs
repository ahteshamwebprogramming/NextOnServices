using System.ComponentModel.DataAnnotations;

namespace NextOnServices.Infrastructure.Models.APIProjects;

public class TorfacMarketplaceSettingDTO
{
    public int TorfacMarketplaceSettingId { get; set; }

    [Required]
    [Url]
    public string? SurveysUrl { get; set; }

    public string? SecretKey { get; set; }

    public int? DefaultClientId { get; set; }

    public string? RespondentIdUrlParts { get; set; }

    public string? RespondentPanelistIdUrlParts { get; set; }

    public List<int> DefaultSupplierIds { get; set; } = new();

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}

public class TorfacMarketplaceSurveyFetchResultDTO
{
    public bool Result { get; set; }

    public string? Message { get; set; }

    public string? SourceUrl { get; set; }

    public int? StatusCode { get; set; }

    public string? ContentType { get; set; }

    public int? SurveyCount { get; set; }

    public string? CollectionPath { get; set; }

    public string? RawResponse { get; set; }

    public bool ResponseTruncated { get; set; }

    public List<string> Columns { get; set; } = new();

    public List<Dictionary<string, string?>> Rows { get; set; } = new();
}

public class TorfacMarketplaceQuotaFetchResultDTO
{
    public bool Result { get; set; }

    public string? Message { get; set; }

    public string? SourceUrl { get; set; }

    public int? StatusCode { get; set; }

    public string? ContentType { get; set; }

    public int? QuotaCount { get; set; }

    public string? RawResponse { get; set; }

    public bool ResponseTruncated { get; set; }

    public string? QuestionLookupSourceUrl { get; set; }

    public int? QuestionLookupStatusCode { get; set; }

    public string? QuestionLookupRawResponse { get; set; }

    public bool QuestionLookupResponseTruncated { get; set; }

    public List<Dictionary<string, string?>> Rows { get; set; } = new();
}
