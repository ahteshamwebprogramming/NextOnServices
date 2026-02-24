using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VT_RecontactPageDetails : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString.Count > 0)
            {
                int ID = Convert.ToInt32(Request.QueryString["Id"]);
                GetRecontactParameters(ID);
                hfPID.Value = ID.ToString();
            }
        }
    }
    void GetRecontactParameters(int id)
    {
        ClsDAL objDal = new ClsDAL();
        DataSet ds = objDal.GetRecontactParametersCalc(id, 0);
        if (ds.Tables[0].Rows.Count > 0)
        {
            lblCompletion.Text = ds.Tables[0].Rows[0]["status"].ToString();
            spncompletes.InnerText = ds.Tables[0].Rows[0]["Complete"].ToString();
            spnTotalcomplete.InnerText = ds.Tables[0].Rows[0]["total"].ToString();
            spnpercentIR.InnerText = ds.Tables[0].Rows[0]["ActIRPercent"].ToString();
            spnLOI.InnerText = ds.Tables[0].Rows[0]["ActLOI"].ToString();
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string UpdateRecontactProjects(Recontact formdata)
    {
        try
        {
            ClsDAL obj = new ClsDAL();
            string result = obj.UpdateRecontactProjects(formdata, 0);
            return result;
        }
        catch (Exception e) { return "error"; }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object FetchRecontactProjects(string ID, int OPT)
    {
        try
        {
            ClsDAL objDAL = new ClsDAL();
            List<Recontact> returnCountryWise = new List<Recontact>();
            List<Recontact> returnSupplierWise = new List<Recontact>();
            List<ClsRedirects> returnRedirects = new List<ClsRedirects>();
            List<Recontact> returnRemainingDetails = new List<Recontact>();
            DataSet dsclient = objDAL.FetchRecontactProjects(ID, OPT);
            if (dsclient.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dtrow in dsclient.Tables[0].Rows)
                {
                    returnCountryWise.Add(new Recontact
                    {
                        ID = System.Convert.ToInt32(dtrow["Id"]),
                        Country = dtrow["countryname"].ToString(),
                        Total = Convert.ToInt32(dtrow["Total"]),
                        Complete = Convert.ToInt32(dtrow["Complete"]),
                        Terminate = Convert.ToInt32(dtrow["Terminate"]),
                        Overquota = Convert.ToInt32(dtrow["Overquota"]),
                        S_term = Convert.ToInt32(dtrow["s_term"]),
                        F_error = Convert.ToInt32(dtrow["f_error"]),
                        Incomplete = Convert.ToInt32(dtrow["incomplete"]),
                        Cancelled = Convert.ToInt32(dtrow["cancelled"])
                    });
                }
            }
            if (dsclient.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dtrow in dsclient.Tables[1].Rows)
                {
                    returnSupplierWise.Add(new Recontact
                    {
                        ID = System.Convert.ToInt32(dtrow["Id"]),
                        Supplier = dtrow["suppliername"].ToString(),
                        Total = Convert.ToInt32(dtrow["Total"]),
                        Complete = Convert.ToInt32(dtrow["Complete"]),
                        Terminate = Convert.ToInt32(dtrow["Terminate"]),
                        Overquota = Convert.ToInt32(dtrow["Overquota"]),
                        S_term = Convert.ToInt32(dtrow["s_term"]),
                        F_error = Convert.ToInt32(dtrow["f_error"]),
                        Incomplete = Convert.ToInt32(dtrow["incomplete"]),
                        Cancelled = Convert.ToInt32(dtrow["cancelled"])
                    });
                }
            }
            if (dsclient.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow dtrow in dsclient.Tables[2].Rows)
                {
                    returnRedirects.Add(new ClsRedirects
                    {
                        ID = System.Convert.ToInt32(dtrow["Id"]),
                        Complete = dtrow["Complete"].ToString(),
                        Terminate = dtrow["Terminate"].ToString(),
                        Overquota = dtrow["Overquota"].ToString(),
                        S_Term = dtrow["S_Term"].ToString(),
                        F_Error = dtrow["F_Error"].ToString(),
                        Var1 = dtrow["Variable1"].ToString(),
                        Var2 = dtrow["Variable2"].ToString()
                    });
                }
            }
            if (dsclient.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow dtrow in dsclient.Tables[3].Rows)
                {
                    returnRemainingDetails.Add(new Recontact
                    {
                        ID = Convert.ToInt32(dtrow["Id"]),
                        PID = dtrow["ProjectNumber"].ToString(),
                        RecontactName = dtrow["RProjectName"].ToString(),
                        RecontactDescription = dtrow["RProjectDescription"].ToString(),
                        LOI = Convert.ToInt32(dtrow["LOI"]),
                        CPI = float.Parse(dtrow["CPI"].ToString()),
                        IR = Convert.ToInt32(dtrow["IR"]),
                        RCQ = Convert.ToInt32(dtrow["RCQ"]),
                        StatusStr = dtrow["Status"].ToString(),
                        Notes = dtrow["Notes"].ToString()
                    });
                }
            }
            object[] obj = new object[4];
            obj[0] = returnCountryWise;
            obj[1] = returnSupplierWise;
            obj[2] = returnRedirects;
            obj[3] = returnRemainingDetails;
            return obj;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    protected void btnDownloadData_Click(object sender, EventArgs e)
    {
        exporttoExcel();
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        //base.VerifyRenderingInServerForm(control);
    }
    public void exporttoExcel()
    {
        string pid = hflblSurveyID.Value;
        string name = hflblSurvayname.Value;
        ClsDAL objDAL = new ClsDAL();
        Response.Clear();
        Response.Buffer = true;
        Response.ClearContent();
        Response.ClearHeaders();
        Response.Charset = "";
        string FileName = pid + "_" + name + ": " + DateTime.Now + ".xls";
        StringWriter strwritter = new StringWriter();
        HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
        StringWriter strwritter1 = new StringWriter();
        HtmlTextWriter htmltextwrtter1 = new HtmlTextWriter(strwritter1);
        StringWriter strwritter2 = new StringWriter();
        HtmlTextWriter htmltextwrtter2 = new HtmlTextWriter(strwritter2);
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
        DataSet ds = objDAL.GetProjectDetailsForDownload(Convert.ToInt32(hfPID.Value), 1);
        gdvProjectDownloadDetails.DataSource = ds;
        gdvProjectDownloadDetails.DataBind();
        gdvProjectDownloadDetails.GridLines = GridLines.Both;
        gdvProjectDownloadDetails.HeaderStyle.Font.Bold = true;
        gdvProjectDownloadDetails.RenderControl(htmltextwrtter);
        //string headerTable1 = @"<table width='100%' class='TestCssStyle'><tr><td><h4>Data Report </h4> </td><td></td><td><h4>" + DateTime.Now.ToString("d") + "</h4></td></tr></table>";
        string headerTable1 = @"<table width='100%' class='TestCssStyle'><tr><td><h4>" + name + "</h4> </td><td></td><td><h4>" + DateTime.Now.ToString("dd/MM/yyyy") + "</h4></td></tr></table>";
        Response.Write(headerTable1);
        Response.Write(strwritter.ToString());
        gdvProjectDownloadDetails.DataSource = null;
        gdvProjectDownloadDetails.DataBind();
        Response.Write(strwritter2.ToString());
        Response.Write(strwritter2.ToString());
        Response.End();
    }
}