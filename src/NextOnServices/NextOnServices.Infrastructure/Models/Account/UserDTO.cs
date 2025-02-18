using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Account;

public class UserDTO
{
    public int UserId { get; set; }

    public string? UserCode { get; set; }

    public string? UserName { get; set; }

    public string? EmailId { get; set; }

    public string? Password { get; set; }

    public string? ContactNumber { get; set; }

    public string? Address { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UserType { get; set; }

    public int? IsActive { get; set; }
    public HttpResponseMessage? HttpMessage { get; set; }
}
