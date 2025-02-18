using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Core.Helper;

public class RequestParams
{
    const int maxPageSize = 1000;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;
    public bool IsIncludes { get; set; } = false;

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = value > maxPageSize ? maxPageSize : value;
        }
    }
    public bool? IsActive { get; set; } = true;
}

