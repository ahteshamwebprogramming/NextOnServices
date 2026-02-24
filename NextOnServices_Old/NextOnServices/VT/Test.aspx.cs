using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Services;

public partial class Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnCheckIP_Click(object sender, EventArgs e)
    {
        //HttpClient client;
        //if (txtIP.Text != "")
        //{

        //}
    }



    protected void uploadCSV_Click(object sender, EventArgs e)
    {
        ////Upload and save the file
        ////  string csvPath = Server.MapPath("~/Files/") + Path.GetFileName(Recontactscsv.PostedFile.FileName);
        //string csvPath = Server.MapPath(Recontactscsv.PostedFile.FileName); //+ Path.GetFileName(Recontactscsv.PostedFile.FileName);
        //Recontactscsv.SaveAs(csvPath);

        //DataTable dt = new DataTable();
        //dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Id", typeof(int)),
        //    new DataColumn("UID", typeof(string)),
        //    new DataColumn("PMID",typeof(string)) });


        //string csvData = File.ReadAllText(csvPath);
        //foreach (string row in csvData.Split('\n'))
        //{
        //    if (!string.IsNullOrEmpty(row))
        //    {
        //        dt.Rows.Add();
        //        int i = 0;
        //        foreach (string cell in row.Split(','))
        //        {
        //            dt.Rows[dt.Rows.Count - 1][i] = cell;
        //            i++;
        //        }
        //    }
        //}

        //foreach (DataRow dr in dt.Rows)
        //{
        //    ClsDAL obj = new ClsDAL();
        //    obj.UpdateRecontactRespondantstbl(dr["UID"].ToString(), dr["PMID"].ToString());
        //}



        //string consString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //using (SqlConnection con = new SqlConnection(consString))
        //{
        //    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
        //    {
        //        //Set the database table name
        //        sqlBulkCopy.DestinationTableName = "dbo.Customers";
        //        con.Open();
        //        sqlBulkCopy.WriteToServer(dt);
        //        con.Close();
        //    }
        //}
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void test(object fileupload)
    {
        try
        {
            //Page page = (Page)HttpContext.Current.Handler;
            // FileUpload fu = (FileUpload)page.FindControl("Recontactscsv");

            // string csvPath = HttpContext.Current.Server.MapPath(fu.PostedFile.FileName);
            // fu.SaveAs(csvPath);
            ClsDAL obj = new ClsDAL();
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Id", typeof(int)),
            new DataColumn("UID", typeof(string)),
            new DataColumn("PMID",typeof(string)) });


            // string csvData = File.ReadAllText(csvPath);
            foreach (string row in fileupload.ToString().Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    dt.Rows.Add();
                    int i = 0;
                    foreach (string cell in row.Split(','))
                    {
                        dt.Rows[dt.Rows.Count - 1][i] = cell;
                        i++;
                    }
                }
            }

            foreach (DataRow dr in dt.Rows)
            {

                //obj.UpdateRecontactRespondantstbl(dr["UID"].ToString(), dr["PMID"].ToString());
            }

        }
        catch (Exception e)
        {
        }
    }
}