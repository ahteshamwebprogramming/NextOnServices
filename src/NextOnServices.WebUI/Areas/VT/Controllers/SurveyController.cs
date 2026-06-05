using NextOnServices.Infrastructure.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
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
using System.Text;
using System.Text.RegularExpressions;
using NextOnServices.Core.Entities;
using NextOnServices.Infrastructure.Models.APIProjects;
using NextOnServices.WebUI.VT.Services;

namespace NextOnServices.WebUI.Areas.VT.Controllers
{
    [Area("VT")]
    public class SurveyController : Controller
    {
        private static Random _random = new Random();

        private readonly ILogger<SurveyController> _logger;
        private readonly SurveyAPIController _surveyAPIController;
        private readonly LucidMarketplaceAPIController _lucidMarketplaceAPIController;
        private readonly ZampliaAPIController _zampliaAPIController;
        private readonly IZampliaLaunchService _zampliaLaunchService;
        private readonly ILegacyProjectStatusService _legacyProjectStatusService;
        private readonly IHashingSettingsService _hashingSettingsService;
        private readonly IConfiguration _configuration;

        private string _browserType;
        private string _iPAddress;

        private string _SID = "", _Code = "", _clientdevice = "", _OLDUID, _enc, _counter = "", _GIPAddress = string.Empty, _Status = "", _Resp = "", _ReqUrl = "", _ClientID = "";
        private int _ISRC = 0, _RC = 0, _cnt = 0, _PID = 0, _PX = 0;
        public SurveyController(
            ILogger<SurveyController> logger,
            SurveyAPIController surveyAPIController,
            LucidMarketplaceAPIController lucidMarketplaceAPIController,
            ZampliaAPIController zampliaAPIController,
            ProfileInfoSurveyAPIController profileInfoSurveyAPIController,
            IZampliaLaunchService zampliaLaunchService,
            ILegacyProjectStatusService legacyProjectStatusService,
            IHashingSettingsService hashingSettingsService,
            IConfiguration configuration)
        {
            _logger = logger;
            _surveyAPIController = surveyAPIController;
            _lucidMarketplaceAPIController = lucidMarketplaceAPIController;
            _zampliaAPIController = zampliaAPIController;
            _zampliaLaunchService = zampliaLaunchService;
            _legacyProjectStatusService = legacyProjectStatusService;
            _hashingSettingsService = hashingSettingsService;
            _configuration = configuration;
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
                _SID = SID ?? string.Empty;
                _Code = Code ?? string.Empty;
                _enc = enc ?? string.Empty;
                _clientdevice = clientdevice;

                _logger.LogInformation(
                    "MaskingURL launch request. SID={SID}, Code={Code}, ENC={ENC}, NID={NID}, IPAddress={IPAddress}, Browser={Browser}, Device={Device}",
                    SID,
                    Code,
                    enc,
                    NID,
                    _iPAddress,
                    _browserType,
                    clientdevice);



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
                    _logger.LogInformation("NID : " + NID);
                    string St = await _surveyAPIController.SaveSupplierProject(IPAddress, _browserType, "InComplete", SID, Code, NID, "", clientdevice, 0, enc);
                    _logger.LogInformation("NID : " + NID);
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
                            string returnedUid = firstRow?.UID?.ToString() ?? string.Empty;
                            if (!string.IsNullOrWhiteSpace(returnedUid) &&
                                !string.Equals(returnedUid, NID, StringComparison.OrdinalIgnoreCase))
                            {
                                _logger.LogWarning(
                                    "MaskingURL ignored UID returned by Usp_GetSupplierUrl to avoid race condition. SID={SID}, PMID={PMID}, GeneratedNID={GeneratedNID}, ReturnedUID={ReturnedUID}",
                                    SID,
                                    Code,
                                    NID,
                                    returnedUid);
                            }

                            string ID = NID;
                            var launchContext = await _surveyAPIController.GetProjectLaunchRuntimeContextAsync(SID ?? string.Empty, Code);

                            if (string.Equals(launchContext?.ProjectFrom, "Zamplia", StringComparison.OrdinalIgnoreCase))
                            {
                                var runtimeLaunchResult = await TryHandleRuntimeProjectLaunchAsync(
                                    launchContext,
                                    SID,
                                    Code,
                                    NID,
                                    St,
                                    Url,
                                    ID,
                                    DSPrescreen,
                                    isToken,
                                    clientdevice,
                                    enc);

                                if (runtimeLaunchResult != null)
                                {
                                    return runtimeLaunchResult;
                                }
                            }

                            if (ContainsRespondentPlaceholder(Url))
                            {
                                Url = ApplyRespondentLaunchPlaceholders(Url, ID, Code, SID);

                                if (isToken == "1")
                                {
                                    if (ContainsTokenPlaceholder(Url))
                                    {
                                        string token = await TokensCheckandFetch(NID, SID, 1);
                                        token = token.Replace("\n", "");
                                        token = token.Replace("\r", "");
                                        if (token != "Error" && token != "NOTOKENSLEFT")
                                        {
                                            Url = Url.Replace("[tokenID]", token);
                                            Url = Url.Replace("[TokenID]", token);
                                            Url = Url.Replace("[TOKENID]", token);
                                            Url = await ApplyProjectMappingLaunchHashingAsync(SID, ID, Url);
                                            if (DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First().Exists?.ToString() == "Exists")
                                            {
                                                string PrID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First().ProjectID?.ToString() ?? "";
                                                HttpContext.Session.SetString("PrID", PrID);

                                                string CntryID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First().CountryID?.ToString() ?? "";
                                                HttpContext.Session.SetString("CntryID", CntryID);
                                                HttpContext.Session.SetString("UID", ID);
                                                HttpContext.Session.SetString("URL", Url);
                                                return Redirect("SurveyMaster.aspx");
                                            }
                                            else
                                            {
                                                return Redirect(Url);
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
                                        await _surveyAPIController.GenericDataFetcher("updateRecontactProjectsIDs", new { @opt = 0, @PMID = SID, @UIDnew = ID, @UID = Code });
                                        Url = await ApplyProjectMappingLaunchHashingAsync(SID, ID, Url);
                                        return Redirect(Url);
                                    }
                                    else
                                    {
                                        ViewBag.Message = "This respondant is not allowed to recontact";
                                        return View();
                                    }
                                }
                                else
                                {
                                    Url = await ApplyProjectMappingLaunchHashingAsync(SID, ID, Url);
                                    if (DSPrescreen != null && DSPrescreen.Any() && (((IDictionary<string, object>)DSPrescreen.First()).Values.ElementAtOrDefault(0)?.ToString()) == "Exists")
                                    {
                                        string PrID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First()[1]?.ToString() ?? "";
                                        HttpContext.Session.SetString("PrID", PrID);

                                        string CntryID = DSPrescreen != null && DSPrescreen.Any() && DSPrescreen.First()[2]?.ToString() ?? "";
                                        HttpContext.Session.SetString("CntryID", CntryID);
                                        HttpContext.Session.SetString("UID", NID);
                                        HttpContext.Session.SetString("URL", Url);
                                        return Redirect("SurveyMaster.aspx");
                                    }
                                    else
                                    {
                                        return Redirect(Url);
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

        private async Task<string> ApplyProjectMappingLaunchHashingAsync(string? sid, string? supplierProjectUid, string launchUrl)
        {
            if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(launchUrl))
            {
                return launchUrl;
            }

            try
            {
                var projectMapping = await _surveyAPIController.GetProjectMappingRecordBYSID(sid);
                if (projectMapping == null)
                {
                    return launchUrl;
                }

                var hashingResult = await _hashingSettingsService.ApplyHashAsync(
                    launchUrl,
                    projectMapping.AddHashing,
                    projectMapping.HashingType,
                    projectMapping.ParameterName);

                if (hashingResult.HashApplied && !string.IsNullOrWhiteSpace(supplierProjectUid))
                {
                    await _surveyAPIController.UpdateSupplierENCByUID(supplierProjectUid, hashingResult.HashCode);

                    _logger.LogInformation(
                        "MaskingURL applied launch hashing. SID={SID}, UID={UID}, HashingType={HashingType}, ParameterName={ParameterName}",
                        sid,
                        supplierProjectUid,
                        projectMapping.HashingType,
                        projectMapping.ParameterName);
                }

                return hashingResult.RequestUrl;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MaskingURL hashing failed. SID={SID}, UID={UID}", sid, supplierProjectUid);
                return launchUrl;
            }
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

        private async Task<string> ResolveLaunchRuntimeUidAsync(string? sid, string? code, string generatedUid, string saveResult)
        {
            if (!string.IsNullOrWhiteSpace(generatedUid) &&
                !string.Equals(saveResult, "3", StringComparison.Ordinal))
            {
                return generatedUid;
            }

            SupplierProjects? supplierProject = null;
            if (string.IsNullOrWhiteSpace(generatedUid))
            {
                _logger.LogWarning(
                    "MaskingURL runtime UID fallback started because generated NID was empty. SID={SID}, Code={Code}, SaveResult={SaveResult}",
                    sid,
                    code,
                    saveResult);
            }
            else
            {
                _logger.LogWarning(
                    "MaskingURL runtime UID fallback started because SaveSupplierProject did not return a usable success state. SID={SID}, Code={Code}, SaveResult={SaveResult}, GeneratedNID={GeneratedNID}",
                    sid,
                    code,
                    saveResult,
                    generatedUid);
            }

            if (!string.IsNullOrWhiteSpace(generatedUid))
            {
                _logger.LogInformation(
                    "MaskingURL runtime UID fallback trying SupplierProjects lookup by generated UID. SID={SID}, Code={Code}, GeneratedNID={GeneratedNID}",
                    sid,
                    code,
                    generatedUid);
                supplierProject = await _surveyAPIController.GetSupplierProjectByUidAsync(generatedUid, sid);
            }

            if (supplierProject == null && !string.IsNullOrWhiteSpace(code))
            {
                _logger.LogWarning(
                    "MaskingURL runtime UID fallback trying SupplierProjects lookup by PMID. SID={SID}, Code={Code}, SaveResult={SaveResult}, GeneratedNID={GeneratedNID}",
                    sid,
                    code,
                    saveResult,
                    generatedUid);
                supplierProject = await _surveyAPIController.GetSupplierProjectByPmidAsync(code, sid);
            }

            var resolvedUid = supplierProject?.UID;
            if (!string.IsNullOrWhiteSpace(resolvedUid))
            {
                if (!string.IsNullOrWhiteSpace(generatedUid) &&
                    !string.Equals(resolvedUid, generatedUid, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning(
                        "MaskingURL runtime UID fallback resolved a UID different from the generated NID. SID={SID}, Code={Code}, SaveResult={SaveResult}, GeneratedNID={GeneratedNID}, RuntimeUID={RuntimeUID}",
                        sid,
                        code,
                        saveResult,
                        generatedUid,
                        resolvedUid);
                }

                return resolvedUid;
            }

            _logger.LogWarning(
                "MaskingURL could not resolve runtime UID after fallback. SID={SID}, Code={Code}, SaveResult={SaveResult}, GeneratedNID={GeneratedNID}",
                sid,
                code,
                saveResult,
                generatedUid);

            return string.Empty;
        }

        private async Task<IActionResult?> TryHandleRuntimeProjectLaunchAsync(
            ProjectLaunchRuntimeContextDTO? launchContext,
            string? sid,
            string? code,
            string generatedUid,
            string saveResult,
            string currentUrl,
            string currentId,
            IEnumerable<dynamic>? prescreenRows,
            string isToken,
            string clientDevice,
            string? enc)
        {
            if (!IsRuntimeLaunchProjectSource(launchContext?.ProjectFrom))
            {
                return null;
            }

            launchContext ??= new ProjectLaunchRuntimeContextDTO
            {
                ProjectMappingSid = sid,
                ProjectMappingCode = code
            };

            var runtimeUid = await ResolveLaunchRuntimeUidAsync(sid, code, generatedUid, saveResult);
            if (!string.IsNullOrWhiteSpace(runtimeUid))
            {
                currentId = runtimeUid;
            }
            else if (string.Equals(saveResult, "1", StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(generatedUid))
            {
                _logger.LogInformation(
                    "MaskingURL using generated NID as runtime UID fallback for project source {ProjectFrom}. SID={SID}, Code={Code}, GeneratedNID={GeneratedNID}",
                    launchContext.ProjectFrom,
                    sid,
                    code,
                    generatedUid);
                currentId = generatedUid;
            }

            if (string.IsNullOrWhiteSpace(currentId))
            {
                ViewBag.Title = "Launch Unavailable";
                ViewBag.Message = "Unable to resolve the respondent identifier for this launch.";
                return View();
            }

            if (ContainsRespondentPlaceholder(currentUrl))
            {
                currentUrl = ApplyRespondentLaunchPlaceholders(currentUrl, currentId, code, sid);
            }

            if (isToken == "1" && ContainsTokenPlaceholder(currentUrl))
            {
                string token = await TokensCheckandFetch(generatedUid, sid, 1);
                token = token.Replace("\n", string.Empty);
                token = token.Replace("\r", string.Empty);

                if (token == "Error" || token == "NOTOKENSLEFT")
                {
                    ViewBag.Message = "You are out of tokens";
                    return View();
                }

                currentUrl = currentUrl.Replace("[tokenID]", token);
                currentUrl = currentUrl.Replace("[TokenID]", token);
                currentUrl = currentUrl.Replace("[TOKENID]", token);
            }

            var resolvedUrl = await TryResolveVendorRedirectUrlAsync(launchContext, currentUrl, currentId, code, clientDevice, enc);
            if (string.IsNullOrWhiteSpace(resolvedUrl))
            {
                ViewBag.Title = "Launch Unavailable";
                ViewBag.Message = BuildRuntimeLaunchUnavailableMessage(launchContext.ProjectFrom);
                return View();
            }

            var prescreenExists = HasPrescreenRows(prescreenRows);
            if (prescreenExists)
            {
                string projectId = GetPrescreenColumnValue(prescreenRows, "ProjectID", 1);
                HttpContext.Session.SetString("PrID", projectId);

                string countryId = GetPrescreenColumnValue(prescreenRows, "CountryID", 2);
                HttpContext.Session.SetString("CntryID", countryId);
                HttpContext.Session.SetString("UID", currentId);
                HttpContext.Session.SetString("URL", resolvedUrl);
                return Redirect("SurveyMaster.aspx");
            }

            return Redirect(resolvedUrl);
        }

        private static bool IsRuntimeLaunchProjectSource(string? projectFrom)
        {
            return string.Equals(projectFrom, "Zamplia", StringComparison.OrdinalIgnoreCase)
                || string.Equals(projectFrom, "LucidMarketplace", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasPrescreenRows(IEnumerable<dynamic>? prescreenRows)
        {
            if (prescreenRows == null)
            {
                return false;
            }

            var firstRow = prescreenRows.FirstOrDefault();
            if (firstRow == null)
            {
                return false;
            }

            if (firstRow is IDictionary<string, object> dictionary)
            {
                return string.Equals(dictionary.Values.ElementAtOrDefault(0)?.ToString(), "Exists", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(dictionary.TryGetValue("Exists", out var existsValue) ? existsValue?.ToString() : null, "Exists", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(dictionary.TryGetValue("Exist", out var existValue) ? existValue?.ToString() : null, "Exists", StringComparison.OrdinalIgnoreCase);
            }

            return string.Equals(firstRow.Exists?.ToString(), "Exists", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetPrescreenColumnValue(IEnumerable<dynamic>? prescreenRows, string columnName, int fallbackIndex)
        {
            var firstRow = prescreenRows?.FirstOrDefault();
            if (firstRow == null)
            {
                return string.Empty;
            }

            if (firstRow is IDictionary<string, object> dictionary)
            {
                if (dictionary.TryGetValue(columnName, out var namedValue) && namedValue != null)
                {
                    return namedValue.ToString() ?? string.Empty;
                }

                return dictionary.Values.ElementAtOrDefault(fallbackIndex)?.ToString() ?? string.Empty;
            }

            return columnName switch
            {
                "ProjectID" => firstRow.ProjectID?.ToString() ?? string.Empty,
                "CountryID" => firstRow.CountryID?.ToString() ?? string.Empty,
                _ => string.Empty
            };
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

        private static bool ContainsRespondentPlaceholder(string url)
        {
            return !string.IsNullOrWhiteSpace(url) &&
                   (url.Contains("[respondentID]", StringComparison.OrdinalIgnoreCase) ||
                    url.Contains("[RespondentID]", StringComparison.OrdinalIgnoreCase) ||
                    url.Contains("[RESPONDENTID]", StringComparison.OrdinalIgnoreCase));
        }

        private static bool ContainsRespondentPanelistPlaceholder(string? url)
        {
            return !string.IsNullOrWhiteSpace(url) &&
                   (url.Contains("[respondentpanelistID]", StringComparison.OrdinalIgnoreCase) ||
                    url.Contains("[RespondentPanelistID]", StringComparison.OrdinalIgnoreCase) ||
                    url.Contains("[RESPONDENTPANELISTID]", StringComparison.OrdinalIgnoreCase));
        }

        private string ApplyRespondentLaunchPlaceholders(string url, string respondentId, string? respondentPanelistId, string? sid)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }

            var resolvedUrl = url
                .Replace("[respondentID]", respondentId ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("[RespondentID]", respondentId ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("[RESPONDENTID]", respondentId ?? string.Empty, StringComparison.OrdinalIgnoreCase);

            if (!ContainsRespondentPanelistPlaceholder(resolvedUrl))
            {
                return resolvedUrl;
            }

            if (!string.IsNullOrWhiteSpace(respondentPanelistId))
            {
                _logger.LogInformation(
                    "Launch URL [respondentpanelistID] placeholder replaced with original respondent ID. SID={SID}, RuntimeUID={RuntimeUID}",
                    sid,
                    respondentId);

                return resolvedUrl
                .Replace("[respondentpanelistID]", respondentPanelistId, StringComparison.OrdinalIgnoreCase)
                .Replace("[RespondentPanelistID]", respondentPanelistId, StringComparison.OrdinalIgnoreCase)
                .Replace("[RESPONDENTPANELISTID]", respondentPanelistId, StringComparison.OrdinalIgnoreCase);
            }

            _logger.LogWarning(
                "Launch URL contains [respondentpanelistID] placeholder but original respondent ID is empty. SID={SID}, RuntimeUID={RuntimeUID}",
                sid,
                respondentId);

            return resolvedUrl;
        }

        private static bool ContainsTokenPlaceholder(string url)
        {
            return !string.IsNullOrWhiteSpace(url) &&
                   (url.Contains("[tokenID]", StringComparison.OrdinalIgnoreCase) ||
                    url.Contains("[TokenID]", StringComparison.OrdinalIgnoreCase) ||
                    url.Contains("[TOKENID]", StringComparison.OrdinalIgnoreCase));
        }

        private static string BuildRuntimeLaunchUnavailableMessage(string? projectFrom)
        {
            if (string.Equals(projectFrom, "LucidMarketplace", StringComparison.OrdinalIgnoreCase))
            {
                return "Lucid Marketplace launch link could not be prepared right now.";
            }

            if (string.Equals(projectFrom, "Zamplia", StringComparison.OrdinalIgnoreCase))
            {
                return "Zamplia launch link could not be generated right now.";
            }

            return "Vendor launch link could not be generated right now.";
        }

        private static string BuildRuntimeLaunchTemplateMessage(string? projectFrom)
        {
            if (string.Equals(projectFrom, "LucidMarketplace", StringComparison.OrdinalIgnoreCase))
            {
                return "Stored Lucid Marketplace link is not configured correctly.";
            }

            if (string.Equals(projectFrom, "Zamplia", StringComparison.OrdinalIgnoreCase))
            {
                return "Runtime launch template is not configured correctly.";
            }

            return "[respondentID] identifier is not found";
        }

        private async Task<string?> TryResolveVendorRedirectUrlAsync(
            ProjectLaunchRuntimeContextDTO launchContext,
            string currentUrl,
            string supplierProjectUid,
            string? sourceRespondentId,
            string clientDevice,
            string? enc)
        {
            if (string.Equals(launchContext.ProjectFrom, "LucidMarketplace", StringComparison.OrdinalIgnoreCase))
            {
                return await TryResolveLucidRedirectUrlAsync(launchContext, currentUrl, supplierProjectUid);
            }

            return await TryResolveZampliaRedirectUrlAsync(launchContext, currentUrl, supplierProjectUid, sourceRespondentId, clientDevice, enc);
        }

        private async Task<string?> TryResolveZampliaRedirectUrlAsync(
            ProjectLaunchRuntimeContextDTO launchContext,
            string currentUrl,
            string supplierProjectUid,
            string? sourceRespondentId,
            string clientDevice,
            string? enc)
        {
            if (!string.Equals(launchContext.ProjectFrom, "Zamplia", StringComparison.OrdinalIgnoreCase))
            {
                return currentUrl;
            }

            var resolution = await _zampliaLaunchService.TryResolveVendorLaunchAsync(new ZampliaLaunchServiceRequest
            {
                ProjectMappingSid = launchContext.ProjectMappingSid,
                ProjectMappingCode = launchContext.ProjectMappingCode,
                SupplierProjectUid = supplierProjectUid,
                SourceRespondentId = sourceRespondentId,
                ClientIp = _iPAddress,
                ClientBrowser = _browserType,
                ClientDevice = clientDevice,
                Enc = enc,
                LaunchRequestUrl = Request.GetDisplayUrl(),
                UserId = HttpContext.Session.GetInt32("UserId")
            });

            if (!resolution.Success || string.IsNullOrWhiteSpace(resolution.VendorLaunchUrl))
            {
                _logger.LogWarning("Unable to resolve Zamplia redirect URL for SID {Sid}. Message: {Message}", launchContext.ProjectMappingSid, resolution.Message);
                return null;
            }

            SetZampliaAttemptCookie(resolution);
            return await ApplyProjectMappingLaunchHashingAsync(
                launchContext.ProjectMappingSid,
                FirstNonEmpty(resolution.SupplierProjectUid, supplierProjectUid),
                resolution.VendorLaunchUrl);
        }

        private async Task<string?> TryResolveLucidRedirectUrlAsync(
            ProjectLaunchRuntimeContextDTO launchContext,
            string currentUrl,
            string supplierProjectUid)
        {
            if (!string.Equals(launchContext.ProjectFrom, "LucidMarketplace", StringComparison.OrdinalIgnoreCase))
            {
                return currentUrl;
            }

            if (!launchContext.ProjectId.HasValue || launchContext.ProjectId.Value <= 0)
            {
                _logger.LogWarning("Unable to resolve Lucid Marketplace redirect URL because the internal project id is missing for SID {Sid}.", launchContext.ProjectMappingSid);
                return null;
            }

            var lucidContext = await _lucidMarketplaceAPIController.GetLucidMarketplaceLaunchContextAsync(
                internalProjectId: launchContext.ProjectId,
                internalProjectMappingId: launchContext.ProjectMappingId);

            if (lucidContext == null || lucidContext.LucidMarketplaceOpportunityId <= 0 || lucidContext.LucidSurveyId <= 0)
            {
                _logger.LogWarning("Unable to resolve Lucid Marketplace launch context for project {ProjectId} / SID {Sid}.", launchContext.ProjectId, launchContext.ProjectMappingSid);
                return null;
            }

            var storedLucidLink = await ResolveStoredLucidBaseLinkAsync(launchContext, currentUrl, lucidContext);
            if (string.IsNullOrWhiteSpace(storedLucidLink))
            {
                _logger.LogWarning("Stored Lucid Marketplace launch link was not found for project {ProjectId} / opportunity {OpportunityId}.", launchContext.ProjectId, lucidContext.LucidMarketplaceOpportunityId);
                return null;
            }

            var vendorLaunchUrl = BuildLucidRespondentLaunchUrl(storedLucidLink, supplierProjectUid, out _, out _);
            if (string.IsNullOrWhiteSpace(vendorLaunchUrl))
            {
                _logger.LogWarning("Unable to build Lucid Marketplace respondent launch URL for project {ProjectId} / opportunity {OpportunityId}.", launchContext.ProjectId, lucidContext.LucidMarketplaceOpportunityId);
                return null;
            }

            var savedAttempt = await PersistLucidCompatibilityAttemptAsync(lucidContext, supplierProjectUid);
            if (savedAttempt == null || savedAttempt.Id <= 0)
            {
                _logger.LogWarning("Unable to create the minimal Lucid Marketplace redirect correlation row for project {ProjectId} / opportunity {OpportunityId}.", launchContext.ProjectId, lucidContext.LucidMarketplaceOpportunityId);
                return null;
            }

            SetLucidAttemptCookie(savedAttempt.Id, lucidContext.InternalProjectMappingId, lucidContext.LucidMarketplaceOpportunityId);
            return vendorLaunchUrl;
        }

        private async Task<string?> ResolveStoredLucidBaseLinkAsync(
            ProjectLaunchRuntimeContextDTO launchContext,
            string currentUrl,
            LucidMarketplaceLaunchContextDTO lucidContext)
        {
            foreach (var candidate in new[]
                     {
                         launchContext.ProjectUrlOriginalUrl,
                         launchContext.ProjectUrl,
                         launchContext.ProjectMappingOlink,
                         currentUrl
                     })
            {
                if (IsUsableLucidVendorLink(candidate))
                {
                    return candidate?.Trim();
                }
            }

            if (lucidContext.LucidMarketplaceOpportunityId <= 0)
            {
                return null;
            }

            var entryLinkResult = await _lucidMarketplaceAPIController.GetLucidMarketplaceEntryLink(lucidContext.LucidMarketplaceOpportunityId);
            if (entryLinkResult is not ObjectResult entryLinkObject || entryLinkObject.StatusCode != StatusCodes.Status200OK)
            {
                return null;
            }

            var entryLink = entryLinkObject.Value as LucidMarketplaceEntryLinkDTO;
            var fallbackLink = string.IsNullOrWhiteSpace(entryLink?.LiveLink) ? entryLink?.TestLink : entryLink.LiveLink;
            return IsUsableLucidVendorLink(fallbackLink)
                ? fallbackLink?.Trim()
                : null;
        }

        private static bool IsUsableLucidVendorLink(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                return false;
            }

            return !url.Contains("/VT/LucidMarketplace/LaunchRespondent", StringComparison.OrdinalIgnoreCase) &&
                   !url.Contains("/VT/MaskingUrl.aspx", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildLucidRespondentLaunchUrl(
            string liveLink,
            string respondentId,
            out bool hashApplied,
            out string? hashParameterName)
        {
            hashApplied = false;
            hashParameterName = null;

            var resolvedUrl = InjectLucidRespondentIdentifier(liveLink, respondentId, ResolveLucidRespondentParameterName());
            if (string.IsNullOrWhiteSpace(resolvedUrl))
            {
                return string.Empty;
            }

            var launchHashSecret = ResolveLucidLaunchHashSecret();
            if (string.IsNullOrWhiteSpace(launchHashSecret))
            {
                return resolvedUrl;
            }

            hashParameterName = ResolveLucidLaunchHashParameterName();
            resolvedUrl = ApplyLucidLaunchHash(resolvedUrl, launchHashSecret, hashParameterName, ResolveLucidLaunchHashAlgorithm());
            hashApplied = !string.IsNullOrWhiteSpace(hashParameterName);
            return resolvedUrl;
        }

        private static string InjectLucidRespondentIdentifier(string liveLink, string respondentId, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(liveLink) || string.IsNullOrWhiteSpace(respondentId))
            {
                return string.Empty;
            }

            var encodedRespondentId = Uri.EscapeDataString(respondentId);
            if (ContainsRespondentPlaceholder(liveLink))
            {
                return liveLink
                    .Replace("[respondentID]", encodedRespondentId, StringComparison.OrdinalIgnoreCase)
                    .Replace("[RespondentID]", encodedRespondentId, StringComparison.OrdinalIgnoreCase)
                    .Replace("[RESPONDENTID]", encodedRespondentId, StringComparison.OrdinalIgnoreCase);
            }

            foreach (var candidateParameterName in new[] { parameterName, "PID" }
                         .Where(value => !string.IsNullOrWhiteSpace(value))
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var parameterRegex = new Regex($"(?i)([?&]){Regex.Escape(candidateParameterName)}=([^&]*)");
                var updated = parameterRegex.Replace(
                    liveLink,
                    match => $"{match.Groups[1].Value}{candidateParameterName}={encodedRespondentId}",
                    1);

                if (!string.Equals(updated, liveLink, StringComparison.Ordinal))
                {
                    return updated;
                }
            }

            var targetParameterName = string.IsNullOrWhiteSpace(parameterName) ? "PID" : parameterName.Trim();
            return liveLink.Contains("?", StringComparison.Ordinal)
                ? $"{liveLink}&{targetParameterName}={encodedRespondentId}"
                : $"{liveLink}?{targetParameterName}={encodedRespondentId}";
        }

        private static string ApplyLucidLaunchHash(
            string url,
            string secret,
            string parameterName,
            string algorithm)
        {
            var normalizedParameterName = string.IsNullOrWhiteSpace(parameterName) ? "hash" : parameterName.Trim();
            var unsignedUrl = RemoveQueryParameter(url, normalizedParameterName);
            var signature = ComputeLucidLaunchHash(secret, unsignedUrl, algorithm);
            if (string.IsNullOrWhiteSpace(signature))
            {
                return unsignedUrl;
            }

            var encodedSignature = Uri.EscapeDataString(signature);
            return unsignedUrl.Contains("?", StringComparison.Ordinal)
                ? $"{unsignedUrl}&{normalizedParameterName}={encodedSignature}"
                : $"{unsignedUrl}?{normalizedParameterName}={encodedSignature}";
        }

        private static string RemoveQueryParameter(string url, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(parameterName))
            {
                return url;
            }

            var parameterRegex = new Regex($"(?i)([?&]){Regex.Escape(parameterName)}=[^&]*");
            var normalized = parameterRegex.Replace(url, "$1", 1);

            normalized = normalized.Replace("?&", "?", StringComparison.Ordinal)
                                   .Replace("&&", "&", StringComparison.Ordinal)
                                   .TrimEnd('&', '?');

            return normalized;
        }

        private static string ComputeLucidLaunchHash(string secret, string message, string algorithm)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret ?? string.Empty);
            var messageBytes = Encoding.UTF8.GetBytes(message ?? string.Empty);
            var normalizedAlgorithm = string.IsNullOrWhiteSpace(algorithm)
                ? "HMACSHA256"
                : algorithm.Trim().ToUpperInvariant();

            byte[] hashBytes = normalizedAlgorithm switch
            {
                "HMACSHA1" => new HMACSHA1(keyBytes).ComputeHash(messageBytes),
                _ => new HMACSHA256(keyBytes).ComputeHash(messageBytes)
            };

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private string? ResolveLucidLaunchHashSecret()
        {
            return FirstNonEmpty(
                _configuration["LucidMarketplace:LaunchHashSecret"],
                _configuration["LucidMarketplace:LaunchSigningSecret"],
                _configuration["LucidMarketplaceLaunchHashSecret"],
                _configuration["LucidMarketplaceLaunchSigningSecret"]);
        }

        private string ResolveLucidLaunchHashParameterName()
        {
            return FirstNonEmpty(
                       _configuration["LucidMarketplace:LaunchHashParameterName"],
                       _configuration["LucidMarketplaceLaunchHashParameterName"])
                   ?? "hash";
        }

        private string ResolveLucidLaunchHashAlgorithm()
        {
            return FirstNonEmpty(
                       _configuration["LucidMarketplace:LaunchHashAlgorithm"],
                       _configuration["LucidMarketplaceLaunchHashAlgorithm"])
                   ?? "HMACSHA256";
        }

        private string ResolveLucidRespondentParameterName()
        {
            return FirstNonEmpty(
                       _configuration["LucidMarketplace:RespondentParameterName"],
                       _configuration["LucidMarketplaceRespondentParameterName"])
                   ?? "PID";
        }

        private async Task<LucidMarketplaceRespondentAttemptDTO?> PersistLucidCompatibilityAttemptAsync(
            LucidMarketplaceLaunchContextDTO lucidContext,
            string supplierProjectUid)
        {
            var attemptPayload = new LucidMarketplaceRespondentAttemptDTO
            {
                LucidMarketplaceOpportunityId = lucidContext.LucidMarketplaceOpportunityId,
                InternalProjectId = lucidContext.InternalProjectId,
                InternalProjectUrlId = lucidContext.InternalProjectUrlId,
                InternalProjectMappingId = lucidContext.InternalProjectMappingId,
                LucidSurveyId = lucidContext.LucidSurveyId,
                SupplierCode = lucidContext.SupplierCode,
                RespondentId = supplierProjectUid,
                AttemptType = "Live",
                AttemptedOn = DateTime.Now,
                Notes = "MaskingURL launch compatibility row."
            };

            var saveAttemptResult = await _lucidMarketplaceAPIController.SaveLucidMarketplaceRespondentAttempt(attemptPayload);
            return saveAttemptResult is ObjectResult saveAttemptObject && saveAttemptObject.StatusCode == StatusCodes.Status200OK
                ? saveAttemptObject.Value as LucidMarketplaceRespondentAttemptDTO
                : null;
        }

        private void SetLucidAttemptCookie(int attemptId, int? internalProjectMappingId, int lucidMarketplaceOpportunityId)
        {
            if (attemptId <= 0 || lucidMarketplaceOpportunityId <= 0)
            {
                return;
            }

            Response.Cookies.Append(
                $"lm_attempt_{internalProjectMappingId.GetValueOrDefault()}_{lucidMarketplaceOpportunityId}",
                attemptId.ToString(),
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps,
                    Expires = DateTimeOffset.UtcNow.AddHours(4)
                });
        }

        private void SetZampliaAttemptCookie(ZampliaLaunchResolution resolution)
        {
            if (!resolution.AttemptId.HasValue || !resolution.ZampliaSurveyId.HasValue || resolution.AttemptId.Value <= 0 || resolution.ZampliaSurveyId.Value <= 0)
            {
                return;
            }

            Response.Cookies.Append($"zamplia_attempt_{resolution.ZampliaSurveyId.Value}", resolution.AttemptId.Value.ToString(), new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                Expires = DateTimeOffset.UtcNow.AddHours(4)
            });
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
            if (string.IsNullOrEmpty(code) || code.Length < 5)
                return "No";

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
                _SID = await ResolveLegacyProjectStatusUidAsync(_SID, TempStat, _Status);

                if (_RC == 1)
                {
                    _PID = Convert.ToInt32(Request.Query["PID"]);
                }

                if (Request.Query.ContainsKey("PX"))
                {
                    _PX = 1;
                }

                var legacyStatusResult = await _legacyProjectStatusService.ApplyAsync(
                    _SID?.Trim() ?? string.Empty,
                    TempStat,
                    _RC,
                    _RC == 1 ? _PID : null,
                    _PX == 1);

                _Status = legacyStatusResult.NormalizedStatus;
                _Resp = legacyStatusResult.UpdateResponse;
                ViewBag.RedirectUrl = legacyStatusResult.RedirectUrl;

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

        private async Task<string> ResolveLegacyProjectStatusUidAsync(string? rawId, string? rawStatus, string normalizedStatus)
        {
            var normalizedId = rawId?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedId))
            {
                return string.Empty;
            }

            var existingSupplierProject = await _surveyAPIController.GetSupplierProjectByUidAsync(normalizedId);
            if (existingSupplierProject != null && !string.IsNullOrWhiteSpace(existingSupplierProject.UID))
            {
                return existingSupplierProject.UID.Trim();
            }

            var fallbackAttempt = await LoadZampliaAttemptForLegacyProjectStatusAsync(normalizedId);
            if (fallbackAttempt != null)
            {
                var runtimeUid = FirstNonEmpty(fallbackAttempt.TransactionId, fallbackAttempt.RespondentId);
                if (!string.IsNullOrWhiteSpace(runtimeUid))
                {
                    var runtimeSupplierProject = await _surveyAPIController.GetSupplierProjectByUidAsync(runtimeUid);
                    if (runtimeSupplierProject != null && !string.IsNullOrWhiteSpace(runtimeSupplierProject.UID))
                    {
                        fallbackAttempt.ReturnCode ??= rawStatus?.Trim();
                        fallbackAttempt.ReturnStatus ??= normalizedStatus;
                        fallbackAttempt.FinalStatus ??= normalizedStatus;
                        fallbackAttempt.FinalStatusSource ??= "LegacyProjectStatusFallback";
                        fallbackAttempt.CompletedOn ??= DateTime.Now;
                        await _zampliaAPIController.SaveZampliaRespondentAttempt(fallbackAttempt);

                        foreach (var cookieName in Request.Cookies.Keys.Where(key => key.StartsWith("zamplia_attempt_", StringComparison.OrdinalIgnoreCase)).ToList())
                        {
                            Response.Cookies.Delete(cookieName);
                        }

                        _logger.LogInformation("Resolved legacy ProjectStatus callback to Zamplia SupplierProjects UID {Uid} from raw ID {RawId}.", runtimeSupplierProject.UID, normalizedId);
                        return runtimeSupplierProject.UID.Trim();
                    }
                }
            }

            var lucidAttempt = await LoadLucidAttemptForLegacyProjectStatusAsync(normalizedId);
            if (lucidAttempt == null)
            {
                return normalizedId;
            }

            var lucidRuntimeUid = FirstNonEmpty(lucidAttempt.RespondentId);
            if (string.IsNullOrWhiteSpace(lucidRuntimeUid))
            {
                return normalizedId;
            }

            var lucidSupplierProject = await _surveyAPIController.GetSupplierProjectByUidAsync(lucidRuntimeUid);
            if (lucidSupplierProject == null || string.IsNullOrWhiteSpace(lucidSupplierProject.UID))
            {
                return normalizedId;
            }

            var redirectStatusInfo = LucidMarketplaceReconciliationHelper.ResolveRedirectStatusInfo(rawStatus);
            lucidAttempt.ReturnCode ??= rawStatus?.Trim();
            lucidAttempt.ReturnStatus ??= LucidMarketplaceReconciliationHelper.NormalizeRedirectStatus(rawStatus) ?? redirectStatusInfo.FinalStatus;
            if (!string.Equals(lucidAttempt.FinalStatusSource, "OutcomesFeed", StringComparison.OrdinalIgnoreCase))
            {
                lucidAttempt.FinalStatus = redirectStatusInfo.FinalStatus;
                lucidAttempt.FinalStatusSource = "LegacyProjectStatusFallback";
                lucidAttempt.IsCompleted = redirectStatusInfo.IsCompleted;
                lucidAttempt.IsTerminated = redirectStatusInfo.IsTerminated;
                lucidAttempt.IsOverQuota = redirectStatusInfo.IsOverQuota;
                lucidAttempt.IsQualityTermination = redirectStatusInfo.IsQualityTermination;
                lucidAttempt.IsDuplicate = redirectStatusInfo.IsDuplicate;
                lucidAttempt.IsSecurityTermination = redirectStatusInfo.IsSecurityTermination;
            }

            lucidAttempt.ModifiedDate = DateTime.Now;
            await _lucidMarketplaceAPIController.SaveLucidMarketplaceRespondentAttempt(lucidAttempt);

            foreach (var cookieName in Request.Cookies.Keys.Where(key => key.StartsWith("lm_attempt_", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                Response.Cookies.Delete(cookieName);
            }

            _logger.LogInformation("Resolved legacy ProjectStatus callback to Lucid Marketplace SupplierProjects UID {Uid} from raw ID {RawId}.", lucidSupplierProject.UID, normalizedId);
            return lucidSupplierProject.UID.Trim();
        }

        private async Task<ZampliaRespondentAttemptDTO?> LoadZampliaAttemptForLegacyProjectStatusAsync(string rawId)
        {
            var cookieAttemptIds = Request.Cookies
                .Where(item => item.Key.StartsWith("zamplia_attempt_", StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Value)
                .Where(value => int.TryParse(value, out var parsed) && parsed > 0)
                .Select(value => int.Parse(value!))
                .Distinct()
                .ToList();

            if (cookieAttemptIds.Count == 0)
            {
                return null;
            }

            var attempts = new List<ZampliaRespondentAttemptDTO>();
            foreach (var attemptId in cookieAttemptIds)
            {
                var result = await _zampliaAPIController.GetZampliaRespondentAttempt(attemptId);
                if (result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK && objectResult.Value is ZampliaRespondentAttemptDTO attempt)
                {
                    attempts.Add(attempt);
                }
            }

            if (attempts.Count == 0)
            {
                return null;
            }

            if (long.TryParse(rawId, out var vendorSurveyId) && vendorSurveyId > 0)
            {
                var surveyMatched = attempts
                    .Where(item => item.SurveyId == vendorSurveyId)
                    .OrderByDescending(item => item.AttemptedOn ?? item.CreatedDate)
                    .FirstOrDefault();
                if (surveyMatched != null)
                {
                    return surveyMatched;
                }
            }

            var exactMatched = attempts
                .Where(item => string.Equals(item.TransactionId, rawId, StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(item.RespondentId, rawId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(item => item.AttemptedOn ?? item.CreatedDate)
                .FirstOrDefault();
            if (exactMatched != null)
            {
                return exactMatched;
            }

            return attempts.Count == 1
                ? attempts[0]
                : attempts.OrderByDescending(item => item.AttemptedOn ?? item.CreatedDate).FirstOrDefault();
        }

        private static string? FirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return null;
        }

        private async Task<LucidMarketplaceRespondentAttemptDTO?> LoadLucidAttemptForLegacyProjectStatusAsync(string rawId)
        {
            var cookieAttemptIds = Request.Cookies
                .Where(item => item.Key.StartsWith("lm_attempt_", StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Value)
                .Where(value => int.TryParse(value, out var parsed) && parsed > 0)
                .Select(value => int.Parse(value!))
                .Distinct()
                .ToList();

            if (cookieAttemptIds.Count == 0)
            {
                return null;
            }

            var attempts = new List<LucidMarketplaceRespondentAttemptDTO>();
            foreach (var attemptId in cookieAttemptIds)
            {
                var result = await _lucidMarketplaceAPIController.GetLucidMarketplaceRespondentAttempt(attemptId);
                if (result is ObjectResult objectResult &&
                    objectResult.StatusCode == StatusCodes.Status200OK &&
                    objectResult.Value is LucidMarketplaceRespondentAttemptDTO attempt)
                {
                    attempts.Add(attempt);
                }
            }

            if (attempts.Count == 0)
            {
                return null;
            }

            var exactMatched = attempts
                .Where(item => string.Equals(item.SessionId, rawId, StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(item.RespondentId, rawId, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(item => item.AsyncLastReceivedOn ?? item.ModifiedDate ?? item.AttemptedOn ?? item.CreatedDate)
                .FirstOrDefault();
            if (exactMatched != null)
            {
                return exactMatched;
            }

            if (int.TryParse(rawId, out var lucidSurveyId) && lucidSurveyId > 0)
            {
                var surveyMatched = attempts
                    .Where(item => item.LucidSurveyId == lucidSurveyId)
                    .OrderByDescending(item => item.AsyncLastReceivedOn ?? item.ModifiedDate ?? item.AttemptedOn ?? item.CreatedDate)
                    .FirstOrDefault();
                if (surveyMatched != null)
                {
                    return surveyMatched;
                }
            }

            return attempts.Count == 1
                ? attempts[0]
                : attempts.OrderByDescending(item => item.AsyncLastReceivedOn ?? item.ModifiedDate ?? item.AttemptedOn ?? item.CreatedDate).FirstOrDefault();
        }

        public async Task<string> ReformStatus(string Status)
        {
            var normalizedStatus = Status?.Trim().ToUpperInvariant() ?? string.Empty;
            var ds = await _surveyAPIController.GenericDataFetcher("ManageCompleteRedirects", new { @Code = "", @opt = 2, @Id = 0 });
            if (ds == null || !ds.Any())
            {
                if (normalizedStatus == "TERMINATE") return "TERMINATE";
                if (normalizedStatus == "QUOTAFULL") return "QUOTAFULL";
                if (normalizedStatus == "SCREENED") return "SCREENED";
                return normalizedStatus;
            }

            string[] Codes = new string[ds.Count];
            for (int i = 0; i < ds.Count; i++)
            {
                var row = (IDictionary<string, object>)ds[i];
                Codes[i] = row.ContainsKey("Code") ? row["Code"]?.ToString() ?? "" : "";
            }

            if (Array.IndexOf(Codes, Status) >= 0)
            {
                return "COMPLETE";
            }
            if (normalizedStatus == "TERMINATE") return "TERMINATE";
            if (normalizedStatus == "QUOTAFULL") return "QUOTAFULL";
            if (normalizedStatus == "SCREENED") return "SCREENED";
            if (normalizedStatus == "OVERQUOTA") return "OVERQUOTA";
            if (normalizedStatus == "SEC_TERM") return "SEC_TERM";
            if (normalizedStatus == "F_ERROR") return "F_ERROR";
            if (normalizedStatus == "INCOMPLETE") return "INCOMPLETE";
            return "SEC_TERM";
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
                    int addHashing = projectMapping?.AddHashing ?? default(int);
                    string parameterName = projectMapping?.ParameterName ?? "";
                    string hashingType = projectMapping?.HashingType ?? "";

                    string hashcode = "";

                    if (_ReqUrl.IndexOf("[respondentID]") > 0 || _ReqUrl.IndexOf("[RespondentID]") > 0 || _ReqUrl.IndexOf("[RESPONDENTID]") > 0)
                    {
                        _ReqUrl = _ReqUrl.Replace("[respondentID]", _ClientID);
                        _ReqUrl = _ReqUrl.Replace("[RespondentID]", _ClientID);
                        _ReqUrl = _ReqUrl.Replace("[RESPONDENTID]", _ClientID);

                        var hashingResult = await _hashingSettingsService.ApplyHashAsync(_ReqUrl, addHashing, hashingType, parameterName);
                        _ReqUrl = hashingResult.RequestUrl;
                        hashcode = hashingResult.HashCode;

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






