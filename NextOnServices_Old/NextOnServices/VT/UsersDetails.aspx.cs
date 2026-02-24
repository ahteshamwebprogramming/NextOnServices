using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UsersDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsUsers> GetUserDetails()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsUsers> returnData = new List<ClsUsers>();
        try
        {

            DataSet resultDS = null;

            resultDS = objDAL.ClientMGRAll(0, "U");

            //resultDS = objDal.GetSurveyFilter(surveyID, GID);


            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {


                returnData.Add(new ClsUsers
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    Name = dtrow["UserName"].ToString(),
                    Email = dtrow["EmailID"].ToString(),
                    Password = dtrow["Password"].ToString(),
                    Address = dtrow["Address"].ToString(),
                    Number = dtrow["ContactNumber"].ToString(),
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
    public static int Delete(string Flag, string Id)
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsUsers> returnData = new List<ClsUsers>();
        try
        {
            DataSet resultDS = null;
            int x = objDAL.Delete(1, Id, Convert.ToInt32(Flag));
            return x;
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return -1;
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static int ChangePassword(int opt, string oldpassword, string newpassword)
    {
        ClsDAL objDAL = new ClsDAL();
        string sessionpassword = HttpContext.Current.Session["pwd"].ToString();
        if (sessionpassword == oldpassword)
        {
            string username = HttpContext.Current.Session["UN"].ToString();
            string userid = HttpContext.Current.Session["UserId"].ToString();
            List<ClsUsers> returnData = new List<ClsUsers>();
            try
            {
                DataSet resultDS = null;
                int x = objDAL.ChangePasswordd(opt, oldpassword, newpassword, username, userid);
                return x;
            }
            catch (Exception ex)
            {
                // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
                return -1;
            }
        }
        else
        {
            return -2;
        }
    }
}

public class ClsUsers
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public int Active { get; set; }




}