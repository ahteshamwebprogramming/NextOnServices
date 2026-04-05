using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Supplier;

namespace NextOnServices.VT.Infrastructure.ViewModels.Project;

public class ZampliaDashboardVM
{
    public int ActiveSettings { get; set; }
    public int ActiveSurveys { get; set; }
    public int MappedSurveys { get; set; }
    public int TotalLogs { get; set; }
    public int SuccessfulApiCalls { get; set; }
    public int FailedApiCalls { get; set; }
    public int TotalAttempts { get; set; }
    public DateTime? LastSyncTime { get; set; }
    public DateTime? LatestReconciliationRunTime { get; set; }
    public int LatestReconciliationMismatchCount { get; set; }
}

public class ZampliaViewModel
{
    public ZampliaDashboardVM Dashboard { get; set; } = new();
    public ZampliaSettingDTO Setting { get; set; } = new();
    public List<ZampliaSurveyDTO> Surveys { get; set; } = new();
    public ZampliaSurveyDTO Survey { get; set; } = new();
    public List<ZampliaSurveyQualificationDTO> Qualifications { get; set; } = new();
    public List<ZampliaSurveyQuotaDTO> Quotas { get; set; } = new();
    public ZampliaProjectMapDTO ProjectMap { get; set; } = new();
    public ZampliaEntryLinkDTO EntryLink { get; set; } = new();
    public List<ZampliaRespondentAttemptDTO> Attempts { get; set; } = new();
    public List<ZampliaSyncLogDTO> Logs { get; set; } = new();
    public List<ZampliaReconciliationRunDTO> ReconciliationRuns { get; set; } = new();
    public List<ZampliaReconciliationItemDTO> ReconciliationItems { get; set; } = new();
    public ZampliaReconciliationRunRequest ReconciliationFilter { get; set; } = new();
    public List<ClientDTO> Clients { get; set; } = new();
    public List<CountryMasterDTO> Countries { get; set; } = new();
    public List<SupplierDTO> Suppliers { get; set; } = new();
    public string? SessionEventsRawJson { get; set; }
}
