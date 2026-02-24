using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MProjectMapping
/// </summary>
public class MProjectMapping
{
    public MProjectMapping()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int ID { get; set; }
    public string quota { get; set; }
    public string cpi { get; set; }
    public string notes { get; set; }
    public string CountryID { get; set; }
    public string SupplierID { get; set; }
    public string ProjectID { get; set; }
    public int AddHashing { get; set; }
    public string ParameterName { get; set; }
    public string HashingType { get; set; }
}