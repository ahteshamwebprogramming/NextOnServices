using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

public partial class VT_SurveyMaster : System.Web.UI.Page
{
    static DataSet dsrecords = new DataSet();
    static DataSet dsOptions = new DataSet();
    ClsDAL objDAL = new ClsDAL();
    static DataRow[] DrVal = null;
    static int QsID = 0, Qtype = 0, CntNo = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            QsID = 0; Qtype = 0; CntNo = 0;
            DrVal = null;
            hdnURL.Value = Convert.ToString(Session["URL"]);
            //Session["PrID"] = 1057;
            //Session["UID"] = "dsafjdsbjsfdbjfsdbjm";
            //Session["URL"] = "";
            // BindColumnsDetails(0,1055,0);
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsPreScreening> GetControls()
    {
        try
        {
            ClsDAL obj = new ClsDAL();
            DataSet ds = new DataSet();
            ClsPreScreening formData = new ClsPreScreening();
            formData.ProjectID = Convert.ToInt32(HttpContext.Current.Session["PrID"]);
            formData.CountryID = Convert.ToInt32(HttpContext.Current.Session["CntryID"]);
            List<ClsPreScreening> RetResult = new List<ClsPreScreening>();
            ds = obj.GetQuestions(formData, 1);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            RetResult.Add(new ClsPreScreening
                            {
                                PreviousButton = Convert.ToInt32(dr["PreviousButton"]),
                                QuestionQID = Convert.ToInt32(dr["QuestionQID"]),
                                Logo = Convert.ToInt32(dr["Logo"])
                            });
                        }
                    }
                }
            }
            return RetResult;


        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<SurveyDetails> GetQuestions(int QID, int Action, int Cnt)
    {
        ClsDAL objDAL = new ClsDAL();
        List<SurveyDetails> returnData = new List<SurveyDetails>();
        try
        {

            if (dsrecords.Tables.Count > 0)
            {
                dsrecords.Tables[0].Clear();
            }
            dsrecords = objDAL.GetSurveyQuestions(QID, Convert.ToInt32(HttpContext.Current.Session["PrID"]), Action, Convert.ToInt32(HttpContext.Current.Session["CntryID"]));
            if (dsrecords.Tables[0].Rows.Count > 0)
            {
                dsrecords.Tables[0].Columns.Add(new DataColumn("Status"));


                foreach (DataRow row in dsrecords.Tables[0].Rows)
                {
                    row["Status"] = "0";

                }

                CntNo = Cnt;

                DrVal = dsrecords.Tables[0].Select("id= " + Cnt + " and Status=0");
                if (DrVal.Length > 0)
                {

                    //lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
                    //QsID = Convert.ToInt32(DrVal[0]["PKID"]);
                    //Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);

                    foreach (DataRow dtrow in DrVal)
                    {
                        returnData.Add(new SurveyDetails
                        {
                            ID = System.Convert.ToInt32(dtrow["ID"]),
                            PKID = System.Convert.ToInt32(dtrow["PKID"]),
                            QID = System.Convert.ToString(dtrow["QID"]),
                            QLabel = System.Convert.ToString(dtrow["QLabel"]),
                            Qtype = System.Convert.ToInt32(dtrow["Qtype"]),
                        });
                    }
                }
                // Cnt = Cnt + 1;
                // PopulateQuestions(Cnt);



            }
            else
            {
                //  return returnData;
            }
        }
        catch (SystemException ex)
        {
            return returnData;
        }
        return returnData;

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<OptionsDetails> GetOptions(int QID, int Action, int Cnt)
    {
        ClsDAL objDAL = new ClsDAL();
        List<OptionsDetails> returnData = new List<OptionsDetails>();

        try
        {

            if (dsOptions.Tables.Count > 0)
            {
                dsOptions.Tables[0].Clear();
            }
            dsOptions = objDAL.GetSurveyQuestions(QID, 0, 1, 0);
            if (dsOptions.Tables[0].Rows.Count > 0)
            {

                //lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
                //QsID = Convert.ToInt32(DrVal[0]["PKID"]);
                //Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);

                foreach (DataRow dtrow in dsOptions.Tables[0].Rows)
                {
                    returnData.Add(new OptionsDetails
                    {
                        optioncode = System.Convert.ToString(dtrow["optioncode"]),
                        OptionLabel = System.Convert.ToString(dtrow["OptionLabel"]),

                    });
                }

                // Cnt = Cnt + 1;
                // PopulateQuestions(Cnt);



            }
            else
            {
                //  return returnData;
            }
        }
        catch (SystemException ex)
        {
            return returnData;
        }
        return returnData;

    }
    protected void BindColumnsDetails(int QID, int PID, int Action)
    {
        //con.Open();

        try
        {
            if (dsrecords.Tables.Count > 0)
            {
                dsrecords.Tables[0].Clear();
            }
            dsrecords = objDAL.GetSurveyQuestions(QID, PID, Action, 0);
            if (dsrecords.Tables[0].Rows.Count > 0)
            {
                dsrecords.Tables[0].Columns.Add(new DataColumn("Status"));


                foreach (DataRow row in dsrecords.Tables[0].Rows)
                {
                    row["Status"] = "0";

                }
                // Cnt = Cnt + 1;
                // PopulateQuestions(Cnt);
                //DrVal = dsrecords.Tables[0].Select("id=1 and Status=0");
                //if (DrVal.Length > 0)
                //{

                //    lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " +  Convert.ToString(DrVal[0]["QLabel"]);
                //    QsID = Convert.ToInt32(DrVal[0]["PKID"]);
                //    Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);

                //    if (Qtype == 1)
                //    {
                //        dsOptions = objDAL.GetSurveyQuestions(QsID, 0, 1);

                //        // chk1.Visible = true;
                //        txt1.Visible = false;
                //        rdo1.Visible = true;
                //        chk1.Visible = false;

                //        if (dsOptions.Tables[0].Rows.Count > 0)
                //        {
                //            rdo1.DataTextField = "OptionLabel";
                //            rdo1.DataValueField = "optioncode";
                //            rdo1.DataSource = dsOptions;
                //            rdo1.DataBind();
                //        }
                //    }
                //    else if (Qtype == 2)
                //    {
                //        dsOptions = objDAL.GetSurveyQuestions(QsID, 0, 1);

                //        chk1.Visible = true;
                //        txt1.Visible = false;
                //        rdo1.Visible =false;

                //        if (dsOptions.Tables[0].Rows.Count > 0)
                //        {
                //            chk1.DataTextField = "OptionLabel";
                //            chk1.DataValueField = "optioncode";
                //            chk1.DataSource = dsOptions;
                //            chk1.DataBind();
                //        }
                //    }
                //    else if (Qtype == 3)
                //    {
                //        chk1.Visible = false;
                //        txt1.Visible = true;
                //        rdo1.Visible = false;




                //    }


                //}


            }
            else
            {

            }
        }
        catch (SystemException ex)
        {
        }

    }
    //public  string PopulateQuestions(int Cnt)
    //{
    //    try
    //    {
    //        DrVal = dsrecords.Tables[0].Select("id= " + Cnt + " and Status=0");
    //        if (DrVal.Length > 0)
    //        {

    //            lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
    //            QsID = Convert.ToInt32(DrVal[0]["PKID"]);
    //            Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);

    //            if (dsOptions.Tables.Count > 0)
    //            {
    //                dsOptions.Tables[0].Clear();
    //            }
    //            if (Qtype == 1)
    //            {
    //                dsOptions = objDAL.GetSurveyQuestions(QsID, 0, 1);

    //                // chk1.Visible = true;
    //                txt1.Visible = false;
    //                rdo1.Visible = true;
    //                chk1.Visible = false;

    //                if (dsOptions.Tables[0].Rows.Count > 0)
    //                {
    //                    rdo1.DataTextField = "OptionLabel";
    //                    rdo1.DataValueField = "optioncode";
    //                    rdo1.DataSource = dsOptions;
    //                    rdo1.DataBind();
    //                }
    //            }
    //            else if (Qtype == 2)
    //            {
    //                dsOptions = objDAL.GetSurveyQuestions(QsID, 0, 1);

    //                chk1.Visible = true;
    //                txt1.Visible = false;
    //                rdo1.Visible = false;

    //                if (dsOptions.Tables[0].Rows.Count > 0)
    //                {
    //                    chk1.DataTextField = "OptionLabel";
    //                    chk1.DataValueField = "optioncode";
    //                    chk1.DataSource = dsOptions;
    //                    chk1.DataBind();
    //                }
    //            }
    //            else if (Qtype == 3)
    //            {
    //                chk1.Visible = false;
    //                txt1.Visible = true;
    //                rdo1.Visible = false;

    //            }


    //        }

    //    }
    //    catch (SystemException ex)
    //    {

    //    }
    //    return "";
    //}
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<SurveyDetails> NextProcess(string QLabel, int QID, string Options, int Action)
    {
        ClsDAL objDAL = new ClsDAL();
        List<SurveyDetails> returnData = new List<SurveyDetails>();
        try
        {
            if (Options.IndexOf(',') >= 0)
            {
                Options = Options.Substring(0, Options.LastIndexOf(','));
            }
            string output = objDAL.MgrQuestionSelectedOptions(0, QID, Options, Convert.ToInt32(HttpContext.Current.Session["PrID"]), Convert.ToString(HttpContext.Current.Session["UID"]));
            string terminateresult = "";
            if (CheckForTerminate(Convert.ToInt32(HttpContext.Current.Session["PrID"]), Convert.ToInt32(HttpContext.Current.Session["CntryID"]), Options, QID, out terminateresult))
            {
                returnData.Add(new SurveyDetails
                {
                    ID = System.Convert.ToInt32(0),
                    PKID = System.Convert.ToInt32(0),
                    QID = System.Convert.ToString(terminateresult),
                    QLabel = System.Convert.ToString("Terminate"),
                    Qtype = System.Convert.ToInt32(0),
                });
                return returnData;
            }
            CntNo = CntNo + 1;
            if (CntNo > dsrecords.Tables[0].Rows.Count)
            {
                returnData.Add(new SurveyDetails
                {
                    ID = 0,
                    PKID = 0,
                    QID = "0",
                    QLabel = "",
                    Qtype = 0,
                });

                return returnData;
            }
            DrVal = dsrecords.Tables[0].Select("id= " + CntNo + " and Status=0");
            if (DrVal.Length > 0)
            {

                //lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
                //QsID = Convert.ToInt32(DrVal[0]["PKID"]);
                //Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);

                foreach (DataRow dtrow in DrVal)
                {
                    returnData.Add(new SurveyDetails
                    {
                        ID = System.Convert.ToInt32(dtrow["ID"]),
                        PKID = System.Convert.ToInt32(dtrow["PKID"]),
                        QID = System.Convert.ToString(dtrow["QID"]),
                        QLabel = System.Convert.ToString(dtrow["QLabel"]),
                        Qtype = System.Convert.ToInt32(dtrow["Qtype"]),
                    });
                }
            }
            //  return "test";
        }
        catch (Exception ex)
        {

            return returnData;
            // return "There is some error. Please contact administrator";
        }
        return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<SurveyDetails> PreviusProcess()
    {
        ClsDAL objDAL = new ClsDAL();
        List<SurveyDetails> returnData = new List<SurveyDetails>();
        try
        {

            CntNo = CntNo - 1;
            DrVal = dsrecords.Tables[0].Select("id= " + CntNo + " and Status=0");
            if (DrVal.Length > 0)
            {

                //lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
                //QsID = Convert.ToInt32(DrVal[0]["PKID"]);
                //Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);

                foreach (DataRow dtrow in DrVal)
                {
                    returnData.Add(new SurveyDetails
                    {
                        ID = System.Convert.ToInt32(dtrow["ID"]),
                        PKID = System.Convert.ToInt32(dtrow["PKID"]),
                        QID = System.Convert.ToString(dtrow["QID"]),
                        QLabel = System.Convert.ToString(dtrow["QLabel"]),
                        Qtype = System.Convert.ToInt32(dtrow["Qtype"]),
                    });
                }
            }
            //  return "test";
        }
        catch (Exception ex)
        {

            return returnData;
            // return "There is some error. Please contact administrator";
        }
        return returnData;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<OptionIDs> GetSelectedOptions(int QID)
    {
        ClsDAL objDAL = new ClsDAL();
        List<OptionIDs> returnData = new List<OptionIDs>();
        try
        {
            dsOptions.Clear();
            dsOptions = objDAL.GetQuestionSelectedOptions(QID, Convert.ToInt32(HttpContext.Current.Session["PrID"]), Convert.ToString(HttpContext.Current.Session["UID"]));
            if (dsOptions.Tables[0].Rows.Count > 0)
            {

                //lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
                //QsID = Convert.ToInt32(DrVal[0]["PKID"]);
                //Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);

                foreach (DataRow dtrow in dsOptions.Tables[0].Rows)
                {

                    //lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
                    //QsID = Convert.ToInt32(DrVal[0]["PKID"]);
                    //Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);


                    returnData.Add(new OptionIDs
                    {
                        optioncode = System.Convert.ToString(dtrow["ITEMS"]),


                    });
                }
            }
            //  return "test";
        }
        catch (Exception ex)
        {

            return returnData;
            // return "There is some error. Please contact administrator";
        }
        return returnData;
    }
    protected void btnNext_Click(object sender, EventArgs e)
    {
        try
        {
            // Cnt = Cnt + 1;
            // PopulateQuestions(Cnt);
            //// string x = objDAL.Updatestatussupplierprojects(id, action, "");
            //  return "test";
        }
        catch (Exception ex)
        {
            //  return "There is some error. Please contact administrator";
        }
    }

    public static bool CheckForTerminate(int PID, int CID, string Options, int QuestionId, out string res)
    {
        ClsDAL clsDAL = new ClsDAL();
        res = clsDAL.CheckForTerminate(PID, CID, Options, QuestionId);
        if (Convert.ToInt32(res) < 0)
            return true;
        else
            return false;
    }

}

public class SurveyDetails
{
    public int ID { get; set; }
    public string QID { get; set; }
    public string QLabel { get; set; }
    public int PKID { get; set; }
    public int Qtype { get; set; }


    //lbl1.Text = "(" + Convert.ToString(DrVal[0]["QID"]) + ") " + Convert.ToString(DrVal[0]["QLabel"]);
    //           QsID = Convert.ToInt32(DrVal[0]["PKID"]);
    //           Qtype = Convert.ToInt32(DrVal[0]["Qtype"]);  rdo1.DataTextField = "OptionLabel";
    // rdo1.DataValueField = "optioncode";
}

public class OptionsDetails
{
    public string optioncode { get; set; }
    public string OptionLabel { get; set; }

}

public class OptionIDs
{
    public string optioncode { get; set; }
    // public string OptionLabel { get; set; }

}