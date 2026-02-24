using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Clients : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblmsg.Visible = false;
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            Status = objDAL.ClientMgr(txtCN.Text, txtCP.Text, txtCNumber.Text, txtCE.Text, ddlCountry.SelectedItem.Text, txtNotes.Text, 0, 1);
            lblmsg.Visible = true;
            if (Status == "1")
            {
                // lblmsg.Text = "Client Created Successfully";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Client created succesfully','success')", true);
                txtCN.Text = "";
                txtCP.Text = "";
                txtCNumber.Text = "";
                txtCE.Text = "";
                txtNotes.Text = "";
            }
            else
            {
                // lblmsg.Text = "Client Creation Failed";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Client Creation Failed','error')", true);

            }
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }
    }
}