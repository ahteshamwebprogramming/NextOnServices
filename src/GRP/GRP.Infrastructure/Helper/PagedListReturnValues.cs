using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRP.Infrastructure.Helper;

public class PagedListReturnValues<T>
{
    public T data { get; set; }
    public int totalRecord { get; set; }
    public int filterRecord { get; set; }
}
