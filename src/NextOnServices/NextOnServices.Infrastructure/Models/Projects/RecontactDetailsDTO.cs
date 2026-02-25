namespace NextOnServices.Infrastructure.Models.Projects;

/// <summary>Maps to RecontactPageDetails SP first result set: @tblcountry (countryname, total, complete, ...).</summary>
public class RecontactCountryDTO
{
    public int id { get; set; }
    public int countryid { get; set; }
    public string countryname { get; set; } = string.Empty;
    public int total { get; set; }
    public int complete { get; set; }
    public int terminate { get; set; }
    public int overquota { get; set; }
    public int s_term { get; set; }
    public int f_error { get; set; }
    public int incomplete { get; set; }
    public int cancelled { get; set; }
}

/// <summary>Maps to RecontactPageDetails SP second result set: @tblsupplier (suppliername, total, ...).</summary>
public class RecontactSupplierDTO
{
    public int id { get; set; }
    public int supplierid { get; set; }
    public string suppliername { get; set; } = string.Empty;
    public int total { get; set; }
    public int complete { get; set; }
    public int terminate { get; set; }
    public int overquota { get; set; }
    public int s_term { get; set; }
    public int f_error { get; set; }
    public int incomplete { get; set; }
    public int cancelled { get; set; }
}

/// <summary>Maps to RecontactPageDetails SP third result set: tblRedirectsForRecontact (Complete, Terminate, S_Term, F_Error, Variable1, Variable2).</summary>
public class RecontactRedirectDTO
{
    public int Id { get; set; }
    public string Complete { get; set; } = string.Empty;
    public string Terminate { get; set; } = string.Empty;
    public string Overquota { get; set; } = string.Empty;
    public string S_Term { get; set; } = string.Empty;
    public string F_Error { get; set; } = string.Empty;
    public string Variable1 { get; set; } = string.Empty;
    public string Variable2 { get; set; } = string.Empty;
}

/// <summary>Maps to RecontactPageDetails SP fourth result set: @remainingdetails (ProjectNumber, RProjectName, RProjectDescription, LOI, CPI, IR, RCQ, Notes, Status).</summary>
public class RecontactSummaryDTO
{
    public int id { get; set; }
    public string ProjectNumber { get; set; } = string.Empty;
    public string RProjectName { get; set; } = string.Empty;
    public string RProjectDescription { get; set; } = string.Empty;
    public int LOI { get; set; }
    public decimal CPI { get; set; }
    public int IR { get; set; }
    public int RCQ { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>Maps to RecontactParametersCalC SP single row: status, Complete, total, ActIRPercent, ActLOI (KPI block on RecontactPageDetails).</summary>
public class RecontactKpiDTO
{
    public string status { get; set; } = string.Empty;
    public int Complete { get; set; }
    public int total { get; set; }
    public decimal? ActIRPercent { get; set; }
    public decimal? ActLOI { get; set; }
}

public class RecontactDetailsDTO
{
    public List<RecontactCountryDTO> Countries { get; set; } = new();
    public List<RecontactSupplierDTO> Suppliers { get; set; } = new();
    public List<RecontactRedirectDTO> Redirects { get; set; } = new();
    public List<RecontactSummaryDTO> Summary { get; set; } = new();
    public RecontactKpiDTO? Kpi { get; set; }
}

public class RecontactUpdateDTO
{
    public int ID { get; set; }
    public string RecontactDescription { get; set; } = string.Empty;
    public int LOI { get; set; }
    public decimal CPI { get; set; }
    public int IR { get; set; }
    public int RCQ { get; set; }
    public string Notes { get; set; } = string.Empty;
}

