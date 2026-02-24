using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;

public partial class ProjectDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProject> GetProjectDetails()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsProject> returnData = new List<ClsProject>();
        try
        {

            DataSet resultDS = null;

            resultDS = objDAL.ClientMGRAll(0, "P");

            //resultDS = objDal.GetSurveyFilter(surveyID, GID);




            int clientId;
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new ClsProject
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    ProjectNumber = dtrow["PID"].ToString(),
                    PName = dtrow["PName"].ToString(),
                    ClientName = dtrow["Company"].ToString(),
                    PManager = dtrow["UserName"].ToString(),
                    SampleSize = dtrow["SampleSize"].ToString(),
                    LOI = dtrow["LOI"].ToString(),
                    SDate = dtrow["startdate"].ToString(),
                    EDate = dtrow["enddate"].ToString(),
                    Country = dtrow["Countes"].ToString(),
                    ClientID = int.TryParse(dtrow["ClientID"].ToString(), out clientId) == true ? clientId : (int?)null,
                    Active = Convert.ToInt32(dtrow["IsActive"])
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
    public static void GetCompleteDetails()
    {
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProject> GetProjectDetails1(string ManagerId, string StatusId, string Flag)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsProject> returnData = new List<ClsProject>();
        try
        {
            int clientId;
            DataSet ds = objDAL.GetDashboardByManager(ManagerId, StatusId, Flag);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dtrow in ds.Tables[0].Rows)
                {
                    returnData.Add(new ClsProject
                    {
                        ID = System.Convert.ToInt32(dtrow["ID"]),
                        ProjectNumber = dtrow["PNO"].ToString(),
                        //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                        PName = dtrow["PNAME"].ToString(),
                        ClientName = dtrow["CLIENT"].ToString(),
                        PManager = dtrow["PM"].ToString(),
                        Status = System.Convert.ToInt32(dtrow["STATUS"]),
                        SampleSize = dtrow["SampleSize"].ToString(),
                        LOI = dtrow["LOI"].ToString(),
                        CPI = Convert.ToDouble(dtrow["CPI"]),
                        IRate = dtrow["IRate"].ToString(),
                        SDate = dtrow["startdate"].ToString(),
                        EDate = dtrow["DATE"].ToString(),
                        Country = dtrow["COUNTRY"].ToString(),
                        total = Convert.ToInt32(dtrow["TOTAL"]),
                        completed = Convert.ToInt32(dtrow["CO"]),
                        incomplete = Convert.ToInt32(dtrow["IC"]),
                        overquota = Convert.ToInt32(dtrow["OQ"]),
                        terminate = Convert.ToInt32(dtrow["TR"]),
                        ClientID = int.TryParse(dtrow["ClientID"].ToString(), out clientId) == true ? clientId : (int?)null,
                        securityterm = Convert.ToInt32(dtrow["ST"]),
                        frauderror = Convert.ToInt32(dtrow["FE"]),
                        ActIR = dtrow["ActIR"].ToString(),
                        ActLOI = dtrow["ActLOI"].ToString(),
                        Flag = dtrow["Flag"].ToString()
                    });
                }
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
    public static int Delete(string Flag, string Id)
    {
        ClsDAL objDAL = new ClsDAL();
        try
        {
            int x = objDAL.Delete(4, Id, Convert.ToInt32(Flag));
            return x;
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
}