using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Account;

public class TBL_UserProfileDTO
{
    public long ID { get; set; }

    public long MID { get; set; }

    public Guid? DispID { get; set; }

    public string? Password { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? LandLine { get; set; }

    public DateTime? DOB { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? District { get; set; }

    public int? State { get; set; }

    public int? Country { get; set; }

    public string? Education { get; set; }

    public string? Occupation { get; set; }

    public double? Income { get; set; }

    public string? Industry { get; set; }

    public string? VehicleOwn { get; set; }

    public string? DriveMost { get; set; }

    public bool? CreditCardOwn { get; set; }

    public string? MotherTounge { get; set; }

    public string? PreferredLanguage { get; set; }

    public string? Religion { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsSubscribed { get; set; }

    public string? Source { get; set; }

    public DateTime? CreatedDate { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }

    public string? EncryptedPassword { get; set; }
}

