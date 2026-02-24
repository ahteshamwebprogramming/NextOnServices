using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VT_RecontactProjects : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {


    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<Recontact> GetRecontactProjects()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<Recontact> returnData = new List<Recontact>();
        try
        {
            // int PID = Convert.ToInt32(HttpContext.Current.Session["PID"]);
            DataSet resultDS = null;
            resultDS = objDAL.GetRecontactProjects();
            //resultDS = objDal.GetSurveyFilter(surveyID, GID);
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new Recontact
                {
                    ID = System.Convert.ToInt32(dtrow["projectid"]),
                    PID = dtrow["pid"].ToString(),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    RecontactName = dtrow["projectname"].ToString(),
                    CPI = float.Parse(dtrow["cpi"].ToString()),
                    Total = System.Convert.ToInt32(dtrow["total"]),
                    Complete = System.Convert.ToInt32(dtrow["complete"]),
                    Terminate = System.Convert.ToInt32(dtrow["terminate"]),
                    Overquota = System.Convert.ToInt32(dtrow["overquota"]),
                    S_term = System.Convert.ToInt32(dtrow["securityterm"]),
                    F_error = System.Convert.ToInt32(dtrow["fauderror"]),
                    Incomplete = System.Convert.ToInt32(dtrow["incomplete"]),
                    Statusint = Convert.ToInt32(dtrow["status"])
                });
            }
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return returnData;
        }
        return returnData;
    }


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string UpdateStatus(string Id, string Status)
    {
        ClsDAL objdal = new ClsDAL();
        int x = objdal.UpdateProjectStatus(Status, Id, "Recontact");

        return x > 0 ? "Updated Succesfully" : "Unable to Update succesfully";

    }

}