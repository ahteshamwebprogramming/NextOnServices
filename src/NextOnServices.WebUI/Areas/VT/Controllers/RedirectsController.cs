using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;
using System.Security.Cryptography;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
public class RedirectsController : Controller
{
    private readonly SurveyAPIController _surveyAPIController;

    public RedirectsController(SurveyAPIController surveyAPIController)
    {
        _surveyAPIController = surveyAPIController;
    }

    [HttpGet]
    [Route("/VT/Redirects")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("/VT/Redirects/GenerateCompleteRedirects")]
    public IActionResult GenerateCompleteRedirects()
    {
        return View();
    }

    /// <summary>Fetch complete redirect links (opt=1 active, opt=6 deleted).</summary>
    [HttpPost]
    [Route("/VT/Redirects/FetchCompleteLink")]
    public async Task<IActionResult> FetchCompleteLink([FromBody] RedirectsRequest request)
    {
        int opt = request?.opt == 6 ? 6 : 1;
        try
        {
            var list = await _surveyAPIController.GenericDataFetcher("ManageCompleteRedirects",
                new { Code = "", opt, Id = 0 });
            var result = new List<object>();
            if (list != null)
            {
                foreach (var row in list)
                {
                    var dict = row as IDictionary<string, object>;
                    if (dict == null) continue;
                    result.Add(new
                    {
                        Code = dict.ContainsKey("Code") ? dict["Code"]?.ToString() ?? "" : "",
                        Enable = dict.ContainsKey("Enable") && dict["Enable"] != null && dict["Enable"] != DBNull.Value
                            ? Convert.ToInt32(dict["Enable"])
                            : 0,
                        Id = dict.ContainsKey("Id") && dict["Id"] != null && dict["Id"] != DBNull.Value
                            ? Convert.ToInt32(dict["Id"])
                            : 0
                    });
                }
            }
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new List<object>());
        }
    }

    /// <summary>Generate random code (8–16 chars).</summary>
    [HttpPost]
    [Route("/VT/Redirects/GenerateRandomLink")]
    public IActionResult GenerateRandomLink([FromBody] GenerateLinkRequest request)
    {
        int len;
        if (request?.Charl == null || !int.TryParse(request.Charl.Trim(), out len) || len < 8 || len > 16)
            return Ok("InvalidNumber101");
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var bytes = new byte[len * 4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        var result = new char[len];
        for (int i = 0; i < len; i++)
            result[i] = chars[(int)(BitConverter.ToUInt32(bytes, i * 4) % (uint)chars.Length)];
        return Ok(new string(result));
    }

    /// <summary>Save complete link (opt=0).</summary>
    [HttpPost]
    [Route("/VT/Redirects/SaveCompleteLink")]
    public async Task<IActionResult> SaveCompleteLink([FromBody] SaveRedirectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Code))
            return Ok(new[] { new { RetVal = -2 } });
        try
        {
            var list = await _surveyAPIController.GenericDataFetcher("ManageCompleteRedirects",
                new { Code = request.Code.Trim(), opt = 0, Id = 0 });
            int retVal = -1;
            if (list != null && list.Count > 0)
            {
                var row = list[0] as IDictionary<string, object>;
                if (row != null && row.ContainsKey("RetVal") && row["RetVal"] != null && row["RetVal"] != DBNull.Value)
                    retVal = Convert.ToInt32(row["RetVal"]);
            }
            return Ok(new[] { new { RetVal = retVal } });
        }
        catch
        {
            return Ok(new[] { new { RetVal = -1 } });
        }
    }

    /// <summary>Update status: Delete=3, Enable=4, Disable=5.</summary>
    [HttpPost]
    [Route("/VT/Redirects/UpdateStatusCompleteLink")]
    public async Task<IActionResult> UpdateStatusCompleteLink([FromBody] UpdateRedirectRequest request)
    {
        if (request == null || request.Id <= 0)
            return Ok(new[] { new { RetVal = 0 } });
        try
        {
            var list = await _surveyAPIController.GenericDataFetcher("ManageCompleteRedirects",
                new { Code = "", opt = request.opt, Id = request.Id });
            int retVal = 0;
            if (list != null && list.Count > 0)
            {
                var row = list[0] as IDictionary<string, object>;
                if (row != null && row.ContainsKey("RetVal") && row["RetVal"] != null && row["RetVal"] != DBNull.Value)
                    retVal = Convert.ToInt32(row["RetVal"]);
            }
            return Ok(new[] { new { RetVal = retVal } });
        }
        catch
        {
            return Ok(new[] { new { RetVal = 0 } });
        }
    }
}

public class RedirectsRequest
{
    public int opt { get; set; }
}

public class GenerateLinkRequest
{
    public string? Charl { get; set; }
}

public class SaveRedirectRequest
{
    public string? Code { get; set; }
    public string? Link { get; set; }
}

public class UpdateRedirectRequest
{
    public int opt { get; set; }
    public int Id { get; set; }
}
