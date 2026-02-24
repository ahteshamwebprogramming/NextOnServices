using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RespondentsReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CLSRespondentsCom> GetRespondents(string id, string type)
    {
        try
        {
            ClsDAL objDAL = new ClsDAL();
            List<CLSRespondentsCom> returnData = new List<CLSRespondentsCom>();
            DataSet dsclient = objDAL.GetResDetails(id, type);
            if (dsclient.Tables.Count > 0)
            {
                if (dsclient.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in dsclient.Tables[0].Rows)
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
                            Device = dtrow["Device"].ToString()
                        });
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
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void Updatestatus(string Id, string status)
    {
        try
        {
            ClsDAL objDAL = new ClsDAL();
            //List<CLSRespondentsCom> returnData = new List<CLSRespondentsCom>();
           int x=  objDAL.UpdateStatus(Id, status);
            
        }
        catch (Exception e)
        {
          
        }

    }
}