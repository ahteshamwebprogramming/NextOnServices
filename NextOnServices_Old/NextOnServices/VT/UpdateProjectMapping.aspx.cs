using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;

public partial class UpdateProjectMapping : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<CLSProjectmapping> GetProjectsDetailsbyid(string ID)
    {
        try
        {
            ClsDAL obj = new ClsDAL();
            DataSet ds = obj.getprojectmapingdetailsbyid(ID);
            List<CLSProjectmapping> returnData = new List<CLSProjectmapping>();
            if (ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow dtrow in ds.Tables[0].Rows)
                {


                    returnData.Add(new CLSProjectmapping
                    {
                        ID = System.Convert.ToInt32(dtrow["ID"]),
                        PNumber = dtrow["PID"].ToString(),
                        ProjectID = Convert.ToInt32(dtrow["ProjectID"]),
                        ProjectName = dtrow["projectname"].ToString(),
                        Country = dtrow["Country"].ToString(),
                        CountryID = Convert.ToInt32(dtrow["CountryID"]),
                        Supplier = dtrow["suppliername"].ToString(),
                        SUpplierID = Convert.ToInt32(dtrow["SUpplierID"]),
                        RespondantQuota = System.Convert.ToInt32(dtrow["Respondants"]),
                        CPI = float.Parse(dtrow["CPI"].ToString()),
                        Notes = dtrow["Notes"].ToString(),
                        TrackingType = System.Convert.ToInt32(dtrow["trackingtype"]),
                        Completes = dtrow["Completes"].ToString(),
                        Terminate = dtrow["Terminate"].ToString(),
                        Overquota = dtrow["Overquota"].ToString(),
                        Security = dtrow["Security"].ToString(),
                        Fraud = dtrow["Fraud"].ToString(),
                        SUCCESS = dtrow["SUCCESS"].ToString(),
                        DEFAULT = dtrow["DEFAULT"].ToString(),
                        FAILURE = dtrow["FAILURE"].ToString(),
                        QUALITY_TERMINATION = dtrow["QUALITY TERMINATION"].ToString(),
                        OVER_QUOTA = dtrow["OVER QUOTA"].ToString(),
                        AddHashing = Convert.ToInt32(dtrow["AddHashing"]),
                        ParameterName = Convert.ToString(dtrow["ParameterName"]),
                        HashingType = Convert.ToString(dtrow["HashingType"])
                    });

                }

            }
            return returnData;
        }
        catch (Exception e) { return null; }


    }
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public static string Updatepromapping(string ID, string quota, string cpi, string notes, string CountryID, string SupplierID, string ProjectID)
    public static string Updatepromapping(MProjectMapping formData)
    {
        try
        {
            NextOnServicesEntities3 db = new NextOnServicesEntities3();

            int id = Convert.ToInt32(formData.ID);

            ClsDAL obj = new ClsDAL();
            string result = obj.UpdateProMap(Convert.ToInt32(formData.ID), Convert.ToInt32(formData.quota), float.Parse(formData.cpi), formData.notes, Convert.ToInt32(formData.CountryID), Convert.ToInt32(formData.SupplierID), Convert.ToInt32(formData.ProjectID), 0);

            var projectMapping = db.ProjectMappinggs.Where(x => x.ID == formData.ID).FirstOrDefault();
            projectMapping.AddHashing = formData.AddHashing;
            projectMapping.ParameterName = formData.ParameterName;
            projectMapping.HashingType = formData.HashingType;
            db.SaveChanges();
            return result;
        }
        catch (Exception e) { return "error"; }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string UpdatepromappingCPI(string ID, string quota, string cpi, string notes)
    {
        try
        {
            ClsDAL obj = new ClsDAL();
            string result = obj.UpdateProMap(Convert.ToInt32(ID), Convert.ToInt32(quota), float.Parse(cpi), notes, 0, 0, 0, 2);
            return result;
        }
        catch (Exception e) { return "error"; }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<ClsCountry> GetCountries(string PID)
    {
        try
        {
            List<ClsCountry> country = new List<ClsCountry>();
            ClsDAL obj = new ClsDAL();
            DataSet dsclient = obj.ClientMGR(Convert.ToInt32(PID), "Url");
            foreach (DataRow dr in dsclient.Tables[0].Rows)
            {
                country.Add(new ClsCountry
                {
                    Id = Convert.ToInt32(dr["CID"]),
                    Country = dr["Country"].ToString()
                });
            }

            return country;
        }
        catch (Exception e) { return null; }
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<Suppliers> GetSuppliers()
    {
        try
        {
            List<Suppliers> suppliers = new List<Suppliers>();
            ClsDAL obj = new ClsDAL();
            DataSet dsclient = obj.ClientMGR(0, "S");
            foreach (DataRow dr in dsclient.Tables[0].Rows)
            {
                suppliers.Add(new Suppliers
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    Name = dr["Name"].ToString()
                });
            }

            return suppliers;
        }
        catch (Exception e) { return null; }
    }
}