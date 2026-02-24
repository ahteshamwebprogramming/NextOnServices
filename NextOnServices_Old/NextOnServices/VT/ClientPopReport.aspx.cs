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

public partial class ClientPopReport : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString.Count > 0)
            {
                int ID = Convert.ToInt32(Request.QueryString["Id"]);
                GetRecords(ID);
            }
        }
    }

    void GetRecords(int ID)
    {
        try
        {
            DataSet dsRecords = objDAL.GetClientViewDetails(ID);
            if (dsRecords.Tables.Count > 0)
            {
                if (dsRecords.Tables[0].Rows.Count > 0)
                {
                    //GrdClientDetails.DataSource = dsRecords.Tables[0];
                    //GrdClientDetails.DataBind();
                    DataTable dt = dsRecords.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        lblClientName.Text = dr["Company"].ToString();
                        lblContactNumber.Text = dr["CNumber"].ToString();
                        lblCountry.Text = dr["Country"].ToString();
                        lblEmail.Text = dr["CEmail"].ToString();
                    }
                }
                if (dsRecords.Tables[1].Rows.Count > 0)
                {
                    GrdOverview.DataSource = dsRecords.Tables[1];
                    GrdOverview.DataBind();
                }
                //if (dsRecords.Tables[2].Rows.Count > 0)
                //{
                //    GrdSupplierDetails.DataSource = dsRecords.Tables[2];
                //    GrdSupplierDetails.DataBind();
                //}
                //if (dsRecords.Tables[3].Rows.Count > 0)
                //{
                //    GrdSurveyLinks.DataSource = dsRecords.Tables[3];
                //    GrdSurveyLinks.DataBind();
                //}
            }
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }
    }

    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public static List<ClsProjectPageDetails> GetDetails(string ID)
    //{
    //    //  Session["StdyID"] = surveyID;
    //    ClsDAL objDAL = new ClsDAL();
    //    List<ClsProjectPageDetails> returnData = new List<ClsProjectPageDetails>();
    //    try
    //    {

    //        DataSet resultDS = null;

    //        resultDS = objDAL.PPDManagement(Convert.ToInt32(ID));

    //        //resultDS = objDal.GetSurveyFilter(surveyID, GID);


    //        foreach (DataRow dtrow in resultDS.Tables[0].Rows)
    //        {


    //            returnData.Add(new ClsProjectPageDetails
    //            {
    //                ID = System.Convert.ToInt32(dtrow["ID"]),
    //                //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
    //                PName = dtrow["PName"].ToString(),


    //            });

    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
    //        return returnData;
    //    }

    //    return returnData;
    //}
}
//public class ClsSuppliers2
//{
//    public int ID { get; set; }
//    public string Name { get; set; }
//    public string Description { get; set; }
//    public string Number { get; set; }
//    public string Email { get; set; }
//    public string PSize { get; set; }
//    public string Country { get; set; }



//}