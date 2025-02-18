using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Account;

public class LoginDetailDTO
{
    public int LoginId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int UserId { get; set; }

    public bool IsActive { get; set; }
    

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }

    public string? EncryptedPassword { get; set; }
    public bool PasswordChanged { get; set; }
}
