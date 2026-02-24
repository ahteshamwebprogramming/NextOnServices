using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AddUser : System.Web.UI.Page
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

            Status = objDAL.UserMgr(txtUserID.Text, txtUN.Text, txtEmail.Text, txtPWD.Text, txtAdd.Text, txtCN.Text, ddlrole.SelectedItem.Text, 0, 1);
            lblmsg.Visible = true;
            if (Status == "1")
            {
                //lblmsg.Text = "User Created Successfully";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('User created succesfully','success')", true);
                txtCN.Text = "";
                txtUN.Text = "";
                txtEmail.Text = "";
                txtPWD.Text = "";
                txtAdd.Text = "";
                txtUserID.Text = "";
            }
            else
            {
                //lblmsg.Text = "User Creation Failed. Userid or Email Address already registered";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','User Creation Failed','error')", true);
            }
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }
    }
}