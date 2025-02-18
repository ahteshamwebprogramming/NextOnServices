using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.Models.Supplier;

public class SupplierLoginDTO
{
    public int Id { get; set; }
    public Int64? SupplierId { get; set; }

    [Required(ErrorMessage = "Username is mandatory")]
    //[EmailAddress(ErrorMessage = "Username must be a valid email address")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is mandatory")]
    public string Password { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
    public bool FirstTimePasswordReset { get; set; }
}
