using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SupplierDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsSuppliers1> GetSuppliersDetails()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsSuppliers1> returnData = new List<ClsSuppliers1>();
        try
        {

            DataSet resultDS = null;

            resultDS = objDAL.ClientMGRAll(0, "S");

            //resultDS = objDal.GetSurveyFilter(surveyID, GID);


            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {


                returnData.Add(new ClsSuppliers1
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    Name = dtrow["Name"].ToString(),
                    Description = dtrow["Description"].ToString(),
                    Email = dtrow["Email"].ToString(),
                    PSize = dtrow["PSize"].ToString(),
                    Country = dtrow["Country"].ToString(),
                    Number = dtrow["Number"].ToString(),
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
        try
        {
            int x = objDAL.Delete(3, Id, Convert.ToInt32(Flag));
            return x;
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
}

public class ClsSuppliers1
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }
    public string PSize { get; set; }
    public string Country { get; set; }
    public int Active { get; set; }



}