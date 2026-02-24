using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VT_QuestionsList : System.Web.UI.Page
{
    DataSet dsrecords;
    ClsDAL objDAL = new ClsDAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindColumnsDetails(0);
        }
    }

    protected void BindColumnsDetails(int ID)
    {
        //con.Open();

        dsrecords = objDAL.GetQuestions(ID);
        if (dsrecords.Tables[0].Rows.Count > 0)
        {
            gvDetails.DataSource = dsrecords;
            gvDetails.DataBind();
        }
        else
        {
            //dsrecords.Tables[0].Rows.Add(dsrecords.Tables[0].NewRow());
            gvDetails.DataSource = null;
            gvDetails.DataBind();
            //int columncount = gvDetails.Rows[0].Cells.Count;
            //gvDetails.Rows[0].Cells.Clear();
            //gvDetails.Rows[0].Cells.Add(new TableCell());
            //gvDetails.Rows[0].Cells[0].ColumnSpan = columncount;
            gvDetails.Rows[0].Cells[0].Text = "No Records Found";
        }

    }
    protected void gvDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridViewRow row = gvDetails.Rows[e.NewEditIndex];
        Label lblId = (Label)row.FindControl("lblid");
        Label lblType = (Label)row.FindControl("lbltype");
        Response.Redirect("Screening.aspx?ID=" + lblId.Text + "&Type=" + lblType.Text, false);
    }
    protected void gvDetails_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow row = gvDetails.Rows[e.RowIndex];
        Label lblId = (Label)row.FindControl("lblid");
    }
}