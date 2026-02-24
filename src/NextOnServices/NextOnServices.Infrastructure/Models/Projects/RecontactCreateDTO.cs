using System.Text.Json.Serialization;

namespace NextOnServices.Infrastructure.Models.Projects;

/// <summary>Project option for Create Recontact dropdown (UspGetAllUsers Type P).</summary>
public class RecontactCreateProjectDTO
{
    public int ID { get; set; }
    public string PName { get; set; } = string.Empty;
}

/// <summary>Row for Create Recontact table (FetchRecontact). SP columns: Id, RecontactProjectId, recontactname, CPI, Notes, SID, MURL.</summary>
public class RecontactCreateListItemDTO
{
    public int ID { get; set; }
    [JsonPropertyName("PID")]
    public string RecontactProjectId { get; set; } = string.Empty;
    public string RecontactName { get; set; } = string.Empty;
    public decimal CPI { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string SID { get; set; } = string.Empty;
    public string MURL { get; set; } = string.Empty;
}

/// <summary>First result row from Recontactmgr insert (opt=0).</summary>
public class InsertRecontactResultRow
{
    public int status { get; set; }
    public int rcid { get; set; }
}

/// <summary>First result row from validateUID for recontact.</summary>
public class ValidateUIDRecontactResultRow
{
    public string pmid { get; set; } = string.Empty;
    public int statuss { get; set; }
    public int promapid { get; set; }
    public int projectid { get; set; }
    public int countryid { get; set; }
    public int supplierid { get; set; }
}

/// <summary>Row from RecontactRespondanttbl result (returnvalue, status, r_message).</summary>
public class RecontactRespondantResultRow
{
    public int returnvalue { get; set; }
    public int status { get; set; }
    public string r_message { get; set; } = string.Empty;
}

/// <summary>First row from DELETERecontactprojects (opt=1) - single value.</summary>
public class DeleteRecontactResultRow
{
    public int status { get; set; }
}

/// <summary>First row from UpdateRecontctRespondants (Add More) - single value.</summary>
public class AddMoreRecontactResultRow
{
    public int status { get; set; }
}

/// <summary>Result from GetMaxPID (single column - name may vary).</summary>
public class NextPidResult
{
    public string? PID { get; set; }
    public string? MaxPID { get; set; }
}

/// <summary>Request payload for SaveData (Create Recontact from CSV). Numeric fields accept number or string via JsonNumberHandling.</summary>
public class RecontactCreateSaveRequest
{
    public string RecontactName { get; set; } = string.Empty;
    [JsonPropertyName("Recontact_Description")]
    public string RecontactDescription { get; set; } = string.Empty;

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? Status { get; set; } = 5;

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public double? LOI { get; set; } = 0;

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public double? IR { get; set; } = 0;

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CPI { get; set; } = 0;

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? RCQ { get; set; } = 0;

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? NoOfVars { get; set; } = 0;

    public string Var1 { get; set; } = string.Empty;
    public string Var2 { get; set; } = string.Empty;
    public string Var3 { get; set; } = string.Empty;
    public string Var4 { get; set; } = string.Empty;
    public string Var5 { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    [JsonPropertyName("exceldata")]
    public string ExcelData { get; set; } = string.Empty; // CSV rows (no header); client sends "exceldata"
}

/// <summary>Row for View Data modal (GetRecontactExcelData).</summary>
public class RecontactDownloadDTO
{
    public string UID { get; set; } = string.Empty;
    public string RedirectLink { get; set; } = string.Empty;
    public string MaskingURL { get; set; } = string.Empty;
    public string PMID { get; set; } = string.Empty;
    public int IsUsed { get; set; }
}

/// <summary>Error item returned from SaveData.</summary>
public class RecontactSaveErrorDTO
{
    public string ErrorReturnDBUID { get; set; } = string.Empty;
    public string ErrorReturnDBMessage { get; set; } = string.Empty;
    public string ErrorReturnDBStatus { get; set; } = string.Empty;
}
