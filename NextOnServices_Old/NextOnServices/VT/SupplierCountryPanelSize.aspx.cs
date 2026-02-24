using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SupplierCountryPanelSize : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblmsg.Visible = false;

            GetCountries();

            if (Request.QueryString.Count > 0)
            {
                Session["SID"] = Convert.ToInt32(Request.QueryString["SID"]);
            }
            else
            {
                Session["SID"] = 0;
            }
        }
    }

    void GetCountries()
    {
        DataSet dsclient = objDAL.ClientMGR(0, "CNT");

        if (dsclient.Tables[0].Rows.Count > 0)
        {
            ddlCountry.DataTextField = "Country";
            ddlCountry.DataValueField = "Id";
            ddlCountry.DataSource = dsclient;
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem("- Select Country -", "0"));
        }
    }



    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsPanelSize> GetSamplePanelSize()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsPanelSize> returnData = new List<ClsPanelSize>();
        try
        {

            DataSet resultDS = null;
            //   HttpContext.Current.Session["..."]
            resultDS = objDAL.GetSupplierPanelSize(0, Convert.ToInt32(HttpContext.Current.Session["SID"]));

            //resultDS = objDal.GetSurveyFilter(surveyID, GID);


            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {


                returnData.Add(new ClsPanelSize
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    PSize = dtrow["PSize"].ToString(),
                    Name = dtrow["Name"].ToString(),
                    Country = dtrow["Country"].ToString()


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
    public static List<ClsPanelSize> FetchDetailsbyId(string id)
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsPanelSize> returnData = new List<ClsPanelSize>();
        try
        {
            DataSet resultDS = null;
            resultDS = objDAL.GetSupplierPanelSize(Convert.ToInt32(id), 0);
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new ClsPanelSize
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    PSize = dtrow["PSize"].ToString(),
                    Name = dtrow["SupplierID"].ToString(),
                    Country = dtrow["CountryID"].ToString()
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
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (hfPanelButtonval.Value == "Submit")
            {
                Status = objDAL.CountryPanelSize(Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(txtSize.Text), Convert.ToInt32(Session["SID"]), 0, 1);

                if (Convert.ToInt32(Status) > 0)
                {
                    txtSize.Text = "";
                    ddlCountry.ClearSelection();
                    Response.Redirect("SupplierCountryPanelSize.aspx?SID=" + Convert.ToInt32(Session["SID"]));
                }
            }
            else if (hfPanelButtonval.Value == "Update")
            {
                Status = objDAL.CountryPanelSize(Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(txtSize.Text), Convert.ToInt32(Session["SID"]), Convert.ToInt32(hfPanelIndividualID.Value), 2);
                if (Convert.ToInt32(Status) > 0)
                {
                    txtSize.Text = "";
                    ddlCountry.ClearSelection();
                    Response.Redirect("SupplierCountryPanelSize.aspx?SID=" + Convert.ToInt32(Session["SID"]));
                }
            }
        }
        catch (SystemException ex)
        {
        }
    }
}
public class ClsPanelSize
{
    public int ID { get; set; }
    public string PSize { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
}