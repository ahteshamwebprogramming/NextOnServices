namespace NextOnServices.Infrastructure.Models.Projects;

/// <summary>
/// Result row for ID Search (Respondents Report). Maps from USP_mgrRespondents output.
/// </summary>
public class RespondentSearchResultDTO
{
    public string PName { get; set; } = "";
    public string PID { get; set; } = "";
    public string SupplierName { get; set; } = "";
    public string SID { get; set; } = "";
    public string UID { get; set; } = "";
    public string Country { get; set; } = "";
    public string SupplierId { get; set; } = "";
    public string Status { get; set; } = "";
    public string Sdate { get; set; } = "";
    public string Edate { get; set; } = "";
    public string Duration { get; set; } = "";
    public string ClientBrowser { get; set; } = "";
    public string ClientIP { get; set; } = "";
    public string Device { get; set; } = "";
    /// <summary>Error message when reconciliation/update returns an error row.</summary>
    public string Error { get; set; } = "";
}
