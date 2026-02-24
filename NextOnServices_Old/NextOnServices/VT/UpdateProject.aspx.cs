using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UpdateProject : System.Web.UI.Page
{
    int ID = 0;
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString.Count > 0)
            {
                GetClients();
                GetPM();
                GetCountries();
                ID = Convert.ToInt32(Request.QueryString["ID"]);
                GetProject(ID);
                ViewState["ID"] = ID;
            }

            lblmsg.Visible = false;

        }
    }
    void GetPM()
    {
        DataSet dsclient = objDAL.ClientMGR(0, "U");

        if (dsclient.Tables[0].Rows.Count > 0)
        {
            ddlPM.DataTextField = "UserName";
            ddlPM.DataValueField = "ID";
            ddlPM.DataSource = dsclient;
            ddlPM.DataBind();
            ddlPM.Items.Insert(0, new ListItem("- Select Project Manager -", "0"));
        }
    }
    void GetClients()
    {
        DataSet dsclient = objDAL.ClientMGR(0, "C");

        if (dsclient.Tables[0].Rows.Count > 0)
        {
            ddlclient.DataTextField = "Company";
            ddlclient.DataValueField = "ID";
            ddlclient.DataSource = dsclient;
            ddlclient.DataBind();
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
    void GetProject(int id)
    {
        try
        {
            ddlCountry.ClearSelection();
            ddlstatus.ClearSelection();
            ddlclient.ClearSelection();
            ddlPM.ClearSelection();
            DataSet resultDS = null;

            resultDS = objDAL.ClientMGR(id, "P");
            if (resultDS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in resultDS.Tables[0].Rows)
                {
                    lblPnumber.Text = Convert.ToString(dr["PID"]);
                    txtPName.Text = Convert.ToString(dr["PName"]);
                    txtDesc.Text = Convert.ToString(dr["Descriptions"]);

                    // txtPM.Text = Convert.ToString(dr["PManager"]);
                    txtLOI.Text = Convert.ToString(dr["LOI"]);
                    txtIRate.Text = Convert.ToString(dr["IRate"]);

                    txtCPI.Text = Convert.ToString(dr["CPI"]);
                    txtSSize.Text = Convert.ToString(dr["SampleSize"]);
                    txtQuota.Text = Convert.ToString(dr["Quota"]);
                    //txtSDate.Text = Convert.ToDateTime(Convert.ToString(dr["SDate"])).ToString("yyyy-MM-dd");
                    //txtEDate.Text = Convert.ToDateTime(Convert.ToString(dr["EDate"])).ToString("yyyy-MM-dd");
                    txtSDate.Text = Convert.ToString(dr["SDate"]);
                    txtEDate.Text = Convert.ToString(dr["EDate"]);


                    txtNotes.Text = Convert.ToString(dr["Notes"]);

                    ddlCountry.Items.FindByValue(Convert.ToString(dr["Country"])).Selected = true;
                    ddlclient.Items.FindByValue(Convert.ToString(dr["ClientID"])).Selected = true;
                    ddlPM.Items.FindByValue(Convert.ToString(dr["PManager"])).Selected = true;
                    ddlstatus.Items.FindByValue(Convert.ToString(dr["Status"])).Selected = true;
                    rdotype.Items.FindByValue(dr["LType"].ToString() == "" ? "1" : dr["LType"].ToString()).Selected = true;

                }
            }

        }
        catch (SystemException ex)
        {

        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        ClsProject objP = new ClsProject();
        try
        {
            objP.ID = Convert.ToInt32(ViewState["ID"]);
            objP.PName = txtPName.Text;
            objP.Descriptions = txtDesc.Text;
            objP.ClientID = Convert.ToInt32(ddlclient.SelectedItem.Value);
            objP.PManager = ddlPM.SelectedItem.Value;
            objP.LOI = txtLOI.Text;
            objP.IRate = txtIRate.Text;
            objP.CPI = Convert.ToDouble(txtCPI.Text);

            objP.SampleSize = txtSSize.Text;
            objP.Quota = txtQuota.Text;
            objP.SDate = txtSDate.Text;
            objP.EDate = txtEDate.Text;
            objP.Country = ddlCountry.SelectedItem.Value;
            //objP.Status = Convert.ToInt32(ddlstatus.SelectedItem.Value);

            objP.LType = Convert.ToInt32(rdotype.SelectedItem.Value);
            objP.Notes = txtNotes.Text;

            Status = objDAL.ProjectMgrEdit(objP, 2);
            if (Status == "1")
            {
                GetClear();
                Response.Redirect("ProjectPageDetails.aspx?Id=" + Convert.ToInt32(ViewState["ID"]), false);
            }
        }
        catch (SystemException ex)
        {
        }

    }

    void GetClear()
    {
        txtNotes.Text = "";
        txtLOI.Text = "";
        txtIRate.Text = "";
        txtEDate.Text = "";
        txtDesc.Text = "";
        txtCPI.Text = "";
        // txtPM.Text = "";
        txtPName.Text = "";
        txtQuota.Text = "";
        txtSDate.Text = "";
        txtSSize.Text = "";
        ddlclient.ClearSelection();
        ddlCountry.ClearSelection();
        ddlstatus.ClearSelection();
        ddlPM.ClearSelection();

    }
}