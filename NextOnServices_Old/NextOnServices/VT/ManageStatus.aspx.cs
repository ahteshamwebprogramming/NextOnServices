using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ManageStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProject> GetProjects()
    {

        ClsDAL objDAL = new ClsDAL();
        List<ClsProject> returnProject = new List<ClsProject>();
        DataSet dsProject = objDAL.ClientMGR(0, "P");
        
        try
        {
            if (dsProject.Tables[0].Rows.Count > 0)
            {
               

                foreach (DataRow dtrow in dsProject.Tables[0].Rows)
                {
                    returnProject.Add(new ClsProject
                    {
                        ID = System.Convert.ToInt32(dtrow["ID"]),
                        PName = dtrow["PCode"].ToString()
                    });

                }

            }
            return returnProject;
        }
        catch (Exception e)
        {
            return null;
        }


    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void ChangeAll(string Id, string status, int type, int PId)
    {
        ClsDAL objDAL = new ClsDAL();
        DataSet ds = objDAL.UpdateMultipleStatus(Id, status, type, PId);
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CLSRespondentsCom> Updatestatus(string Id, string status, int type, int PId)
    {
        try
        {
            List<CLSRespondentsCom> returnData = new List<CLSRespondentsCom>();
            ClsDAL objDAL = new ClsDAL();

            DataSet ds = objDAL.UpdateMultipleStatus(Id, status, type, PId);
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
                                Error = ""
                            });
                        }


                    }
                }
            }
            return returnData;

        }
        catch (Exception ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(HttpContext.Current.Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
            return null;
        }

    }
}