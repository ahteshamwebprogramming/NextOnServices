using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AddClient : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {
            lblmsg.Visible = false;
        }

        //   btnSubmit.Attributes.Add("Onclick", "return validation();");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
           
            // Status = objDAL.AddClient(txtCN.Text, txtCP.Text, txtCNumber.Text, txtCE.Text, ddlCountry.SelectedItem.Text, txtNotes.Text);
            lblmsg.Visible = true;
            if (Status == "1")
            {
                lblmsg.Text = "Client Created Successfully";

                txtCN.Text = "";
                txtCP.Text = "";
                txtCNumber.Text = "";
                txtCE.Text = "";
                txtNotes.Text = "";
            }
            else
            {
                lblmsg.Text = "Client Creation Failed";
            }
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());

        }
    }
}