using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectPageDetails : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["userid"] != null)
            {
                if (Request.QueryString.Count > 0)
                {
                    int ID = Convert.ToInt32(Request.QueryString["Id"]);
                    GetRecords(ID);
                    hfPID.Value = ID.ToString();
                }
            }
            else
            {
                Response.Redirect("Index.html");
            }
        }
    }

    void GetRecords(int PID)
    {
        try
        {
            DataSet dsRecords = objDAL.GetProjectViewDetails(PID);
            if (dsRecords.Tables.Count > 0)
            {
                if (dsRecords.Tables[0].Rows.Count > 0)
                {

                    // DataTable dt = rotategrid(dsRecords.Tables[0]);
                    DataTable dt = dsRecords.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        lblSurveyID.Text = dr["PID"].ToString();
                        lblSurvayname.Text = dr["PName"].ToString();
                        lblClient.Text = dr["Company"].ToString();
                        lblProjectManager.Text = dr["UserName"].ToString();
                        lblNotes.Text = dr["Notes"].ToString();
                        lblCountry.Text = dr["country"].ToString();
                        lblLOI.Text = dr["LOI"].ToString();
                        lblCPI.Text = dr["CPI"].ToString();
                        lblEDate.Text = dr["edate"].ToString();
                    }
                    //GrdSurveyDetails.ShowHeader = false;
                    // GrdSurveyDetails.DataSource = dt; //dsRecords.Tables[0];
                    // GrdSurveyDetails.DataBind();
                    // foreach (GridViewRow row in GrdSurveyDetails.Rows)
                    // {
                    //     row.Cells[0].CssClass = "header";
                    // }
                }
                if (dsRecords.Tables[1].Rows.Count > 0)
                {
                    //GrdSurveySpecs.DataSource = dsRecords.Tables[1];
                    //GrdSurveySpecs.DataBind();
                    //hstatus.InnerText = dsRecords.Tables[1].Rows[0]["Status"].ToString();
                }
                if (dsRecords.Tables[2].Rows.Count > 0)
                {
                    //GrdSupplierDetails.DataSource = dsRecords.Tables[2];
                    //GrdSupplierDetails.DataBind();
                }
                if (dsRecords.Tables[3].Rows.Count > 0)
                {
                    //GrdSurveyLinks.DataSource = dsRecords.Tables[3];
                    //GrdSurveyLinks.DataBind();
                }
                if (dsRecords.Tables[4].Rows.Count > 0)
                {
                    lblCompletion.Text = dsRecords.Tables[4].Rows[0]["CompletePercent"].ToString();
                }
            }
        }
        catch (SystemException ex)
        {
        }
    }
    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public static void abc()
    //{

    //}

    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public static List<ClsProjectPageDetails> GetProjectDetails()
    //{
    //    //  Session["StdyID"] = surveyID;
    //    ClsDAL objDAL = new ClsDAL();
    //    List<ClsProjectPageDetails> returnData = new List<ClsProjectPageDetails>();
    //    try
    //    {

    //        DataSet resultDS = null;

    //        resultDS = objDAL.PPDManagement(0, "P");

    //        //resultDS = objDal.GetSurveyFilter(surveyID, GID);


    //        foreach (DataRow dtrow in resultDS.Tables[0].Rows)
    //        {


    //            returnData.Add(new ClsProjectPageDetails
    //            {
    //                ID = System.Convert.ToInt32(dtrow["ID"]),
    //                //  OLDColumnName = dtrow["OLDColumnName"].ToString(),
    //                PName = dtrow["PName"].ToString(),


    //            });

    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
    //        return returnData;
    //    }

    //    return returnData;
    //}
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
        Response.Clear();
        Response.Buffer = true;
        Response.ClearContent();
        Response.ClearHeaders();
        Response.Charset = "";
        string FileName = lblSurveyID.Text + "_" + lblSurvayname.Text + ": " + DateTime.Now + ".xls";
        StringWriter strwritter = new StringWriter();
        HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
        StringWriter strwritter1 = new StringWriter();
        HtmlTextWriter htmltextwrtter1 = new HtmlTextWriter(strwritter1);
        StringWriter strwritter2 = new StringWriter();
        HtmlTextWriter htmltextwrtter2 = new HtmlTextWriter(strwritter2);
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = "application/vnd.ms-excel";
        Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
        DataSet ds = objDAL.GetProjectDetailsForDownload(Convert.ToInt32(hfPID.Value), 0);
        gdvProjectDownloadDetails.DataSource = ds;
        gdvProjectDownloadDetails.DataBind();
        gdvProjectDownloadDetails.GridLines = GridLines.Both;
        gdvProjectDownloadDetails.HeaderStyle.Font.Bold = true;
        gdvProjectDownloadDetails.RenderControl(htmltextwrtter);
        //string headerTable1 = @"<table width='100%' class='TestCssStyle'><tr><td><h4>Data Report </h4> </td><td></td><td><h4>" + DateTime.Now.ToString("d") + "</h4></td></tr></table>";
        string headerTable1 = @"<table width='100%' class='TestCssStyle'><tr><td><h4>" + lblSurveyID.Text + "_" + lblSurvayname.Text + "</h4> </td><td></td><td><h4>" + DateTime.Now.ToString("dd/MM/yyyy") + "</h4></td></tr></table>";
        Response.Write(headerTable1);
        Response.Write(strwritter.ToString());
        gdvProjectDownloadDetails.DataSource = null;
        gdvProjectDownloadDetails.DataBind();
        Response.Write(strwritter2.ToString());
        Response.Write(strwritter2.ToString());
        Response.End();
    }
    public void exporttoExcel2()
    {
        DataSet ds = objDAL.GetProjectDetailsForDownload(Convert.ToInt32(hfPID.Value), 0);
        gdvProjectDownloadDetails.DataSource = ds;
        gdvProjectDownloadDetails.DataBind();
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetProjectsDetails(int projectid)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<ClsProject> returnProject = new List<ClsProject>();
        List<Suppliers> returnData = new List<Suppliers>();
        List<SurveySpecs> returnDataSS = new List<SurveySpecs>();
        List<ClsProject> returnDataCW = new List<ClsProject>();
        List<SurvayLinks> returnSurveyLinks = new List<SurvayLinks>();
        try
        {
            // int PID = Convert.ToInt32(HttpContext.Current.Session["PID"]);
            DataSet resultDS = null;
            resultDS = objDAL.GetProjectViewDetails(projectid);
            //resultDS = objDal.GetSurveyFilter(surveyID, GID);
            foreach (DataRow dtrow in resultDS.Tables[5].Rows)
            {
                returnData.Add(new Suppliers
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    Country = dtrow["Country"].ToString(),
                    Name = dtrow["Name"].ToString(),
                    CPI = float.Parse(dtrow["CPI"].ToString()),
                    IR = float.Parse(dtrow["IR"].ToString()),
                    ActLOI = dtrow["LOI"].ToString(),
                    Total = Convert.ToInt32(dtrow["Total"]),
                    Completes = dtrow["Complete"].ToString(),
                    Incomplete = dtrow["Incomplete"].ToString(),
                    //Screened = dtrow["Screened"].ToString(),
                    Terminate = dtrow["Terminate"].ToString(),
                    Overquota = dtrow["Overquota"].ToString(),
                    Security = dtrow["Security Term"].ToString(),
                    Fraud = dtrow["Fraud Error"].ToString(),
                    Block = Convert.ToInt32(dtrow["Block"]),
                    PreScreening = Convert.ToInt32(dtrow["PreScreening"]),
                    Quota = Convert.ToInt32(dtrow["Quota"]),
                    TrackingType = Convert.ToInt32(dtrow["TrackingType"])
                });
            }
            foreach (DataRow dtrow in resultDS.Tables[1].Rows)
            {
                returnDataSS.Add(new SurveySpecs
                {
                    id = System.Convert.ToInt32(dtrow["PUID"]),
                    country = dtrow["country"].ToString(),
                    Survey_Quota = System.Convert.ToInt32(dtrow["Survey Quota"]),
                    loi = System.Convert.ToInt32(dtrow["loi"].ToString()),
                    cpi = float.Parse(dtrow["cpi"].ToString()),
                    irate = System.Convert.ToInt32(dtrow["irate"]),
                    countryid = System.Convert.ToInt32(dtrow["countryid"]),
                    total = System.Convert.ToInt32(dtrow["total"]),
                    complete = System.Convert.ToInt32(dtrow["complete"]),
                    completepercent = float.Parse(dtrow["completepercent"].ToString()),
                    Status = Convert.ToInt32(dtrow["status"]),
                    IRPercent = Convert.ToInt32(dtrow["IRpercent"]),
                    ActLOI = Convert.ToInt32(dtrow["ActLOI"]),
                    Notes = dtrow["Notes"].ToString()
                });
            }
            foreach (DataRow dtrow in resultDS.Tables[6].Rows)
            {
                returnDataCW.Add(new ClsProject
                {
                    Country = dtrow["Country"].ToString(),
                    total = System.Convert.ToInt32(dtrow["Total"]),
                    completed = System.Convert.ToInt32(dtrow["Complete"].ToString()),
                    incomplete = System.Convert.ToInt32(dtrow["Incomplete"]),
                    //screened = System.Convert.ToInt32(dtrow["Screened"]),
                    terminate = System.Convert.ToInt32(dtrow["Terminate"]),
                    overquota = System.Convert.ToInt32(dtrow["Overquota"]),
                    //quotafull = System.Convert.ToInt32(dtrow["Quotafull"]),
                    securityterm = System.Convert.ToInt32(dtrow["Sec_Term"]),
                    frauderror = System.Convert.ToInt32(dtrow["F_Error"]),
                    Cancelled = Convert.ToInt32(dtrow["Cancelled"])
                });
            }
            foreach (DataRow dtrow in resultDS.Tables[0].Rows)
            {
                returnProject.Add(new ClsProject
                {
                    DeviceBlock = dtrow["BlockDevice"].ToString(),
                    Status = Convert.ToInt32(dtrow["Status"])
                });
            }
            foreach (DataRow dtrow in resultDS.Tables[3].Rows)
            {
                returnSurveyLinks.Add(new SurvayLinks
                {
                    ID = Convert.ToInt32(dtrow["id"]),
                    Country = dtrow["Country"].ToString(),
                    SupplierName = dtrow["Suppliers"].ToString(),
                    ClientSurveyLink = dtrow["OLink"].ToString(),
                    SupplierLink = dtrow["MLink"].ToString()
                });
            }
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            return null;
        }
        object[] obj = new object[5];
        obj[0] = returnData;
        obj[1] = returnDataSS;
        obj[2] = returnDataCW;
        obj[3] = returnProject;
        obj[4] = returnSurveyLinks;
        return obj;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string UpdateRedirectsInProMap(CLSProjectmapping formdata)
    {
        try
        {
            ClsDAL obj = new ClsDAL();
            string result = obj.UpdateRedirctsInProMap(formdata);
            return result;
        }
        catch (Exception e) { return "error"; }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetFractionComplete(string ProjectId)
    {
        try
        {
            List<FractionComplete> returnProject = new List<FractionComplete>();
            ClsDAL obj = new ClsDAL();
            DataSet result = obj.GetFractionComplete(ProjectId, 0);
            foreach (DataRow dtrow in result.Tables[0].Rows)
            {
                returnProject.Add(new FractionComplete
                {
                    Total = Convert.ToInt32(dtrow["Total"]),
                    Complete = Convert.ToInt32(dtrow["Complete"])
                });
            }
            DataSet result1 = obj.GetFractionComplete(ProjectId, 1);
            List<FractionComplete> returnIRSummary = new List<FractionComplete>();
            foreach (DataRow dtrow in result1.Tables[0].Rows)
            {
                returnIRSummary.Add(new FractionComplete
                {
                    ExpectedIR = dtrow["ExpectedIR"].ToString(),
                    Complete = Convert.ToInt32(dtrow["complete"]),
                    Incomplete = dtrow["incomplete"].ToString(),
                    ActualIR = dtrow["ActualIR"].ToString(),
                    IRPercent = dtrow["IRPERCENT"].ToString()
                });
            }
            DataSet result2 = obj.GetFractionComplete(ProjectId, 3);
            List<FractionComplete> returnLOISummary = new List<FractionComplete>();
            foreach (DataRow dtrow in result2.Tables[0].Rows)
            {
                returnLOISummary.Add(new FractionComplete
                {
                    ExpectedLOI = dtrow["Expected_LOI"].ToString(),
                    ActualLOI = dtrow["Actual_LOI"].ToString(),
                    Total = Convert.ToInt32(dtrow["Total"])
                });
            }
            object[] obje = new object[3];
            obje[0] = returnProject;
            obje[1] = returnIRSummary;
            obje[2] = returnLOISummary;
            return obje;
        }

        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void ManageBlocking(int ID, int OPT)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<Suppliers> returnData = new List<Suppliers>();
        try
        {
            objDAL.ManageBlocking(ID, OPT);
        }
        catch (Exception ex)
        {
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            //return returnData;
        }
        //  return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string DeviceControl(int ID, string code, string OPT)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();

        try
        {
            return objDAL.DeviceControl(ID, Convert.ToInt32(OPT), code);
        }
        catch (Exception ex)
        {
            return "Error";
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            //return returnData;
        }
        //  return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string UpdateStatus(string Id, string Status)
    {
        ClsDAL objdal = new ClsDAL();
        int x = objdal.UpdateProjectStatus(Status, Id, "PPDetails");
        DataSet ds = objdal.UpdateStatusinProjects(Id);
        string res = ds.Tables[0].Rows[0][0].ToString();
        //return x > 0 ? "Updated Succesfully" : "Unable to Update succesfully";
        return res == "1" ? "Closed" : res == "2" ? "Live" : res == "3" ? "On Hold" : res == "4" ? "Cancelled" : res == "5" ? "Awarded" : "Invoiced";
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<IPSecuritymgr> IPSecuritymgr(int ProjectID, string count, string opt)
    {
        //  Session["StdyID"] = surveyID;
        ClsDAL objDAL = new ClsDAL();
        List<IPSecuritymgr> returndata = new List<IPSecuritymgr>();
        try
        {
            DataSet ds = objDAL.IPSecurityControlmgr(ProjectID, Convert.ToInt32(opt), count);
            if (opt == "0")
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[0].Rows)
                    {
                        returndata.Add(new IPSecuritymgr
                        {
                            Message = dtrow["Message"].ToString()
                        });
                    }
                }
            }
            return returndata;
        }
        catch (Exception ex)
        {
            return null;
            // LogWriter.WriteErrorLogs("Error occured in SurveyDetailsReport page on GetFilterData()event : " + ex.Message);
            //return returnData;
        }
        //  return returnData;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string SaveQuotaCW(string Quota, string opt, string projecturlid)
    {
        string result = "";
        try
        {
            ClsDAL obj = new ClsDAL();
            DataSet output = obj.MgrQuota(Quota, opt, projecturlid);
            if (output.Tables[0].Rows.Count > 0)
            {
                result = output.Tables[0].Rows[0][0].ToString();
            }
            return result;
        }
        catch (Exception e)
        {
            return "error";
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string SaveQuotaCWandProject(string Quota, string opt, string projecturlid)
    {
        string result = "";
        try
        {
            ClsDAL obj = new ClsDAL();
            DataSet output = obj.MgrQuota(Quota, opt, projecturlid);
            if (output.Tables[0].Rows.Count > 0)
            {
                result = output.Tables[0].Rows[0][0].ToString();
            }
            return result;
        }
        catch (Exception e)
        {
            return "error";
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CountryDetails> GetCountries()
    {
        ClsDAL objDAL = new ClsDAL();
        List<CountryDetails> returnData = new List<CountryDetails>();
        DataSet dsclient = objDAL.ClientMGR(0, "CNTOLD");

        if (dsclient.Tables[0].Rows.Count > 0)
        {

            foreach (DataRow dtrow in dsclient.Tables[0].Rows)
            {
                returnData.Add(new CountryDetails
                {
                    ID = System.Convert.ToInt32(dtrow["Id"]),

                    Country = dtrow["Country"].ToString()

                });

            }
        }
        return returnData;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CountryDetails> mgrMappingCountries(string ProjectURLID, string CountryID, int OPT)
    {
        ClsDAL objDAL = new ClsDAL();
        List<CountryDetails> returnData = new List<CountryDetails>();
        DataSet dsclient = objDAL.mgrMappingCountries(ProjectURLID, CountryID, OPT);
        if (OPT == 1)
        {
            if (dsclient.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dtrow in dsclient.Tables[0].Rows)
                {
                    returnData.Add(new CountryDetails
                    {
                        ID = System.Convert.ToInt32(dtrow["Id"]),
                        Country = dtrow["country"].ToString()
                    });
                }
            }
        }
        return returnData;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CLSProjectmapping> mgrVendorPreScreening(CLSProjectmapping formData)
    {
        ClsDAL objDAL = new ClsDAL();
        List<CLSProjectmapping> returnData = new List<CLSProjectmapping>();
        DataSet dsclient = objDAL.mgrVendorPreScreening(formData);
        if (dsclient.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow dtrow in dsclient.Tables[0].Rows)
            {
                returnData.Add(new CLSProjectmapping
                {
                    RetVal = System.Convert.ToInt32(dtrow["RetVal"]),
                    RetMessage = dtrow["RetMessage"].ToString()
                });
            }
        }
        return returnData;
    }
    //public static NextOnServicesModel.NextOnServicesEntities db = new NextOnServicesModel.NextOnServicesEntities();
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ResposeDataDownload> SurvayResponseData(SurveyResponse formData)
    {

        ClsDAL objDAL = new ClsDAL();
        List<SurveyResponse> surveyResponses = new List<SurveyResponse>();
        List<SurveyResponse> surveyResponses1 = new List<SurveyResponse>();
        List<SurveyResponse> surveyResponses2 = new List<SurveyResponse>();
        List<ResposeDataDownload> resposeDataDownloads = new List<ResposeDataDownload>();
        DataSet ds = objDAL.GetSurveyResponse(formData);
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                resposeDataDownloads = FormDataForSurvayResponse(ds);
            }
        }
        return resposeDataDownloads;

    }
    public static List<ResposeDataDownload> FormDataForSurvayResponse(DataSet ds)
    {

        int columncount = ds.Tables[0].Columns.Count;
        List<ResposeDataDownload> surveyResponses = new List<ResposeDataDownload>();
        switch (columncount)
        {
            case 2:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"])
                    });
                }
                return surveyResponses;

            case 3:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"])
                    });
                }
                return surveyResponses;
            case 4:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"])
                    });
                }
                return surveyResponses;
            case 5:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"])
                    });
                }
                return surveyResponses;
            case 6:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"])
                    });
                }
                return surveyResponses;
            case 7:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"])
                    });
                }
                return surveyResponses;
            case 8:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"])
                    });
                }
                return surveyResponses;
            case 9:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"])
                    });
                }
                return surveyResponses;
            case 10:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                    });
                }
                return surveyResponses;
            case 11:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                    });
                }
                return surveyResponses;
            case 12:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                    });
                }
                return surveyResponses;
            case 13:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                    });
                }
                return surveyResponses;
            case 14:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                    });
                }
                return surveyResponses;
            case 15:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                    });
                }
                return surveyResponses;
            case 16:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                    });
                }
                return surveyResponses;
            case 17:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                    });
                }
                return surveyResponses;
            case 18:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                    });
                }
                return surveyResponses;
            case 19:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                    });
                }
                return surveyResponses;
            case 20:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                    });
                }
                return surveyResponses;
            case 21:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                    });
                }
                return surveyResponses;
            case 22:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                    });
                }
                return surveyResponses;
            case 23:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                    });
                }
                return surveyResponses;
            case 24:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                    });
                }
                return surveyResponses;
            case 25:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                    });
                }
                return surveyResponses;
            case 26:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                    });
                }
                return surveyResponses;
            case 27:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                    });
                }
                return surveyResponses;
            case 28:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                    });
                }

                return surveyResponses;
            case 29:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                    });
                }
                return surveyResponses;
            case 30:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                    });
                }
                return surveyResponses;
            case 31:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                    });
                }
                return surveyResponses;
            case 32:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                    });
                }
                return surveyResponses;
            case 33:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                    });
                }
                return surveyResponses;
            case 34:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                    });
                }
                return surveyResponses;
            case 35:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                    });
                }
                return surveyResponses;
            case 36:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                    });
                }
                return surveyResponses;
            case 37:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                    });
                }
                return surveyResponses;
            case 38:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                    });
                }
                return surveyResponses;
            case 39:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                    });
                }
                return surveyResponses;
            case 40:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                    });
                }
                return surveyResponses;
            case 41:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                    });
                }
                return surveyResponses;
            case 42:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                    });
                }
                return surveyResponses;
            case 43:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                    });
                }
                return surveyResponses;
            case 44:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                        Col42 = Convert.ToString(item["Col42"]),
                    });
                }
                return surveyResponses;
            case 45:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                        Col42 = Convert.ToString(item["Col42"]),
                        Col43 = Convert.ToString(item["Col43"]),
                    });
                }
                return surveyResponses;
            case 46:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                        Col42 = Convert.ToString(item["Col42"]),
                        Col43 = Convert.ToString(item["Col43"]),
                        Col44 = Convert.ToString(item["Col44"]),
                    });
                }
                return surveyResponses;
            case 47:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                        Col42 = Convert.ToString(item["Col42"]),
                        Col43 = Convert.ToString(item["Col43"]),
                        Col44 = Convert.ToString(item["Col44"]),
                        Col45 = Convert.ToString(item["Col45"]),
                    });
                }
                return surveyResponses;
            case 48:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                        Col42 = Convert.ToString(item["Col42"]),
                        Col43 = Convert.ToString(item["Col43"]),
                        Col44 = Convert.ToString(item["Col44"]),
                        Col45 = Convert.ToString(item["Col45"]),
                        Col46 = Convert.ToString(item["Col46"]),
                    });
                }
                return surveyResponses;
            case 49:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                        Col42 = Convert.ToString(item["Col42"]),
                        Col43 = Convert.ToString(item["Col43"]),
                        Col44 = Convert.ToString(item["Col44"]),
                        Col45 = Convert.ToString(item["Col45"]),
                        Col46 = Convert.ToString(item["Col46"]),
                        Col47 = Convert.ToString(item["Col47"]),
                    });
                }
                return surveyResponses;
            case 50:
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    surveyResponses.Add(new ResposeDataDownload
                    {
                        Id = Convert.ToInt32(item["Id"]),
                        UID = Convert.ToString(item["UID"]),
                        Col1 = Convert.ToString(item["Col1"]),
                        Col2 = Convert.ToString(item["Col2"]),
                        Col3 = Convert.ToString(item["Col3"]),
                        Col4 = Convert.ToString(item["Col4"]),
                        Col5 = Convert.ToString(item["Col5"]),
                        Col6 = Convert.ToString(item["Col6"]),
                        Col7 = Convert.ToString(item["Col7"]),
                        Col8 = Convert.ToString(item["Col8"]),
                        Col9 = Convert.ToString(item["Col9"]),
                        Col10 = Convert.ToString(item["Col10"]),
                        Col11 = Convert.ToString(item["Col11"]),
                        Col12 = Convert.ToString(item["Col12"]),
                        Col13 = Convert.ToString(item["Col13"]),
                        Col14 = Convert.ToString(item["Col14"]),
                        Col15 = Convert.ToString(item["Col15"]),
                        Col16 = Convert.ToString(item["Col16"]),
                        Col17 = Convert.ToString(item["Col17"]),
                        Col18 = Convert.ToString(item["Col18"]),
                        Col19 = Convert.ToString(item["Col19"]),
                        Col20 = Convert.ToString(item["Col20"]),
                        Col21 = Convert.ToString(item["Col21"]),
                        Col22 = Convert.ToString(item["Col22"]),
                        Col23 = Convert.ToString(item["Col23"]),
                        Col24 = Convert.ToString(item["Col24"]),
                        Col25 = Convert.ToString(item["Col25"]),
                        Col26 = Convert.ToString(item["Col26"]),
                        Col27 = Convert.ToString(item["Col27"]),
                        Col28 = Convert.ToString(item["Col28"]),
                        Col29 = Convert.ToString(item["Col29"]),
                        Col30 = Convert.ToString(item["Col30"]),
                        Col31 = Convert.ToString(item["Col31"]),
                        Col32 = Convert.ToString(item["Col32"]),
                        Col33 = Convert.ToString(item["Col33"]),
                        Col34 = Convert.ToString(item["Col34"]),
                        Col35 = Convert.ToString(item["Col35"]),
                        Col36 = Convert.ToString(item["Col36"]),
                        Col37 = Convert.ToString(item["Col37"]),
                        Col38 = Convert.ToString(item["Col38"]),
                        Col39 = Convert.ToString(item["Col39"]),
                        Col40 = Convert.ToString(item["Col40"]),
                        Col41 = Convert.ToString(item["Col41"]),
                        Col42 = Convert.ToString(item["Col42"]),
                        Col43 = Convert.ToString(item["Col43"]),
                        Col44 = Convert.ToString(item["Col44"]),
                        Col45 = Convert.ToString(item["Col45"]),
                        Col46 = Convert.ToString(item["Col46"]),
                        Col47 = Convert.ToString(item["Col47"]),
                        Col48 = Convert.ToString(item["Col48"]),
                    });
                }
                return surveyResponses;
            default:
                return surveyResponses;
        }
    }
}

public interface IObjectContextAdapter
{
    System.Data.Objects.ObjectContext ObjectContext { get; }
}

public class CountryDetails
{
    public int ID { get; internal set; }
    public string Country { get; internal set; }
}