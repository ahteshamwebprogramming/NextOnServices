using System.ComponentModel.DataAnnotations.Schema;

namespace NextOnServices.Core.Entities;

[Dapper.Contrib.Extensions.Table("HashingSetting")]
public class HashingSetting
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HashingSettingId { get; set; }

    public string? HashingType { get; set; }

    public string? HashingKey { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
}
