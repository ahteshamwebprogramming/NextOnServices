using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectMapping : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    private static Random random = new Random();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString.Count > 0)
            {
                string id = Request.QueryString["Id"].ToString();
                GetProjects();
                if (id != "")
                {
                    ddlPN.SelectedValue = id;
                    GetCountries(Convert.ToInt32(id));
                    GetSupplier();
                }

            }
            else
            {
                lblmsg.Visible = false;
                GetProjects();
                // GetProjectMapping(0);
                GetSupplier();
                Session["PID"] = "0";
            }

        }
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            // ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Working on this part and will share today EOD');", true);
            string KID = RandomString(32);
            string SID = RandomString(8);
            string MUrl = ConfigurationSettings.AppSettings["MaskingUrl"].ToString() + "?SID=" + SID + "&ID=XXXXXXXXXX";
            ////string OUrl = txtSLink.Text + "?SID=" + SID + "&Code=" + KID;
            //string OUrl = OrgUrl.Value + "?ID=" + KID;
            string OUrl = OrgUrl.Value;
            // string MUrl = ConfigurationSettings.AppSettings["MaskingUrl"].ToString();
            Session["PID"] = Convert.ToInt32(ddlPN.SelectedItem.Value);
            lblmsg.Visible = true;

            int addHashing = chkAddHashing.Checked == true ? 1 : 0;

            Status = objDAL.ProjectMapping(Convert.ToInt32(ddlPN.SelectedItem.Value), Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(ddlsupplier.SelectedItem.Value), OUrl, MUrl, SID, KID, 0, 1, Convert.ToInt32(txtRCQ.Text), float.Parse(txtCPI.Text), txtNotes.Text, addHashing, txtParameterName.Text, txtHashingType.SelectedValue);
            if (Status == "-1")
            {
                //lblmsg.Text = "Project already aligned with this Supplier";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Project already aligned with this Supplier','warning')", true);
            }
            else if (Status == "-2")
            {
                //lblmsg.Text = "Project not aligned more then 5 countries";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Project not aligned more then 5 countries','warning')", true);
            }
            else if (Status == "-3")
            {
                //lblmsg.Text = "Project aligned not aligned more then one country";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Project aligned not aligned more then one country','warning')", true);
            }
            else if (Status == "1")
            {
                // txtSLink.Text = "";
                ddlPN.ClearSelection();
                ddlCountry.ClearSelection();
                ddlsupplier.ClearSelection();
                txtCPI.Text = string.Empty;
                txtRCQ.Text = string.Empty;
                txtNotes.Text = string.Empty;

                //lblmsg.Text = "Project aligned with this Supplier successfully";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Project aligned with this Supplier successfully','success')", true);
            }
            else
            {
                lblmsg.Text = "Transaction Failed";
            }
            GetProjectMapping(Convert.ToInt32(ddlPN.SelectedItem.Value));

        }
        catch (SystemException ex)
        {
        }

    }

    void GetProjects()
    {
        DataSet dsProject = objDAL.ClientMGR(0, "P");
        // ddlPN.Items.Add(new ListItem("-- Select --", "0"));
        if (dsProject.Tables[0].Rows.Count > 0)
        {
            ddlPN.DataTextField = "PCode";
            ddlPN.DataValueField = "ID";
            ddlPN.DataSource = dsProject;
            ddlPN.DataBind();
            ddlPN.Items.Insert(0, new ListItem("- Select Project -", "0"));

        }
    }

    void GetSupplier()
    {
        DataSet dsProject = objDAL.ClientMGR(0, "S");
        // ddlPN.Items.Add(new ListItem("-- Select --", "0"));
        if (dsProject.Tables[0].Rows.Count > 0)
        {
            ddlsupplier.DataTextField = "Name";
            ddlsupplier.DataValueField = "ID";
            ddlsupplier.DataSource = dsProject;
            ddlsupplier.DataBind();
            ddlsupplier.Items.Insert(0, new ListItem("- Select Supplier -", "0"));

        }
    }
    void GetCountries(int PID)
    {
        DataSet dsclient = objDAL.ClientMGR(PID, "Url");

        if (dsclient.Tables[0].Rows.Count > 0)
        {

            ddlCountry.DataTextField = "Country";
            ddlCountry.DataValueField = "CID";
            ddlCountry.DataSource = dsclient;
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem("- Select Country -", "0"));
        }
    }

    void GetProjectMapping(int PID)
    {
        DataSet dsProject = objDAL.GetProjectMappings(PID);
        //  ddlPN.Items.Add(new ListItem("-- Select --", "0"));
        if (dsProject.Tables[0].Rows.Count > 0)
        {
            grdlink.DataSource = dsProject;
            grdlink.DataBind();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProjectMapping> GetProjectsDetails()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsProjectMapping> returnData = new List<ClsProjectMapping>();
        try
        {
            // int PID = Convert.ToInt32(HttpContext.Current.Session["PID"]);
            DataSet resultDS = null;
            resultDS = objDAL.GetProjectMappings(Convert.ToInt32(HttpContext.Current.Session["PID"]));
            //resultDS = objDal.GetSurveyFilter(surveyID, GID);
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new ClsProjectMapping
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    ProjectID = System.Convert.ToInt32(dtrow["projectid"]),
                    PNumber = dtrow["PID"].ToString(),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    PName = dtrow["PName"].ToString(),
                    Country = dtrow["Country"].ToString(),
                    Supplier = dtrow["Name"].ToString(),
                    OLink = dtrow["OLink"].ToString(),
                    MLink = dtrow["MLink"].ToString(),
                    //   Code = dtrow["Code"].ToString(),
                    Respoondants = dtrow["Respondants"].ToString(),
                    Status = System.Convert.ToInt32(dtrow["Status"]),
                    Notes = dtrow["Notes"].ToString()
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
    public static List<ClsProjectMapping> GetProjectsDetailsbyid(string PID)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsProjectMapping> returnData = new List<ClsProjectMapping>();
        try
        {
            // int PID = Convert.ToInt32(HttpContext.Current.Session["PID"]);
            DataSet resultDS = null;
            resultDS = objDAL.GetProjectMappings(Convert.ToInt32(PID));
            //resultDS = objDal.GetSurveyFilter(surveyID, GID);
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new ClsProjectMapping
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    ProjectID = System.Convert.ToInt32(dtrow["projectid"]),
                    PNumber = dtrow["PID"].ToString(),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    PName = dtrow["PName"].ToString(),
                    Country = dtrow["Country"].ToString(),
                    Supplier = dtrow["Name"].ToString(),
                    OLink = dtrow["OLink"].ToString(),
                    MLink = dtrow["MLink"].ToString(),
                    //   Code = dtrow["Code"].ToString(),
                    Respoondants = dtrow["Respondants"].ToString(),
                    Status = System.Convert.ToInt32(dtrow["Status"]),
                    Notes = dtrow["Notes"].ToString()
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
    public static string updatestatus(string id, string action)
    {
        ClsDAL objDAL = new ClsDAL();
        try
        {
            string x = objDAL.Updatestatussupplierprojects(id, action, "");
            return x.ToString();
        }
        catch (Exception ex)
        {
            return "There is some error. Please contact administrator";
        }
    }
    protected void ddlPN_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Convert.ToInt32(ddlPN.SelectedItem.Value) > 0)
        {
            GetCountries(Convert.ToInt32(ddlPN.SelectedItem.Value));
        }
    }
    protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Convert.ToInt32(ddlCountry.SelectedItem.Value) > 0)
        {
            DataSet dsclient = objDAL.GetProjectsUrl(Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(ddlPN.SelectedItem.Value));

            if (dsclient.Tables[0].Rows.Count > 0)
            {
                OrgUrl.Value = Convert.ToString(dsclient.Tables[0].Rows[0]["Url"]);
            }
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<Suppliers> GetRedirects(string ID)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<Suppliers> returnData = new List<Suppliers>();
        try
        {
            // int PID = Convert.ToInt32(HttpContext.Current.Session["PID"]);
            DataSet resultDS = null;
            resultDS = objDAL.GetRedirects(Convert.ToInt32(ID));
            //resultDS = objDal.GetSurveyFilter(surveyID, GID);
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new Suppliers
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    Name = dtrow["Name"].ToString(),
                    Description = dtrow["Description"].ToString(),
                    Number = dtrow["Number"].ToString(),
                    Email = dtrow["Email"].ToString(),
                    PSize = dtrow["PSize"].ToString(),
                    Country = dtrow["Country"].ToString(),
                    Notes = dtrow["Notes"].ToString(),
                    Completes = dtrow["Completes"].ToString(),
                    Terminate = dtrow["Terminate"].ToString(),
                    Quotafull = dtrow["Quotafull"].ToString(),
                    Screened = dtrow["Screened"].ToString(),
                    Overquota = dtrow["Overquota"].ToString(),
                    Incomplete = dtrow["Incomplete"].ToString(),
                    Security = dtrow["Security"].ToString(),
                    Fraud = dtrow["Fraud"].ToString(),
                    SStatus = System.Convert.ToInt32(dtrow["SStatus"]),
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

}

public class ClsProjectMapping
{
    public int ID { get; set; }
    public int ProjectID { get; set; }
    public string PName { get; set; }
    public string Country { get; set; }
    public string Supplier { get; set; }
    public string OLink { get; set; }
    public string MLink { get; set; }
    public string Code { get; set; }
    public string Respoondants { get; set; }
    public string PNumber { get; set; }
    public int Status { get; set; }
    public string Notes { get; set; }
}