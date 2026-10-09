using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Helper;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Services.DBContext;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.Json;
namespace NextOnServices.Endpoints.Masters;

[Route("api/[controller]")]
[ApiController]
public class CountryAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    public CountryAPIController(IUnitOfWork unitOfWork, ILogger<CountryAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }
    [HttpPost(Name = "GetCountries")]
    public async Task<IActionResult> GetCountries()
    {
        try
        {
            RequestParams requestParams = new RequestParams();
            requestParams.PageSize = 10000;
            requestParams.PageNumber = 1;

            IList<CountryMasterDTO> outputModel = new List<CountryMasterDTO>();
            //outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetAllPagedAsync(10, 0, "where isactive=1", "order by id desc"));
            outputModel = _mapper.Map<IList<CountryMasterDTO>>(await _unitOfWork.CountryMaster.GetAll(requestParams, null, null));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetCountries)}");
            throw;
        }
    }

    /// <summary>
    /// Inserts a country via the legacy AddCountry stored procedure (@item parameter).
    /// </summary>
    [HttpPost(Name = "AddCountry")]
    public async Task<IActionResult> AddCountry([FromBody] AddCountryRequest request)
    {
        try
        {
            var countryName = request?.CountryName?.Trim();
            if (string.IsNullOrWhiteSpace(countryName))
            {
                return BadRequest("Country name is required.");
            }

            if (countryName.Length > 255)
            {
                return BadRequest("Country name cannot exceed 255 characters.");
            }

            const string duplicateQuery =
                "SELECT CountryId, Country FROM CountryMaster WHERE LOWER(LTRIM(RTRIM(Country))) = LOWER(LTRIM(RTRIM(@Country)))";
            var existing = await _unitOfWork.ProjectsUrl.GetTableData<CountryMasterDTO>(
                duplicateQuery,
                new { Country = countryName });
            if (existing != null && existing.Count > 0)
            {
                return Ok(existing[0]);
            }

            using var connection = new SqlConnection(_dapperDBSetting.ConnectionString);
            await connection.OpenAsync();

            var spRows = (await connection.QueryAsync(
                "AddCountry",
                new { item = countryName },
                commandType: CommandType.StoredProcedure)).Cast<object>().ToList();
            object? spRow = spRows.FirstOrDefault();

            var countryId = TryParseCountryIdFromSpResult(spRow);
            if (countryId <= 0)
            {
                var inserted = await _unitOfWork.ProjectsUrl.GetTableData<CountryMasterDTO>(
                    duplicateQuery,
                    new { Country = countryName });
                if (inserted != null && inserted.Count > 0)
                {
                    countryId = inserted[0].CountryId;
                }
            }

            if (countryId <= 0)
            {
                var spRowLog = FormatSpRowForLog(spRow);
                _logger.LogWarning(
                    "AddCountry did not yield a CountryId for {CountryName}. SP row: {SpRow}",
                    countryName,
                    spRowLog);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Country was not created or the new country ID could not be determined.");
            }

            return Ok(new CountryMasterDTO
            {
                CountryId = countryId,
                Country = countryName
            });
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                "SQL error {SqlNumber} in {Method} for country {CountryName}",
                ex.Number,
                nameof(AddCountry),
                request?.CountryName);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Database error while adding country. Contact support if this continues.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", nameof(AddCountry));
            throw;
        }
    }

    private static string FormatSpRowForLog(object? spRow)
    {
        if (spRow == null)
        {
            return "(null)";
        }

        try
        {
            if (spRow is IDictionary<string, object> dict)
            {
                return JsonSerializer.Serialize(dict);
            }

            return JsonSerializer.Serialize(spRow);
        }
        catch
        {
            return spRow.ToString() ?? "(unserializable)";
        }
    }

    private static int TryParseCountryIdFromSpResult(object? spRow)
    {
        if (spRow == null)
        {
            return 0;
        }

        if (spRow is IDictionary<string, object> dict)
        {
            foreach (var key in new[] { "CountryId", "countryId", "Id", "id", "ID" })
            {
                if (dict.TryGetValue(key, out var namedValue) && TryParseInt(namedValue, out var namedId))
                {
                    return namedId;
                }
            }

            foreach (var value in dict.Values)
            {
                if (TryParseInt(value, out var id))
                {
                    return id;
                }
            }
        }

        var rowType = spRow.GetType();
        foreach (var prop in rowType.GetProperties())
        {
            if (string.Equals(prop.Name, "CountryId", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(prop.Name, "Id", StringComparison.OrdinalIgnoreCase))
            {
                if (TryParseInt(prop.GetValue(spRow), out var preferredId))
                {
                    return preferredId;
                }
            }
        }

        foreach (var prop in rowType.GetProperties())
        {
            if (TryParseInt(prop.GetValue(spRow), out var id))
            {
                return id;
            }
        }

        return TryParseInt(spRow, out var scalarId) ? scalarId : 0;
    }

    private static bool TryParseInt(object? value, out int id)
    {
        id = 0;
        if (value == null || value is DBNull)
        {
            return false;
        }

        switch (value)
        {
            case int intValue:
                id = intValue;
                return id > 0;
            case long longValue when longValue > 0 && longValue <= int.MaxValue:
                id = (int)longValue;
                return true;
            case short shortValue when shortValue > 0:
                id = shortValue;
                return true;
            case byte byteValue when byteValue > 0:
                id = byteValue;
                return true;
            case decimal decimalValue when decimalValue > 0 && decimalValue <= int.MaxValue:
                id = (int)decimalValue;
                return true;
            case double doubleValue when doubleValue > 0 && doubleValue <= int.MaxValue:
                id = (int)doubleValue;
                return true;
            case float floatValue when floatValue > 0 && floatValue <= int.MaxValue:
                id = (int)floatValue;
                return true;
        }

        if (decimal.TryParse(
                Convert.ToString(value, CultureInfo.InvariantCulture),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var decimalParsed) &&
            decimalParsed > 0 &&
            decimalParsed <= int.MaxValue)
        {
            id = (int)decimalParsed;
            return true;
        }

        return int.TryParse(
            Convert.ToString(value, CultureInfo.InvariantCulture),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out id) && id > 0;
    }
}
