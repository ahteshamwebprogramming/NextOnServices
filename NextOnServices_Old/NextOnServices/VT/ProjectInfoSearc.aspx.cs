using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectInfoSearc : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CLSRespondentsCom> Getstatus(int opt, string id)
    {
        try
        {
            List<CLSRespondentsCom> returnData = new List<CLSRespondentsCom>();
            ClsDAL objDAL = new ClsDAL();

            DataSet ds = objDAL.GetMultipleStatus(opt, id);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[0].Rows)
                    {
                        if (ds.Tables[0].Columns.Contains("Error"))
                        {
                            returnData.Add(new CLSRespondentsCom
                            {
                                Error = dtrow["Error"].ToString()
                            });
                        }
                        else
                        {
                            returnData.Add(new CLSRespondentsCom
                            {
                                SupplierName = dtrow["Supplier Name"].ToString(),
                                SID = dtrow["SID"].ToString(),
                                UID = dtrow["UID"].ToString(),
                                Country = dtrow["Country"].ToString(),
                                SupplierId = dtrow["Supplier ID"].ToString(),
                                Status = dtrow["Status"].ToString(),
                                Sdate = dtrow["StartDate"].ToString(),
                                Edate = dtrow["EndDate"].ToString(),
                                Duration = dtrow["Duration"].ToString(),
                                ClientBrowser = dtrow["ClientBrowser"].ToString(),
                                ClientIP = dtrow["ClientIP"].ToString(),
                                Device = dtrow["Device"].ToString(),
                                Error = "",
                                PName = dtrow["PName"].ToString(),
                                PID = dtrow["PID"].ToString()
                            });
                        }


                    }
                }
            }
            return returnData;

        }
        catch (Exception e)
        {
            return null;
        }

    }
}