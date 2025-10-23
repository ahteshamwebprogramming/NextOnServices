namespace NextOnServices.VT.Infrastructure.ViewModels.Supplier;

public class SupplierDeliverySummary
{
    public string? Country { get; set; }
    public int Total { get; set; }
    public int Completes { get; set; }
    public int Incompletes { get; set; }
    public int Screened { get; set; }
    public int QuotaFull { get; set; }
}
