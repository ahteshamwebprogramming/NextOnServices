using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VT_Screening : System.Web.UI.Page
{
    DataSet dsrecords;
    ClsDAL objDAL = new ClsDAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["QID"] = 0;
            //  ViewState["ID"] = 0;
            dvoptions.Visible = false;


            // lblUser.Text = Convert.ToString(Session["UserName"]);
            //ShowStudyDetails();
            if (Request.QueryString.Count > 0)
            {
                ViewState["QID"] = Request.QueryString["ID"];
                int QType = Convert.ToInt32(Request.QueryString["Type"]);
                GetQuestionDetails(Convert.ToInt32(ViewState["QID"]));
                // BindColumnsDetails(Convert.ToInt32(ViewState["ID"]));
                if (QType == 3)
                {
                    dvoptions.Visible = false;
                }
                else
                {
                    dvoptions.Visible = true;
                }
                //  lblproject.Text = Convert.ToString(Session["Project"]);
            }
            BindColumnsDetails(Convert.ToInt32(ViewState["QID"]));
        }

    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            // return;
            string st = objDAL.PreScreening(0, txtQuestion.Text, QLabel.Text, Convert.ToInt32(ddlQType.SelectedItem.Value), 1);

            if (Convert.ToInt32(st) > 0)
            {
                btnSubmit.Visible = false;
                ViewState["QID"] = st;
                if (Convert.ToInt32(ddlQType.SelectedItem.Value) == 3)
                {
                    dvoptions.Visible = false;
                    btnSubmit.Visible = true;
                }
                else
                {
                    dvoptions.Visible = true;
                }
                txtQuestion.Text = "";
                ddlQType.ClearSelection();
                QLabel.Text = "";
                //lblmsg.Text = "Option added";
            }

            else if (Convert.ToInt32(st) > 0)
            {
                dvoptions.Visible = false;
                lblmsg.Text = "Question ID already existing";
            }
            else
            {
                dvoptions.Visible = false;
                lblmsg.Text = "transaction failed";
            }

        }
        catch (SystemException ex)
        {
            // objLog.WriteErrorLog(ex.Message.ToString() + "|" + "Error in cDAL class in InsertKeywordsDetail()");
            //return "Trandaction failed";
        }
    }
    protected void BindColumnsDetails(int ID)
    {
        //con.Open();

        dsrecords = objDAL.GetOptions(ID);
        if (dsrecords.Tables[0].Rows.Count > 0)
        {
            gvDetails.DataSource = dsrecords;
            gvDetails.DataBind();
        }
        else
        {
            dsrecords.Tables[0].Rows.Add(dsrecords.Tables[0].NewRow());
            gvDetails.DataSource = dsrecords;
            gvDetails.DataBind();
            int columncount = gvDetails.Rows[0].Cells.Count;
            gvDetails.Rows[0].Cells.Clear();
            gvDetails.Rows[0].Cells.Add(new TableCell());
            gvDetails.Rows[0].Cells[0].ColumnSpan = columncount;
            gvDetails.Rows[0].Cells[0].Text = "No Records Found";
        }

    }


    protected void GetQuestionDetails(int ID)
    {
        //con.Open();
        ddlQType.ClearSelection();
        dsrecords = objDAL.GetQuestions(ID);
        if (dsrecords.Tables[0].Rows.Count > 0)
        {
            txtQuestion.Text = Convert.ToString(dsrecords.Tables[0].Rows[0]["QuestionID"]);
            QLabel.Text = Convert.ToString(dsrecords.Tables[0].Rows[0]["QuestionLabel"]);
            txtQuestion.Text = Convert.ToString(dsrecords.Tables[0].Rows[0]["QuestionID"]);
            for (int i = 0; i < ddlQType.Items.Count; i++)
            {
                if (ddlQType.Items[i].Value == Convert.ToString(dsrecords.Tables[0].Rows[0]["QuestionType"]).Trim())
                {
                    ddlQType.Items[i].Selected = true;
                }
            }
            //  ddlQType.Items.FindByValue(Convert.ToString(dsrecords.Tables[0].Rows[0]["QuestionType"]));
            // gvDetails.DataSource = dsrecords;
            // gvDetails.DataBind();
        }
        //else
        //{
        //    dsrecords.Tables[0].Rows.Add(dsrecords.Tables[0].NewRow());
        //    gvDetails.DataSource = dsrecords;
        //    gvDetails.DataBind();
        //    int columncount = gvDetails.Rows[0].Cells.Count;
        //    gvDetails.Rows[0].Cells.Clear();
        //    gvDetails.Rows[0].Cells.Add(new TableCell());
        //    gvDetails.Rows[0].Cells[0].ColumnSpan = columncount;
        //    gvDetails.Rows[0].Cells[0].Text = "No Records Found";
        //}

    }
    protected void gvDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("AddNew"))
        {

            TextBox txtNewvalue = (TextBox)gvDetails.FooterRow.FindControl("txtNewvalue");
            TextBox txtnewtext = (TextBox)gvDetails.FooterRow.FindControl("txtnewtext");


            //objproperty.Value = Convert.ToInt32(txtNewvalue.Text);
            //objproperty.ColumnName = lblquestion.Text.Trim();
            //objproperty.ColumnLabel = txtnewtext.Text.Trim();

            //objproperty.StudyID = Convert.ToInt64(Session["StudyId"]);

            //objproperty.Action = 0;
            string st = objDAL.MgrQuestionsOptions(0, Convert.ToInt32(ViewState["QID"]), txtnewtext.Text, txtNewvalue.Text, 1);

            if (st == "1")
            {
                lblmsg.Text = "Option added";
            }
            if (st == "0")
            {
                lblmsg.Text = "Option failed";
            }
            if (st == "-2")
            {
                lblmsg.Text = "This option already existing";
            }

            BindColumnsDetails(Convert.ToInt32(ViewState["QID"]));
        }
    }

    protected void gvDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvDetails.EditIndex = e.NewEditIndex;
        BindColumnsDetails(Convert.ToInt32(ViewState["QID"]));
    }

    protected void gvDetails_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblid = (Label)gvDetails.Rows[e.RowIndex].FindControl("lblid");
        TextBox txtNewvalue = (TextBox)gvDetails.Rows[e.RowIndex].FindControl("txtvalue");
        TextBox txtnewtext = (TextBox)gvDetails.Rows[e.RowIndex].FindControl("txttext");

        string st = objDAL.MgrQuestionsOptions(Convert.ToInt32(lblid.Text), Convert.ToInt32(ViewState["QID"]), txtnewtext.Text, txtNewvalue.Text, 2);

        gvDetails.EditIndex = -1;
        BindColumnsDetails(Convert.ToInt32(ViewState["QID"]));
    }

    protected void gvDetails_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvDetails.EditIndex = -1;
        BindColumnsDetails(Convert.ToInt32(ViewState["QID"]));

    }

    protected void gvDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void gvDetails_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        //objproperty.Id = Convert.ToInt32(gvDetails.DataKeys[e.RowIndex].Value);
        //objproperty.Action = 2;
        //objdal.AddVariables(objproperty);

        //gvDetails.EditIndex = -1;
        //BindColumnsDetails();
        Label lblid = (Label)gvDetails.Rows[e.RowIndex].FindControl("lblid");
        //TextBox txtNewvalue = (TextBox)gvDetails.Rows[e.RowIndex].FindControl("txtvalue");
        //TextBox txtnewtext = (TextBox)gvDetails.Rows[e.RowIndex].FindControl("txttext");

        string st = objDAL.MgrQuestionsOptions(Convert.ToInt32(lblid.Text), Convert.ToInt32(ViewState["QID"]), "", "", 3);

        gvDetails.EditIndex = -1;
        BindColumnsDetails(Convert.ToInt32(ViewState["QID"]));

    }
}