using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consultings.WebUI.Models;

[Table("CRS_VisitorsSessionLog")]
public class CRS_VisitorsSessionLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string SessionId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(100)]
    public string? Browser { get; set; }

    [MaxLength(50)]
    public string? BrowserVersion { get; set; }

    [MaxLength(100)]
    public string? OperatingSystem { get; set; }

    [MaxLength(50)]
    public string? DeviceType { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? UserAgent { get; set; }

    [MaxLength(500)]
    public string? Referrer { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? Region { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? Longitude { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}

