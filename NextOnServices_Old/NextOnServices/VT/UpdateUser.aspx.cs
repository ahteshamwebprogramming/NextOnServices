using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UpdateUser : System.Web.UI.Page
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
                ID = Convert.ToInt32(Request.QueryString["ID"]);
                GetUsers(ID);
                ViewState["ID"] = ID;
            }

            lblmsg.Visible = false;

        }
    }

    void GetUsers(int id)
    {
        try
        {

            DataSet resultDS = null;

            resultDS = objDAL.ClientMGR(id, "U");
            if (resultDS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in resultDS.Tables[0].Rows)
                {
                    txtUN.Text = Convert.ToString(dr["UserName"]);
                    txtUserid.Text = Convert.ToString(dr["UserId"]);
                    txtEmail.Text = Convert.ToString(dr["EmailID"]);
                    txtPWD.Text = Convert.ToString(dr["Password"]);
                    txtCN.Text = Convert.ToString(dr["ContactNumber"]);
                    txtAdd.Text = Convert.ToString(dr["Address"]);
                    if (dr["UserType"].ToString() == "A")
                    {
                        ddlrole.SelectedValue = "1";
                    }
                    else
                    {
                        ddlrole.SelectedValue = "2";
                    }


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
            Status = objDAL.UserMgr(txtUserid.Text, txtUN.Text, txtEmail.Text, txtPWD.Text, txtAdd.Text, txtCN.Text, ddlrole.SelectedItem.Text, Convert.ToInt32(ViewState["ID"]), 2);
            lblmsg.Visible = true;
            if (Status == "1")
            {
                // lblmsg.Text = "User Updated Successfully";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('User updated succesfully','success')", true);
                txtCN.Text = "";
                txtUN.Text = "";
                txtEmail.Text = "";
                txtPWD.Text = "";
                txtAdd.Text = "";
                Response.Redirect("UsersDetails.aspx", false);
            }
            else
            {
                // lblmsg.Text = "User Updated Failed";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt('','Unable to update user','error')", true);
            }
        }
        catch (SystemException ex)
        {
        }
    }
}