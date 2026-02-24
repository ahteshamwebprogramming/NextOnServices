using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectsStatus : System.Web.UI.Page
{
    ClsDAL objclsdal = new ClsDAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            populateddlProject();
            if (Request.QueryString.Count > 0)
            {
                string id = Request.QueryString["ID"].ToString();
                populateddlProject(id);
                ViewState["ID"] = id;
            }

        }
    }

    public void populateddlProject()
    {
        ListItem selection = new ListItem("Select", "0");
        ddlProject.DataSource = objclsdal.getProjectsName();
        ddlProject.DataTextField = "PName";
        ddlProject.DataValueField = "ID";
        ddlProject.DataBind();
        ddlProject.Items.Insert(0, selection);
    }
    public void populateddlProject(string id)
    {
        ddlProject.SelectedValue = id;
        populateddlStatus();
        btSubmit.Text = "Update";
    }
    protected void ddlProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        populateddlStatus();
    }

    public void populateddlStatus()
    {

        ddlStatus.DataSource = objclsdal.getStatusbyProjectID(ddlProject.SelectedValue);
        DataTable dt = objclsdal.getStatusbyProjectID(ddlProject.SelectedValue);
        foreach (DataRow dr in dt.Rows)
        {
            ddlStatus.SelectedValue = dr["Status"].ToString();
        }
    }
    protected void btSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            int x = objclsdal.UpdateProjectStatus(ddlStatus.SelectedValue, ddlProject.SelectedValue, "dashboard");
            if (x > 0)
            {
                if (btSubmit.Text == "Submit")
                {
                    ddlProject.SelectedValue = "0";
                    ddlStatus.SelectedValue = "0";
                    // Response.Write("<script>alert('Submitted Succesfully')</script>");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Submitted Succesfully','success')", true);
                }
                else if (btSubmit.Text == "Update")
                {
                    btSubmit.Text = "Submit";
                    Response.Redirect("ProjectPageDetails.aspx?ID=" + ViewState["ID"].ToString());
                }

            }
            else
            {
                //Response.Write("<script>alert('Error in Submitting Data')</script>");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "success('Error in Submitting Data','error')", true);
            }
        }
        catch (Exception ex)
        {

        }
    }
}


public class CLSStatus
{
    public int ID { get; set; }
    public string Status { get; set; }
}

public class CLSStatusEnum
{

}