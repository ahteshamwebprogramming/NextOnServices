using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.ViewModels;

public class DataTablesRequest
{
    public int draw { get; set; }
    //public int? Page { get; set; }
    //public int? PageSize { get; set; }
    //public int Draw { get; set; } // Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by DataTables.

    public int start { get; set; } // Paging first record indicator. This is the start point in the current data set.

    public int? length { get; set; } // Number of records that the table can display in the current draw.

    public Search? search { get; set; } // Global search value.

    public List<Order>? order { get; set; } // List of ordering columns.

    public List<Column>? columns { get; set; } // List of columns in the DataTable.
    //public DataTablesRequest()
    //{
    //    Search = new Search();
    //    Order = new List<Order>();
    //    Columns = new List<Column>();
    //}
}
public class Search
{
    public string? value { get; set; } // Global search value.

    public bool? regex { get; set; } // Whether the global search should be treated as a regular expression for advanced searching.
}
public class Order
{
    public int? column { get; set; } // Column index for ordering.

    public string? dir { get; set; } // Order direction "asc" or "desc".
}
public class Column
{
    public string? data { get; set; } // Column data source.

    public string? name { get; set; } // Column name.

    public bool searchable { get; set; } // Whether the column is searchable.

    public bool orderable { get; set; } // Whether the column is orderable.

    public Search? search { get; set; } // Search applied to this specific column.
}
