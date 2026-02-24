namespace NextOnServices.Infrastructure.Models.Projects;

/// <summary>
/// Project item for Reconciliation dropdown. Display as PID_PName.
/// </summary>
public class ReconciliationProjectDTO
{
    public int Id { get; set; }
    public string Pid { get; set; } = "";
    public string PName { get; set; } = "";
    /// <summary>Combined display for dropdown: Pid_PName.</summary>
    public string DisplayText { get; set; } = "";
}
