using GRP.Infrastructure.Helper;
using GRP.Infrastructure.Models.Masters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Account;

public class UserDTO
{

    public int UserId { get; set; }

    [Required(ErrorMessage = "Please enter your Email Address")]
    public string? EmailId { get; set; }

    [Required(ErrorMessage = "Please enter your Name")]
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [ValidateNever]
    public int? Gender { get; set; }
    [ValidateNever]
    public int? CountryId { get; set; }

    public bool IsActive { get; set; }

    [MustBeTrueAttribute(ErrorMessage = "You must agree to the terms and conditions")]
    public bool AgreedToTACPP { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }

    public List<CountryMasterDTO>? Countries { get; set; } = null;
}
