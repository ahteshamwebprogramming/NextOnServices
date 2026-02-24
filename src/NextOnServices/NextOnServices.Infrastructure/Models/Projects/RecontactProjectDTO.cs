using System;

namespace NextOnServices.Infrastructure.Models.Projects;

/// <summary>
/// Row item for Recontact Projects grid.
/// </summary>
public class RecontactProjectDTO
{
    public int ID { get; set; }
    public string PID { get; set; } = string.Empty;
    public string RecontactName { get; set; } = string.Empty;
    public decimal CPI { get; set; }
    public int Total { get; set; }
    public int Complete { get; set; }
    public int Terminate { get; set; }
    public int Overquota { get; set; }
    public int S_term { get; set; }
    public int F_error { get; set; }
    public int Incomplete { get; set; }
    public int Statusint { get; set; }
}

