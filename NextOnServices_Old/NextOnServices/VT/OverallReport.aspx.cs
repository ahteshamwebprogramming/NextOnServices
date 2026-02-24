using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OverallReport : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsClients> GetClients()
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsClients> returnData = new List<ClsClients>();
        DataSet dsclient = objDAL.ReportManagement(null, null, 0, 0, 0, 0, (int)report.client, (int)Operation.select);

        if (dsclient.Tables[0].Rows.Count > 0)
        {

            foreach (DataRow dtrow in dsclient.Tables[0].Rows)
            {


                returnData.Add(new ClsClients
                {
                    Id = System.Convert.ToInt32(dtrow["ID"]),
                    ClientName = dtrow["Company"].ToString()

                });

            }

        }
        return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsSuppliers2> GetSuppliers()
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsSuppliers2> returnData = new List<ClsSuppliers2>();
        DataSet dsclient = objDAL.ReportManagement(null, null, 0, 0, 0, 0, (int)report.supplier, (int)Operation.select);

        if (dsclient.Tables[0].Rows.Count > 0)
        {

            foreach (DataRow dtrow in dsclient.Tables[0].Rows)
            {


                returnData.Add(new ClsSuppliers2
                {
                    Id = System.Convert.ToInt32(dtrow["ID"]),

                    SupplierName = dtrow["Name"].ToString()

                });

            }

        }
        return returnData;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetPieChartData()
    {
        try
        {
            ClsDAL objDAL = new ClsDAL();
            List<CLSPieChart> returnDataPie = new List<CLSPieChart>();
            List<CLSBarChart> returnDatabar = new List<CLSBarChart>();
            DataSet dsclient = objDAL.GetPieChart();
            if (dsclient.Tables.Count > 0)
            {
                if (dsclient.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in dsclient.Tables[1].Rows)
                    {
                        returnDataPie.Add(new CLSPieChart
                        {
                            Min = System.Convert.ToInt32(dtrow["Minn"]),
                            Max = System.Convert.ToInt32(dtrow["Maxx"]),
                            Mean = System.Convert.ToInt32(dtrow["Mean"])

                        });
                    }
                }
                if (dsclient.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in dsclient.Tables[0].Rows)
                    {
                        returnDatabar.Add(new CLSBarChart
                        {
                            Months = dtrow["Months"].ToString(),
                            Completes = System.Convert.ToInt32(dtrow["Completes"])
                            // Mean = System.Convert.ToInt32(dtrow["Mean"])

                        });
                    }
                }
            }
            object[] obj = new object[2];
            obj[0] = returnDataPie;
            obj[1] = returnDatabar;

            return obj;
        }
        catch (Exception e)
        {
            return null;
        }


    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetDataForFilters(string CountryId, string Clientid, string Supplierid, DateTime? sdate, DateTime? edate)
    {
        try
        {
            List<ClsSuppliers2> returnsuppliers = new List<ClsSuppliers2>();
            List<ClsClients> returnClients = new List<ClsClients>();
            List<CLSTotal> returntotal = new List<CLSTotal>();
            List<ClsCountry> returncountry = new List<ClsCountry>();
            //List<ClsIRate> ReturnIrate = new List<ClsIRate>();
            ClsDAL objdal = new ClsDAL();
            DataSet ds = objdal.ReportManagement(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId), 0, (int)report.report, (int)Operation.nothing);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[0].Rows)
                    {
                        returnsuppliers.Add(new ClsSuppliers2
                        {
                            Id = System.Convert.ToInt32(dtrow["ID"]),

                            SupplierName = dtrow["suppname"].ToString(),
                            complete = dtrow["completes"].ToString(),
                            percentage = dtrow["percentage"].ToString()

                        });
                    }
                    // gdvSupplierWise.DataSource = ds.Tables[0];
                    //gdvSupplierWise.DataBind();
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[1].Rows)
                    {


                        returnClients.Add(new ClsClients
                        {
                            Id = System.Convert.ToInt32(dtrow["id"]),

                            ClientName = dtrow["clieentname"].ToString(),
                            complete = dtrow["complete"].ToString(),
                            percentage = dtrow["percentage"].ToString()

                        });

                    }
                    //gdvClientWise.DataSource = ds.Tables[1];
                    //gdvClientWise.DataBind();
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {
                        returncountry.Add(new ClsCountry
                        {
                            Id = System.Convert.ToInt32(dr["id"]),
                            Country = dr["countryname"].ToString(),
                            NumericCode = dr["countryid"].ToString(),
                            Total = dr["total"].ToString(),
                            Percentage = dr["percentage"].ToString()
                        });
                    }
                    //gdvCountry.DataSource = ds.Tables[2];
                    //gdvCountry.DataBind();
                }

                if (ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {
                        returntotal.Add(new CLSTotal
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Total = dr["Total"].ToString(),
                            Closed = dr["Closed"].ToString(),
                            Inprogress = dr["InProgress"].ToString(),
                            Onhold = dr["Onhold"].ToString(),
                            Cancelled = dr["Cancelled"].ToString()
                        });
                    }
                    //gdvprojectstotal.DataSource = ds.Tables[3];
                    //gdvprojectstotal.DataBind();
                }
            }
            List<ClsIRate> ReturnIrate = new List<ClsIRate>();
            List<ClsLOI> returnLOI = new List<ClsLOI>();
            List<ClsCPI> returnCPI = new List<ClsCPI>();
            List<ClsActualLOI> returnacLOI = new List<ClsActualLOI>();
            List<CLSGetValue> ReturnValue = new List<CLSGetValue>();
            DataSet dsremain = objdal.RemainingTable(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId));
            if (dsremain.Tables.Count > 0)
            {
                if (dsremain.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[0].Rows)
                    {
                        if (dr["Max Irate"].ToString() != "")
                        {
                            ReturnIrate.Add(new ClsIRate
                            {
                                MaxIRate = Convert.ToInt32(dr["Max Irate"]),
                                MinIRate = Convert.ToInt32(dr["Min Irate"]),
                                AvgIRate = Convert.ToInt32(dr["Average Irate"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[1].Rows)
                    {
                        if (dr["Max LOI"].ToString() != "")
                        {
                            returnLOI.Add(new ClsLOI
                            {
                                MaxLOI = Convert.ToInt32(dr["Max LOI"]),
                                MinLOI = Convert.ToInt32(dr["Min LOI"]),
                                AvgLOI = Convert.ToInt32(dr["Average LOI"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[2].Rows)
                    {
                        if (dr["Max CPI"].ToString() != "")
                        {
                            returnCPI.Add(new ClsCPI
                            {
                                MaxCPI = Convert.ToInt32(dr["Max CPI"]),
                                MinCPI = Convert.ToInt32(dr["Min CPI"]),
                                AvgCPI = Convert.ToInt32(dr["Average CPI"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[3].Rows)
                    {
                        if (dr["Max Actual LOI"].ToString() != "")
                        {
                            returnacLOI.Add(new ClsActualLOI
                            {
                                MaxLOI = Convert.ToInt32(dr["Max Actual LOI"]),
                                MinLOI = Convert.ToInt32(dr["Min Actual LOI"]),
                                AvgLOI = Convert.ToInt32(dr["Average Actual LOI"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[4].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[4].Rows)
                    {
                        if (dr["summ"].ToString() != "")
                        {
                            ReturnValue.Add(new CLSGetValue
                            {
                                Summ = Convert.ToInt32(dr["summ"]),

                            });
                        }
                    }
                }
            }
            List<CLsRespondants> Returnrespondants = new List<CLsRespondants>();
            List<CLsSuccessRate> ReturnSuccesRate = new List<CLsSuccessRate>();
            DataSet dsRespondants = objdal.Respondants(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId));
            if (dsRespondants.Tables.Count > 0)
            {
                if (dsRespondants.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRespondants.Tables[0].Rows)
                    {
                        if (dr["Project Success Rate"].ToString() != "")
                        {
                            ReturnSuccesRate.Add(new CLsSuccessRate
                            {
                                ProjectSuccessRate = Convert.ToInt32(dr["Project Success Rate"]),
                                IRSuccessRate = Convert.ToInt32(dr["IR Success Rate"])
                            });
                        }
                    }
                }
                if (dsRespondants.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRespondants.Tables[1].Rows)
                    {
                        if (dr["Total"].ToString() != "")
                        {
                            Returnrespondants.Add(new CLsRespondants
                            {
                                Total = Convert.ToInt32(dr["Total"]),
                                Complete = Convert.ToInt32(dr["Complete"]),
                                Incomplete = Convert.ToInt32(dr["Incomplete"]),
                                Screened = Convert.ToInt32(dr["Screened"]),
                                Quotafull = Convert.ToInt32(dr["Quotafull"]),
                                Terminate = Convert.ToInt32(dr["Terminate"])
                            });
                        }
                    }
                }
            }
            List<CLSGetValueSupp> ReturnValueTablesupp = new List<CLSGetValueSupp>();
            List<CLSGetValueClient> ReturnValueTablecli = new List<CLSGetValueClient>();
            List<CLSGetValueCountry> ReturnValueTablecont = new List<CLSGetValueCountry>();
            DataSet dsRevenueTable = objdal.GetRevenueTable(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId));
            if (dsRevenueTable.Tables.Count > 0)
            {
                if (dsRevenueTable.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[0].Rows)
                    {

                        ReturnValueTablesupp.Add(new CLSGetValueSupp
                        {
                            SupplierName = dr["parameter"].ToString(),
                            suppcost = float.Parse(dr["revenue"].ToString()),
                            SuppPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
                if (dsRevenueTable.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[1].Rows)
                    {

                        ReturnValueTablecli.Add(new CLSGetValueClient
                        {
                            ClientName = dr["parameter"].ToString(),
                            ClientValue = float.Parse(dr["revenue"].ToString()),
                            ClientPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
                if (dsRevenueTable.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[2].Rows)
                    {

                        ReturnValueTablecont.Add(new CLSGetValueCountry
                        {
                            CountryName = dr["parameter"].ToString(),
                            ContValue = float.Parse(dr["revenue"].ToString()),
                            ContPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
            }
            object[] obj = new object[14];
            obj[0] = returnsuppliers;
            obj[1] = returnClients;
            obj[2] = returncountry;
            obj[3] = returntotal;
            obj[4] = ReturnIrate;
            obj[5] = returnLOI;
            obj[6] = returnCPI;
            obj[7] = ReturnSuccesRate;
            obj[8] = Returnrespondants;
            obj[9] = returnacLOI;
            obj[10] = ReturnValue;
            obj[11] = ReturnValueTablesupp;
            obj[12] = ReturnValueTablecli;
            obj[13] = ReturnValueTablecont;
            return obj;

        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsManager> ManagerWiseReport(OverallReportData formdata)
    {
        try
        {
            List<ClsManager> ReturnManagers = new List<ClsManager>();

            ClsDAL objdal = new ClsDAL();
            DataSet ds = objdal.ManagerReportManagement(formdata, formdata.sdate, formdata.edate, 0, 0, 0);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[0].Rows)
                    {
                        ReturnManagers.Add(new ClsManager
                        {
                            Name = dtrow["username"].ToString(),
                            Total = dtrow["total"].ToString(),
                            Totalpercentage = dtrow["percentagetotal"].ToString(),
                            complete = dtrow["complete"].ToString(),
                            Completepercentage = dtrow["percentcomple"].ToString(),
                            Revenue = dtrow["revenue"].ToString(),
                            Revenuepercentage = dtrow["percentrev"].ToString()
                        });
                    }
                    // gdvSupplierWise.DataSource = ds.Tables[0];
                    //gdvSupplierWise.DataBind();
                }
            }
            return ReturnManagers;

        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetReportForCompletionWD(string CountryId, string Clientid, string Supplierid, DateTime? sdate, DateTime? edate)
    {
        try
        {
            List<ClsSuppliers2> returnsuppliers = new List<ClsSuppliers2>();
            List<ClsClients> returnClients = new List<ClsClients>();
            List<CLSTotal> returntotal = new List<CLSTotal>();
            List<ClsCountry> returncountry = new List<ClsCountry>();
            //List<ClsIRate> ReturnIrate = new List<ClsIRate>();
            ClsDAL objdal = new ClsDAL();
            DataSet ds = objdal.ReportManagement(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId), 0, (int)report.report, (int)Operation.nothing);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[0].Rows)
                    {
                        returnsuppliers.Add(new ClsSuppliers2
                        {
                            Id = System.Convert.ToInt32(dtrow["ID"]),
                            SupplierName = dtrow["suppname"].ToString(),
                            complete = dtrow["completes"].ToString(),
                            percentage = dtrow["percentage"].ToString()
                        });
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dtrow in ds.Tables[1].Rows)
                    {
                        returnClients.Add(new ClsClients
                        {
                            Id = System.Convert.ToInt32(dtrow["id"]),
                            ClientName = dtrow["clieentname"].ToString(),
                            complete = dtrow["complete"].ToString(),
                            percentage = dtrow["percentage"].ToString()

                        });
                    }
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[2].Rows)
                    {
                        returncountry.Add(new ClsCountry
                        {
                            Id = System.Convert.ToInt32(dr["id"]),
                            Country = dr["countryname"].ToString(),
                            NumericCode = dr["countryid"].ToString(),
                            Total = dr["total"].ToString(),
                            Percentage = dr["percentage"].ToString()
                        });
                    }
                }

                if (ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[3].Rows)
                    {
                        returntotal.Add(new CLSTotal
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Total = dr["Total"].ToString(),
                            Closed = dr["Closed"].ToString(),
                            Inprogress = dr["InProgress"].ToString(),
                            Onhold = dr["Onhold"].ToString(),
                            Cancelled = dr["Cancelled"].ToString()
                        });
                    }
                }
            }
            object[] obj = new object[14];
            obj[0] = returnsuppliers;
            obj[1] = returnClients;
            obj[2] = returncountry;
            obj[3] = returntotal;
            return obj;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetReportForCompletion()
    {
        List<ClsSuppliers2> returnsuppliers = new List<ClsSuppliers2>();
        List<ClsClients> returnClients = new List<ClsClients>();
        List<CLSTotal> returntotal = new List<CLSTotal>();
        List<ClsCountry> returncountry = new List<ClsCountry>();
        ClsDAL objdal = new ClsDAL();
        DataSet ds = objdal.ReportManagement(null, null, 0, 0, 0, 0, (int)report.report, (int)Operation.nothing);
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dtrow in ds.Tables[0].Rows)
                {
                    returnsuppliers.Add(new ClsSuppliers2
                    {
                        Id = System.Convert.ToInt32(dtrow["ID"]),

                        SupplierName = dtrow["suppname"].ToString(),
                        complete = dtrow["completes"].ToString(),
                        percentage = dtrow["percentage"].ToString()

                    });
                }
                // gdvSupplierWise.DataSource = ds.Tables[0];
                //gdvSupplierWise.DataBind();
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dtrow in ds.Tables[1].Rows)
                {


                    returnClients.Add(new ClsClients
                    {
                        Id = System.Convert.ToInt32(dtrow["id"]),

                        ClientName = dtrow["clieentname"].ToString(),
                        complete = dtrow["complete"].ToString(),
                        percentage = dtrow["percentage"].ToString()

                    });

                }
                //gdvClientWise.DataSource = ds.Tables[1];
                //gdvClientWise.DataBind();
            }
            if (ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    returncountry.Add(new ClsCountry
                    {
                        Id = System.Convert.ToInt32(dr["id"]),
                        Country = dr["countryname"].ToString(),
                        NumericCode = dr["countryid"].ToString(),
                        Total = dr["total"].ToString(),
                        Percentage = dr["percentage"].ToString()
                    });
                }
                //gdvCountry.DataSource = ds.Tables[2];
                //gdvCountry.DataBind();
            }
            if (ds.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    returntotal.Add(new CLSTotal
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Total = dr["Total"].ToString(),
                        Closed = dr["Closed"].ToString(),
                        Inprogress = dr["InProgress"].ToString(),
                        Onhold = dr["Onhold"].ToString(),
                        Cancelled = dr["Cancelled"].ToString()
                    });
                }
                //gdvprojectstotal.DataSource = ds.Tables[3];
                //gdvprojectstotal.DataBind();
            }
        }
        object[] obj = new object[4];
        obj[0] = returnsuppliers;
        obj[1] = returnClients;
        obj[2] = returncountry;
        obj[3] = returntotal;
        return obj;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetReportForRateWD(string CountryId, string Clientid, string Supplierid, DateTime? sdate, DateTime? edate)
    {
        try
        {
            ClsDAL objdal = new ClsDAL();
            List<ClsIRate> ReturnIrate = new List<ClsIRate>();
            List<ClsLOI> returnLOI = new List<ClsLOI>();
            List<ClsCPI> returnCPI = new List<ClsCPI>();
            List<ClsActualLOI> returnacLOI = new List<ClsActualLOI>();
            List<CLSGetValue> ReturnValue = new List<CLSGetValue>();
            DataSet dsremain = objdal.RemainingTable(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId));
            if (dsremain.Tables.Count > 0)
            {
                if (dsremain.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[0].Rows)
                    {
                        if (dr["Max Irate"].ToString() != "")
                        {
                            ReturnIrate.Add(new ClsIRate
                            {
                                MaxIRate = Convert.ToInt32(dr["Max Irate"]),
                                MinIRate = Convert.ToInt32(dr["Min Irate"]),
                                AvgIRate = Convert.ToInt32(dr["Average Irate"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[1].Rows)
                    {
                        if (dr["Max LOI"].ToString() != "")
                        {
                            returnLOI.Add(new ClsLOI
                            {
                                MaxLOI = Convert.ToInt32(dr["Max LOI"]),
                                MinLOI = Convert.ToInt32(dr["Min LOI"]),
                                AvgLOI = Convert.ToInt32(dr["Average LOI"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[2].Rows)
                    {
                        if (dr["Max CPI"].ToString() != "")
                        {
                            returnCPI.Add(new ClsCPI
                            {
                                MaxCPI = Convert.ToInt32(dr["Max CPI"]),
                                MinCPI = Convert.ToInt32(dr["Min CPI"]),
                                AvgCPI = Convert.ToInt32(dr["Average CPI"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[3].Rows)
                    {
                        if (dr["Max Actual LOI"].ToString() != "")
                        {
                            returnacLOI.Add(new ClsActualLOI
                            {
                                MaxLOI = Convert.ToInt32(dr["Max Actual LOI"]),
                                MinLOI = Convert.ToInt32(dr["Min Actual LOI"]),
                                AvgLOI = Convert.ToInt32(dr["Average Actual LOI"])
                            });
                        }
                    }
                }
                if (dsremain.Tables[4].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsremain.Tables[4].Rows)
                    {
                        if (dr["summ"].ToString() != "")
                        {
                            ReturnValue.Add(new CLSGetValue
                            {
                                Summ = Convert.ToInt32(dr["summ"]),

                            });
                        }
                    }
                }
            }
            object[] obj = new object[5];
            obj[0] = ReturnIrate;
            obj[1] = returnLOI;
            obj[2] = returnCPI;
            obj[3] = returnacLOI;
            obj[4] = ReturnValue;
            return obj;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetReportForRate()
    {
        ClsDAL objdal = new ClsDAL();
        List<ClsIRate> ReturnIrate = new List<ClsIRate>();
        List<ClsLOI> returnLOI = new List<ClsLOI>();
        List<ClsCPI> returnCPI = new List<ClsCPI>();
        List<ClsActualLOI> returnacLOI = new List<ClsActualLOI>();
        List<CLSGetValue> ReturnValue = new List<CLSGetValue>();
        DataSet dsremain = objdal.RemainingTable(null, null, 0, 0, 0);
        if (dsremain.Tables.Count > 0)
        {
            if (dsremain.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsremain.Tables[0].Rows)
                {
                    if (dr["Max Irate"].ToString() != "")
                    {
                        ReturnIrate.Add(new ClsIRate
                        {
                            MaxIRate = Convert.ToInt32(dr["Max Irate"]),
                            MinIRate = Convert.ToInt32(dr["Min Irate"]),
                            AvgIRate = Convert.ToInt32(dr["Average Irate"])
                        });
                    }
                }
            }
            if (dsremain.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in dsremain.Tables[1].Rows)
                {
                    if (dr["Max LOI"].ToString() != "")
                    {
                        returnLOI.Add(new ClsLOI
                        {
                            MaxLOI = Convert.ToInt32(dr["Max LOI"]),
                            MinLOI = Convert.ToInt32(dr["Min LOI"]),
                            AvgLOI = Convert.ToInt32(dr["Average LOI"])
                        });
                    }
                }
            }
            if (dsremain.Tables[2].Rows.Count > 0)
            {

                foreach (DataRow dr in dsremain.Tables[2].Rows)
                {
                    if (dr["Max CPI"].ToString() != "")
                    {
                        returnCPI.Add(new ClsCPI
                        {
                            MaxCPI = Convert.ToInt32(dr["Max CPI"]),
                            MinCPI = Convert.ToInt32(dr["Min CPI"]),
                            AvgCPI = Convert.ToInt32(dr["Average CPI"])
                        });
                    }
                }
            }
            if (dsremain.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow dr in dsremain.Tables[3].Rows)
                {
                    if (dr["Max Actual LOI"].ToString() != "")
                    {
                        returnacLOI.Add(new ClsActualLOI
                        {
                            MaxLOI = Convert.ToInt32(dr["Max Actual LOI"]),
                            MinLOI = Convert.ToInt32(dr["Min Actual LOI"]),
                            AvgLOI = Convert.ToInt32(dr["Average Actual LOI"])
                        });
                    }
                }
            }
            if (dsremain.Tables[4].Rows.Count > 0)
            {
                foreach (DataRow dr in dsremain.Tables[4].Rows)
                {
                    if (dr["summ"].ToString() != "")
                    {
                        ReturnValue.Add(new CLSGetValue
                        {
                            Summ = Convert.ToInt32(dr["summ"]),

                        });
                    }
                }
            }
        }
        object[] obj = new object[5];
        obj[0] = ReturnIrate;
        obj[1] = returnLOI;
        obj[2] = returnCPI;
        obj[3] = returnacLOI;
        obj[4] = ReturnValue;
        return obj;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetReportForRespondantsWD(string CountryId, string Clientid, string Supplierid, DateTime? sdate, DateTime? edate)
    {
        try
        {
            ClsDAL objdal = new ClsDAL();
            List<CLsRespondants> Returnrespondants = new List<CLsRespondants>();
            List<CLsSuccessRate> ReturnSuccesRate = new List<CLsSuccessRate>();
            DataSet dsRespondants = objdal.Respondants(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId));
            if (dsRespondants.Tables.Count > 0)
            {
                if (dsRespondants.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRespondants.Tables[0].Rows)
                    {
                        if (dr["Project Success Rate"].ToString() != "")
                        {
                            ReturnSuccesRate.Add(new CLsSuccessRate
                            {
                                ProjectSuccessRate = Convert.ToInt32(dr["Project Success Rate"]),
                                IRSuccessRate = Convert.ToInt32(dr["IR Success Rate"])
                            });
                        }
                    }
                }
                if (dsRespondants.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRespondants.Tables[1].Rows)
                    {
                        if (dr["Total"].ToString() != "")
                        {
                            Returnrespondants.Add(new CLsRespondants
                            {
                                Total = Convert.ToInt32(dr["Total"]),
                                Complete = Convert.ToInt32(dr["Complete"]),
                                Incomplete = Convert.ToInt32(dr["Incomplete"]),
                                Screened = Convert.ToInt32(dr["Screened"]),
                                Quotafull = Convert.ToInt32(dr["Quotafull"]),
                                Terminate = Convert.ToInt32(dr["Terminate"])
                            });
                        }
                    }
                }
            }
            object[] obj = new object[2];
            obj[0] = ReturnSuccesRate;
            obj[1] = Returnrespondants;
            return obj;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetReportForRespondants()
    {
        ClsDAL objdal = new ClsDAL();
        List<CLsRespondants> Returnrespondants = new List<CLsRespondants>();
        List<CLsSuccessRate> ReturnSuccesRate = new List<CLsSuccessRate>();
        DataSet dsRespondants = objdal.Respondants(null, null, 0, 0, 0);
        if (dsRespondants.Tables.Count > 0)
        {
            if (dsRespondants.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsRespondants.Tables[0].Rows)
                {
                    if (dr["Project Success Rate"].ToString() != "")
                    {
                        ReturnSuccesRate.Add(new CLsSuccessRate
                        {
                            ProjectSuccessRate = Convert.ToInt32(dr["Project Success Rate"]),
                            IRSuccessRate = Convert.ToInt32(dr["IR Success Rate"])
                        });
                    }
                }
            }
            if (dsRespondants.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in dsRespondants.Tables[1].Rows)
                {
                    if (dr["Total"].ToString() != "")
                    {
                        Returnrespondants.Add(new CLsRespondants
                        {
                            Total = Convert.ToInt32(dr["Total"]),
                            Complete = Convert.ToInt32(dr["Complete"]),
                            Incomplete = Convert.ToInt32(dr["Incomplete"]),
                            Screened = Convert.ToInt32(dr["Screened"]),
                            Quotafull = Convert.ToInt32(dr["Quotafull"]),
                            Terminate = Convert.ToInt32(dr["Terminate"])
                        });
                    }
                }
            }
        }
        object[] obj = new object[2];
        obj[0] = ReturnSuccesRate;
        obj[1] = Returnrespondants;
        return obj;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object ReturnValuesWD(string CountryId, string Clientid, string Supplierid, DateTime? sdate, DateTime? edate)
    {
        try
        {
            ClsDAL objdal = new ClsDAL();
            List<CLSGetValueSupp> ReturnValueTablesupp = new List<CLSGetValueSupp>();
            List<CLSGetValueClient> ReturnValueTablecli = new List<CLSGetValueClient>();
            List<CLSGetValueCountry> ReturnValueTablecont = new List<CLSGetValueCountry>();
            DataSet dsRevenueTable = objdal.GetRevenueTable(sdate, edate, Convert.ToInt32(Supplierid), Convert.ToInt32(Clientid), Convert.ToInt32(CountryId));
            if (dsRevenueTable.Tables.Count > 0)
            {
                if (dsRevenueTable.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[0].Rows)
                    {

                        ReturnValueTablesupp.Add(new CLSGetValueSupp
                        {
                            SupplierName = dr["parameter"].ToString(),
                            suppcost = float.Parse(dr["revenue"].ToString()),
                            SuppPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
                if (dsRevenueTable.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[1].Rows)
                    {

                        ReturnValueTablecli.Add(new CLSGetValueClient
                        {
                            ClientName = dr["parameter"].ToString(),
                            ClientValue = float.Parse(dr["revenue"].ToString()),
                            ClientPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
                if (dsRevenueTable.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[2].Rows)
                    {

                        ReturnValueTablecont.Add(new CLSGetValueCountry
                        {
                            CountryName = dr["parameter"].ToString(),
                            ContValue = float.Parse(dr["revenue"].ToString()),
                            ContPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
            }
            object[] obj = new object[3];
            obj[0] = ReturnValueTablesupp;
            obj[1] = ReturnValueTablecli;
            obj[2] = ReturnValueTablecont;
            return obj;

        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object ReturnValues()
    {
        try
        {
            ClsDAL objdal = new ClsDAL();
            List<CLSGetValueSupp> ReturnValueTablesupp = new List<CLSGetValueSupp>();
            List<CLSGetValueClient> ReturnValueTablecli = new List<CLSGetValueClient>();
            List<CLSGetValueCountry> ReturnValueTablecont = new List<CLSGetValueCountry>();
            DataSet dsRevenueTable = objdal.GetRevenueTable(null, null, 0, 0, 0);
            if (dsRevenueTable.Tables.Count > 0)
            {
                if (dsRevenueTable.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[0].Rows)
                    {
                        ReturnValueTablesupp.Add(new CLSGetValueSupp
                        {
                            SupplierName = dr["parameter"].ToString(),
                            suppcost = float.Parse(dr["revenue"].ToString()),
                            SuppPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
                if (dsRevenueTable.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[1].Rows)
                    {

                        ReturnValueTablecli.Add(new CLSGetValueClient
                        {
                            ClientName = dr["parameter"].ToString(),
                            ClientValue = float.Parse(dr["revenue"].ToString()),
                            ClientPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
                if (dsRevenueTable.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsRevenueTable.Tables[2].Rows)
                    {

                        ReturnValueTablecont.Add(new CLSGetValueCountry
                        {
                            CountryName = dr["parameter"].ToString(),
                            ContValue = float.Parse(dr["revenue"].ToString()),
                            ContPercent = float.Parse(dr["percentt"].ToString())
                        });

                    }
                }
            }
            object[] obj = new object[3];
            obj[0] = ReturnValueTablesupp;
            obj[1] = ReturnValueTablecli;
            obj[2] = ReturnValueTablecont;
            return obj;
        }
        catch (Exception e)
        {
            return null;
        }
    }

}

public class CLSTotal
{
    public int Id { get; set; }
    public string Total { get; set; }
    public string Closed { get; set; }
    public string Inprogress { get; set; }
    public string Onhold { get; set; }
    public string Cancelled { get; set; }
}
