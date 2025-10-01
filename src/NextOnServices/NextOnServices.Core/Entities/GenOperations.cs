using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("GenOperations")]
    public class GenOperations
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? opt { get; set; }
    }
}
