using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Xml;

public partial class MaskingUrl : System.Web.UI.Page
{
    ClsDAL objDAL = new ClsDAL();
    string SID = "", Code = ""; string clientdevice = ""; int ISRC = 0; string OLDUID; string enc;
    private static Random random = new Random();
    string counter = "";
    string GIPAddress = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        //string URLFORENC = "http://nexton.us/VT/ProjectStatus.aspx?Status=COMPLETE&RC=0&ID=456453267";
        //char Seperator = '?';
        //string BaseURL = URLFORENC.Split(Seperator)[0].ToString();
        //URLFORENC = URLFORENC.Split(Seperator)[1].ToString();


        //string ENCURL = BaseURL + "?" + Encryption.Encrypt(URLFORENC, "sblw-3hn8-sqoy25");



        //string DECRIPTURL = ENCURL.Split(Seperator)[1];
        //DECRIPTURL = BaseURL + "?" + Encryption.Decrypt(DECRIPTURL, "sblw-3hn8-sqoy25");

        //Response.Write(DECRIPTURL);

        //Uri myUri = new Uri(DECRIPTURL);

        //if (HttpUtility.ParseQueryString(myUri.Query).Get("Status") != null)
        //{

        //}

        //string param1 = HttpUtility.ParseQueryString(myUri.Query).Get("Stat");



        //return;

        if (!IsPostBack)
        {
            if (Request.QueryString.Count > 0 && (Request.QueryString["SID"] != null && Request.QueryString["ID"] != null) && (Request.QueryString["CID"] == null))
            {
                //ClsDAL.WriteErrorLog("----------Welcome-------------");
                string NID = RandomString(32);
                SID = Convert.ToString(Request.QueryString["SID"]);
                Code = Convert.ToString(Request.QueryString["ID"]);
                enc = Request.QueryString["ENC"];
                // ISRC = Convert.ToInt32(Request.QueryString["RC"]);
                clientdevice = Device();
                // check if the checkbox is active or not
                string res = objDAL.Updatestatussupplierprojects("0", "CheckOverquota", SID);
                string resblock = objDAL.Updatestatussupplierprojects("0", "CheckBlock", SID);
                string live = objDAL.Updatestatussupplierprojects("0", "IsLive", SID);
                string DeviceLockCode = objDAL.Updatestatussupplierprojects("0", "DeviceBlock", SID);
                string DeviceLock = checkDeviceEligibllity(DeviceLockCode);
                string IPValidation = AuthenticateIP(SID, Code);
                string isToken = TokensCheckandFetch(NID, SID, 0);
                //ClsDAL.WriteErrorLog("Token ! or 0 : " + isToken);
                if (DeviceLock == "LockDevice")
                {
                    counter = counter + " || Lock Device || SecTerm ";
                    string message = "Unfortunatley, the survey is not designed for your current device";
                    // Response.Redirect("ErrorPage.aspx?Msg=" + message);

                    lblmsg.Text = message;
                    ProjectStatus(SID, "SEC_TERM", "SEC_TERM", GetIPAddress(), Request.Browser.Type, NID, enc);
                }
                else if (res == "quotafull")
                {
                    counter = counter + " || quotafull || QUOTAFULL ";
                    // string message = "Thank you very much for your participation, but at this time we have received specific numbers of completes.";
                    //  Response.Redirect("ErrorPage.aspx?Msg=" + message);
                    // Response.Write("This [Respondant ID Quota is full]");
                    lblmsg.Text = "Thank you very much for your participation, but at this time we have received specific numbers of completes.";
                    ProjectStatus(SID, "QUOTAFULL", "QUOTAFULL", GetIPAddress(), Request.Browser.Type, NID, enc);
                }
                else if (resblock == "Blocked")
                {
                    counter = counter + " || Blocked || SEC_TERM ";
                    string message = "The survey might not be live at the moment. Please try again later.";
                    // Response.Redirect("ErrorPage.aspx?Msg=" + message);
                    lblmsg.Text = message;
                    ProjectStatus(SID, "SEC_TERM", "SEC_TERM", GetIPAddress(), Request.Browser.Type, NID, enc);
                }
                else if (live == "1" || live == "3" || live == "4" || live == "6")
                {
                    counter = counter + " || Not Live || SEC_TERM ";
                    string message = "The survey might not be live at the moment. Please try again later.";
                    // Response.Redirect("ErrorPage.aspx?Msg=" + message);
                    lblmsg.Text = message;
                    ProjectStatus(SID, "SEC_TERM", "SEC_TERM", GetIPAddress(), Request.Browser.Type, NID, enc);
                }
                //else if (IPValidation == "Unable to capture IP")
                //{
                //    counter = counter + " || Unable To Capture IP || SEC_TERM ";
                //    string message = "We are unable to capture your IP. Please try again.";
                //    // Response.Redirect("ErrorPage.aspx?Msg=" + message);
                //    lblmsg.Text = message;
                //    ProjectStatus(SID, "SEC_TERM", "SEC_TERM", GetIPAddress(), Request.Browser.Type, NID);
                //}
                else if (IPValidation == "Block")
                {
                    counter = counter + " || IP Blocked || SEC_TERM ";
                    string message = "Unforttunately, the survey is not live for your current country location.";
                    // Response.Redirect("ErrorPage.aspx?Msg=" + message);
                    lblmsg.Text = message;
                    ProjectStatus(SID, "SEC_TERM", "SEC_TERM", GetIPAddress(), Request.Browser.Type, NID, enc);
                }
                else if (IPValidation == "Error")
                {
                    counter = counter + " || Exception at IP || SEC_TERM ";
                    string message = "Some error has occurred. Sorry for the inconvenience";
                    //  Response.Redirect("ErrorPage.aspx?Msg=" + message);
                    lblmsg.Text = message;
                    ProjectStatus(SID, "SEC_TERM", "SEC_TERM", GetIPAddress(), Request.Browser.Type, NID, enc);
                }
                else
                {
                    if (Code.IndexOf("[XXXXXXXXXX]") >= 0 || Code.IndexOf("XXXXXXXXXX") >= 0 || Code.IndexOf("XXXXXX") >= 0)
                    {

                        string message = "Oops! Something might not be right with your survey URL";
                        Response.Redirect("ErrorPage.aspx?Msg=" + message);
                        //Response.Write("Please replace [XXXXXXXXXX] or XXXXXXXXXX  with own ID value");
                        return;
                    }
                    //if (Code.Length < 32)
                    //{
                    //    Response.Write("ID value length should be 32 Characters ");
                    //    return;
                    //}
                    string IPAddress = GetIPAddress();
                    counter = counter + " || IP Address Captured Again(" + IPAddress + ")";
                    string BType = Request.Browser.Type;
                    //string url = HttpContext.Current.Request.Url.AbsoluteUri;

                    string St = objDAL.SaveSupplierProject(IPAddress, BType, "InComplete", SID, Code, NID, "", clientdevice, 0, enc);
                    if (St == "3")
                    {
                        string message = "SID value does not exist";
                        Response.Redirect("ErrorPage.aspx?Msg=" + message);
                        //Response.Write("SID value does not exist");
                        return;
                    }
                    //else if (St == "2")
                    //{
                    //    Response.Write("ID value already exist, Please change with other unique value");
                    //    return;
                    //}
                    else
                    {
                        DataSet DS = objDAL.GetSupplierUrl(SID, Code, 0);
                        DataSet DSPrescreen = objDAL.GetSupplierUrl(SID, Code, 2);
                        //ClsDAL.WriteErrorLog("SID - " + SID + "--------------Code" + Code);
                        if (DS.Tables[0].Rows.Count > 0)
                        {
                            //  objDAL.UpdateProjectDetails(IPAddress, BType, "InComplete", SID, Code, 1, 1);
                            // string Url = Convert.ToString(DS.Tables[0].Rows[0]["OLink"]) + "?ID=" + Convert.ToString(DS.Tables[0].Rows[0]["Code"]);
                            string Url = Convert.ToString(DS.Tables[0].Rows[0]["OLink"]);
                            string ID = Convert.ToString(DS.Tables[0].Rows[0]["UID"]);

                            if (Url.IndexOf("[respondentID]") > 0 || Url.IndexOf("[RespondentID]") > 0 || Url.IndexOf("[RESPONDENTID]") > 0)
                            {
                                Url = Url.Replace("[respondentID]", ID);
                                Url = Url.Replace("[RespondentID]", ID);
                                Url = Url.Replace("[RESPONDENTID]", ID);

                                if (isToken == "1")
                                {
                                    if (Url.IndexOf("[tokenID]") > 0 || Url.IndexOf("[TokenID]") > 0 || Url.IndexOf("[TOKENID]") > 0)
                                    {
                                        string token = TokensCheckandFetch(NID, SID, 1);
                                        token = token.Replace("\n", "");
                                        token = token.Replace("\r", "");
                                        if (token != "Error" && token != "NOTOKENSLEFT")
                                        {
                                            Url = Url.Replace("[tokenID]", token);
                                            Url = Url.Replace("[TokenID]", token);
                                            Url = Url.Replace("[TOKENID]", token);
                                            if (Convert.ToString(DSPrescreen.Tables[0].Rows[0][0]) == "Exists")
                                            {
                                                Session["PrID"] = Convert.ToString(DSPrescreen.Tables[0].Rows[0][1]);
                                                Session["CntryID"] = Convert.ToString(DSPrescreen.Tables[0].Rows[0][2]);
                                                Session["UID"] = ID;
                                                Session["URL"] = Url;
                                                Response.Redirect("SurveyMaster.aspx", false);
                                            }
                                            else
                                            {
                                                //ClsDAL.WriteErrorLog("----------A------------");
                                                Response.Redirect(Url, false);
                                            }
                                        }
                                        else
                                        {
                                            string message = "You are out of tokens";
                                            Response.Redirect("ErrorPage.aspx?Msg=" + message);
                                        }
                                    }
                                    else
                                    {
                                        string message = "URL is not in a correct format";
                                        Response.Redirect("ErrorPage.aspx?Msg=" + message);
                                    }
                                }
                                else if (ISRC == 1)
                                {
                                    if (objDAL.CheckUIDForRecontact(Code, SID))
                                    {
                                        objDAL.UpdateRecontactProjectIds(0, SID, ID, Code);
                                        //ClsDAL.WriteErrorLog("--------B---------");
                                        Response.Redirect(Url, false);
                                    }
                                    else
                                    {
                                        string message = "This respondant is not allowed to recontact";
                                        Response.Redirect("ErrorPage.aspx?Msg=" + message);
                                    }
                                }
                                else
                                {
                                    //ClsDAL.WriteErrorLog(DSPrescreen.Tables[0].Rows[0][0].ToString());
                                    if (Convert.ToString(DSPrescreen.Tables[0].Rows[0][0]) == "Exists")
                                    {
                                        //ClsDAL.WriteErrorLog("  ---  Entered in Prescreening Part ---------    ");
                                        Session["PrID"] = Convert.ToString(DSPrescreen.Tables[0].Rows[0][1]);
                                        Session["CntryID"] = Convert.ToString(DSPrescreen.Tables[0].Rows[0][2]);
                                        Session["UID"] = NID;
                                        Session["URL"] = Url;
                                        Response.Redirect("SurveyMaster.aspx", false);
                                    }
                                    else
                                    {
                                        //ClsDAL.WriteErrorLog("   ----------  Entered in else part ------------      ");
                                        Response.Redirect(Url, false);
                                    }
                                }
                            }
                            else
                            {
                                string message = "[respondentID] identifier is not found";
                                Response.Redirect("ErrorPage.aspx?Msg=" + message);
                                //Response.Write("[respondentID] identifier is not found");
                            }
                        }
                    }
                }
            }
            else if (Request.QueryString.Count >= 3 && Request.QueryString["CID"] != null)
            {
                string NID = RandomString(32);
                string PrevUID = "";
                string PID = ""; string Var1 = ""; string Var2 = ""; string Var3 = ""; string Var4 = ""; string Var5 = "";
                clientdevice = Device();
                SID = Convert.ToString(Request.QueryString["SID"]);
                Code = Convert.ToString(Request.QueryString["CID"]);
                PrevUID = Convert.ToString(Request.QueryString["ID"]);

                for (int idx = 0; idx < Request.QueryString.Keys.Count; idx++)
                {
                    switch (Request.QueryString.Keys[idx])
                    {
                        case "PID":
                            PID = Request.QueryString[Request.QueryString.Keys[idx]];
                            break;
                        case "Var1":
                            Var1 = Request.QueryString[Request.QueryString.Keys[idx]];
                            break;
                        case "Var2":
                            Var2 = Request.QueryString[Request.QueryString.Keys[idx]];
                            break;
                        case "Var3":
                            Var3 = Request.QueryString[Request.QueryString.Keys[idx]];
                            break;
                        case "Var4":
                            Var4 = Request.QueryString[Request.QueryString.Keys[idx]];
                            break;
                        case "Var5":
                            Var5 = Request.QueryString[Request.QueryString.Keys[idx]];
                            break;
                    }
                    //string parm = "3";
                    //Response.Write("Key: " + Request.QueryString.Keys[idx] +
                    //", Value:" + Request.QueryString[Request.QueryString.Keys[idx]]);
                }
                string live = objDAL.ValidationForTriggeringRecontact("0", "IsLive", SID);
                if (live == "1" || live == "3" || live == "4" || live == "6")
                {
                    string message = "The project is not live";
                    Response.Redirect("ErrorPage.aspx?Msg=" + message);
                }
                else
                {
                    if (Code.IndexOf("[XXXXXXXXXX]") >= 0 || Code.IndexOf("XXXXXXXXXX") >= 0 || Code.IndexOf("XXXXXX") >= 0)
                    {
                        string message = "Oops! Something might not be right with your survey URL";
                        Response.Redirect("ErrorPage.aspx?Msg=" + message);
                        return;
                    }
                    string IPAddress = GetIPAddress();
                    string BType = Request.Browser.Type;
                    string St = objDAL.SaveSupplierProjectForRecontact(IPAddress, BType, "InComplete", SID, Code, NID, PrevUID, clientdevice, 1, PID, Var1, Var2, Var3, Var4, Var5);
                    if (St == "3")
                    {
                        string message = "SID value does not exist";
                        Response.Redirect("ErrorPage.aspx?Msg=" + message);
                        //Response.Write("SID value does not exist");
                        return;
                    }
                    else if (St == "2")
                    {
                        string message = "Not eligible for recontact";
                        Response.Redirect("ErrorPage.aspx?Msg=" + message);
                        return;
                    }
                    else
                    {
                        VariablesManager(PrevUID, SID);
                        DataSet DS = objDAL.GetSupplierUrl(SID, Code, 1);
                        if (DS.Tables[0].Rows.Count > 0)
                        {


                            //  objDAL.UpdateProjectDetails(IPAddress, BType, "InComplete", SID, Code, 1, 1);
                            // string Url = Convert.ToString(DS.Tables[0].Rows[0]["OLink"]) + "?ID=" + Convert.ToString(DS.Tables[0].Rows[0]["Code"]);
                            string Url = Convert.ToString(DS.Tables[0].Rows[0]["RedirectLink"]);
                            string ID = Convert.ToString(DS.Tables[0].Rows[0]["UID"]);
                            //Response.Redirect(Url, false);
                            if (Url.IndexOf("[respondentID]") > 0 || Url.IndexOf("[RespondentID]") > 0 || Url.IndexOf("[RESPONDENTID]") > 0)
                            {
                                Url = Url.Replace("[respondentID]", ID);
                                Url = Url.Replace("[RespondentID]", ID);
                                Url = Url.Replace("[RESPONDENTID]", ID);
                                Response.Redirect(Url, false);
                            }
                            else
                            {
                                string message = "[respondentID] identifier is not found";
                                Response.Redirect("ErrorPage.aspx?Msg=" + message);
                                //Response.Write("[respondentID] identifier is not found");
                            }
                        }
                    }
                }
            }
            else
            {
                string message = "Required Parameters is missing with Url";
                Response.Redirect("ErrorPage.aspx?Msg=" + message);
                // Response.Write("Required Parameters is missing with Url");
            }
        }
    }
    public void VariablesManager(string UID, string SID)
    {
        ClsDAL objdal = new ClsDAL();
        string varname; string varvalue;
        DataSet ds = objdal.GetVariables(UID, 1, 0, SID);
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                varname = dr["VariableName"].ToString();
                varvalue = Convert.ToString(Request.QueryString[varname]);
                objDAL.UpdateVariables("", 2, Convert.ToInt32(dr["id"]), varvalue);
            }
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    varname = dt.Rows[i]["VariableName"].ToString();
            //    varvalue = Convert.ToString(Request.QueryString[varname]);
            //    DataSet dstemp = objDAL.UpdateVariables("", 2, Convert.ToInt32(dt.Rows[i]["id"]), varvalue);
            //}
        }

    }
    public string Device()
    {
        string strUserAgent = Request.UserAgent.ToString().ToLower();

        if (Request.Browser.IsMobileDevice == true)
        {
            if (strUserAgent.Contains("iphone"))
            {
                return "iOS";
            }
            else if (strUserAgent.Contains("mobile") || strUserAgent.Contains("blackberry") || strUserAgent.Contains("windows ce"))
            {
                return "Mobile";
            }
            else
            {
                return "tablet";
            }
        }
        else
        {
            return "Desktop";
        }
    }
    
    public string checkDeviceEligibllity(string code)
    {
        char[] array = code.ToCharArray();
        // string device = hfdevice.Value;
        string device = Device();

        if (array[0].ToString() == "1" && device == "Mobile")
            return "LockDevice";
        if (array[1].ToString() == "1" && device == "iOS")
            return "LockDevice";
        if (array[2].ToString() == "1" && device == "Desktop")
            return "LockDevice";
        if (array[3].ToString() == "1" && device == "tablet")
            return "LockDevice";
        if (array[4].ToString() == "1" && IPcheck() == "ReachedLimit")
            return "LockDevice";

        return "No";

    }
    public string IPcheck()
    {
        int pid = 0;
        int IPcount = 0;
        int IPcountindb = 0;
        SID = Convert.ToString(Request.QueryString["SID"]);
        string currentIP = GetIPAddress();

        DataSet ds = objDAL.IPSecurityControlmgr(0, Convert.ToInt32(1), SID);
        if (ds.Tables[0].Rows.Count > 0)
        {
            IPcount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            pid = Convert.ToInt32(ds.Tables[0].Rows[0][1]);

        }
        DataSet ds1 = objDAL.IPSecurityControlmgr(pid, Convert.ToInt32(2), currentIP);
        if (ds.Tables[0].Rows.Count > 0)
        {
            IPcountindb = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

        }
        if (IPcountindb < IPcount)
        {

            return "left";
        }
        else
        {

            return "ReachedLimit";
        }
    }
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    protected string GetIPAddress()
    {
        string IPAddress = string.Empty;
        string SearchName = string.Empty;
        String strHostName = HttpContext.Current.Request.UserHostAddress.ToString();
        IPAddress = System.Net.Dns.GetHostAddresses(strHostName).GetValue(0).ToString();
        return IPAddress;

    }
    protected string GetCountryCodeByIP(string countryIP)
    {
        try
        {
            // string countryIP = GetIPAddress();
            string WebUrl = "https://ipfind.co";
            string createdURL = WebUrl + "?ip=" + countryIP;
            //  Create the request and send data to Ozeki NG SMS Gateway Server by HTTP connection
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(createdURL);
            //Get response from Ozeki NG SMS Gateway Server and read the answer
            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            string responseString = respStreamReader.ReadToEnd();
            DataSet ds = new DataSet();
            XmlDocument xdRecognize = new XmlDocument();
            responseString = "{ \"rootNode\": {" + responseString.Trim().TrimStart('{').TrimEnd('}') + "} }";

            xdRecognize = JsonConvert.DeserializeXmlNode(responseString);

            ds.ReadXml(new XmlNodeReader(xdRecognize));
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Columns.Contains("country_code"))
                {
                    objDAL.GetIPFromDatabase(ds.Tables[0].Rows[0]["ip_address"].ToString(), ds.Tables[0].Rows[0]["country_code"].ToString(), ds.Tables[0].Rows[0]["country"].ToString(), ds.Tables[0].Rows[0]["city"].ToString(), 1);

                    return (ds.Tables[0].Rows[0]["country_code"].ToString());
                }
                else
                {
                    //ClsDAL.WriteErrorLog("Error IP : Api did not return the column Country_code");
                    return "0";
                }
            }
            else
            {
                //ClsDAL.WriteErrorLog("Error IP : Did not get the response from IP finding API - - - " + countryIP);
                return "0";
            }
        }
        catch (Exception e)
        {
            counter = counter + " || Exception occured in IPFind 1 || ";
            //  ClsDAL.WriteErrorLog("Exception While Capturing IP : " + e.Message.ToString());
            return "0";
        }
    }

    protected string GetCountryCodeByIP2(string countryIP)
    {
        try
        {
            string WebUrl = "https://ipapi.co/" + countryIP + "/json";
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(WebUrl);
            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            string responseString = respStreamReader.ReadToEnd();
            DataSet ds = new DataSet();
            XmlDocument xdRecognize = new XmlDocument();
            responseString = "{ \"rootNode\": {" + responseString.Trim().TrimStart('{').TrimEnd('}') + "} }";
            xdRecognize = JsonConvert.DeserializeXmlNode(responseString);
            ds.ReadXml(new XmlNodeReader(xdRecognize));
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Columns.Contains("country"))
                {
                    objDAL.GetIPFromDatabase(ds.Tables[0].Rows[0]["ip"].ToString(), ds.Tables[0].Rows[0]["country"].ToString(), ds.Tables[0].Rows[0]["country_name"].ToString(), ds.Tables[0].Rows[0]["city"].ToString(), 1);

                    //  ClsDAL.WriteErrorLog("The code (2) is " + Convert.ToString(ds.Tables[0].Rows[0]["country"]));
                    return (Convert.ToString(ds.Tables[0].Rows[0]["country"]));
                }
                else
                {
                    //ClsDAL.WriteErrorLog("Error : Api did not return the column Country_code");
                    return "0";
                }
            }
            else
            {
                //ClsDAL.WriteErrorLog("Error : Did not get the response from IP finding API - - - " + countryIP);
                return "0";
            }
        }
        catch (Exception e)
        {
            counter = counter + " || Exception occured in IPFind 2 || ";
            //   ClsDAL.WriteErrorLog("(" + countryIP + ")Exception While Capturing IP 2 : " + e.Message.ToString());
            return "0";
        }
    }

    protected string AuthenticateIP(string SID, string Code)
    {
        ClsDAL obj = new ClsDAL();
        string IPADD = GetIPAddress();
        counter = "IP Address is " + IPADD + " || ";
        string countrycode = "";
        if ((countrycode = obj.GetIPFromDatabase(IPADD, "", "", "", 0)) != "0")
        {
            counter = counter + "The Country Code(" + IPADD + ") is " + countrycode + " from database ||";
            //countrycode = obj.GetIPFromDatabase(IPADD, "", "", "", 0);
        }
        else if ((countrycode = GetCountryCodeByIP(IPADD)) != "0")
        {
            counter = counter + "The Country Code(" + IPADD + ") is " + countrycode + " from IP1 ||";
            //countrycode = GetCountryCodeByIP(IPADD);
        }
        else if ((countrycode = GetCountryCodeByIP2(IPADD)) != "0")
        {
            counter = counter + "The Country Code(" + IPADD + ") is " + countrycode + " from IP2 ||";
            // countrycode = GetCountryCodeByIP2(IPADD);
        }
        else countrycode = "";
        //  return "";
        try
        {

            //if (countrycode == "")
            //{
            //    return "Unable to capture IP";
            //}
            //else
            //{
            DataSet ds = obj.AuthenticateIP(SID, 0);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["ALPHA2"].ToString() == countrycode)
                    {
                        return "Allow";
                    }
                }
                return "Block";
            }
            else
            {
                return "Allow";
            }
            // }
        }
        catch (Exception e)
        {
            return "Error";
        }
    }
    public string TokensCheckandFetch(string NID, string sid, int opt)
    {
        try
        {
            ClsDAL objdal = new ClsDAL();
            DataSet result = objdal.TokenMgr(NID, sid, opt);
            if (opt == 0)
            {
                return result.Tables[0].Rows[0][0].ToString();
            }
            else if (opt == 1)
            { return result.Tables[0].Rows[0][0].ToString(); }

            else
                return "Error";
        }
        catch (Exception ex)
        {
            ClsDAL.WriteErrorLog("Page Name : " + Path.GetFileName(Request.Path).ToString() + " ; Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name.ToString() + " ; Error :  " + ex.Message.ToString());
            return "error";
        }
    }


    public void ProjectStatus(string SID, string Status, string RespStatus, string IPAddress, string BType, string NID, string enc)
    {
        clientdevice = Device();
        string res;
        ClsDAL objDAL = new ClsDAL();
        try
        {
            string St = objDAL.SaveSupplierProject(IPAddress, BType, "InComplete", SID, Code, NID, "", clientdevice, 0, enc);
            if (St == "3")
            {

                string message = "SID value does not exist";
                Response.Redirect("ErrorPage.aspx?Msg=" + message);
                //Response.Write("SID value does not exist");
                return;
            }
            else
            {
                res = objDAL.UpdateProjectDetails("", "", Status, "", NID, 1, 2);
                if (res == "1")
                {
                    RequestUrl(NID, 0, RespStatus);
                }
            }
        }
        catch (Exception e)
        {

        }
    }

    int RequestUrl(string ID, int RC, string RetStatus)
    {

        ClsDAL objDAL = new ClsDAL();
        string SID = "", Status = "", Resp = "", ReqUrl = "", ClientID = "";
        int cnt = 0;
        SID = ""; Status = ""; ClientID = ""; int opt;
        // string ReqUrl = "http://localhost:2359/RequestUrl.aspx?survey_id=12999&SID=XXXXXXX&redirected_from=survey&status=Screened";
        // string createdURL = ReqUrl +
        // "&identifier=344555" +
        // "&Status=InComplete" ;
        try
        {
            if (RC == 0)
            {
                opt = 0;
            }
            else
            {
                opt = 1;
            }
            DataSet DS = objDAL.GetRequestUrlImmidiate(ID, opt, RetStatus);
            if (DS.Tables[0].Rows.Count > 0)
            {
                SID = Convert.ToString(DS.Tables[0].Rows[0]["SuplierID"]);
                Status = Convert.ToString(DS.Tables[0].Rows[0]["PStatus"]);
                ReqUrl = Convert.ToString(DS.Tables[0].Rows[0]["RequestUrl"]);
                ClientID = Convert.ToString(DS.Tables[0].Rows[0]["ClientID"]);
                if (ReqUrl.IndexOf("[respondentID]") > 0 || ReqUrl.IndexOf("[RespondentID]") > 0 || ReqUrl.IndexOf("[RESPONDENTID]") > 0)
                {

                    ReqUrl = ReqUrl.Replace("[respondentID]", ClientID);
                    ReqUrl = ReqUrl.Replace("[RespondentID]", ClientID);
                    ReqUrl = ReqUrl.Replace("[RESPONDENTID]", ClientID);

                    // Response.Redirect(ReqUrl, false);
                    HtmlMeta meta = new HtmlMeta();
                    meta.HttpEquiv = "Refresh";
                    meta.Content = "2;url=" + ReqUrl;
                    this.Page.Controls.Add(meta);
                    //HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(ReqUrl);
                    ////Get response from Ozeki NG SMS Gateway Server and read the answer
                    //HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                    //System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                    //string responseString = respStreamReader.ReadToEnd();
                    //respStreamReader.Close();
                    //myResp.Close();
                    objDAL.UpdateRequestStatus(ID);
                    return 1;
                }
                else
                {
                    // Response.Write("[respondentID] identifier is not found");
                    return 1;
                }
            }
            else
            {
                return 0;
            }
        }
        catch (SystemException ex)
        {
            return 0;
        }
    }

}
