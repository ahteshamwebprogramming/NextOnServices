using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UpdateClient : System.Web.UI.Page
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
                GetClient(ID);
                ViewState["ID"] = ID;
            }

             lblmsg.Visible = false;
           
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            Status = objDAL.ClientMgr(txtCN.Text, txtCP.Text, txtCNumber.Text, txtCE.Text, ddlCountry.SelectedItem.Text, txtNotes.Text,Convert.ToInt32(ViewState["ID"]),2);
            lblmsg.Visible = true;
            if (Status == "1")
            {
                lblmsg.Text = "Client Updated Successfully";

                txtCN.Text = "";
                txtCP.Text = "";
                txtCNumber.Text = "";
                txtCE.Text = "";
                txtNotes.Text = "";
                Response.Redirect("ClientsDetails.aspx", false);
            }
            else
            {
                lblmsg.Text = "Client Creation Failed";
            }
        }
        catch (SystemException ex)
        {
        }

    }

    void GetClient(int id)
    {
        try
        {
            ddlCountry.ClearSelection();
            DataSet resultDS = null;

            resultDS = objDAL.ClientMGR(id, "C");
            if (resultDS.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in resultDS.Tables[0].Rows)
                {
                    txtCN.Text = Convert.ToString(dr["Company"]);
                    txtCP.Text = Convert.ToString(dr["CPerson"]);
                    txtCNumber.Text = Convert.ToString(dr["CNumber"]);
                    txtCE.Text = Convert.ToString(dr["CEmail"]);
                    txtNotes.Text = Convert.ToString(dr["Notes"]);
                    ddlCountry.Items.FindByText(Convert.ToString(dr["Country"])).Selected = true;

                }
            }
          
        }
        catch (SystemException ex)
        {

        }
    }
}