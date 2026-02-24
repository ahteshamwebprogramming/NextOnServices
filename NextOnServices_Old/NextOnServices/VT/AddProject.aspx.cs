using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

public partial class AddProject : System.Web.UI.Page
{
    static NextOnServicesEntities3 db = new NextOnServicesEntities3();
    ClsDAL objDAL = new ClsDAL();
    string Status = "";
    private static Random random = new Random();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblmsg.Visible = false;
            GetClients();
            GetPM();
            GetCountries();
        }
    }

    [WebMethod]
    public static async Task<object> AddProjectFromAPI(string ProjectId, double? CPI, int? LOI, int? IR, int? TotalRemaining, string LiveLink)
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Session["UserId"].ToString()))
            {
                throw new Exception("Session expired");
            }


            if (db.Projects.Where(x => x.ProjectIdFromAPI == ProjectId && x.ProjectFrom == "Lucid").Count() > 0)
            {
                throw new Exception("Already added");
            }

            //string pid = db.Projects.Where(x => x.ProjectFrom != null && x.ProjectIdFromAPI != null).OrderByDescending(x => x.ID).Select(x => x.PID).FirstOrDefault();
            string pid = db.Projects.OrderByDescending(x => x.ID).Select(x => x.PID).FirstOrDefault();
            int resInt;
            if (int.TryParse(pid.Substring(3), out resInt))
            {
                resInt = resInt + 1;
                while (db.Projects.Where(x => x.PID == "NXT" + resInt).Count() > 0)
                {
                    resInt = resInt + 1;
                }
                Project project = new Project();
                project.PName = ProjectId;
                project.Descriptions = ProjectId + "-Pulled from Lucid API";
                project.PManager = HttpContext.Current.Session["UserId"].ToString();
                project.ClientID = 53;//ForSago;
                project.LOI = LOI.ToString();
                project.IRate = IR.ToString();
                project.CPI = CPI;
                project.SampleSize = TotalRemaining.ToString();
                project.Quota = TotalRemaining.ToString();
                project.Country = "235";
                project.SDate = System.DateTime.Now.ToString("MM-dd-yyyy");
                project.EDate = System.DateTime.Now.AddMonths(1).ToString("MM-dd-yyyy");
                project.Status = 5;
                project.IsActive = 1;
                project.BlockDevice = "00000";
                project.CreationDate = System.DateTime.Now;
                project.Notes = "Added from Lucid from API";
                project.ProjectIdFromAPI = ProjectId;
                project.PID = "NXT" + resInt;
                project.ProjectFrom = "Lucid";
                project.LType = 0;
                db.Projects.Add(project);
                db.SaveChanges();


                ProjectsUrl pu = new ProjectsUrl();
                pu.PID = project.ID;
                pu.CID = Convert.ToInt32(project.Country);
                pu.Url = LiveLink.Replace("[#scid#]&uid=[#scid2#]", "[respondentID]");
                pu.Notes = "Added from lucid";
                pu.CPI = project.CPI;
                pu.Quota = project.SampleSize;
                pu.Status = project.Status;
                pu.CreationDate = System.DateTime.Now;
                db.ProjectsUrls.Add(pu);

                db.SaveChanges();

                string KID = RandomString(32);
                string SID = RandomString(8);
                string MUrl = ConfigurationSettings.AppSettings["MaskingUrl"].ToString() + "?SID=" + SID + "&ID=XXXXXXXXXX";
                string OUrl = pu.Url;
                HttpContext.Current.Session["PID"] = project.ID;
                int addHashing = 0;

                ProjectMappingg pm = new ProjectMappingg();
                pm.ProjectID = project.ID;
                pm.CountryID = Convert.ToInt32(project.Country);
                pm.SUpplierID = 1064; // Arete Research
                pm.OLink = OUrl;
                pm.MLink = MUrl;
                pm.SID = SID;
                pm.Code = KID;




                return new { Result = true, message = "Success" };
            }
            else
            {
                throw new Exception("Unable to get the project id from existing records");
            }





        }
        catch (Exception ex)
        {
            return new { Result = false, Message = ex.Message };
        }
    }


    [WebMethod]
    public static async Task<object> AddProjectFromSpectrumAPI(string ProjectId, string ProjectName, double? CPI, int? LOI, int? IR, int? TotalRemaining, string LiveLink)
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Current.Session["UserId"].ToString()))
            {
                throw new Exception("Session expired");
            }


            if (db.Projects.Where(x => x.ProjectIdFromAPI == ProjectId && x.ProjectFrom == "Spectrum").Count() > 0)
            {
                throw new Exception("Already added");
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("access-token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY1NmY4YmY4YzIwZjM0NGM3YjYyZGI4ZCIsInVzcl9pZCI6IjE5NzAxIiwiaWF0IjoxNzAxODA5MTQ0fQ.1rzvWDMOvFBqzw8U2k8jFpOFOglgNWIXTrWykub-USc");
            client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");

            //var uri = "https://api.spectrumsurveys.com/suppliers/v2/surveys";
            var uri = "https://api.spectrumsurveys.com/suppliers/v2/surveys/register/22516233";

            var values = new Dictionary<string, string>
            { };
            var content = new FormUrlEncodedContent(values);

            SpectrumAPISurveys spectrumAPISurveys = new SpectrumAPISurveys();
            var response = client.PostAsync(uri, content).Result;
            if (response.IsSuccessStatusCode)
            {
                string stringData = response.Content.ReadAsStringAsync().Result;


                JObject json = JObject.Parse(stringData);

                if (json["apiStatus"].ToString() == "success")
                {

                    string survey_entry_url = json["survey_entry_url"].ToString();

                    string pid = db.Projects.OrderByDescending(x => x.ID).Select(x => x.PID).FirstOrDefault();
                    int resInt;
                    if (int.TryParse(pid.Substring(3), out resInt))
                    {
                        resInt = resInt + 1;
                        while (db.Projects.Where(x => x.PID == "NXT" + resInt).Count() > 0)
                        {
                            resInt = resInt + 1;
                        }
                        Project project = new Project();
                        project.PName = ProjectName;
                        project.Descriptions = ProjectId + "-Pulled from Spectrum API";
                        project.PManager = HttpContext.Current.Session["UserId"].ToString();
                        project.ClientID = 53;//ForSago;
                        project.LOI = LOI.ToString();
                        project.IRate = IR.ToString();
                        project.CPI = CPI;
                        project.SampleSize = TotalRemaining.ToString();
                        project.Quota = TotalRemaining.ToString();
                        project.Country = "235";
                        project.SDate = System.DateTime.Now.ToString("MM-dd-yyyy");
                        project.EDate = System.DateTime.Now.AddMonths(1).ToString("MM-dd-yyyy");
                        project.Status = 5;
                        project.IsActive = 1;
                        project.BlockDevice = "00000";
                        project.CreationDate = System.DateTime.Now;
                        project.Notes = "Added from Spectrum from API";
                        project.ProjectIdFromAPI = ProjectId;
                        project.PID = "NXT" + resInt;
                        project.ProjectFrom = "Spectrum";
                        project.LType = 0;
                        db.Projects.Add(project);
                        db.SaveChanges();


                        ProjectsUrl pu = new ProjectsUrl();
                        pu.PID = project.ID;
                        pu.CID = Convert.ToInt32(project.Country);
                        pu.Url = survey_entry_url + "&ID=[RespondentID]";//LiveLink.Replace("[#scid#]&uid=[#scid2#]", "[respondentID]");
                        pu.Notes = "Added from Spectrum";
                        pu.CPI = project.CPI;
                        pu.Quota = project.SampleSize;
                        pu.Status = project.Status;
                        pu.CreationDate = System.DateTime.Now;
                        db.ProjectsUrls.Add(pu);

                        db.SaveChanges();

                        string KID = RandomString(32);
                        string SID = RandomString(8);
                        string MUrl = ConfigurationSettings.AppSettings["MaskingUrl"].ToString() + "?SID=" + SID + "&ID=XXXXXXXXXX";
                        string OUrl = pu.Url;
                        HttpContext.Current.Session["PID"] = project.ID;
                        int addHashing = 0;

                        ProjectMappingg pm = new ProjectMappingg();
                        pm.ProjectID = project.ID;
                        pm.CountryID = Convert.ToInt32(project.Country);
                        pm.SUpplierID = 1064; // Arete Research
                        pm.OLink = OUrl;
                        pm.MLink = MUrl;
                        pm.SID = SID;
                        pm.Code = KID;
                        return new { Result = true, message = "Success" };
                    }
                    else
                    {
                        throw new Exception("Unable to get the project id from existing records");
                    }
                }

                else
                {
                    throw new Exception("Unable to get the project id from existing records");
                }




            }
            else
            {
                throw new Exception("Unable to get the project id from existing records");
            }


        }
        catch (Exception ex)
        {
            return new { Result = false, Message = ex.Message };
        }
    }

    [WebMethod]
    public static async void TestAPI()
    {
        //string pi = ProjectId;

        HttpClient hc = new HttpClient();
        hc.BaseAddress = new Uri("https://api.sample-cube.com/api/");
        var consumeAPI = hc.GetAsync("Survey/GetSupplierAllocatedSurveys/1595/084853e8-1b98-4828-9af8-15332e5fe165");
        consumeAPI.Wait();

        var readd = consumeAPI.Result;



        var client = new HttpClient();
        //client.DefaultRequestHeaders.Add("X-MC-SUPPLY-KEY", "1002:0466198A-A78B-4F6C-8CE2-4A79FBC8FA41");
        client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
        var uri = "https://api.sample-cube.com/api/Survey/GetSupplierAllocatedSurveys/1595/084853e8-1b98-4828-9af8-15332e5fe165";
        var response = await client.GetAsync(uri);

        bool re = true;

    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    void GetClients()
    {
        try
        {
            DataSet dsclient = objDAL.ClientMGR(0, "C");

            if (dsclient.Tables[0].Rows.Count > 0)
            {
                ddlclient.DataTextField = "Company";
                ddlclient.DataValueField = "ID";
                ddlclient.DataSource = dsclient;
                ddlclient.DataBind();

                ddlclient.Items.Insert(0, new ListItem("- Select Client -", "0"));
            }
        }
        catch (Exception ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }
    }

    void GetPM()
    {
        try
        {
            DataSet dsclient = objDAL.ClientMGR(0, "U");

            if (dsclient.Tables[0].Rows.Count > 0)
            {
                ddlPM.DataTextField = "UserName";
                ddlPM.DataValueField = "ID";
                ddlPM.DataSource = dsclient;
                ddlPM.DataBind();
                ddlPM.Items.Insert(0, new ListItem("- Select Project Manager -", "0"));
            }
        }
        catch (Exception ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }
    }

    void GetCountries()
    {
        try
        {
            DataSet dsclient = objDAL.ClientMGR(0, "CNT");

            if (dsclient.Tables[0].Rows.Count > 0)
            {
                ddlCountry.DataTextField = "Country";
                ddlCountry.DataValueField = "Id";
                ddlCountry.DataSource = dsclient;
                ddlCountry.DataBind();
                ddlCountry.Items.Insert(0, new ListItem("- Select Country -", "0"));
            }
        }
        catch (Exception ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        ClsProject objP = new ClsProject();
        try
        {
            //string projectid = IncrementID(objDAL.GetProjectId());
            objP.ID = 0;
            objP.PName = txtPName.Text;
            objP.Descriptions = txtDesc.Text;
            objP.PID = IncrementID(objDAL.GetProjectId());
            objP.ClientID = Convert.ToInt32(ddlclient.SelectedItem.Value);
            objP.PManager = Convert.ToString(ddlPM.SelectedItem.Value); ;
            objP.LOI = txtLOI.Text;
            objP.IRate = txtIRate.Text;
            objP.CPI = Convert.ToDouble(txtCPI.Text);

            objP.SampleSize = "";
            objP.Quota = txtQuota.Text;

            objP.SDate = txtSDate.Text;
            objP.EDate = txtEDate.Text;
            objP.Country = ddlCountry.SelectedItem.Value;

            objP.LType = Convert.ToInt32(rdotype.SelectedItem.Value);
            objP.Status = Convert.ToInt32(ddlstatus.SelectedItem.Value);
            objP.Notes = txtNotes.Text;
            // ClsDAL.WriteErrorLog("main method");
            //return;
            Status = objDAL.ProjectMgr(objP, 1);
            if (Status == "1")
            {
                Response.Redirect("ProjectDetails.aspx", false);
                GetClear();
            }
            else
            {
                Response.Write("<script>alert('abc');</script>");
            }
        }
        catch (SystemException ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
        }

    }




    static string IncrementID(string startValue)
    {
        if (startValue == "" || int.Parse(startValue.Substring(3)) < 200000)
        {
            startValue = "NXT200000";
        }
        char letter1 = startValue[0];
        char letter2 = startValue[1];
        char letter3 = startValue[2];
        int len = startValue.Length - 3;
        int number = int.Parse(startValue.Substring(3));
        number++;
        if (number >= Math.Pow(10, len)) number = 1; // start again at 1

        string PID = String.Format("{0}{1}{2}{3:D" + len.ToString() + "}", letter1, letter2, letter3, number);

        while (db.Projects.Where(x => x.PID == PID).Count() > 0)
        {
            number++;
            if (number >= Math.Pow(10, len)) number = 1; // start again at 1
            PID = String.Format("{0}{1}{2}{3:D" + len.ToString() + "}", letter1, letter2, letter3, number);
        }
        return PID;
    }
    void GetClear()
    {
        txtNotes.Text = "";
        txtLOI.Text = "";
        txtIRate.Text = "";
        txtEDate.Text = "";
        txtEDate.Text = "";
        txtDesc.Text = "";
        txtCPI.Text = "";
        // txtPM.Text = "";
        txtPName.Text = "";
        txtQuota.Text = "";
        // txtSDate.Text = "";
        // txtSSize.Text = "";
        ddlclient.ClearSelection();
        ddlCountry.ClearSelection();

    }

    // protected void btntest_Click(object sender, EventArgs e)
    //{
    //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "alertt()", true);
    //}
}

