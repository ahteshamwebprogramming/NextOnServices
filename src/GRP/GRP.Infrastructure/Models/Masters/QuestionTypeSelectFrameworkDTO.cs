using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Models.Masters;

public class QuestionTypeSelectFrameworkDTO
{
    public int QuestionTypeSelectFrameworkId { get; set; }

    public int? ControlId { get; set; }

    public string? Label { get; set; }

    public string? Value { get; set; }
}
