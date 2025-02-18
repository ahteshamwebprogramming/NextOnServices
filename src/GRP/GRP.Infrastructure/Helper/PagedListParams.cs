using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Helper;

public class PagedListParams
{
    //string searchValue, string sortColumn, string sortColumnDirection, int skip, int pageSize
    public string searchValue { get; set; }
    public string sortColumn { get; set; }
    public string sortColumnDirection { get; set; }
    public int? sortColumnIndex { get; set; }
    public int skip { get; set; }
    public int pageSize { get; set; }
}
