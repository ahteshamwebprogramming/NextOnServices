using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Infrastructure.Models.Projects;
using System.Text;

namespace NextOnServices.WebUI.Areas.VT.Controllers;

[Area("VT")]
public class RecontactCreateController : Controller
{
    private readonly ILogger<RecontactCreateController> _logger;
    private readonly SurveyAPIController _surveyAPIController;
    private readonly IConfiguration _configuration;

    public RecontactCreateController(
        ILogger<RecontactCreateController> logger,
        SurveyAPIController surveyAPIController,
        IConfiguration configuration)
    {
        _logger = logger;
        _surveyAPIController = surveyAPIController;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("/VT/RecontactCreate")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Create Recontact";
        return View();
    }

    [HttpPost]
    [Route("/VT/RecontactCreate/GetProjects")]
    public async Task<IActionResult> GetProjects()
    {
        try
        {
            var list = await _surveyAPIController.GetProjectsForRecontactCreate();
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetProjects (RecontactCreate) failed");
            return StatusCode(500, new { message = "Failed to load projects." });
        }
    }

    [HttpPost]
    [Route("/VT/RecontactCreate/LoadRecontacts")]
    public async Task<IActionResult> LoadRecontacts([FromBody] LoadRecontactsRequest request)
    {
        try
        {
            int pid = request?.PID ?? 0;
            int cid = request?.CID ?? 0;
            int sid = request?.SID ?? 0;
            int opt = request?.OPT ?? 2;
            var list = await _surveyAPIController.LoadRecontactProjectsForCreate(pid, cid, sid, opt);
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadRecontacts failed");
            return StatusCode(500, new { message = "Failed to load recontacts." });
        }
    }

    public class LoadRecontactsRequest
    {
        public int PID { get; set; }
        public int CID { get; set; }
        public int SID { get; set; }
        public int OPT { get; set; }
    }

    [HttpPost]
    [Route("/VT/RecontactCreate/SaveData")]
    public async Task<IActionResult> SaveData([FromBody] SaveDataRequest request)
    {
        if (request?.StrData == null)
            return BadRequest(new { message = "StrData is required." });

        List<RecontactSaveErrorDTO> errors = new List<RecontactSaveErrorDTO>();
        try
        {
            var jsonOpts = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };
            var data = System.Text.Json.JsonSerializer.Deserialize<RecontactCreateSaveRequest>(request.StrData, jsonOpts);
            if (data == null)
                return BadRequest(new { message = "Invalid JSON in StrData." });

            string rawCsv = data.ExcelData ?? string.Empty;
            int noOfVars = data.NoOfVars ?? 0;
            var rows = ParseCsvToRows(rawCsv, 2 + noOfVars);
            if (rows == null || rows.Count == 0)
            {
                errors.Add(new RecontactSaveErrorDTO { ErrorReturnDBStatus = "3", ErrorReturnDBMessage = "Please try again later" });
                return Json(errors);
            }

            string nextPidRaw = await _surveyAPIController.GetNextRecontactPID();
            string pid = IncrementID(string.IsNullOrEmpty(nextPidRaw) ? "NXT000000" : nextPidRaw);

            string sid = RandomString(4);
            string maskingUrl = _configuration.GetValue<string>("MaskingUrl") ?? "";
            string mUrl = maskingUrl + "?SID=" + sid + "&ID=XXXXXXXXXX&CID=XXXXXXXXXX";
            string[] varNames = new[] { data.Var1, data.Var2, data.Var3, data.Var4, data.Var5 }.Take(noOfVars).ToArray();
            for (int v = 0; v < varNames.Length; v++)
                mUrl += "&" + varNames[v] + "=XXXXXXXXXX";

            int status = data.Status ?? 5;
            double loi = data.LOI ?? 0;
            double ir = data.IR ?? 0;
            int rcq = data.RCQ ?? 0;

            var insertResult = await _surveyAPIController.InsertRecontactCreate(
                data.RecontactName,
                (data.CPI ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture),
                data.Notes ?? "",
                data.RecontactDescription ?? "",
                status,
                loi,
                ir,
                rcq,
                sid,
                mUrl,
                pid,
                0);

            if (insertResult == null || insertResult.status != 0)
            {
                errors.Add(new RecontactSaveErrorDTO { ErrorReturnDBStatus = "3", ErrorReturnDBMessage = "Please try again later" });
                return Json(errors);
            }

            int rcid = insertResult.rcid;
            foreach (var row in rows)
            {
                string uid = row.Count > 0 ? (row[0] ?? "").Trim().Trim('"') : "";
                string clientUrl = row.Count > 1 ? (row[1] ?? "").Trim().Trim('"') : "";
                if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(clientUrl))
                    continue;

                var valRow = await _surveyAPIController.ValidateUIDForRecontact(uid, 0, sid);
                if (valRow == null || valRow.statuss != 1)
                    continue;

                string var1 = row.Count > 2 ? (row[2] ?? "").Trim().Trim('"') : "";
                string var2 = row.Count > 3 ? (row[3] ?? "").Trim().Trim('"') : "";
                string var3 = row.Count > 4 ? (row[4] ?? "").Trim().Trim('"') : "";
                string var4 = row.Count > 5 ? (row[5] ?? "").Trim().Trim('"') : "";
                string var5 = row.Count > 6 ? (row[6] ?? "").Trim().Trim('"') : "";

                var respRows = await _surveyAPIController.UpdateRecontactRespondantstblCreate(
                    uid, clientUrl, var1, var2, var3, var4, var5,
                    valRow.pmid ?? "", mUrl, rcid.ToString(), sid,
                    valRow.promapid, valRow.projectid, valRow.countryid, valRow.supplierid, 2);

                if (respRows == null || respRows.Count == 0)
                    continue;

                int returnValue = respRows[0].returnvalue;
                foreach (var r in respRows)
                {
                    if (r.status != 1)
                        errors.Add(new RecontactSaveErrorDTO
                        {
                            ErrorReturnDBUID = r.returnvalue.ToString(),
                            ErrorReturnDBMessage = r.r_message ?? "",
                            ErrorReturnDBStatus = "1"
                        });
                }

                foreach (var varName in varNames.Where(x => !string.IsNullOrEmpty(x)))
                    await _surveyAPIController.InsertRecontactVariable(varName, returnValue, 0);
            }

            return Json(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SaveData (RecontactCreate) failed");
            errors.Add(new RecontactSaveErrorDTO { ErrorReturnDBStatus = "4", ErrorReturnDBMessage = "There is an exception." });
            return Json(errors);
        }
    }

    public class SaveDataRequest
    {
        public string? StrData { get; set; }
    }

    [HttpPost]
    [Route("/VT/RecontactCreate/DeleteData1")]
    public async Task<IActionResult> DeleteData1([FromBody] DeleteData1Request request)
    {
        if (request == null)
            return BadRequest(new { message = "Invalid request." });

        try
        {
            int failedCount = 0;
            string[] lines = (request.ROWS ?? "").Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string uid = line.Trim().Trim('"');
                if (string.IsNullOrEmpty(uid)) continue;
                int n = await _surveyAPIController.DeleteRecontactUid(uid, request.ID, 1);
                failedCount += n;
            }
            return Json(failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteData1 failed");
            return StatusCode(500, -1);
        }
    }

    public class DeleteData1Request
    {
        public string? ROWS { get; set; }
        public int ID { get; set; }
    }

    [HttpPost]
    [Route("/VT/RecontactCreate/AddMore")]
    public async Task<IActionResult> AddMore([FromBody] AddMoreRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Invalid request." });

        try
        {
            var rows = ParseCsvToRows(request.ROWS ?? "", 2);
            if (rows == null) return Json(0);

            string maskingUrl = _configuration.GetValue<string>("MaskingUrl") ?? "";
            string mUrl = maskingUrl + "?SID=XXXX&ID=XXXXXXXXXX&CID=XXXXXXXXXX";
            int failedCount = 0;
            foreach (var row in rows)
            {
                string uid = row.Count > 0 ? (row[0] ?? "").Trim().Trim('"') : "";
                string clientUrl = row.Count > 1 ? (row[1] ?? "").Trim().Trim('"') : "";
                if (string.IsNullOrEmpty(uid)) continue;
                int st = await _surveyAPIController.UpdateRecontactProjectsAddMore(uid, clientUrl, mUrl, request.ID, 1);
                failedCount += st;
            }
            return Json(failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddMore failed");
            return StatusCode(500, -1);
        }
    }

    public class AddMoreRequest
    {
        public string? ROWS { get; set; }
        public int ID { get; set; }
    }

    [HttpPost]
    [Route("/VT/RecontactCreate/DownloadData1")]
    public async Task<IActionResult> DownloadData1([FromBody] DownloadData1Request request)
    {
        if (request == null || request.ID <= 0)
            return BadRequest(new { message = "Invalid ID." });

        try
        {
            var list = await _surveyAPIController.GetRecontactExcelDataForCreate(request.ID);
            return Json(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DownloadData1 failed");
            return StatusCode(500, new { message = "Failed to load data." });
        }
    }

    public class DownloadData1Request
    {
        public int ID { get; set; }
    }

    [HttpGet]
    [Route("/VT/RecontactCreate/ExampleCsv")]
    public IActionResult ExampleCsv()
    {
        var csv = "UID,Links\r\n";
        var bytes = Encoding.UTF8.GetBytes(csv);
        return File(bytes, "text/csv", "RecontactExample.csv");
    }

    private static List<List<string>>? ParseCsvToRows(string rawCsv, int maxColumns)
    {
        if (string.IsNullOrWhiteSpace(rawCsv)) return null;

        var rows = new List<List<string>>();

        foreach (var line in rawCsv.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            var cells = ParseCsvLine(trimmed);

            // Skip header-like row safely
            if (cells.Count >= 2 &&
                cells[0].Trim().Equals("UID", StringComparison.OrdinalIgnoreCase) &&
                (cells[1].Trim().Equals("Links", StringComparison.OrdinalIgnoreCase) ||
                 cells[1].Trim().Equals("ClientURL", StringComparison.OrdinalIgnoreCase) ||
                 cells[1].Trim().Equals("RedirectLink", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            // Ensure minimum required columns exist
            if (cells.Count < 2) continue;

            // Trim to maxColumns, pad missing cells
            if (cells.Count > maxColumns) cells = cells.Take(maxColumns).ToList();
            while (cells.Count < maxColumns) cells.Add(string.Empty);

            for (int i = 0; i < cells.Count; i++)
                cells[i] = (cells[i] ?? string.Empty).Trim();

            rows.Add(cells);
        }

        return rows.Count == 0 ? null : rows;
    }

    private static List<string> ParseCsvLine(string line)
    {
        // Handles commas inside quotes, and escaped quotes ("")
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        result.Add(sb.ToString());
        return result;
    }

    private static string IncrementID(string startValue)
    {
        if (string.IsNullOrEmpty(startValue)) startValue = "NXT000000";
        if (startValue.Length < 4) return startValue;
        try
        {
            char l1 = startValue[0], l2 = startValue[1], l3 = startValue[2];
            int len = startValue.Length - 3;
            int number = int.Parse(startValue.Substring(3));
            number++;
            if (number >= Math.Pow(10, len)) number = 1;
            return string.Format("{0}{1}{2}{3:D" + len + "}", l1, l2, l3, number);
        }
        catch { return startValue; }
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
