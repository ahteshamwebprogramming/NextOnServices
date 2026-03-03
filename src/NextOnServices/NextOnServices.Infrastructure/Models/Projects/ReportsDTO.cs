namespace NextOnServices.Infrastructure.Models.Projects;

public class OverallReportFilterDTO
{
    public int CountryId { get; set; }
    public int ClientId { get; set; }
    public int SupplierId { get; set; }
    public DateTime? SDate { get; set; }
    public DateTime? EDate { get; set; }
}

public class OverallNamedValuePercentDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Percentage { get; set; }
}

public class OverallProjectTotalsDTO
{
    public int Total { get; set; }
    public int Closed { get; set; }
    public int Inprogress { get; set; }
    public int Onhold { get; set; }
    public int Cancelled { get; set; }
}

public class OverallRangeMetricDTO
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
    public decimal Average { get; set; }
}

public class OverallRateSummaryDTO
{
    public OverallRangeMetricDTO IRate { get; set; } = new();
    public OverallRangeMetricDTO ExpectedLoi { get; set; } = new();
    public OverallRangeMetricDTO Cpi { get; set; } = new();
    public OverallRangeMetricDTO ActualLoi { get; set; } = new();
}

public class OverallSuccessRateDTO
{
    public int ProjectSuccessRate { get; set; }
    public int IrSuccessRate { get; set; }
}

public class OverallRespondentTotalsDTO
{
    public int Total { get; set; }
    public int Complete { get; set; }
    public int Incomplete { get; set; }
    public int Screened { get; set; }
    public int Quotafull { get; set; }
    public int Terminate { get; set; }
}

public class OverallManagerSummaryDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal Totalpercentage { get; set; }
    public decimal Complete { get; set; }
    public decimal Completepercentage { get; set; }
    public decimal Revenue { get; set; }
    public decimal Revenuepercentage { get; set; }
}

public class OverallPieChartDTO
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
    public decimal Mean { get; set; }
}

public class OverallMonthlyCompleteDTO
{
    public string Months { get; set; } = string.Empty;
    public decimal Completes { get; set; }
}

public class OverallReportSnapshotDTO
{
    public List<OverallNamedValuePercentDTO> SupplierCompletions { get; set; } = new();
    public List<OverallNamedValuePercentDTO> ClientCompletions { get; set; } = new();
    public List<OverallNamedValuePercentDTO> CountryCompletions { get; set; } = new();

    public List<OverallNamedValuePercentDTO> SupplierRevenue { get; set; } = new();
    public List<OverallNamedValuePercentDTO> ClientRevenue { get; set; } = new();
    public List<OverallNamedValuePercentDTO> CountryRevenue { get; set; } = new();

    public OverallProjectTotalsDTO ProjectTotals { get; set; } = new();
    public OverallRateSummaryDTO Rates { get; set; } = new();
    public OverallSuccessRateDTO SuccessRate { get; set; } = new();
    public OverallRespondentTotalsDTO Respondents { get; set; } = new();
    public decimal SumValue { get; set; }

    public List<OverallManagerSummaryDTO> ManagerSummary { get; set; } = new();
    public OverallPieChartDTO CpiChart { get; set; } = new();
    public List<OverallMonthlyCompleteDTO> CompleteTrend { get; set; } = new();
}

public class ProjectWiseReportProjectOptionDTO
{
    public int Id { get; set; }
    public string PName { get; set; } = string.Empty;
}

public class ProjectWiseOverviewDTO
{
    public string PNumber { get; set; } = string.Empty;
    public string PStatus { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Manager { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Complete { get; set; }
    public decimal Cpc { get; set; }
    public decimal RevenueFromCompletes { get; set; }
}

public class ProjectWiseCountryValueDTO
{
    public int CountryId { get; set; }
    public string Country { get; set; } = string.Empty;
    public decimal Complete { get; set; }
    public decimal Cpc { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class ProjectWiseSupplierValueDTO
{
    public int CountryId { get; set; }
    public int SupplierId { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal Complete { get; set; }
    public decimal Cpc { get; set; }
    public decimal Cost { get; set; }
}

public class ProjectWiseTotalsDTO
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal Margin { get; set; }
}

public class ProjectWiseReportDataDTO
{
    public ProjectWiseOverviewDTO Overview { get; set; } = new();
    public ProjectWiseTotalsDTO Totals { get; set; } = new();
    public List<ProjectWiseCountryValueDTO> CountryValues { get; set; } = new();
    public List<ProjectWiseSupplierValueDTO> SupplierValues { get; set; } = new();
}
