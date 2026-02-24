using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class VT_RecontactCreate : System.Web.UI.Page
{
    private static Random random = new Random();
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProject> GetProjectsDetails()
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsProject> returnData = new List<ClsProject>();
        try
        {
            // int PID = Convert.ToInt32(HttpContext.Current.Session["PID"]);
            DataSet resultDS = null;
            resultDS = objDAL.ClientMGR(0, "P");
            //resultDS = objDal.GetSurveyFilter(surveyID, GID);
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new ClsProject
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
                    PName = dtrow["PName"].ToString(),
                });
            }
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return returnData;
        }
        return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<Recontact> SaveData(string dataa)
    {
        ClsDAL objDAL = new ClsDAL();
        List<Recontact> returnData = new List<Recontact>();
        try
        {
            string PID = IncrementID(objDAL.GetProjectId());
            StrData data = new StrData();
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            data = jsonSerializer.Deserialize<StrData>(dataa);
            DataTable dt = insertRecontacts(data.exceldata, 2);
            // string KID = RandomString(32);
            string SID = RandomString(4);
            string MUrl = ConfigurationSettings.AppSettings["MaskingUrl"].ToString() + "?SID=" + SID + "&ID=XXXXXXXXXX&CID=XXXXXXXXXX";
            string[] arrvars = new string[data.NoOfVars];
            switch (data.NoOfVars)
            {
                case 0:
                    break;
                case 1:
                    MUrl = MUrl + "&" + data.Var1 + "=XXXXXXXXXX";
                    arrvars[0] = data.Var1;
                    break;
                case 2:
                    MUrl = MUrl + "&" + data.Var1 + "=XXXXXXXXXX&" + data.Var2 + "=XXXXXXXXXX";
                    arrvars[0] = data.Var1;
                    arrvars[1] = data.Var2;
                    break;
                case 3:
                    MUrl = MUrl + "&" + data.Var1 + "=XXXXXXXXXX&" + data.Var2 + "=XXXXXXXXXX&" + data.Var3 + "=XXXXXXXXXX";
                    arrvars[0] = data.Var1;
                    arrvars[1] = data.Var2;
                    arrvars[2] = data.Var3;
                    break;
                case 4:
                    MUrl = MUrl + "&" + data.Var1 + "=XXXXXXXXXX&" + data.Var2 + "=XXXXXXXXXX&" + data.Var3 + "=XXXXXXXXXX&" + data.Var4 + "=XXXXXXXXXX";
                    arrvars[0] = data.Var1;
                    arrvars[1] = data.Var2;
                    arrvars[2] = data.Var3;
                    arrvars[3] = data.Var4;
                    break;
                case 5:
                    MUrl = MUrl + "&" + data.Var1 + "=XXXXXXXXXX&" + data.Var2 + "=XXXXXXXXXX&" + data.Var3 + "=XXXXXXXXXX&" + data.Var4 + "=XXXXXXXXXX&" + data.Var5 + "=XXXXXXXXXX";
                    arrvars[0] = data.Var1;
                    arrvars[1] = data.Var2;
                    arrvars[2] = data.Var3;
                    arrvars[3] = data.Var4;
                    arrvars[4] = data.Var5;
                    break;
            }
            DataSet resultDS = null;
            string Errorreturn = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                resultDS = objDAL.InsertRecontact(data.recontactname, data.cpi.ToString(), data.notes, data.Recontact_Description, data.Status, data.LOI, data.IR, data.RCQ, SID, MUrl, PID, data.opt);
                if (resultDS.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToInt32(resultDS.Tables[0].Rows[0]["status"]) == 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["UID"].ToString() != "" && dr["UID"].ToString() != null && dr["ClientURL"].ToString() != "" && dr["ClientURL"].ToString() != null)
                            {
                                DataSet valds = objDAL.ValidateUIDforRecontact(dr["UID"].ToString(), 0,SID);
                                if (valds.Tables[0].Rows.Count > 0)
                                {
                                    if (Convert.ToInt32(valds.Tables[0].Rows[0]["statuss"]) == 1)
                                    {
                                        //string MUrl = "";// ConfigurationSettings.AppSettings["MaskingUrl"].ToString() + "?SID=XXXX&ID=XXXXXXXXXX&CID=XXXXXXXXXX";
                                        DataSet dsrec = objDAL.UpdateRecontactRespondantstbl(dr["UID"].ToString(), dr["ClientURL"].ToString(), dr["Var1"].ToString(), dr["Var2"].ToString(), dr["Var3"].ToString(), dr["Var4"].ToString(), dr["Var5"].ToString(), valds.Tables[0].Rows[0]["pmid"].ToString(), MUrl, resultDS.Tables[0].Rows[0]["rcid"].ToString(), SID, Convert.ToInt32(valds.Tables[0].Rows[0]["promapid"]), Convert.ToInt32(valds.Tables[0].Rows[0]["projectid"]), Convert.ToInt32(valds.Tables[0].Rows[0]["countryid"]), Convert.ToInt32(valds.Tables[0].Rows[0]["supplierid"]), 2);
                                        if (dsrec.Tables[0].Rows.Count > 0)
                                        {
                                            for (int arrri = 0; arrri < arrvars.Length; arrri++)
                                            {
                                                objDAL.InsertVariables(arrvars[arrri], Convert.ToInt32(dsrec.Tables[0].Rows[0]["returnvalue"]), 0);
                                            }
                                            foreach (DataRow drr in dsrec.Tables[0].Rows)
                                            {
                                                if (Convert.ToInt32(drr["status"]) != 1)
                                                {
                                                    returnData.Add(new Recontact
                                                    {
                                                        ErrorReturnDBUID = drr["returnvalue"].ToString(),
                                                        ErrorReturnDBMessage = drr["r_message"].ToString(),
                                                        ErrorReturnDBStatus = "1"
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                                //DataSet ds = objDAL.UpdateRecontactRespondantstbl(dr["UID"].ToString(), dr["ClientURL"].ToString(), dr["Var1"].ToString(), dr["Var2"].ToString(), dr["Var3"].ToString(), dr["Var4"].ToString(), dr["Var5"].ToString(), "", MUrl, resultDS.Tables[0].Rows[0][0].ToString(), SID);
                                //if (ds.Tables[0].Rows.Count > 0)
                                //{
                                //    foreach (DataRow drr in ds.Tables[0].Rows)
                                //    {
                                //        if (Convert.ToInt32(drr["status"]) != 1)
                                //        {
                                //            returnData.Add(new Recontact
                                //            {
                                //                ErrorReturnDBUID = drr["returnvalue"].ToString(),
                                //                ErrorReturnDBMessage = drr["r_message"].ToString(),
                                //                ErrorReturnDBStatus = "1"
                                //            });
                                //        }
                                //    }
                                //}
                            }

                        }
                        return returnData;
                    }
                    else
                    {
                        returnData.Add(new Recontact { ErrorReturnDBStatus = "3", ErrorReturnDBMessage = "Please try again later" });
                        return returnData;
                    }

                }
                else
                {
                    returnData.Add(new Recontact { ErrorReturnDBStatus = "2", ErrorReturnDBMessage = "Please try again later" });
                    return returnData;
                }
            }
            else
            {
                returnData.Add(new Recontact { ErrorReturnDBStatus = "3", ErrorReturnDBMessage = "Please try again later" });
                return returnData;
            }

        }
        catch (Exception ex)
        {
            returnData.Add(new Recontact { ErrorReturnDBStatus = "4", ErrorReturnDBMessage = "There is an exception." });
            ClsDAL.WriteErrorLog("Error occured in SurveyDetailsReport page on SaveData()event : " + ex.Message);
            return returnData;
        }
        return null;
    }

    public static DataTable insertRecontacts(object fileupload, int NoOfColumns)
    {
        try
        {
            // string MURL = GetMaskingURL();
            ClsDAL obj = new ClsDAL();
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("UID", typeof(string)),
            new DataColumn("ClientURL", typeof(string)) ,  new DataColumn("Var1", typeof(string)),  new DataColumn("Var2", typeof(string)),  new DataColumn("Var3", typeof(string)),  new DataColumn("Var4", typeof(string)),  new DataColumn("Var5", typeof(string)) });
            foreach (string row in fileupload.ToString().Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    dt.Rows.Add();
                    int i = 0;
                    string[] cell = new string[NoOfColumns];
                    cell = row.Split(',');
                    for (i = 0; i < NoOfColumns; i++)
                    {
                        dt.Rows[dt.Rows.Count - 1][i] = cell[i].ToString().Trim().Trim('"').Trim();
                    }
                    //foreach (string cell in row.Split(','))
                    //{
                    //    dt.Rows[dt.Rows.Count - 1][i] = cell.Trim();
                    //    i++;
                    //}
                }
            }
            return dt;
            //foreach (DataRow dr in dt.Rows)
            //{
            //    obj.UpdateRecontactRespondantstbl(dr["UID"].ToString(), dr["ClientURL"].ToString(), dr["PMID"].ToString(), MURL, id, SID);
            //}
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public DataTable CreateExampleTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.AddRange(new DataColumn[2] { new DataColumn("UID", typeof(string)),
            new DataColumn("ClientURL", typeof(string))  });
        dt.Rows.Add();
        dt.Rows[dt.Rows.Count - 1][0] = "UID";
        dt.Rows[dt.Rows.Count - 1][1] = "ClientURL";
        return dt;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProject> SaveData1(string PID, string CID, string SID, string RCQ, string CPI, string Notes, string IDs, int OPT)
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsProject> returnData = new List<ClsProject>();
        try
        {

        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return returnData;
        }
        return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<Recontact> LoadRecontactCountries(string PID, string CID, string SID, int OPT)
    {
        ClsDAL objDAL = new ClsDAL();
        List<Recontact> returnData = new List<Recontact>();
        try
        {
            DataSet resultDS = null;
            resultDS = objDAL.LoadRecontactProjects(PID, CID, SID, OPT);
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnData.Add(new Recontact
                {
                    RecontactName = dtrow["recontactname"].ToString(),
                    ID = System.Convert.ToInt32(dtrow["Id"]),
                    PID = dtrow["RecontactProjectId"].ToString(),
                    //PName = dtrow["PName"].ToString(),
                    //Country = dtrow["country"].ToString(),
                    //Supplier = dtrow["name"].ToString(),
                    //RCQ = Convert.ToInt32(dtrow["RCQ"]),
                    CPI = Convert.ToInt32(dtrow["CPI"]),
                    Notes = dtrow["Notes"].ToString(),
                    //ProMapID = Convert.ToInt32(dtrow["ProMapId"]),
                    //RCcnt = Convert.ToInt32(dtrow["RCcnt"]),
                    //pidd = Convert.ToInt32(dtrow["projectid"]),
                    //CID = Convert.ToInt32(dtrow["countryid"]),
                    SID = dtrow["SID"].ToString(),
                    MURL = dtrow["MURL"].ToString()
                });
            }
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return returnData;
        }
        return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string DeleteData(int ID)
    {
        ClsDAL objDAL = new ClsDAL();
        List<Recontact> returnData = new List<Recontact>();
        try
        {
            DataSet resultDS = null;
            resultDS = objDAL.DeleteRecontact("", ID, 0);
            if (resultDS.Tables[0].Rows.Count > 0)
            {
                return resultDS.Tables[0].Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return "-1";
        }
        return "";
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static int DeleteData1(string ROWS, int ID)
    {
        ClsDAL objDAL = new ClsDAL();
        List<Recontact> returnData = new List<Recontact>();
        string UID;
        int cnt = 0;
        try
        {
            foreach (string row in ROWS.ToString().Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    UID = row.Trim().Trim('"').Trim();
                    DataSet ds = objDAL.DeleteRecontact(UID, ID, 1);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        cnt = cnt + Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    }
                }
            }
            return cnt;
            //DataSet resultDS = null;
            //resultDS = objDAL.DeleteRecontact(ID);
            //if (resultDS.Tables[0].Rows.Count > 0)
            //{
            //    return resultDS.Tables[0].Rows[0][0].ToString();
            //}
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return -1;
        }
        return 0;
    }
    //protected void btnDownloadData_Click(object sender, EventArgs e)
    //{
    //    // exporttoExcel();
    //}
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static int AddMore(string ROWS, int ID)
    {
        ClsDAL objDAL = new ClsDAL();
        List<Recontact> returnData = new List<Recontact>();
        string UID;

        try
        {
            int count = 0;
            string MUrl = ConfigurationSettings.AppSettings["MaskingUrl"].ToString() + "?SID=XXXX&ID=XXXXXXXXXX&CID=XXXXXXXXXX";
            DataTable dt = insertRecontacts(ROWS, 2);
            //DataTable dt = insertRecontacts(ROWS, 7);
            foreach (DataRow dr in dt.Rows)
            {
                DataSet ds = objDAL.UpdateRecontactProjects(dr["UID"].ToString(), dr["ClientURL"].ToString(), MUrl, ID, 1);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    count = count + Convert.ToInt32(ds.Tables[0].Rows[0]["status"]);
                }
            }
            return count;
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return -1;
        }
        return 0;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<RecontactDownload> DownloadData1(string ID)
    {
        ClsDAL objDAL = new ClsDAL();
        DataSet ds = objDAL.GetRecontactExcelData(1, "0", "0", ID);
        List<RecontactDownload> ret = new List<RecontactDownload>();
        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow dtrow in ds.Tables[0].Rows)
            {
                ret.Add(new RecontactDownload
                {
                    UID = dtrow["UID"].ToString(),
                    RedirectLink = dtrow["RedirectLink"].ToString(),
                    MaskingURL = dtrow["MaskingURL"].ToString(),
                    PMID = dtrow["PMID"].ToString(),
                    IsUsed = Convert.ToInt32(dtrow["IsUsed"])
                });
            }
        }
        return ret;
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        //base.VerifyRenderingInServerForm(control);
    }
    public void exporttoExcel()
    {
        ClsDAL objDAL = new ClsDAL();
        string pid = "0"; // ddlPN.SelectedIndex.ToString();
        string proid = "0";
        string Country = "0";
        string spp = "0";
        Response.Clear();
        Response.Buffer = true;
        Response.ClearContent();
        Response.ClearHeaders();
        Response.Charset = "";
        string FileName = "NextOnServices Project Data:" + "download" + ": " + DateTime.Now + ".csv";
        StringWriter strwritter = new StringWriter();
        HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
        StringWriter strwritter1 = new StringWriter();
        HtmlTextWriter htmltextwrtter1 = new HtmlTextWriter(strwritter1);
        StringWriter strwritter2 = new StringWriter();
        HtmlTextWriter htmltextwrtter2 = new HtmlTextWriter(strwritter2);
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
        //  ClsDAL.WriteErrorLog("reached");
        DataTable ds = CreateExampleTable();
        gdvProjectDownloadDetails.DataSource = ds;
        gdvProjectDownloadDetails.DataBind();
        gdvProjectDownloadDetails.GridLines = GridLines.Both;
        gdvProjectDownloadDetails.HeaderStyle.Font.Bold = true;
        gdvProjectDownloadDetails.RenderControl(htmltextwrtter);
        //string headerTable1 = @"<table width='100%' class='TestCssStyle'><tr><td><h4>Data Report </h4> </td><td></td><td><h4>" + DateTime.Now.ToString("d") + "</h4></td></tr></table>";
        // string headerTable1 = @"<table width='100%' class='TestCssStyle'><tr><td><h4>" + "download" + "</h4> </td><td></td><td><h4>" + DateTime.Now.ToString("dd/MM/yyyy") + "</h4></td></tr></table>";
        //  Response.Write(headerTable1);
        Response.Write(strwritter.ToString());
        gdvProjectDownloadDetails.DataSource = null;
        gdvProjectDownloadDetails.DataBind();
        //  Response.Write(strwritter2.ToString());
        //  Response.Write(strwritter2.ToString());
        Response.End();
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    //public static string GetMaskingURL()
    //{
    //    try
    //    {
    //        // ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Working on this part and will share today EOD');", true);
    //        string KID = RandomString(32);
    //        string SID = RandomString(4);
    //        string MUrl = ConfigurationSettings.AppSettings["MaskingUrl"].ToString() + "?SID=" + SID + "&ID=XXXXXXXXXX&NEWID=XXXXXXXXXX";
    //        return MUrl;
    //    }
    //    catch (SystemException ex)
    //    {
    //        return "error";
    //    }
    //}

    static string IncrementID(string startValue)
    {
        if (startValue == "")
        {
            startValue = "NXT000000";
        }
        char letter1 = startValue[0];
        char letter2 = startValue[1];
        char letter3 = startValue[2];
        int len = startValue.Length - 3;
        int number = int.Parse(startValue.Substring(3));
        number++;
        if (number >= Math.Pow(10, len)) number = 1; // start again at 1
        return String.Format("{0}{1}{2}{3:D" + len.ToString() + "}", letter1, letter2, letter3, number);
    }

    protected void btntest_Click(object sender, CommandEventArgs e)
    {
        //exporttoExcel("2");
        string abc = e.CommandArgument.ToString();

    }

    protected void btnExampleCsv_Click(object sender, EventArgs e)
    {
        // exporttoExcel();

        DataTable dt = new DataTable();
        dt.Columns.Add("UID");
        dt.Columns.Add("Links");
        DataRow drRow = null;
        for (int i = 0; i <= -1; i++)
        {
            drRow = dt.NewRow();
            drRow[0] = "UID";
            drRow[1] = "ClientURL";
            dt.Rows.Add(drRow);
        }

        StringBuilder sb = new StringBuilder();

        foreach (DataColumn col in dt.Columns)
        {
            sb.Append(string.Format("{0},", col.ColumnName));
        }
        foreach (DataRow row in dt.Rows)
        {
            sb.Append(string.Format("{0},{1}", row[0], row[1]));
        }

        byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());

        if (bytes != null)
        {
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Length", bytes.Length.ToString());
            Response.AddHeader("Content-disposition", "attachment; filename=\"sample" + DateTime.Now + ".csv" + "\"");
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
    }
}

public class StrData
{
    public string Var1 { get; set; }
    public string Var2 { get; set; }
    public string Var3 { get; set; }
    public string Var4 { get; set; }
    public string Var5 { get; set; }
    public string recontactname { get; set; }
    public float cpi { get; set; }
    public string notes { get; set; }
    public int NoOfVars { get; set; }
    public string exceldata { get; set; }
    public int opt { get; set; }
    public string Recontact_Description { get; set; }
    public int Status { get; set; }
    public float LOI { get; set; }
    public float IR { get; set; }
    public int RCQ { get; set; }
}
public class RecontactDownload
{
    public string UID { get; set; }
    public string RedirectLink { get; set; }
    public string MaskingURL { get; set; }
    public string PMID { get; set; }
    public int IsUsed { get; set; }
}