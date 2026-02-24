using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SupplierReport : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Respodents.Visible = false;
            //lblmsg.Visible = false;

            //GetCountries();
        }

    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<SupplierPStatus> GetSupplierProjectStatus(string FDate, string TDate,int CountryID)
    {
   // string TDate=""; int CountryID=0;
        ClsDAL objDAL = new ClsDAL();
        List<SupplierPStatus> returnData = new List<SupplierPStatus>();
        DataSet dsclient = objDAL.GetSupplierStatusReport(CountryID, FDate, TDate,0);

        if (dsclient.Tables[0].Rows.Count > 0)
        {

            foreach (DataRow dtrow in dsclient.Tables[0].Rows)
            {


                returnData.Add(new SupplierPStatus
                {
                    ID = Convert.ToInt32(dtrow["id"]),

                    Suppliers = Convert.ToString(dtrow["Suppliers"]),
                    Total = Convert.ToInt32(dtrow["Total"]),
                     Closed = Convert.ToInt32(dtrow["Closed"]),
                    InProgress = Convert.ToInt32(dtrow["InProgress"]),
                    OnHold = Convert.ToInt32(dtrow["OnHold"]),
                    Cancelled = Convert.ToInt32(dtrow["Cancelled"])

                });

            }


            //ddlCountry.DataTextField = "Country";
            //ddlCountry.DataValueField = "NumericCode";
            //ddlCountry.DataSource = dsclient;
            //ddlCountry.DataBind();
            //ddlCountry.Items.Insert(0, new ListItem("- Select Country -", "0"));
        }
        return returnData;
    }



    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<SupplierRespondents> GetSupplierProjectRespondents(string FDate, string TDate, int CountryID)
    {
        // string TDate=""; int CountryID=0;
        ClsDAL objDAL = new ClsDAL();
        List<SupplierRespondents> returnData = new List<SupplierRespondents>();
        DataSet dsclient = objDAL.GetSupplierStatusReport(CountryID, FDate, TDate,  1);

        if (dsclient.Tables[0].Rows.Count > 0)
        {

            foreach (DataRow dtrow in dsclient.Tables[0].Rows)
            {


                returnData.Add(new SupplierRespondents
                {
                    ID = Convert.ToInt32(dtrow["id"]),

                    Suppliers = Convert.ToString(dtrow["Suppliers"]),
                    Total = Convert.ToInt32(dtrow["Total"]),
                    Completes = Convert.ToInt32(dtrow["Completes"]),
                    Incompletes = Convert.ToInt32(dtrow["Incompletes"]),
                    Screened = Convert.ToInt32(dtrow["Screened"]),
                    Quotafull = Convert.ToInt32(dtrow["Quotafull"])

                });

            }


            //ddlCountry.DataTextField = "Country";
            //ddlCountry.DataValueField = "NumericCode";
            //ddlCountry.DataSource = dsclient;
            //ddlCountry.DataBind();
            //ddlCountry.Items.Insert(0, new ListItem("- Select Country -", "0"));
        }
        return returnData;
    }
    //void GetCountries()
    //{
    //    DataSet dsclient = objDAL.ClientMGR(0, "CNT");

    //    if (dsclient.Tables[0].Rows.Count > 0)
    //    {
    //        ddlCountry.DataTextField = "Country";
    //        ddlCountry.DataValueField = "NumericCode";
    //        ddlCountry.DataSource = dsclient;
    //        ddlCountry.DataBind();
    //        ddlCountry.Items.Insert(0, new ListItem("- Select Country -", "0"));
    //    }
    //}
    //protected void ddlstatus_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (ddlstatus.SelectedItem.Value == "S")
    //    {
    //        Respodents.Visible = false;
    //        PStatus.Visible = true;
    //    }
    //    else
    //    {
    //        Respodents.Visible = true;
    //        PStatus.Visible = false;
    //    }
    //}
}

public class SupplierPStatus
{
    public int ID { get; set; }
    public string Suppliers { get; set; }
    public int Total { get; set; }
    public int Closed { get; set; }
    public int InProgress { get; set; }
    public int OnHold { get; set; }
    public int Cancelled { get; set; }


}

public class SupplierRespondents
{
    public int ID { get; set; }
    public string Suppliers { get; set; }
    public int Total { get; set; }
    public int Completes { get; set; }
    public int Incompletes { get; set; }
    public int Screened { get; set; }
    public int Quotafull { get; set; }


}