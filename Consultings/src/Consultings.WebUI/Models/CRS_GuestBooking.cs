using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consultings.WebUI.Models;

[Table("CRS_GuestBooking")]
public class CRS_GuestBooking
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    public int? CountryId { get; set; }

    public int? CityId { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }

    public int? NoOfNights { get; set; }

    public int? TotalAdults { get; set; }

    public int? TotalChildren { get; set; }

    public int? TotalRooms { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalPrice { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? RoomDetails { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? GuestDetails { get; set; }

    public bool IsPaymentDone { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PaymentAmount { get; set; }

    [MaxLength(100)]
    public string? PaymentMethod { get; set; }

    [MaxLength(255)]
    public string? PaymentTransactionId { get; set; }

    [MaxLength(50)]
    public string? PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? PaymentRemarks { get; set; }

    public bool IsPostedToPms { get; set; }

    public DateTime? PmsPostedDate { get; set; }

    [MaxLength(100)]
    public string? PmsGroupId { get; set; }

    [MaxLength(500)]
    public string? PmsGuestIds { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? PmsResponse { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? PmsError { get; set; }

    public DateTime? BookingCreatedDate { get; set; }

    public DateTime? BookingModifiedDate { get; set; }

    [MaxLength(100)]
    public string? BookingId { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? Remarks { get; set; }

    public int? Status { get; set; }

    [MaxLength(255)]
    public string? SessionId { get; set; }

    public int? UserId { get; set; }
}

