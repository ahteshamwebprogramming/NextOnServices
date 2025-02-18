using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Account;

public class ResetPasswordViewModel
{

    public long MID { get; set; }

    [Required(ErrorMessage = "Please enter your password"), MinLength(6, ErrorMessage = "Password Length should be min 6 characters")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Please enter your password"), MinLength(6, ErrorMessage = "Password Length should be min 6 characters")]
    [Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
    public string? ConfirmPassword { get; set; }
}

public class ChangePasswordViewModel
{

    public int MID { get; set; }

    [Required(ErrorMessage = "Please enter old password"), MinLength(6, ErrorMessage = "Password Length should be min 6 characters")]
    public string? OldPassword { get; set; }
    [Required(ErrorMessage = "Please enter new password"), MinLength(6, ErrorMessage = "Password Length should be min 6 characters")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Please enter new password"), MinLength(6, ErrorMessage = "Password Length should be min 6 characters")]
    [Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
    public string? ConfirmPassword { get; set; }
}
