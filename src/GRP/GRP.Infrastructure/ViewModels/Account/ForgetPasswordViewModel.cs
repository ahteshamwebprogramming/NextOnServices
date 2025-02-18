using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.ViewModels.Account;

public class ForgetPasswordViewModel
{
    [Required(ErrorMessage ="Please enter your email")]
    public string Email { get; set; }
}
