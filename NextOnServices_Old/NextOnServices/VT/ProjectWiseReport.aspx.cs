using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectWiseReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsProject> GetProjects()
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsProject> returnProject = new List<ClsProject>();
        DataSet dsProject = objDAL.ClientMGR(0, "P");
        if (dsProject.Tables[0].Rows.Count > 0)
        {

            foreach (DataRow dtrow in dsProject.Tables[0].Rows)
            {


                returnProject.Add(new ClsProject
                {
                    ID = System.Convert.ToInt32(dtrow["ID"]),
                    PName = dtrow["PCode"].ToString()
                });
            }
        }
        return returnProject;
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static object GetProjectDetails(int Id)
    {
        ClsDAL objDAL = new ClsDAL();
        List<ClsProjectWiseReport> returnProject = new List<ClsProjectWiseReport>();
        List<ClsProjectWiseReport> returnProjectCountryWise = new List<ClsProjectWiseReport>();
        List<ClsProjectWiseReport> returnProjectSupplierWise = new List<ClsProjectWiseReport>();
        List<ClsProjectWiseReport> ReturnOverAll = new List<ClsProjectWiseReport>();
        DataSet dsProject = objDAL.ProjectReports(Id);
        if (dsProject.Tables.Count > 0)
        {
            if (dsProject.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dtrow in dsProject.Tables[0].Rows)
                {
                    returnProject.Add(new ClsProjectWiseReport
                    {
                        PNumber = Convert.ToString(dtrow["PNumber"]),
                        PStatus = Convert.ToString(dtrow["PStatus"]),
                        Company = Convert.ToString(dtrow["Company"]),
                        Manager = Convert.ToString(dtrow["Manager"]),
                        Country = Convert.ToString(dtrow["Country"]),
                        Complete = float.Parse(Convert.ToString(dtrow["Complete"])),
                        CPC = float.Parse(Convert.ToString(dtrow["CPC"])),
                        RevenueFromCompletes = float.Parse(Convert.ToString(dtrow["RevenueFromCompletes"]))
                    });
                }
            }
            if (dsProject.Tables[1].Rows.Count > 0)
            {

                foreach (DataRow dtrow in dsProject.Tables[1].Rows)
                {


                    returnProjectCountryWise.Add(new ClsProjectWiseReport
                    {
                        CountryID = System.Convert.ToInt32(dtrow["CID"]),
                        Country = Convert.ToString(dtrow["Country"]),
                        Complete = float.Parse(Convert.ToString(dtrow["Complete"])),
                        CPC = float.Parse(Convert.ToString(dtrow["CPC"])),
                        TotalRevenue = float.Parse(Convert.ToString(dtrow["TotalRevenue"]))
                    });

                }
            }
            if (dsProject.Tables[2].Rows.Count > 0)
            {

                foreach (DataRow dtrow in dsProject.Tables[2].Rows)
                {


                    returnProjectSupplierWise.Add(new ClsProjectWiseReport
                    {
                        CountryID = System.Convert.ToInt32(dtrow["CountryID"]),
                        SupplierID = System.Convert.ToInt32(dtrow["SupplierID"]),
                        Country = Convert.ToString(dtrow["Country"]),
                        Supplier = Convert.ToString(dtrow["Supplier"]),
                        Complete = float.Parse(dtrow["Complete"].ToString()),
                        CPC = float.Parse(dtrow["CPC"].ToString()),
                        Cost = float.Parse(dtrow["Cost"].ToString())

                    });

                }
            }
            if (dsProject.Tables[3].Rows.Count > 0)
            {

                foreach (DataRow dtrow in dsProject.Tables[3].Rows)
                {


                    ReturnOverAll.Add(new ClsProjectWiseReport
                    {
                        TotalRevenue = float.Parse(dtrow["TotalRevenue"].ToString()),
                        TotalCost = float.Parse(dtrow["TotalCost"].ToString()),
                        GrossProfit = float.Parse(dtrow["GrossProfit"].ToString()),
                        Margin = float.Parse(dtrow["Margin"].ToString())

                    });

                }
            }
        }
        object[] obj = new object[4];
        obj[0] = returnProject;
        obj[1] = returnProjectCountryWise;
        obj[2] = returnProjectSupplierWise;
        obj[3] = ReturnOverAll;
        return obj;

    }
}