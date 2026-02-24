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

public partial class Deshboard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                List<ClsDeshBoard> objBord = GetDeshboardDetails(Session["userid"].ToString());
                if (objBord.Count > 0)
                {
                    lblproject.Text = objBord[0].Project;
                    lblActive.Text = objBord[0].Active;
                    lblInactive.Text = objBord[0].Inactive;
                    lblArchived.Text = objBord[0].Archived;
                }
            }
            else
            {
                Response.Redirect("Index.html");
            }
        }
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string UpdateStatus(string Id, string Status)
    {
        ClsDAL objdal = new ClsDAL();
        int x = objdal.UpdateProjectStatus(Status, Id, "Dashboard");
        int y = objdal.UpdateProjectStatus(Status, Id, "UpdateUrlStatus");
        return x > 0 ? "Updated Succesfully" : "Unable to Update succesfully";

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CLSManager> GetManager()
    {
        try
        {
            ClsDAL objdal = new ClsDAL();
            DataSet ds = objdal.getmanager();
            List<CLSManager> returnData = new List<CLSManager>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dtrow in ds.Tables[0].Rows)
                {
                    returnData.Add(new CLSManager
                    {
                        Id = System.Convert.ToInt32(dtrow["Id"]),
                        //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                        Manager = dtrow["Manager"].ToString(),



                    });
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

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsDeshBoard> GetDeshboardDetails(string ID)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsDeshBoard> returnData = new List<ClsDeshBoard>();
        try
        {

            DataSet resultDS = null;

            resultDS = objDAL.GetDeshBoard(ID);

            //resultDS = objDal.GetSurveyFilter(surveyID, GID);


            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {


                returnData.Add(new ClsDeshBoard
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    Project = dtrow["TProject"].ToString(),
                    Active = dtrow["TActive"].ToString(),
                    Inactive = dtrow["TInactive"].ToString(),
                    Archived = dtrow["TArchived"].ToString()


                });

            }

        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(HttpContext.Current.Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
            return returnData;
        }

        return returnData;
    }
}

public class ClsDeshBoard
{
    public int ID { get; set; }
    public string Project { get; set; }
    public string Supplier { get; set; }
    public string Active { get; set; }
    public string Client { get; set; }
    public string Completed { get; set; }
    public string Inactive { get; set; }
    public string Archived { get; set; }


}
