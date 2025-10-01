using NextOnServices.Infrastructure.Helper;
using Microsoft.AspNetCore.Mvc;
using System;
using GRP.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Metrics;
using Newtonsoft.Json;
using System.Net;
using System.Xml;
using Microsoft.Identity.Client;
using Microsoft.CodeAnalysis;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Security.Cryptography;
using NextOnServices.Core.Entities;

namespace NextOnServices.WebUI.Areas.VT.Controllers
{
    [Area("VT")]
    public class SurveyController : Controller
    {
        private static Random _random = new Random();

        private readonly ILogger<SurveyController> _logger;
        private readonly SurveyAPIController _surveyAPIController;

        private string _browserType;
        private string _iPAddress;

        private string _SID = "", _Code = "", _clientdevice = "", _OLDUID, _enc, _counter = "", _GIPAddress = string.Empty, _Status = "", _Resp = "", _ReqUrl = "", _ClientID = "";
        private int _ISRC = 0, _RC = 0, _cnt = 0, _PID = 0, _PX = 0;
        public SurveyController(ILogger<SurveyController> logger, SurveyAPIController surveyAPIController, ProfileInfoSurveyAPIController profileInfoSurveyAPIController)
        {
            _logger = logger;
            _surveyAPIController = surveyAPIController;
        }

        [HttpGet]
        [Route("/VT/MaskingUrl.aspx")]
        //[Route("/VT/MaskingUrl/{SID}/{ID}")]
        public async Task<IActionResult> MaskingURL()
        {
            _browserType = CommonHelper.ParseBrowser(Request);
            _iPAddress = GetIPAddress();

            if (Request.Query.Count > 0 && !string.IsNullOrEmpty(Request.Query["SID"]) && !string.IsNullOrEmpty(Request.Query["ID"]) && string.IsNullOrEmpty(Request.Query["CID"]))
            {
                string NID = CommonHelper.RandomString(32);
                string? SID = Request.Query["SID"];
                string? Code = Request.Query["ID"];
                string? enc = Request.Query["ENC"];
                string clientdevice = CommonHelper.Device(Request);



                string res = "";
                var resActionResult = await _surveyAPIController.Updatestatussupplierprojects("0", "CheckOverquota", SID ?? "");
                if (resActionResult is OkObjectResult resOkResult)
                {
                    res = resOkResult.Value?.ToString() ?? "";
                }

                string resblock = "";
                var resblockActionResult = await _surveyAPIController.Updatestatussupplierprojects("0", "CheckBlock", SID ?? "");
                if (resblockActionResult is OkObjectResult resblockOkResult)
                {
                    resblock = resblockOkResult.Value?.ToString() ?? "";
                }

                string live = "";
                var liveActionResult = await _surveyAPIController.Updatestatussupplierprojects("0", "IsLive", SID ?? "");
                if (liveActionResult is OkObjectResult liveOkResult)
                {
                    live = liveOkResult.Value?.ToString() ?? "";
                }

                string DeviceLockCode = "";
                var DeviceLockCodeActionResult = await _surveyAPIController.Updatestatussupplierprojects("0", "DeviceBlock", SID ?? "");
                if (DeviceLockCodeActionResult is OkObjectResult DeviceLockCodeOkResult)
                {
                    DeviceLockCode = DeviceLockCodeOkResult.Value?.ToString() ?? "";
                }

                string DeviceLock = await CheckDeviceEligibllity(DeviceLockCode, Request);

                string IPValidation = await AuthenticateIP(SID, Code);
                string isToken = await TokensCheckandFetch(NID, SID, 0);

                await ProjectStatus(SID, "SEC_TERM", "SEC_TERM", _iPAddress, _browserType, NID, enc ?? "");

                if (DeviceLock == "LockDevice")
                {

                    ViewBag.Title = "Device Not Supported";
                    ViewBag.Message = "Unfortunatley, the survey is not designed for your current device";
                    //ProjectStatus(SID, "SEC_TERM", "SEC_TERM", GetIPAddress(), Request.Browser.Type, NID, enc);
                    await ProjectStatus(SID, "SEC_TERM", "SEC_TERM", _iPAddress, _browserType, NID, enc ?? "");
                }
                else if (res == "quotafull")
                {
                    ViewBag.Title = "Quotafull";
                    ViewBag.Message = "Thank you very much for your participation, but at this time we have received specific numbers of completes.";
                    await ProjectStatus(SID, "QUOTAFULL", "QUOTAFULL", _iPAddress, _browserType, NID, enc ?? "");
                }
                else if (resblock == "Blocked")
                {
                    ViewBag.Title = "Blocked";
                    ViewBag.Message = "The survey might not be live at the moment. Please try again later.";
                    await ProjectStatus(SID, "SEC_TERM", "SEC_TERM", _iPAddress, _browserType, NID, enc ?? "");
                }
                else if (live == "1" || live == "3" || live == "4" || live == "6")
                {
                    ViewBag.Title = "Not Live";
                    ViewBag.Message = "The survey might not be live at the moment. Please try again later.";
                    await ProjectStatus(SID, "SEC_TERM", "SEC_TERM", _iPAddress, _browserType, NID, enc ?? "");
                }
                else if (IPValidation == "Block")
                {
                    ViewBag.Title = "Blocked";
                    ViewBag.Message = "Unforttunately, the survey is not live for your current country location.";
                    await ProjectStatus(SID, "SEC_TERM", "SEC_TERM", _iPAddress, _browserType, NID, enc ?? "");
                }
                else if (IPValidation == "Error")
                {
                    ViewBag.Title = "Link Broken";
                    ViewBag.Message = "Some error has occurred. Sorry for the inconvenience";
                    await ProjectStatus(SID, "SEC_TERM", "SEC_TERM", _iPAddress, _browserType, NID, enc ?? "");
                }
                else
                {
                    if (Code.IndexOf("[XXXXXXXXXX]") >= 0 || Code.IndexOf("XXXXXXXXXX") >= 0 || Code.IndexOf("XXXXXX") >= 0)
                    {
                        ViewBag.Title = "Link Broken";
                        ViewBag.Message = "Oops! Something might not be right with your survey URL";
                        return View();
                    }
                    string IPAddress = _iPAddress;
                    string St = await _surveyAPIController.SaveSupplierProject(IPAddress, _browserType, "InComplete", SID, Code, NID, "", clientdevice, 0, enc);
                    if (St == "3")
                    {
                        ViewBag.Message = "SID value does not exist";
                        return View();
                        //Response.Redirect("ErrorPage.aspx?Msg=" + message);                        
                    }
                    else
                    {
                        //DataSet DS = objDAL.GetSupplierUrl(SID, Code, 0);
                        var DS = await _surveyAPIController.GenericDataFetcher("Usp_GetSupplierUrl", new { @SID = SID, @PMID = Code, @opt = 0 });
                        //DataSet DSPrescreen = objDAL.GetSupplierUrl(SID, Code, 2);
                        var DSPrescreen = await _surveyAPIController.GenericDataFetcher("Usp_GetSupplierUrl", new { @SID = SID, @PMID = Code, @opt = 2 });
                        //ClsDAL.WriteErrorLog("SID - " + SID + "--------------Code" + Code);
                        if (DS != null && DS.Any())
                        {
                            var firstRow = DS.FirstOrDefault();

                            string Url = firstRow?.OLink ?? "";
                            string ID = firstRow?.UID ?? "";
                            if (Url.IndexOf("[respondentID]") > 0 || Url.IndexOf("[RespondentID]") > 0 || Url.IndexOf("[RESPONDENTID]") > 0)
                            {
                                Url = Url.Replace("[respondentID]", ID);
                                Url = Url.Replace("[RespondentID]", ID);
                                Url = Url.Replace("[RESPONDENTID]", ID);
                                if (isToken == "1")
                                {
                                    if (Url.IndexOf("[tokenID]") > 0 || Url.IndexOf("[TokenID]") > 0 || Url.IndexOf("[TOKENID]") > 0)
                                    {
                                        string token = await TokensCheckandFetch(NID, SID, 1);
                                        token = token.Replace("\n", "");
                                        token = token.Replace("\r", "");
                                        if (token != "Error" && token != "NOTOKENSLEFT")
                                        {
                                            Url = Url.Replace("[tokenID]", token);
                                            Url = Url.Replace("[TokenID]", token);
                                            Url = Url.Replace("[TOKENID]", token);
                                            //if (Convert.ToString(DSPrescreen.Tables[0].Rows[0][0]) == "Exists")                                            
                                            if (DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First().Exists?.ToString() == "Exists")
                                            {
                                                string PrID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First().ProjectID?.ToString() ?? "";
                                                HttpContext.Session.SetString("PrID", PrID);

                                                string CntryID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First().CountryID?.ToString() ?? "";
                                                HttpContext.Session.SetString("CntryID", CntryID);
                                                HttpContext.Session.SetString("UID", ID);
                                                HttpContext.Session.SetString("URL", Url);
                                                Response.Redirect("SurveyMaster.aspx", false);
                                            }
                                            else
                                            {
                                                return Redirect(Url);
                                                //Response.Redirect(Url, false);
                                            }
                                        }
                                        else
                                        {
                                            ViewBag.Message = "You are out of tokens";
                                            return View();
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.Message = "URL is not in a correct format";
                                        return View();
                                    }
                                }
                                else if (_ISRC == 1)
                                {
                                    if (await _surveyAPIController.CheckUIDForRecontact(Code, SID))
                                    {
                                        //await _surveyAPIController.UpdateRecontactProjectIds(0, SID, ID, Code);
                                        await _surveyAPIController.GenericDataFetcher("updateRecontactProjectsIDs", new { @opt = 0, @PMID = SID, @UIDnew = ID, @UID = Code });
                                        return Redirect(Url); //Response.Redirect(Url, false);
                                    }
                                    else
                                    {
                                        ViewBag.Message = "This respondant is not allowed to recontact";
                                        return View();
                                    }
                                }
                                else
                                {
                                    if (DSPrescreen != null && DSPrescreen.Any() && (((IDictionary<string, object>)DSPrescreen.First()).Values.ElementAtOrDefault(0)?.ToString()) == "Exists")
                                    {

                                        //string PrID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First()[1]?.ToString() ?? "";
                                        string PrID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First()[1]?.ToString() ?? "";
                                        HttpContext.Session.SetString("PrID", PrID);

                                        string CntryID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First()[2]?.ToString() ?? "";
                                        HttpContext.Session.SetString("CntryID", CntryID);
                                        HttpContext.Session.SetString("UID", ID);
                                        HttpContext.Session.SetString("URL", Url);
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
                                ViewBag.Message = "[respondentID] identifier is not found";
                                return View();
                            }
                        }
                    }
                }

                //if (DeviceLock == "LockDevice")
                //{
                //    // Show error view with custom message
                //    ViewBag.Message = "Unfortunately, the survey is not designed for your current device.";
                //    return View("ErrorPage"); // You need to create a view named "ErrorPage.cshtml"
                //}
                //else
                //{
                //    // Redirect to another URL
                //    string redirectUrl = $"https://example.com/survey-start?SID={SID}&NID={NID}";
                //    return Redirect(redirectUrl);
                //}
            }

            return View();
        }
        protected async Task<string> AuthenticateIP(string SID, string Code)
        {
            string IPADD = _iPAddress;
            string countrycode = "";
            if ((countrycode = await _surveyAPIController.GetIPFromDatabase(IPADD, "", "", "", 0)) != "0")
            {
                //counter = counter + "The Country Code(" + IPADD + ") is " + countrycode + " from database ||";
                //countrycode = obj.GetIPFromDatabase(IPADD, "", "", "", 0);
            }
            else if ((countrycode = await GetCountryCodeByIP(IPADD)) != "0")
            {
                //counter = counter + "The Country Code(" + IPADD + ") is " + countrycode + " from IP1 ||";
                //countrycode = GetCountryCodeByIP(IPADD);
            }
            else if ((countrycode = await GetCountryCodeByIP2(IPADD)) != "0")
            {
                //counter = counter + "The Country Code(" + IPADD + ") is " + countrycode + " from IP2 ||";
                // countrycode = GetCountryCodeByIP2(IPADD);
            }
            else countrycode = "";
            //  return "";
            try
            {
                var ds = await _surveyAPIController.AuthenticateIP(SID, 0);
                if (ds != null && ds.Any())
                {
                    foreach (var row in ds)
                    {
                        if (row["ALPHA2"].ToString() == countrycode)
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
            }
            catch (Exception e)
            {
                return "Error";
            }
        }
        protected string GetIPAddress()
        {
            // Get the remote IP address from the current HTTP context
            //return HttpContext.Connection.RemoteIpAddress?.ToString();
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;

            if (remoteIpAddress == null)
                return string.Empty;

            // If it's IPv6 mapped to IPv4, convert to IPv4 string
            if (remoteIpAddress.IsIPv4MappedToIPv6)
                remoteIpAddress = remoteIpAddress.MapToIPv4();

            return remoteIpAddress.ToString();
        }

        protected async Task<string> GetCountryCodeByIP(string countryIP)
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
                        await _surveyAPIController.GetIPFromDatabase(ds.Tables[0].Rows[0]["ip_address"].ToString(), ds.Tables[0].Rows[0]["country_code"].ToString(), ds.Tables[0].Rows[0]["country"].ToString(), ds.Tables[0].Rows[0]["city"].ToString(), 1);
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
                //counter = counter + " || Exception occured in IPFind 1 || ";
                //  ClsDAL.WriteErrorLog("Exception While Capturing IP : " + e.Message.ToString());
                return "0";
            }
        }
        protected async Task<string> GetCountryCodeByIP2(string countryIP)
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
                        await _surveyAPIController.GetIPFromDatabase(ds.Tables[0].Rows[0]["ip"].ToString(), ds.Tables[0].Rows[0]["country"].ToString(), ds.Tables[0].Rows[0]["country_name"].ToString(), ds.Tables[0].Rows[0]["city"].ToString(), 1);

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
                //counter = counter + " || Exception occured in IPFind 2 || ";
                //   ClsDAL.WriteErrorLog("(" + countryIP + ")Exception While Capturing IP 2 : " + e.Message.ToString());
                return "0";
            }
        }
        public async Task<string> CheckDeviceEligibllity(string code, HttpRequest request)
        {
            char[] array = code.ToCharArray();
            // string device = hfdevice.Value;
            string device = CommonHelper.Device(request);

            if (array[0].ToString() == "1" && device == "Mobile")
                return "LockDevice";
            if (array[1].ToString() == "1" && device == "iOS")
                return "LockDevice";
            if (array[2].ToString() == "1" && device == "Desktop")
                return "LockDevice";
            if (array[3].ToString() == "1" && device == "tablet")
                return "LockDevice";
            if (array[4].ToString() == "1" && await IPcheck() == "ReachedLimit")
                return "LockDevice";

            return "No";
        }
        public async Task<string> IPcheck()
        {
            int pid = 0;
            int IPcount = 0;
            int IPcountindb = 0;
            string SID = Convert.ToString(Request.Query["SID"]);
            string currentIP = _iPAddress;

            var ds = await _surveyAPIController.IPSecurityControlmgr(0, Convert.ToInt32(1), SID);
            if (ds != null && ds.Any())
            {
                var firstRow = ds.First();
                IPcount = Convert.ToInt32(firstRow[0]);
                pid = Convert.ToInt32(firstRow[1]);
            }
            var ds1 = await _surveyAPIController.IPSecurityControlmgr(pid, Convert.ToInt32(2), currentIP);
            if (ds1 != null && ds1.Any())
            {
                var firstRow = ds1.First();
                IPcountindb = Convert.ToInt32(firstRow[0]);
            }

            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    IPcountindb = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

            //}
            if (IPcountindb < IPcount)
            {

                return "left";
            }
            else
            {

                return "ReachedLimit";
            }
        }
        public async Task<string> TokensCheckandFetch(string NID, string sid, int opt)
        {
            try
            {
                //ClsDAL objdal = new ClsDAL();
                //DataSet result = objdal.TokenMgr(NID, sid, opt);
                var result = await _surveyAPIController.GenericDataFetcher("Tokensmgr", new { @opt = opt, @sid = sid, @nid = NID });
                if (result != null && result.Any())
                {
                    var firstRow = result.FirstOrDefault();
                    if (firstRow is IDictionary<string, object> rowDict && rowDict.Any())
                    {
                        var firstColumnValue = rowDict.Values.FirstOrDefault();
                        if (opt == 0 || opt == 1)
                        {
                            return firstColumnValue?.ToString() ?? "Error";
                        }
                    }
                }
                return "Error";
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public async Task<IActionResult> ProjectStatus(string SID, string Status, string RespStatus, string IPAddress, string BType, string NID, string enc)
        {
            string clientdevice = CommonHelper.Device(Request);
            string res;
            try
            {
                string St = await _surveyAPIController.SaveSupplierProject(IPAddress, BType, "InComplete", SID, _Code, NID, "", clientdevice, 0, enc);
                if (St == "3")
                {
                    ViewBag.Title = "Invalid Coutry";
                    ViewBag.Message = "SID value does not exist";
                    return View("MaskingURL");
                }
                else
                {
                    res = await _surveyAPIController.UpdateProjectDetails("", "", Status, "", NID, 1, 2);
                    if (res == "1")
                    {
                        await RequestUrl(NID, 0, RespStatus);
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        async Task<IActionResult> RequestUrl(string ID, int RC, string RetStatus)
        {
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
                //DataSet DS = objDAL.GetRequestUrlImmidiate(ID, opt, RetStatus);
                var DS = await _surveyAPIController.GenericDataFetcher("usp_StatusImmidiateOError", new { @ID = ID, @opt = opt, @Status = RetStatus });
                if (DS != null && DS.Any())
                {
                    var firstRow = DS.FirstOrDefault();

                    SID = firstRow?.SuplierID ?? "";
                    Status = firstRow?.PStatus ?? "";
                    ReqUrl = firstRow?.RequestUrl ?? "";
                    ClientID = firstRow?.ClientID ?? "";

                    if (ReqUrl.IndexOf("[respondentID]") > 0 || ReqUrl.IndexOf("[RespondentID]") > 0 || ReqUrl.IndexOf("[RESPONDENTID]") > 0)
                    {
                        await _surveyAPIController.UpdateRequestStatus(ID);
                        return Redirect(ReqUrl);
                        return Ok(1);
                    }
                    else
                    {
                        // Response.Write("[respondentID] identifier is not found");
                        return Ok(1);
                    }
                }
                else
                {
                    return BadRequest(0);
                }
            }
            catch (SystemException ex)
            {
                return BadRequest(0);
            }
        }


        [HttpGet]
        [Route("/VT/ProjectStatus.aspx")]
        public async Task<IActionResult> ProjectStatus()
        {
            _browserType = CommonHelper.ParseBrowser(Request);
            _iPAddress = GetIPAddress();

            if (Request.Query.Count > 1)
            {
                //string NID = CommonHelper.RandomString(32);
                _SID = Request.Query["ID"];
                string? TempStat = Request.Query["Status"];
                _Status = await ReformStatus(Request.Query["Status"]);
                _RC = Convert.ToInt32(Request.Query["RC"]);
                _SID = _SID?.Replace("[", "").Replace("]", "");

                if (_RC == 0)
                {
                    _Resp = await _surveyAPIController.UpdateProjectDetails("", TempStat ?? "", _Status, "", _SID?.Trim() ?? "", 1, 2);
                }
                else if (_RC == 1)
                {
                    _PID = Convert.ToInt32(Request.Query["PID"]);
                    _Resp = await _surveyAPIController.UpdateProjectDetails("", TempStat ?? "", _Status, _PID.ToString(), _SID?.Trim() ?? "", 1, 3);
                }

                if (Request.Query.ContainsKey("PX"))
                {
                    _PX = 1;
                }
                if (_Resp == "1")
                {
                    if (_Status.ToUpper() == "COMPLETE")
                    {
                        ViewBag.Message = "Congratulations! You have successfully completed the survey.";
                        //return View("MaskingURL");
                    }
                    else if (_Status.ToUpper() == "TERMINATE")
                    {
                        ViewBag.Message = "Thank you very much for your participation. <br> Unfortunately, at the moment we are looking for a different profile to match survey's conditions.";
                        // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you have not completed the survey.");
                    }
                    else if (_Status.ToUpper() == "QUOTAFULL")
                    {
                        ViewBag.Message = "Thank you very much for your participation, but at this time we have received specific numbers of completes.";
                        // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.");
                    }
                    else if (_Status.ToUpper() == "SCREENED")
                    {
                        ViewBag.Message = "Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.";
                        // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you did not match the criteria required for participation.");
                    }
                    else if (_Status.ToUpper() == "OVERQUOTA")
                    {
                        ViewBag.Message = "Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.";
                        // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.");
                    }
                    else if (_Status.ToUpper() == "SEC_TERM")
                    {
                        ViewBag.Message = "Thank you very much for your participation. <br> Unfortunately, at the moment we are lookng for a different profile to match survey's conditions.";
                        // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.");
                    }
                    else if (_Status.ToUpper() == "F_ERROR")
                    {
                        ViewBag.Message = "Thank you very much for your participation. <br> Unfortunately, at the moment we are lookng for a different profile to match survey's conditions.";
                        // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, the quota was reached for your demographic group.");
                    }
                    else
                    {
                        // Response.Write("Thank you for your interest in this survey. <br> Unfortunately, you have not completed the survey.");
                    }
                    await RequestUrlPS(_SID?.Trim() ?? "", _RC, _PID, _PX);
                }
                else
                {
                    ViewBag.Message = "Thank you for your interest in this survey.";
                }
            }
            else
            {
                ViewBag.Message = "Required Parameters";
            }
            return View();
        }

        public async Task<string> ReformStatus(string Status)
        {
            //DataSet ds = clsDAL.ManageCompleteRedirects("", 2, 0);
            var ds = await _surveyAPIController.GenericDataFetcher("ManageCompleteRedirects", new { @Code = "", @opt = 2, @Id = 0 });
            string[] Codes = new string[ds.Count];

            if (ds != null && ds.Any())
            {
                for (int i = 0; i < ds.Count; i++)
                {
                    var row = (IDictionary<string, object>)ds[i];
                    Codes[i] = row.ContainsKey("Code") ? row["Code"]?.ToString() ?? "" : "";
                }
            }


            if (Array.IndexOf(Codes, Status) >= 0)
            {
                return "COMPLETE";
            }
            else if (Status.ToUpper() == "TERMINATE")
            {
                return "TERMINATE";
            }
            else if (Status.ToUpper() == "QUOTAFULL")
            {
                return "QUOTAFULL";
            }
            else if (Status.ToUpper() == "SCREENED")
            {
                return "SCREENED";
            }
            else if (Status.ToUpper() == "OVERQUOTA")
            {
                return "OVERQUOTA";
            }
            else if (Status.ToUpper() == "SEC_TERM")
            {
                return "SEC_TERM";
            }
            else if (Status.ToUpper() == "F_ERROR")
            {
                return "F_ERROR";
            }
            else if (Status.ToUpper() == "INCOMPLETE")
            {
                return "INCOMPLETE";
            }
            else
            {
                return "SEC_TERM";
            }
        }

        async Task<IActionResult> RequestUrlPS(string ID, int RC, int PID, int PX)
        {
            _SID = ""; _Status = ""; _ClientID = ""; int opt;
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
                int trackingtype = await _surveyAPIController.CheckForPixelTracking(ID, opt, PID);
                //DataSet DS = objDAL.GetRequestUrl(ID, opt, PID);
                var DS = await _surveyAPIController.GenericDataFetcher("usp_ShareStatusOnRequest", new { @ID = ID, @opt = opt, @pid = PID });
                if (DS != null && DS.Any())
                {
                    var firstRow = DS.FirstOrDefault();

                    _SID = firstRow?.SuplierID ?? "";
                    _Status = firstRow?.PStatus ?? "";
                    _ReqUrl = firstRow?.RequestUrl ?? "";
                    _ClientID = firstRow?.ClientID ?? "";

                    var projectMapping = await _surveyAPIController.GetProjectMappingRecordBYSID(_SID);
                    int addHashing = projectMapping.AddHashing ?? default(int);
                    string parameterName = projectMapping?.ParameterName ?? "";
                    string hashingType = projectMapping?.HashingType ?? "";

                    string hashcode = "";

                    if (_ReqUrl.IndexOf("[respondentID]") > 0 || _ReqUrl.IndexOf("[RespondentID]") > 0 || _ReqUrl.IndexOf("[RESPONDENTID]") > 0)
                    {
                        _ReqUrl = _ReqUrl.Replace("[respondentID]", _ClientID);
                        _ReqUrl = _ReqUrl.Replace("[RespondentID]", _ClientID);
                        _ReqUrl = _ReqUrl.Replace("[RESPONDENTID]", _ClientID);

                        if (addHashing == 1 && hashingType.ToUpper().Trim() == "SHA3")
                        {
                            hashcode = Encryption.HashSHA3(_ReqUrl, "9c1ea7525fdb73c4498bbc55b961b381c1054d6b");
                            _ReqUrl = _ReqUrl + "&" + parameterName + "=" + hashcode;
                        }
                        else if (addHashing == 1 && hashingType.ToUpper().Trim() == "SHA1")
                        {
                            hashcode = Encryption.HashSHA1_C(_ReqUrl, "50e8a826d47d488cbf7a036b38fab5");
                            _ReqUrl = _ReqUrl + "&" + parameterName + "=" + hashcode;
                        }

                        if (trackingtype == 1)
                        {
                            string iframeHtml = $"<iframe src=\"{_ReqUrl}\" scrolling=\"no\" frameborder=\"0\" width=\"1\" height=\"1\"></iframe>";
                            return Content(iframeHtml, "text/html");
                        }
                        else if (PX == 1)
                        {
                            //Response.Write("<script>top.window.location.href = '" + ReqUrl + "';</script>");
                            //return Content($"<script>top.window.location.href = '{_ReqUrl}';</script>", "text/html");
                            return Redirect(_ReqUrl);
                        }
                        else
                        {
                            DateTime now = DateTime.Now;
                            //DataSet ds = _surveyAPIController.GetRequestUrl(ID, 2, 0);
                            var ds = await _surveyAPIController.GenericDataFetcher("usp_ShareStatusOnRequest", new { @ID = ID, @opt = 2, @pid = 0 });
                            if (DS != null && DS.Any())
                            {
                                var firstRow1 = DS.FirstOrDefault();
                                int allowHashing = Convert.ToInt32(firstRow1?.AllowHashing ?? 0);
                                if (addHashing == 1)
                                {
                                    string HashingURL = firstRow1?.HashingURL ?? "";
                                    string CompleteStatus = "10";//Convert.ToString(ds.Tables[0].Rows[0]["CompleteStatus"]);
                                    string AuthorizationKey = firstRow1?.AuthorizationKey ?? "";
                                    if (_Status.ToUpper() == "COMPLETE")
                                    {
                                        if (HashingURL.IndexOf("[respondentID]") > 0 || HashingURL.IndexOf("[RespondentID]") > 0 || HashingURL.IndexOf("[RESPONDENTID]") > 0)
                                        {
                                            HashingURL = HashingURL.Replace("[respondentID]", _ClientID);
                                            HashingURL = HashingURL.Replace("[RespondentID]", _ClientID);
                                            HashingURL = HashingURL.Replace("[RESPONDENTID]", _ClientID);
                                        }
                                        if (HashingURL.IndexOf("[StatusCode]") > 0 || HashingURL.IndexOf("[StatusCODE]") > 0 || HashingURL.IndexOf("[STATUSCODE]") > 0)
                                        {
                                            HashingURL = HashingURL.Replace("[StatusCode]", CompleteStatus);
                                            HashingURL = HashingURL.Replace("[StatusCODE]", CompleteStatus);
                                            HashingURL = HashingURL.Replace("[STATUSCODE]", CompleteStatus);
                                        }
                                        HashingTest hashingTest = new HashingTest();
                                        //string stt = Status.ToUpper() == "Complete" ? "10" : "70";
                                        hashingTest.CallSSCB(HashingURL, CompleteStatus, AuthorizationKey, now);
                                    }
                                }

                            }


                            //ClsDAL.WriteErrorLog("ReqURL URL at time " + now + " is : " + ReqUrl);
                            //HtmlMeta meta = new HtmlMeta();
                            //meta.HttpEquiv = "Refresh";
                            //meta.Content = "2;url=" + ReqUrl;
                            //this.Page.Controls.Add(meta);
                            ViewBag.RedirectUrl = _ReqUrl;
                        }

                        await _surveyAPIController.UpdateRequestStatus(ID);

                        //objDAL.UpdateRequestStatus(ID);

                        await _surveyAPIController.UpdateSupplierENCByUID(ID, hashcode);

                        //var sp =await _surveyAPIController.GetSupplierProjectsByUID(ID);
                        //sp.ENC = hashcode;
                        //db.SaveChanges();
                        return Ok("");
                    }
                    else
                    {
                        // Response.Write("[respondentID] identifier is not found");
                        return Ok("");
                    }
                }
                else
                {
                    return BadRequest("");
                }
            }
            catch (SystemException ex)
            {
                //ClsDAL.WriteErrorLog("Exception in ProjectStatus at " + DateTime.Now + " : " + ex.Message);
                return BadRequest("");

            }
        }

    }
}
