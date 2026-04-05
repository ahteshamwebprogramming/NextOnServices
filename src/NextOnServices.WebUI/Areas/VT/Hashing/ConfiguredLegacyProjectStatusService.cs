using System.Globalization;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Helper;

namespace NextOnServices.WebUI.VT.Services;

public sealed class ConfiguredLegacyProjectStatusService : ILegacyProjectStatusService
{
    private readonly SurveyAPIController _surveyAPIController;
    private readonly IHashingSettingsService _hashingSettingsService;
    private readonly ILogger<ConfiguredLegacyProjectStatusService> _logger;

    public ConfiguredLegacyProjectStatusService(
        SurveyAPIController surveyAPIController,
        IHashingSettingsService hashingSettingsService,
        ILogger<ConfiguredLegacyProjectStatusService> logger)
    {
        _surveyAPIController = surveyAPIController;
        _hashingSettingsService = hashingSettingsService;
        _logger = logger;
    }

    public async Task<LegacyProjectStatusExecutionResult> ApplyAsync(
        string supplierProjectUid,
        string? rawStatus,
        int rc = 0,
        int? pid = null,
        bool px = false)
    {
        var normalizedUid = supplierProjectUid?.Trim() ?? string.Empty;
        var normalizedRawStatus = rawStatus?.Trim() ?? string.Empty;
        var normalizedStatus = await NormalizeStatusAsync(normalizedRawStatus);

        if (string.IsNullOrWhiteSpace(normalizedUid))
        {
            return new LegacyProjectStatusExecutionResult
            {
                SupplierProjectUid = string.Empty,
                RawStatus = normalizedRawStatus,
                NormalizedStatus = normalizedStatus,
                UpdateResponse = string.Empty
            };
        }

        string updateResponse;
        if (rc == 1 && pid.HasValue)
        {
            updateResponse = await _surveyAPIController.UpdateProjectDetails(
                "",
                normalizedRawStatus,
                normalizedStatus,
                pid.Value.ToString(CultureInfo.InvariantCulture),
                normalizedUid,
                1,
                3);
        }
        else
        {
            updateResponse = await _surveyAPIController.UpdateProjectDetails(
                "",
                normalizedRawStatus,
                normalizedStatus,
                "",
                normalizedUid,
                1,
                2);
        }

        string? redirectUrl = null;
        string? hashCode = null;

        if (string.Equals(updateResponse, "1", StringComparison.OrdinalIgnoreCase))
        {
            var shareResult = await ExecuteShareStatusAsync(normalizedUid, rc, pid ?? 0, px ? 1 : 0);
            redirectUrl = shareResult.RedirectUrl;
            hashCode = shareResult.HashCode;
        }

        return new LegacyProjectStatusExecutionResult
        {
            SupplierProjectUid = normalizedUid,
            RawStatus = normalizedRawStatus,
            NormalizedStatus = normalizedStatus,
            UpdateResponse = updateResponse,
            RedirectUrl = redirectUrl,
            HashCode = hashCode
        };
    }

    public async Task<string> NormalizeStatusAsync(string? rawStatus)
    {
        var normalizedStatus = rawStatus?.Trim().ToUpperInvariant() ?? string.Empty;
        var ds = await _surveyAPIController.GenericDataFetcher("ManageCompleteRedirects", new { @Code = "", @opt = 2, @Id = 0 });
        if (ds == null || !ds.Any())
        {
            return normalizedStatus switch
            {
                "TERMINATE" => "TERMINATE",
                "QUOTAFULL" => "QUOTAFULL",
                "SCREENED" => "SCREENED",
                _ => normalizedStatus
            };
        }

        string[] codes = new string[ds.Count];
        for (int i = 0; i < ds.Count; i++)
        {
            var row = (IDictionary<string, object>)ds[i];
            codes[i] = row.ContainsKey("Code") ? row["Code"]?.ToString() ?? string.Empty : string.Empty;
        }

        if (Array.IndexOf(codes, rawStatus) >= 0)
        {
            return "COMPLETE";
        }

        return normalizedStatus switch
        {
            "TERMINATE" => "TERMINATE",
            "QUOTAFULL" => "QUOTAFULL",
            "SCREENED" => "SCREENED",
            "OVERQUOTA" => "OVERQUOTA",
            "SEC_TERM" => "SEC_TERM",
            "F_ERROR" => "F_ERROR",
            "INCOMPLETE" => "INCOMPLETE",
            _ => "SEC_TERM"
        };
    }

    private async Task<LegacyProjectStatusShareResult> ExecuteShareStatusAsync(string supplierProjectUid, int rc, int pid, int px)
    {
        var result = new LegacyProjectStatusShareResult();

        try
        {
            var opt = rc == 0 ? 0 : 1;
            var trackingType = await _surveyAPIController.CheckForPixelTracking(supplierProjectUid, opt, pid);
            var rows = await _surveyAPIController.GenericDataFetcher("usp_ShareStatusOnRequest", new { @ID = supplierProjectUid, @opt = opt, @pid = pid });
            if (rows == null || !rows.Any())
            {
                return result;
            }

            var firstRow = rows.FirstOrDefault();
            var sid = firstRow?.SuplierID ?? string.Empty;
            var currentStatus = firstRow?.PStatus ?? string.Empty;
            var requestUrl = firstRow?.RequestUrl ?? string.Empty;
            var clientId = firstRow?.ClientID ?? string.Empty;

            var projectMapping = await _surveyAPIController.GetProjectMappingRecordBYSID(sid);
            var addHashing = projectMapping?.AddHashing ?? default;
            var parameterName = projectMapping?.ParameterName ?? string.Empty;
            var hashingType = projectMapping?.HashingType ?? string.Empty;

            if (requestUrl.IndexOf("[respondentID]", StringComparison.OrdinalIgnoreCase) <= 0 &&
                requestUrl.IndexOf("[RespondentID]", StringComparison.OrdinalIgnoreCase) <= 0 &&
                requestUrl.IndexOf("[RESPONDENTID]", StringComparison.OrdinalIgnoreCase) <= 0)
            {
                return result;
            }

            requestUrl = requestUrl.Replace("[respondentID]", clientId, StringComparison.OrdinalIgnoreCase)
                                   .Replace("[RespondentID]", clientId, StringComparison.OrdinalIgnoreCase)
                                   .Replace("[RESPONDENTID]", clientId, StringComparison.OrdinalIgnoreCase);

            var hashingResult = await _hashingSettingsService.ApplyHashAsync(requestUrl, addHashing, hashingType, parameterName);
            requestUrl = hashingResult.RequestUrl;

            if (trackingType != 1 && px != 1)
            {
                var now = DateTime.Now;
                var completionRows = await _surveyAPIController.GenericDataFetcher("usp_ShareStatusOnRequest", new { @ID = supplierProjectUid, @opt = 2, @pid = 0 });
                if (completionRows != null && completionRows.Any())
                {
                    var completionRow = completionRows.FirstOrDefault();
                    if (addHashing == 1)
                    {
                        var hashingUrl = completionRow?.HashingURL ?? string.Empty;
                        const string completeStatus = "10";
                        var authorizationKey = completionRow?.AuthorizationKey ?? string.Empty;

                        if (currentStatus.Equals("COMPLETE", StringComparison.OrdinalIgnoreCase))
                        {
                            hashingUrl = hashingUrl.Replace("[respondentID]", clientId, StringComparison.OrdinalIgnoreCase)
                                                   .Replace("[RespondentID]", clientId, StringComparison.OrdinalIgnoreCase)
                                                   .Replace("[RESPONDENTID]", clientId, StringComparison.OrdinalIgnoreCase)
                                                   .Replace("[StatusCode]", completeStatus, StringComparison.OrdinalIgnoreCase)
                                                   .Replace("[StatusCODE]", completeStatus, StringComparison.OrdinalIgnoreCase)
                                                   .Replace("[STATUSCODE]", completeStatus, StringComparison.OrdinalIgnoreCase);

                            var hashingTest = new HashingTest();
                            hashingTest.CallSSCB(hashingUrl, completeStatus, authorizationKey, now);
                        }
                    }
                }

                result.RedirectUrl = requestUrl;
            }

            await _surveyAPIController.UpdateRequestStatus(supplierProjectUid);
            await _surveyAPIController.UpdateSupplierENCByUID(supplierProjectUid, hashingResult.HashCode);
            result.HashCode = hashingResult.HashCode;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(ExecuteShareStatusAsync));
            return result;
        }
    }

    private sealed class LegacyProjectStatusShareResult
    {
        public string? RedirectUrl { get; set; }

        public string? HashCode { get; set; }
    }
}
