using System;
using System.Data;
using System.Web.UI.WebControls;

public partial class UpdateSupplier : System.Web.UI.Page
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
                GetCountries();
                ID = Convert.ToInt32(Request.QueryString["ID"]);
                GetSuppliers(ID);
                ViewState["ID"] = ID;
            }

            lblmsg.Visible = false;

        }
    }

    void GetCountries()
    {
        DataSet dsclient = objDAL.ClientMGR(0, "CNT");

        if (dsclient.Tables.Count > 0)
        {
            if (dsclient.Tables[0].Rows.Count > 0)
            {
                ddlCountry.DataTextField = "Country";
                ddlCountry.DataValueField = "Id";
                ddlCountry.DataSource = dsclient;
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, new ListItem("- Select Country -", "0"));
            }
        }
    }

    void GetSuppliers(int id)
    {
        try
        {
            ddlCountry.ClearSelection();
            DataSet resultDS = null;

            resultDS = objDAL.ClientMGR(id, "S");
            if (resultDS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in resultDS.Tables[0].Rows)
                {
                    txtName.Text = Convert.ToString(dr["Name"]);
                    txtDesc.Text = Convert.ToString(dr["Description"]);
                    txtCNumber.Text = Convert.ToString(dr["Number"]);
                    txtCE.Text = Convert.ToString(dr["Email"]);
                    txtSize.Text = Convert.ToString(dr["PSize"]);
                    txtcomplete.Text = Convert.ToString(dr["Completes"]);
                    //   txtIncomplete.Text = Convert.ToString(dr["Incomplete"]);
                    txtOverquota.Text = Convert.ToString(dr["Overquota"]);
                    txtSecurity.Text = Convert.ToString(dr["Security"]);
                    txtTerminate.Text = Convert.ToString(dr["Terminate"]);
                    txtFraud.Text = Convert.ToString(dr["Fraud"]);
                    txtNotes.Text = Convert.ToString(dr["Notes"]);
                    ddlCountry.Items.FindByText(Convert.ToString(dr["Country"])).Selected = true;
                    txtSuccess.Text = Convert.ToString(dr["SUCCESS"]);
                    txtDefault.Text = Convert.ToString(dr["DEFAULT"]);
                    txtFailure.Text = Convert.ToString(dr["FAILURE"]);
                    ddlQuality.Text = Convert.ToString(dr["QUALITY TERMINATION"]);
                    ddlOverquota.Text = Convert.ToString(dr["OVER QUOTA"]);
                    //txtHashingLink.Text = Convert.ToString(dr["HashingURL"]);
                    //txtCompleteStatus.Text = Convert.ToString(dr["CompleteStatus"]);
                    //txtOtherThenComplete.Text = Convert.ToString(dr["OtherThenComplete"]);
                    //rdbAllowHashing.SelectedValue = Convert.ToString(dr["AllowHashing"]);
                    //txtAuthorizationKey.Text = Convert.ToString(dr["AuthorizationKey"]);
                }
            }

        }
        catch (SystemException ex)
        {

        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            //rdbAllowHashing.SelectedValue;
            //return;
            Status = objDAL.SupplierMgr(txtName.Text, txtDesc.Text, txtCNumber.Text, txtCE.Text, txtSize.Text, ddlCountry.SelectedItem.Text, txtNotes.Text, Convert.ToInt32(ViewState["ID"]), 2, txtcomplete.Text, txtTerminate.Text, txtOverquota.Text, txtSecurity.Text, txtFraud.Text, txtSuccess.Text, txtDefault.Text, txtFailure.Text, ddlQuality.Text, ddlOverquota.Text);

            lblmsg.Visible = true;
            if (Status == "1")
            {
                Response.Redirect("SupplierDetails.aspx", false);
                lblmsg.Text = "Supplier Updated Successfully";
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
                lblmsg.Text = "Supplier Updated Failed";
            }
        }
        catch (SystemException ex)
        {
        }
    }
}