using System;
using System.Web.UI;

public partial class UserControls_Header : System.Web.UI.UserControl
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {

        //if (Session["UserId"] == null)
        //{
        //    Response.Redirect("Index.html");
        //}
        //if (Session["UserType"].ToString() == "A")
        //{
        //    admin_Report.Visible = true;
        //    user_admin.Visible = true;
        //    li_CompleteRedirects.Visible = true;
        //}
        //else
        //{
        //    admin_Report.Visible = false;
        //    user_admin.Visible = false;
        //    li_CompleteRedirects.Visible = false;
        //}
    }
    protected void BtnCPWD_Click(object sender, EventArgs e)
    {
        try
        {
            string UID = Session["UserId"].ToString();
            Status = objDAL.ChangePassword(UID, txtOldPwd.Text, txtNewPwd.Text);
            if (Status == "-1")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Old Password is wrong')", true);
            }
            else if (Status == "1")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Password change Successfully')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Failed transaction')", true);
            }
        }
        catch (SystemException ex)
        {
        }
    }
}