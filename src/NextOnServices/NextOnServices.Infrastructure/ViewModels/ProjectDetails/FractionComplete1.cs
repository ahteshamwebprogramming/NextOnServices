using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels.ProjectDetails;

public class FractionComplete1
{
    public int ExpectedIR { get; set; }
    public int Complete { get; set; }
    public int Incomplete { get; set; }
    public int ActualIR { get; set; }
    public int IRPercent { get; set; }
}
