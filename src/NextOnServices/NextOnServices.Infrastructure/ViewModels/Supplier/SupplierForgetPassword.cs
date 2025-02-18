using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.Supplier
{
    public class SupplierForgetPassword
    {
        public int? Id { get; set; }
        public string? encId { get; set; }

        [Required(ErrorMessage = "Supplier Code is required.")]
     
        public string? SupplierCode { get; set; }
    }
}
