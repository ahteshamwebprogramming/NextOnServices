using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AddSupplier : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblmsg.Visible = false;
            GetCountries();
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
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            Status = objDAL.SupplierMgr(txtName.Text, txtDesc.Text, txtCNumber.Text, txtCE.Text, txtSize.Text, ddlCountry.SelectedItem.Text, txtNotes.Text, 0, 1, txtcomplete.Text, txtTerminate.Text, txtOverquota.Text, txtSecurity.Text, txtFraud.Text, txtSuccess.Text, txtDefault.Text, txtFailure.Text, ddlQuality.Text, ddlOverquota.Text);

            lblmsg.Visible = true;
            if (Convert.ToInt32(Status) > 0)
            {
                Response.Redirect("SupplierCountryPanelSize.aspx?SID=" + Status, false);
                // lblmsg.Text = "Supplier Created Successfully";
                txtName.Text = "";
                txtDesc.Text = "";
                txtCNumber.Text = "";
                txtCE.Text = "";
                txtSize.Text = "";
                txtNotes.Text = "";
                txtOverquota.Text = "";
                //txtIncomplete.Text = "";
                txtSecurity.Text = "";
                txtFraud.Text = "";
                txtSuccess.Text = "";
                txtDefault.Text = "";
                txtFailure.Text = "";
                ddlQuality.Text = "";
                ddlOverquota.Text = "";
            }
            else
            {
                // lblmsg.Text = "Supplier Creation Failed";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Supplier Creation Failed','error')", true);
            }
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }
    }
}