using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GRP.Core.Entities;
[Dapper.Contrib.Extensions.Table("QuestionTypeSelectFramework")]
public class QuestionTypeSelectFramework
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int QuestionTypeSelectFrameworkId { get; set; }

    public int? ControlId { get; set; }

    public string? Label { get; set; }

    public string? Value { get; set; }
}
