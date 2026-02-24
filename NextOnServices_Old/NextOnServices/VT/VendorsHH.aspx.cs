using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;

public partial class VT_VendorsHH : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<VendorsHH> GetSuppliers(VendorsHH formData)
    {
        try
        {
            ClsDAL objdal = new ClsDAL();
            DataSet ds = objdal.GetSuppliers(formData);
            List<VendorsHH> vendorsHHs = new List<VendorsHH>();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        vendorsHHs.Add(new VendorsHH
                        {
                            Name = Convert.ToString(item["Name"])
                        });
                    }
                }
            }
            return vendorsHHs;

        }
        catch (Exception e)
        {
            return null;
        }


    }
}

