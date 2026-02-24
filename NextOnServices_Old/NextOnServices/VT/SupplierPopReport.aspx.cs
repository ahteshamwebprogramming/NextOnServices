using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SupplierPopReport : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString.Count > 0)
            {
                int ID = Convert.ToInt32(Request.QueryString["Id"]);
                GetRecords(ID);
            }
        }
    }

    void GetRecords(int PID)
    {
        try
        {
            DataSet dsRecords = objDAL.GetSupplierViewDetails(PID);
            if (dsRecords.Tables.Count > 0)
            {
                if (dsRecords.Tables[0].Rows.Count > 0)
                {
                    //GrdSupplierDetails.DataSource = dsRecords.Tables[0];
                    //GrdSupplierDetails.DataBind();
                    DataTable dt = dsRecords.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        lblName.Text=dr["Name"].ToString();
                        lblDiscription.Text=dr["Description"].ToString();
                        lblEmail.Text=dr["Email"].ToString();
                        lblNumber.Text=dr["Number"].ToString();
                        hfSuppID.Value = dr["ID"].ToString();
                    }
                }
                if (dsRecords.Tables[1].Rows.Count > 0)
                {
                    GrdSupplierSpecs.DataSource = dsRecords.Tables[1];
                    GrdSupplierSpecs.DataBind();
                }
                if (dsRecords.Tables[2].Rows.Count > 0)
                {
                    GrdOverview.DataSource = dsRecords.Tables[2];
                    GrdOverview.DataBind();
                }
                if (dsRecords.Tables[3].Rows.Count > 0)
                {
                    GrdSurveyLinks.DataSource = dsRecords.Tables[3];
                    GrdSurveyLinks.DataBind();
                }
            }
        }
        catch (SystemException ex)
        {
        }
    }


}


public class ClsSuppliers
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Number { get; set; }
    public string Email { get; set; }
    public string PSize { get; set; }
    public string Country { get; set; }



}