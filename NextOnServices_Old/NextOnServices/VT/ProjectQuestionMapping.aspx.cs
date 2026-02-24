using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;

public partial class VT_ProjectQuestionMapping : System.Web.UI.Page
{
    static ClsDAL obj = new ClsDAL();
    static DataSet ds = new DataSet();
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProject> GetProjects(int ProjectID)
    {
        try
        {
            ClsDAL objDAL = new ClsDAL();
            List<ClsProject> returnProject = new List<ClsProject>();
            DataSet dsProject = objDAL.ClientMGR(ProjectID, "P");
            if (dsProject.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dtrow in dsProject.Tables[0].Rows)
                {
                    returnProject.Add(new ClsProject
                    {
                        ID = System.Convert.ToInt32(dtrow["ID"]),
                        PName = dtrow["PNAME"].ToString()
                    });
                }
            }
            return returnProject;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsPreScreening> GetQuestions(ClsPreScreening formData)
    {
        try
        {
            List<ClsPreScreening> RetResult = new List<ClsPreScreening>();
            ds = obj.GetQuestions(formData, 0);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString() == "Fresh")
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            RetResult.Add(new ClsPreScreening
                            {
                                ID = Convert.ToInt32(dr["ID"]),
                                QuestionID = Convert.ToString(dr["QuestionID"]),
                                QuestionLabel = Convert.ToString(dr["QuestionLabel"]),
                                QuestionType = Convert.ToInt32(dr["QuestionType"])
                            });
                        }
                    }
                    else if (ds.Tables[0].Rows[0][0].ToString() == "Exists")
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            RetResult.Add(new ClsPreScreening
                            {
                                ID = Convert.ToInt32(dr["ID"]),
                                QuestionID = Convert.ToString(dr["QuestionID"]),
                                QuestionLabel = Convert.ToString(dr["QuestionLabel"]),
                                QuestionType = Convert.ToInt32(dr["QuestionType"])
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
    public static List<ClsPreScreening> GetControls(ClsPreScreening formData)
    {
        try
        {
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
    public static List<ClsPreScreening> MapQuestions(ClsPreScreening formData)
    {
        try
        {
            List<ClsPreScreening> RetResult = new List<ClsPreScreening>();
            ds = obj.MapQuestions(formData);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        RetResult.Add(new ClsPreScreening
                        {
                            RetStat = Convert.ToString(dr["RetStat"])
                        });
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
    public static List<ClsPreScreening> MapOptions(ClsPreScreening formData)
    {
        try
        {
            List<ClsPreScreening> RetResult = new List<ClsPreScreening>();
            ds = obj.MapOptions(formData);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        RetResult.Add(new ClsPreScreening
                        {
                            RetStat = Convert.ToString(dr["RetStat"])
                        });
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
    public static int MapOptions1(List<OptionMapping> formData)
    {
        try
        {
            int res = 1;
            if (formData.Count > 0)
            {
                foreach (OptionMapping item in formData)
                {
                    res = obj.MapOptions1(item);
                    if (res == 0)
                        return 0;
                }
            }


            return 1;
        }
        catch (Exception e)
        {
            return 0;
        }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<QuestionOptions> FetchOptions(QuestionOptions formData)
    {
        try
        {
            List<QuestionOptions> RetResult = new List<QuestionOptions>();
            ds = obj.FetchOptions(formData);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        RetResult.Add(new QuestionOptions
                        {
                            Id = Convert.ToInt32(dr["ID"]),
                            QID = Convert.ToInt32(dr["QID"]),
                            OptionLabel = Convert.ToString(dr["OptionLabel"]),
                            OptionCode = Convert.ToString(dr["OptionCode"]),
                            Logic = Convert.ToInt32(dr["Logic"]),
                            Quota = Convert.ToInt32(dr["Quota"])
                        });
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
}