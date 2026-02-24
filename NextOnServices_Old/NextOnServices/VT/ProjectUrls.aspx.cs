using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectUrls : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblmsg.Visible = false;

            GetCountries();

            if (Request.QueryString["ID"] != null)
            {
                Session["PID"] = Convert.ToInt32(Request.QueryString["ID"]);
                // Session["Projectid"] = Convert.ToInt32(Request.QueryString["ProjectID"]);
                // lblpname.Text = Convert.ToString(Request.QueryString["Name"]);

            }
            else if (Request.QueryString["IID"] != null)
            {
                Session["ID"] = Convert.ToInt32(Request.QueryString["IID"]);
                PopulateControls(Convert.ToInt32(Session["ID"]));
            }
            else
            {
                Session["PID"] = 0;
            }
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (btnSubmit.Text == "Submit")
        {
            try
            {
                //string parameterName = txtParameterName.Text;
                //string parameterValue = txtParameterValue.Text;
                //int applyToSupplier = chkApplyToSupplier.Checked == true ? 1 : 0;
                //int applyToSupplier1 = chkApplyToSupplier1.Checked == true ? 1 : 0;
                string projectURL = txtUrl.Text;
                string originalURL = txtUrl.Text;
                //if (applyToSupplier == 1)
                //{
                //    projectURL = projectURL + "&" + parameterName + "=" + parameterValue;
                //}
                if (chkTokens.Checked)
                {
                    if (txtTokensWithURL.Text == "")
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Tokens cannot be left blank','error')", true);
                    }
                    else
                    {
                        Status = objDAL.ProjectUrl(Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(Session["PID"]), projectURL, 0, 2, txtNotes.Text, txtRQ.Text, txtCPI.Text);
                        string tokensall = txtTokensWithURL.Text;
                        char[] spliton = { '\n' };
                        string[] arrtokens = txtTokensWithURL.Text.Split(spliton);


                        //if (Status == "1")
                        //{
                        //    txtUrl.Text = "";
                        //    ddlCountry.ClearSelection();
                        //    txtNotes.Text = string.Empty;
                        //    Response.Redirect("ProjectUrls.aspx?ID=" + Convert.ToInt32(Session["PID"]) + "", false);

                        //}
                        if (Status == "SingleCountryMode")
                        {
                            //lblmsg.Visible = true;
                            //lblmsg.Text = "Cannot assign more then one country with this project";
                            //lblmsg.ForeColor = System.Drawing.Color.Red;
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Cannot assign more then one country with this project','error')", true);
                        }
                        else if (Status == "duplicate")
                        {
                            //lblmsg.Visible = true;
                            //lblmsg.Text = "Try with another country.";
                            //lblmsg.ForeColor = System.Drawing.Color.Red;
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Try with another country.','error')", true);
                        }
                        else if (Status == "-1")
                        {
                            //lblmsg.Visible = true;
                            //lblmsg.Text = "Some Error Occured. Please try again later";
                            //lblmsg.ForeColor = System.Drawing.Color.Red;
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Some Error Occured. Please try again later','error')", true);
                        }
                        else
                        {
                            foreach (string singletoken in arrtokens)
                            {
                                if (singletoken != "") { DataSet resultDS = objDAL.SaveTokens("0", Status, singletoken); }
                            }

                            txtUrl.Text = "";
                            ddlCountry.ClearSelection();
                            txtNotes.Text = string.Empty;
                            Response.Redirect("ProjectUrls.aspx?ID=" + Convert.ToInt32(Session["PID"]) + "", false);
                        }

                    }
                }
                else
                {
                    Status = objDAL.ProjectUrl(Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(Session["PID"]), projectURL, 0, 1, txtNotes.Text, txtRQ.Text, txtCPI.Text);

                    if (Status == "1")
                    {
                        txtUrl.Text = "";
                        ddlCountry.ClearSelection();
                        txtNotes.Text = string.Empty;
                        Response.Redirect("ProjectUrls.aspx?ID=" + Convert.ToInt32(Session["PID"]) + "", false);

                    }
                    else if (Status == "SingleCountryMode")
                    {
                        //lblmsg.Visible = true;
                        //lblmsg.Text = "Cannot assign more then one country with this project";
                        //lblmsg.ForeColor = System.Drawing.Color.Red;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Cannot assign more then one country with this project','error')", true);
                    }
                    else if (Status == "duplicate")
                    {
                        //lblmsg.Visible = true;
                        //lblmsg.Text = "Try with another country.";
                        //lblmsg.ForeColor = System.Drawing.Color.Red;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Try with another country.','error')", true);
                    }
                    else
                    {
                        //lblmsg.Visible = true;
                        //lblmsg.Text = "Some Error Occured. Please try again later";
                        //lblmsg.ForeColor = System.Drawing.Color.Red;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Some Error Occured. Please try again later','error')", true);
                    }
                }

            }
            catch (SystemException ex)
            {
            }
        }
        else if (btnSubmit.Text == "Update")
        {
            if (txtUrl.Text != "" && ddlCountry.SelectedValue != "0")
            {
                UpdateDetails();
            }
            else
            {
                //Response.Write("<script>alert('Please fill all the details')</script>");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Please fill all the details','error')", true);
            }
        }


    }

    public void UpdateDetails()
    {
        //string parameterName = txtParameterName.Text;
        //string parameterValue = txtParameterValue.Text;
        //int applyToSupplier = chkApplyToSupplier.Checked == true ? 1 : 0;
        //int applyToSupplier1 = chkApplyToSupplier1.Checked == true ? 1 : 0;
        string projectURL = txtUrl.Text;
        string originalURL = txtUrl.Text;
        //if (applyToSupplier == 1)
        //{
        //    projectURL = projectURL + "&" + parameterName + "=" + parameterValue;
        //}
        if (chkTokens.Checked)
        {
            if (txtTokensWithURL.Text == "")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Tokens cannot be left blank','error')", true);
            }
            else
            {
                string result = objDAL.UpdateProjectURL(Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(Session["PID"]), projectURL, Convert.ToInt32(lblid.Text), 0, txtNotes.Text, txtRQ.Text, txtCPI.Text, 1);
                string tokensall = txtTokensWithURL.Text;
                char[] spliton = { '\n' };
                string[] arrtokens = txtTokensWithURL.Text.Split(spliton);
                if (Convert.ToInt32(result) > 0)
                {
                    if (Request.QueryString["IID"] != null)
                    {
                        string id = Request.QueryString["IID"].ToString();
                        foreach (string singletoken in arrtokens)
                        {
                            if (singletoken != "")
                            {
                                DataSet resultDS = objDAL.SaveTokens("0", id, singletoken);
                            }

                        }
                    }

                    txtUrl.Text = "";
                    txtNotes.Text = string.Empty;
                    txtCPI.Text = string.Empty;
                    txtRQ.Text = string.Empty;
                    txtTokensWithURL.Text = string.Empty;
                    chkTokens.Checked = false;

                    ddlCountry.ClearSelection();


                    //  Response.Redirect("ProjectUrls.aspx?ID=" + Convert.ToInt32(Session["PID"]) + "&Name=" + lblpname.Text + "", false);
                    btnSubmit.Text = "Submit";
                    // Response.Write("<script>alert('Updated Succesfully')</script>");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Updated Succesfully','success')", true);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "Populatetableonload()", true);
                }
            }
        }
        else
        {
            string result = objDAL.UpdateProjectURL(Convert.ToInt32(ddlCountry.SelectedItem.Value), Convert.ToInt32(Session["PID"]), projectURL, Convert.ToInt32(lblid.Text), 0, txtNotes.Text, txtRQ.Text, txtCPI.Text, 0);
            if (Convert.ToInt32(result) > 0)
            {
                txtUrl.Text = "";
                txtNotes.Text = string.Empty;
                txtCPI.Text = string.Empty;
                txtRQ.Text = string.Empty;

                ddlCountry.ClearSelection();
                //  Response.Redirect("ProjectUrls.aspx?ID=" + Convert.ToInt32(Session["PID"]) + "&Name=" + lblpname.Text + "", false);
                btnSubmit.Text = "Submit";
                // Response.Write("<script>alert('Updated Succesfully')</script>");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Updated Succesfully','success')", true);

            }
        }
    }

    public void PopulateControls(int id)
    {
        DataSet ds = objDAL.GetProjectUrlById(id);
        foreach (DataRow dtrow in ds.Tables[0].Rows)
        {
            if (Convert.ToInt32(dtrow["Token"]) == 1)
            {
                chkTokens.Checked = true;
            }


            txtUrl.Text = dtrow["URL"].ToString();
            txtNotes.Text = dtrow["Notes"].ToString();
            ddlCountry.SelectedValue = dtrow["CID"].ToString();
            lblid.Text = dtrow["ID"].ToString();
            txtRQ.Text = dtrow["Quota"].ToString();
            txtCPI.Text = dtrow["CPI"].ToString();
            // Label2.Text = dtrow["PID"].ToString();
            //         lblpname.Text = dtrow["PName"].ToString();
            btnSubmit.Text = "Update";
            //lblmsg.Text = "ABC";
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
            ddlCountry.Items.Insert(0, new ListItem("--Select Country--", "0"));
        }
    }



    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProjectUrl> GetProjectUrls()
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsProjectUrl> returnData = new List<ClsProjectUrl>();
        try
        {
            DataSet resultDS = null;
            //   HttpContext.Current.Session["..."]
            resultDS = objDAL.GetProjectUrl(0, Convert.ToInt32(HttpContext.Current.Session["PID"]));
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new ClsProjectUrl
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    Url = dtrow["Url"].ToString(),
                    PName = dtrow["PName"].ToString(),
                    projectid = dtrow["PID"].ToString(),
                    Country = dtrow["Country"].ToString(),
                    Notes = dtrow["Notes"].ToString(),
                    Quota = dtrow["Quota"].ToString(),
                    CPI = dtrow["CPI"].ToString(),
                    PID = Convert.ToInt32(dtrow["PID1"]),
                    Token = Convert.ToInt32(dtrow["Token"])
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
    public static List<ClsProjectUrlforUpdate> EditDetails(string Id)
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsProjectUrlforUpdate> returnData = new List<ClsProjectUrlforUpdate>();
        List<string> abc = new List<string>();

        try
        {

            DataSet resultDS = null;
            //   HttpContext.Current.Session["..."]
            resultDS = objDAL.GetProjectUrlById(Convert.ToInt32(Id));

            //resultDS = objDal.GetSurveyFilter(surveyID, GID);


            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new ClsProjectUrlforUpdate
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    Url = dtrow["Url"].ToString(),
                    PName = dtrow["PName"].ToString(),
                    Country = Convert.ToInt32(dtrow["CID"]),
                    Notes = dtrow["Notes"].ToString(),
                    Quota = dtrow["Quota"].ToString(),
                    CPI = dtrow["CPI"].ToString(),


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
    public static List<ClsToken> SaveTokens(string OPT, string PROJECTURLID, string TOKEN)
    {
        DataSet resultDS;
        ClsDAL objDAL = new ClsDAL();
        List<ClsToken> returnData = new List<ClsToken>();
        List<string> abc = new List<string>();
        try
        {
            resultDS = objDAL.SaveTokens(OPT, PROJECTURLID, TOKEN);
            if (OPT == "0")
            {
                if (resultDS.Tables.Count > 0)
                {
                    if (resultDS.Tables[0].Rows.Count > 0)
                    {
                        returnData.Add(new ClsToken
                        {
                            Message = resultDS.Tables[0].Rows[0][0].ToString()
                        });
                    }
                }

            }
            else if (OPT == "1")
            {
                if (resultDS.Tables.Count > 0)
                {
                    if (resultDS.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dtrow in resultDS.Tables[0].Rows)
                        {
                            returnData.Add(new ClsToken
                            {
                                ID = System.Convert.ToInt32(dtrow["ID"]),
                                Token = dtrow["Token"].ToString()
                            });
                        }
                    }
                }
            }

            return returnData;
        }
        catch (Exception ex)
        {
            return null;
        }

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string findprojectid(string Id)
    {
        string resultDS = "";
        ClsDAL objDAL = new ClsDAL();
        List<ClsProjectUrlforUpdate> returnData = new List<ClsProjectUrlforUpdate>();
        List<string> abc = new List<string>();

        try
        {


            //   HttpContext.Current.Session["..."]
            resultDS = objDAL.findprojectid(Convert.ToInt32(Id));



        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            resultDS = "error";
        }

        return resultDS;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string AddItem(string ITEM)
    {
        ClsDAL objDAL = new ClsDAL();
        string code = objDAL.insertcountry(ITEM);
        return code;
    }

}
public class ClsProjectUrl
{
    public int ID { get; set; }
    public string Url { get; set; }
    public string PName { get; set; }
    public string Country { get; set; }
    public string Notes { get; set; }
    public string projectid { get; set; }
    public string Quota { get; set; }
    public string CPI { get; set; }
    public int PID { get; set; }
    public int Token { get; set; }

}

public class ClsProjectUrlforUpdate
{
    public int ID { get; set; }
    public string Url { get; set; }
    public string PName { get; set; }
    public int Country { get; set; }

    public string Notes { get; set; }
    public string Quota { get; set; }
    public string CPI { get; set; }
}



