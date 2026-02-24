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

public partial class ClientsDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsClient> GetClientDetails()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsClient> returnData = new List<ClsClient>();
        try
        {

            DataSet resultDS = null;

            resultDS = objDAL.ClientMGRAll(0, "C");

            //resultDS = objDal.GetSurveyFilter(surveyID, GID);


            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {


                returnData.Add(new ClsClient
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    Company = dtrow["Company"].ToString(),
                    Person = dtrow["CPerson"].ToString(),
                    Email = dtrow["CEmail"].ToString(),
                    Country = dtrow["Country"].ToString(),
                    Number = dtrow["CNumber"].ToString(),
                    Active = Convert.ToInt32(dtrow["IsActive"])
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
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static int Delete(string Flag, string Id)
    {
        ClsDAL objDAL = new ClsDAL();
        try
        {
            int x = objDAL.Delete(2, Id, Convert.ToInt32(Flag));
            return x;
        }
        catch (Exception ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(HttpContext.Current.Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
            return -1;
        }
    }
}

public class ClsClient
{
    public int ID { get; set; }
    public string Company { get; set; }
    public string Person { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public int Active { get; set; }


}