using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Core.Entities;

public class User
{
    public int UserId { get; set; }

    public string? EmailId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public int? Gender { get; set; }
    public int? CountryId { get; set; }

    public bool IsActive { get; set; }
    public bool AgreedToTACPP { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public int? RoleId { get; set; }
}
