using System.ComponentModel.DataAnnotations;

namespace NextOnServices.Infrastructure.Models.Settings;

public class HashingSettingDTO
{
    public int HashingSettingId { get; set; }

    [Required(ErrorMessage = "Hashing type is required.")]
    [StringLength(50)]
    public string? HashingType { get; set; }

    [Required(ErrorMessage = "Hashing key is required.")]
    [StringLength(500)]
    public string? HashingKey { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
