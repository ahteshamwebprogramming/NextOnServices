using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.Infrastructure.ViewModels.Supplier;
using NextOnServices.Services.DBContext;
using NextOnServices.VT.Infrastructure.ViewModels.Supplier;
using System.Text;

namespace NextOnServices.Endpoints.Suppliers;

[Route("api/[controller]")]
[ApiController]
public class SuppliersAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SuppliersAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    private readonly IConfiguration _configuration;

    public SuppliersAPIController(IUnitOfWork unitOfWork, ILogger<SuppliersAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
        _configuration = configuration;
    }
    [HttpPost(Name = "GetSuppliers")]
    public async Task<IActionResult> GetSuppliers()
    {
        try
        {
            IList<SupplierDTO> outputModel = new List<SupplierDTO>();
            //outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetAllPagedAsync(10, 0, "where isactive=1", "order by id desc"));
            outputModel = _mapper.Map<IList<SupplierDTO>>(await _unitOfWork.Suppliers.GetFilterAll(x => x.IsActive == 1));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetSuppliers)}");
            throw;
        }
    }
    [HttpPost(Name = "GetAllSuppliers")]
    public async Task<IActionResult> GetAllSuppliers()
    {
        try
        {
            IList<SupplierDTO> outputModel = new List<SupplierDTO>();
            //outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetAllPagedAsync(10, 0, "where isactive=1", "order by id desc"));
            //outputModel = _mapper.Map<IList<SupplierDTO>>(await _unitOfWork.Suppliers.GetFilterAll(null));
            outputModel = await _unitOfWork.Suppliers.GetTableData<SupplierDTO>("Select * from Suppliers");
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetSuppliers)}");
            throw;
        }
    }

    public async Task<IActionResult> GetSupplierCardsData(int SupplierId)
    {
        try
        {
            string query = "Select \r\n\r\n(Select count(1) from ProjectMapping pm\r\nleft join Projects p on pm.ProjectID=p.ProjectId\r\nleft join CountryMaster cm on pm.CountryId=cm.CountryId\r\nwhere pm.SUpplierID=@SupplierId) as ProjectBid\r\n\r\n,(Select count(1) from ProjectMapping pm\r\nleft join Projects p on pm.ProjectID=p.ProjectId\r\nleft join CountryMaster cm on pm.CountryId=cm.CountryId\r\nwhere pm.SUpplierID=@SupplierId and p.Status=6) as ProjectsCompleted\r\n\r\n\r\n\r\n\r\n,(Select avg( Convert(int,LOI)) from ProjectMapping pm\r\nleft join Projects p on pm.ProjectID=p.ProjectId\r\nleft join CountryMaster cm on pm.CountryId=cm.CountryId\r\nwhere pm.SUpplierID=@SupplierId) as AverageDuration\r\n\r\n,(Select avg( Convert(int,pm.CPI)) from ProjectMapping pm\r\nleft join Projects p on pm.ProjectID=p.ProjectId\r\nleft join CountryMaster cm on pm.CountryId=cm.CountryId\r\nwhere pm.SUpplierID=@SupplierId) as AverageProjectValue\r\n\r\n,(Select count( distinct(pm.CountryId)) from ProjectMapping pm\r\nleft join Projects p on pm.ProjectID=p.ProjectId\r\nleft join CountryMaster cm on pm.CountryId=cm.CountryId\r\nwhere pm.SUpplierID=@SupplierId ) as CountriesCovered\r\n\r\n,(Select datediff(year,CreationDate,getdate()) from Suppliers where id=@SupplierId) as SupplierSince\r\n";
            var parameters = new { @SupplierId = SupplierId };
            var res = await _unitOfWork.SupplierPanelSize.GetEntityData<ProjectDashboardCardsDTO>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetSuppliers)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSupplier(int SupplierId)
    {
        try
        {
            SupplierDTO outputModel = new SupplierDTO();
            outputModel = _mapper.Map<SupplierDTO>(await _unitOfWork.Suppliers.FindByIdAsync(SupplierId));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetSuppliers)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSupplierProjectsBySupplierId1(int SupplierId, int pageNumber = 1, int pageSize = 50, string searchValue = "", int orderByColumn = 0, string orderByDirection = "asc")
    {
        try
        {
            string sQuery = @"
            WITH SupplierCounts AS (
                SELECT
                    pm.ID ProjectMappingId,
                    pm.ProjectID ProjectId,
                    pm.SupplierID SupplierId,
                    s.Name SupplierName,
                    p.PName ProjectName,
                    p.STATUS Status,
                    cm.Country,
                    pm.CPI,
                    pm.Respondants,
                    COUNT(sp.Status) AS Total,
                    SUM(CASE WHEN sp.Status = 'Complete' THEN 1 ELSE 0 END) AS Complete,
                    SUM(CASE WHEN sp.Status = 'Terminate' THEN 1 ELSE 0 END) AS Terminate,
                    SUM(CASE WHEN sp.Status = 'OVERQUOTA' THEN 1 ELSE 0 END) AS Overquota,
                    SUM(CASE WHEN sp.Status = 'SEC_TERM' THEN 1 ELSE 0 END) AS SecurityTerm,
                    SUM(CASE WHEN sp.Status = 'F_ERROR' THEN 1 ELSE 0 END) AS FraudError,
                    SUM(CASE WHEN sp.Status = 'InComplete' THEN 1 ELSE 0 END) AS Incomplete,
                    ISNULL(chat.UnreadCount, 0) AS UnreadCount,
                    chat.LastMessageUtc,
                    LOI = AVG(CASE
                        WHEN sp.Status = 'Complete' AND sp.StartDate IS NOT NULL AND sp.EndDate IS NOT NULL
                            THEN DATEDIFF(MINUTE, sp.StartDate, sp.EndDate)
                        ELSE NULL
                    END)
                FROM
                    ProjectMapping pm
                JOIN
                    Projects p ON pm.ProjectID = p.ProjectId
                JOIN
                    Suppliers s ON pm.SupplierID = s.ID
                JOIN
                    CountryMaster cm ON pm.CountryID = cm.CountryId
                LEFT JOIN
                    SupplierProjects sp ON sp.SID = pm.SID
                LEFT JOIN
                    (
                        SELECT
                            ProjectMappingId,
                            SUM(CASE WHEN FromSupplier = 0 AND IsRead = 0 THEN 1 ELSE 0 END) AS UnreadCount,
                            MAX(CreatedUtc) AS LastMessageUtc
                        FROM SupplierProjectMessages
                        GROUP BY ProjectMappingId
                    ) chat ON chat.ProjectMappingId = pm.ID
                WHERE
                    pm.SupplierID = @SupplierID
                    AND p.IsActive = 1
                    AND s.IsActive = 1
                GROUP BY
                    pm.ID, pm.ProjectID, pm.SupplierID, s.Name, p.PName, p.STATUS, cm.Country, pm.CPI, pm.Respondants, chat.UnreadCount, chat.LastMessageUtc
            )
            SELECT *,
                IRPercent = CASE
                    WHEN (Complete + Terminate) = 0 OR Complete = 0 THEN 0
                    ELSE CAST(Complete * 100.0 / (Complete + Terminate) AS DECIMAL(10, 2))
                END
            FROM SupplierCounts ORDER BY ProjectName;
            ";
            //var sParam = new { @SupplierID = SupplierId };

            var orderDirection = orderByDirection.ToLower() == "desc" ? "DESC" : "ASC";
            var sParam = new
            {
                @SupplierID = SupplierId,
                @PageSize = pageSize,
                @Offset = (pageNumber - 1) * pageSize,
                @SearchValue = searchValue,
                @OrderByColumn = orderByColumn,
                OrderByDirection = orderDirection
            };

            var dto = await _unitOfWork.Suppliers.GetTableData<SupplierProjectsDTO>(sQuery, sParam);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetSuppliers)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSupplierProjectsBySupplierId(int SupplierId, int pageNumber = 1, int pageSize = 50, string searchValue = "", string orderByColumn = "", string orderByDirection = "asc")
    {
        try
        {
            string orderByColumnName = orderByColumn == null ? "p.PName" : orderByColumn;//orderByColumn == 0 ? "p.PName" : "cm.Country";
            string orderDirection = orderByDirection.ToLower() == "desc" ? "DESC" : "ASC";

            var sql = new StringBuilder();

            string sQuery = @"
            WITH SupplierCounts AS (
                SELECT
                    pm.ID ProjectMappingId,
                    pm.ProjectID ProjectId,
                    pm.SupplierID SupplierId,
                    s.Name SupplierName,
                    p.PName ProjectName,
                    p.STATUS Status,
                    cm.Country,
                    pm.CPI,
                    pm.Respondants,
                    ISNULL(pm.IsChecked,0) ProjectMappingChecked,
                    COUNT(sp.Status) AS Total,
                    SUM(CASE WHEN sp.Status = 'Complete' THEN 1 ELSE 0 END) AS Complete,
                    SUM(CASE WHEN sp.Status = 'Terminate' THEN 1 ELSE 0 END) AS Terminate,
                    SUM(CASE WHEN sp.Status = 'OVERQUOTA' THEN 1 ELSE 0 END) AS Overquota,
                    SUM(CASE WHEN sp.Status = 'SEC_TERM' THEN 1 ELSE 0 END) AS SecurityTerm,
                    SUM(CASE WHEN sp.Status = 'F_ERROR' THEN 1 ELSE 0 END) AS FraudError,
                    SUM(CASE WHEN sp.Status = 'InComplete' THEN 1 ELSE 0 END) AS Incomplete,
                    ISNULL(chat.UnreadCount, 0) AS UnreadCount,
                    chat.LastMessageUtc,
                    LOI = AVG(CASE
                        WHEN sp.Status = 'Complete' AND sp.StartDate IS NOT NULL AND sp.EndDate IS NOT NULL
                        THEN DATEDIFF(MINUTE, sp.StartDate, sp.EndDate)
                        ELSE NULL
                    END)
                FROM
                    ProjectMapping pm
                JOIN
                    Projects p ON pm.ProjectID = p.ProjectId
                JOIN
                    Suppliers s ON pm.SupplierID = s.ID
                JOIN
                    CountryMaster cm ON pm.CountryID = cm.CountryId
                LEFT JOIN
                    SupplierProjects sp ON sp.SID = pm.SID
                LEFT JOIN
                    (
                        SELECT
                            ProjectMappingId,
                            SUM(CASE WHEN FromSupplier = 0 AND IsRead = 0 THEN 1 ELSE 0 END) AS UnreadCount,
                            MAX(CreatedUtc) AS LastMessageUtc
                        FROM SupplierProjectMessages
                        GROUP BY ProjectMappingId
                    ) chat ON chat.ProjectMappingId = pm.ID
                WHERE
                    pm.SupplierID = @SupplierID
                    AND p.IsActive = 1
                    AND s.IsActive = 1
                    AND (p.PName LIKE '%' + @SearchValue + '%' OR cm.Country LIKE '%' + @SearchValue + '%')
                GROUP BY
                    pm.ID, pm.ProjectID, pm.SupplierID, s.Name, p.PName, p.STATUS, cm.Country, pm.CPI, pm.Respondants,pm.IsChecked, chat.UnreadCount, chat.LastMessageUtc
            ORDER BY " + orderByColumnName + " " + orderDirection + @"
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            )
            SELECT *,
                IRPercent = CASE
                    WHEN (Complete + Terminate) = 0 OR Complete = 0 THEN 0
                    ELSE CAST(Complete * 100.0 / (Complete + Terminate) AS DECIMAL(10, 2))
                END
            FROM SupplierCounts; ";

            var whereClause = new StringBuilder();
            whereClause.Append(" pm.SupplierID = @SupplierID ");
            whereClause.Append(" AND p.IsActive = 1 AND s.IsActive = 1 ");
            whereClause.Append(" AND ");
            whereClause.Append(" ( ");
            whereClause.Append(" p.PName LIKE '%' + @SearchValue + '%' ");
            whereClause.Append(" OR cm.Country LIKE '%' + @SearchValue + '%'");
            whereClause.Append(" OR cm.Country LIKE '%' + @SearchValue + '%'");
            whereClause.Append(" ) ");
            // Map parameters

            var sParam = new
            {
                @SupplierID = SupplierId,
                @PageSize = pageSize,
                //@Offset = (pageNumber - 1) * pageSize,
                @Offset = pageNumber,
                @SearchValue = searchValue
            };

            // Fetch data
            var dto = await _unitOfWork.Suppliers.GetTableData<SupplierProjectsDTO>(sQuery, sParam);

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving Supplier Projects {nameof(GetSupplierProjectsBySupplierId)}");
            return StatusCode(500, "Internal server error");
        }
    }

    public async Task<IActionResult> GetSupplierProjectsBySupplierId1(string getType, int SupplierId, int pageNumber = 1, int pageSize = 50, string searchValue = "", string orderByColumn = "", string orderByDirection = "asc")
    {
        try
        {
            string orderByColumnName = (orderByColumn == null || orderByColumn == "") ? "ProjectName" : orderByColumn;//orderByColumn == 0 ? "p.PName" : "cm.Country";
            string orderDirection = orderByDirection.ToLower() == "desc" ? "DESC" : "ASC";

            var sql = new StringBuilder();
            string select = "*";
            string orderbyoffsetAndPages = " ORDER BY " + orderByColumnName + " " + orderDirection + " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY ; ";
            if (getType == "Count")
            {
                select = "count(ProjectName)";
                orderbyoffsetAndPages = "";
            }

            string sQuery = @"
            WITH SupplierCounts AS (
                SELECT
                    pm.ID ProjectMappingId,
                    pm.ProjectID ProjectId,
                    pm.SupplierID SupplierId,
                    s.Name SupplierName,
                    p.PName ProjectName,
                    p.STATUS Status,
                    p.PID,
                    cm.Country,
                    pm.CPI,
                    pm.Respondants,
                    pm.Notes,
                                        pm.MLink,
                                        pm.OLink,
                    ISNULL(pm.IsChecked,0) ProjectMappingChecked,
                    COUNT(sp.Status) AS Total,
                    SUM(CASE WHEN sp.Status = 'Complete' THEN 1 ELSE 0 END) AS Complete,
                    SUM(CASE WHEN sp.Status = 'Terminate' THEN 1 ELSE 0 END) AS Terminate,
                    SUM(CASE WHEN sp.Status = 'OVERQUOTA' THEN 1 ELSE 0 END) AS Overquota,
                    SUM(CASE WHEN sp.Status = 'SEC_TERM' THEN 1 ELSE 0 END) AS SecurityTerm,
                    SUM(CASE WHEN sp.Status = 'F_ERROR' THEN 1 ELSE 0 END) AS FraudError,
                    SUM(CASE WHEN sp.Status = 'InComplete' THEN 1 ELSE 0 END) AS Incomplete,
                    ISNULL(chat.UnreadCount, 0) AS UnreadCount,
                    chat.LastMessageUtc,
                    LOI = AVG(CASE
                        WHEN sp.Status = 'Complete' AND sp.StartDate IS NOT NULL AND sp.EndDate IS NOT NULL
                        THEN DATEDIFF(MINUTE, sp.StartDate, sp.EndDate)
                        ELSE NULL
                    END)
                FROM
                    ProjectMapping pm
                JOIN
                    Projects p ON pm.ProjectID = p.ProjectId
                JOIN
                    Suppliers s ON pm.SupplierID = s.ID
                JOIN
                    CountryMaster cm ON pm.CountryID = cm.CountryId
                LEFT JOIN
                    SupplierProjects sp ON sp.SID = pm.SID
                LEFT JOIN
                    (
                        SELECT
                            ProjectMappingId,
                            SUM(CASE WHEN FromSupplier = 0 AND IsRead = 0 THEN 1 ELSE 0 END) AS UnreadCount,
                            MAX(CreatedUtc) AS LastMessageUtc
                        FROM SupplierProjectMessages
                        GROUP BY ProjectMappingId
                    ) chat ON chat.ProjectMappingId = pm.ID
                WHERE
                    pm.SupplierID = @SupplierID
                    AND p.IsActive = 1
                    AND s.IsActive = 1
                GROUP BY
                    pm.ID, pm.ProjectID, pm.SupplierID, s.Name, p.PName, p.STATUS, cm.Country, pm.CPI, pm.Respondants,pm.IsChecked,p.PID,pm.Notes,pm.OLink,pm.MLink, chat.UnreadCount, chat.LastMessageUtc
            ),
            SupplierCountsWithIR as (SELECT *,
                IRPercent = CASE
                    WHEN (Complete + Terminate) = 0 OR Complete = 0 THEN 0
                    ELSE CAST(Complete * 100.0 / (Complete + Terminate) AS DECIMAL(10, 2))
                END
            FROM SupplierCounts)
            SELECT " + select + @" FROM SupplierCountsWithIR
                        where  (ProjectName LIKE '%' + @SearchValue + '%'
                                        OR Country LIKE '%' + @SearchValue + '%'
                                        OR CPI LIKE '%' + @SearchValue + '%'
                                        OR Respondants LIKE '%' + @SearchValue + '%'
                                        OR Total LIKE '%' + @SearchValue + '%'
                                        OR Complete LIKE '%' + @SearchValue + '%'
                                        OR Terminate LIKE '%' + @SearchValue + '%'
                                        OR Overquota LIKE '%' + @SearchValue + '%'
                                        OR SecurityTerm LIKE '%' + @SearchValue + '%'
                                        OR FraudError LIKE '%' + @SearchValue + '%'
                                        OR Incomplete LIKE '%' + @SearchValue + '%'
                                        OR LOI LIKE '%' + @SearchValue + '%'
                                        OR IRPercent LIKE '%' + @SearchValue + '%'
                                        )
              " + orderbyoffsetAndPages + @" ; ";

            // Map parameters

            var sParam = new
            {
                @SupplierID = SupplierId,
                @PageSize = pageSize,
                //@Offset = (pageNumber - 1) * pageSize,
                @Offset = pageNumber,
                @SearchValue = searchValue
            };
            if (getType == "Count")
            {
                var dto = await _unitOfWork.Suppliers.GetTableData<int>(sQuery, sParam);
                return Ok(dto);
            }
            else
            {
                // Fetch data
                var dto = await _unitOfWork.Suppliers.GetTableData<SupplierProjectsDTO>(sQuery, sParam);
                return Ok(dto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving Supplier Projects {nameof(GetSupplierProjectsBySupplierId)}");
            return StatusCode(500, "Internal server error");
        }
    }

    public async Task<IActionResult> GetSupplierCountryPanelSizeById(int Id)
    {
        try
        {
            SupplierPanelSizeDTO outputModel = new SupplierPanelSizeDTO();
            outputModel = _mapper.Map<SupplierPanelSizeDTO>(await _unitOfWork.SupplierPanelSize.FindByIdAsync(Id));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetSuppliers)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSupplierCountryPanelSizeBySupplierId(int Id)
    {
        try
        {
            string query = "Select \r\nsps.ID\r\n,sps.SupplierID\r\n,s.Name Supplier\r\n,sps.CountryID,sps.PSize\r\n,cm.Country Country\r\n\r\nfrom SupplierPanelSize sps\r\nJoin Suppliers s on sps.SupplierID=s.ID\r\nJoin CountryMaster cm on sps.CountryID=cm.CountryId\r\nwhere SupplierID=@SupplierId";
            var parameters = new { @SupplierId = Id };
            var res = await _unitOfWork.SupplierPanelSize.GetTableData<SupplierPanelSizeWithChild>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetSuppliers)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSupplierDeliverySummary(int supplierId)
    {
        try
        {
            const string query = @"SELECT
                                        ISNULL(cm.Country, 'N/A') AS Country,
                                        COUNT(sp.ID) AS Total,
                                        ISNULL(SUM(CASE WHEN sp.Status = 'Complete' THEN 1 ELSE 0 END), 0) AS Completes,
                                        ISNULL(SUM(CASE WHEN sp.Status = 'InComplete' THEN 1 ELSE 0 END), 0) AS Incompletes,
                                        ISNULL(SUM(CASE WHEN sp.Status = 'Terminate' THEN 1 ELSE 0 END), 0) AS Screened,
                                        ISNULL(SUM(CASE WHEN sp.Status = 'OVERQUOTA' THEN 1 ELSE 0 END), 0) AS QuotaFull
                                   FROM ProjectMapping pm
                                   LEFT JOIN Projects p ON pm.ProjectID = p.ProjectId
                                   LEFT JOIN Suppliers s ON pm.SupplierID = s.ID
                                   LEFT JOIN CountryMaster cm ON pm.CountryID = cm.CountryId
                                   LEFT JOIN SupplierProjects sp ON sp.SID = pm.SID
                                   WHERE pm.SupplierID = @SupplierId
                                     AND (p.IsActive = 1 OR p.IsActive IS NULL)
                                     AND (s.IsActive = 1 OR s.IsActive IS NULL)
                                   GROUP BY ISNULL(cm.Country, 'N/A')
                                   ORDER BY Country;";
            var parameters = new { @SupplierId = supplierId };
            var res = await _unitOfWork.Suppliers.GetTableData<SupplierDeliverySummary>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving Supplier Delivery Summary {nameof(GetSupplierDeliverySummary)}");
            throw;
        }
    }
    public async Task<IActionResult> AddSupplier(SupplierDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {
                if (inputData.Id > 0)
                {
                    await UpdateSupplierUserName(inputData.Id);
                    string queryCount = "Select count(1) from Suppliers where ltrim(rtrim(Name))=@SupplierName and Id!=@Id";
                    //string queryCount = "Select count(1) from Suppliers where ltrim(rtrim(Email))=@Email and Id!=@Id";
                    var parameterCount = new { @SupplierName = inputData?.Name?.Trim(), @Id = inputData.Id };
                    //var parameterCount = new { @Email = inputData?.Email?.Trim(), @Id = inputData?.Id };
                    int SupplierCount = await _unitOfWork.Suppliers.GetEntityCount(queryCount, parameterCount);
                    if (SupplierCount > 0)
                    {
                        return BadRequest("Duplicate Supplier");
                    }
                    else
                    {
                        NextOnServices.Core.Entities.Supplier suppliers = await _unitOfWork.Suppliers.FindByIdAsync(inputData.Id);
                        inputData.Sstatus = suppliers.Sstatus;
                        inputData.IsActive = suppliers.IsActive;
                        inputData.AllowHashing = suppliers.AllowHashing;
                        inputData.CreationDate = suppliers.CreationDate;
                        inputData.SupplierCode = suppliers.SupplierCode;
                        var res = await _unitOfWork.Suppliers.UpdateAsync(_mapper.Map<Core.Entities.Supplier>(inputData));
                        if (res)
                        {
                            return Ok(res);
                        }
                        else
                        {
                            return BadRequest("Error in updating supplier");
                        }
                    }
                }
                else
                {
                    string queryCount = "Select count(1) from Suppliers where ltrim(rtrim(Name))=@SupplierName";
                    var parameterCount = new { @SupplierName = inputData?.Name?.Trim(), @Id = inputData?.Id };
                    int SupplierCount = await _unitOfWork.Suppliers.GetEntityCount(queryCount, parameterCount);
                    if (SupplierCount > 0)
                    {
                        return BadRequest("Duplicate Supplier");
                    }
                    else
                    {
                        if (inputData != null)
                        {
                            #region SupplierCode
                            string SupplierUsername = "";
                            bool usernameExists = true;
                            do
                            {
                                SupplierUsername = "NXSUP" + CommonHelper.GenerateRandomNumber(1000, 100000);
                                string eQuery = "Select * from SupplierLogin where username=@username";
                                var eParam = new { @username = SupplierUsername };
                                usernameExists = await _unitOfWork.SupplierLogin.IsExists(eQuery, eParam);
                            } while (usernameExists);
                            #endregion
                            inputData.SupplierCode = SupplierUsername;
                            inputData.Id = await _unitOfWork.Suppliers.AddAsync(_mapper.Map<Core.Entities.Supplier>(inputData));
                            if (inputData.Id > 0)
                            {
                                inputData.encId = CommonHelper.EncryptURLHTML(inputData.Id.ToString());
                                await CreateSupplierLogin(inputData.Id);
                                return Ok(inputData);
                            }
                            else
                            {
                                return BadRequest("Error in adding supplier");
                            }
                        }
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(AddSupplier)}");
            throw;
        }
    }

    public async Task<IActionResult> UpdateSupplierUserName(int SupplierId)
    {
        string sQuery = "Select * from Suppliers where Id=@Id";
        var sParam = new { @Id = SupplierId };
        Supplier supplier = await _unitOfWork.Suppliers.GetEntityData<Supplier>(sQuery, sParam);
        if (supplier != null && String.IsNullOrEmpty(supplier.SupplierCode))
        {
            string SupplierUsername = "";
            bool usernameExists = true;
            do
            {
                SupplierUsername = "NXSUP" + CommonHelper.GenerateRandomNumber(1000, 100000);
                string eQuery = "Select * from SupplierLogin where username=@username";
                var eParam = new { @username = SupplierUsername };
                usernameExists = await _unitOfWork.SupplierLogin.IsExists(eQuery, eParam);
            } while (usernameExists);
            supplier.SupplierCode = SupplierUsername;
            await _unitOfWork.Suppliers.UpdateAsync(supplier);
        }

        string lQuery = "Select * from SupplierLogin where SupplierId=@Id";
        var lParam = new { @Id = SupplierId };
        bool supplierLoginExists = await _unitOfWork.SupplierLogin.IsExists(lQuery, lParam);
        if (!supplierLoginExists)
        {
            await CreateSupplierLogin(SupplierId);
        }

        return Ok();
    }

    public async Task<IActionResult> CreateSupplierLogin(int SupplierId)
    {
        #region Create Login For Supplier
        string sQuery = "Select * from Suppliers where Id=@Id";
        var sParam = new { @Id = SupplierId };
        Supplier supplier = await _unitOfWork.Suppliers.GetEntityData<Supplier>(sQuery, sParam);

        SupplierLogin supplierLogin = new SupplierLogin();
        supplierLogin.SupplierId = SupplierId;

        supplierLogin.UserName = supplier.SupplierCode == null ? "" : supplier.SupplierCode;
        string password = CommonHelper.GeneratePassword(9);
        supplierLogin.Password = CommonHelper.Encrypt(password);
        supplierLogin.IsActive = true;
        supplierLogin.FirstTimePasswordReset = false;
        supplierLogin.CreatedDate = DateTime.Now;

        supplierLogin.Id = await _unitOfWork.SupplierLogin.AddAsync(supplierLogin);
        if (supplierLogin.Id == 0)
        {
            return BadRequest("Unable to create the login at the moment. Please try again");
        }
        else
        {
            var ServerDomain = _configuration["ServerDomain"];
            string body = $"<p>Dear {supplier.Name},</p>\r\n<p>We are excited to inform you that your registration was successful! Below are your login credentials:</p>\r\n<p><strong>Supplier Code</strong>: {supplier.SupplierCode}<br /><strong>Username</strong>: {supplierLogin.UserName}<br><strong>Password</strong>: {password}</p>\r\n<p>To access your account, please click the link below:</p>\r\n<p><a href=\"{ServerDomain}/VT/Supplier/Login\" rel=\"noopener\" target=\"_new\"><strong>Login&nbsp;Here</strong></a></p>\r\n<p>For your security, we recommend changing your password after your first login.</p>\r\n<p>If you have any questions, feel free to reach out to our support team.</p>\r\n<p>Thank you,<br>Nexton Services<br>XX-XXXXXXXXXX</p>";
            string subject = "Welcome! Your Registration is Successful";
            if (!String.IsNullOrEmpty(supplier.Email))
            {
                MailHelper.SendEmail(subject, body, supplier.Email);
            }
            //MailHelper.SendEmail(subject, body, "mhdahtesham@gmail.com");
            return Ok(supplierLogin);
        }

        #endregion
    }


    public async Task<IActionResult> AddSupplierCountryPanelSize(SupplierPanelSizeDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {
                if (inputData.Id > 0)
                {
                    string queryCount = "Select count(1) from SupplierPanelSize where SupplierID=@SupplierId and CountryID=@CountryId and ID!=@Id";
                    var parametersCount = new { @SupplierId = inputData.SupplierId, @CountryId = inputData.CountryId, @Id = inputData.Id };
                    int countSupplierCountryPanelSize = await _unitOfWork.SupplierPanelSize.GetEntityCount(queryCount, parametersCount);
                    if (countSupplierCountryPanelSize > 0)
                    {
                        return BadRequest("The panel for this country already exists. Please try for another country");
                    }
                    else
                    {
                        var updated = await _unitOfWork.SupplierPanelSize.UpdateAsync(_mapper.Map<SupplierPanelSize>(inputData));
                        if (updated)
                        {
                            return Ok("Panel Size Updated");
                        }
                        else
                        {
                            return BadRequest("Error Occurred While Updating");
                        }
                    }
                }
                else
                {
                    string queryCount = "Select count(1) from SupplierPanelSize where SupplierID=@SupplierId and CountryID=@CountryId";
                    var parametersCount = new { @SupplierId = inputData.SupplierId, @CountryId = inputData.CountryId };
                    int countSupplierCountryPanelSize = await _unitOfWork.SupplierPanelSize.GetEntityCount(queryCount, parametersCount);
                    if (countSupplierCountryPanelSize > 0)
                    {
                        return BadRequest("The panel for this country already exists. Please try for another country");
                    }
                    else
                    {
                        var updated = await _unitOfWork.SupplierPanelSize.AddAsync(_mapper.Map<SupplierPanelSize>(inputData));
                        if (updated > 0)
                        {
                            return Ok("Panel Size Added");
                        }
                        else
                        {
                            return BadRequest("Error Occurred While Adding");
                        }
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(AddSupplier)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> ChangeSupplierStatus(SupplierDTO inputData)
    {
        try
        {
            if (inputData != null)
            {
                Supplier supplier = await _unitOfWork.Suppliers.FindByIdAsync(inputData.Id);
                supplier.IsActive = inputData.IsActive;
                var res = await _unitOfWork.Suppliers.UpdateAsync(supplier);
                if (res)
                {
                    return Ok("Status Changed");
                }
                else
                {
                    return BadRequest("Unable to change the status");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ChangeSupplierStatus)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSupplierFromProfile(SupplierFromProfile inputData)
    {
        try
        {
            if (inputData != null)
            {
                inputData.Key = inputData.Key == "Default" ? "[Default]" : inputData.Key;
                string sQuery = $"Update Suppliers set {inputData.Key}=@Value where Id=@Id";
                var sParam = new { @Value = inputData.Value, @Id = inputData.Id };
                bool res = await _unitOfWork.Suppliers.ExecuteQueryAsync(sQuery, sParam);
                if (res)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ChangeSupplierStatus)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetSupplierLoginDetail(SupplierLoginDTO inputDTO)
    {
        try
        {
            string query = "Select s.*,sl.FirstTimePasswordReset from SupplierLogin sl Join Suppliers s on sl.SupplierId=s.ID  where UserName=@UserName and Password=@Password and sl.IsActive=1";
            var parameters = new { UserName = inputDTO.UserName, Password = CommonHelper.Encrypt(inputDTO.Password) };
            var res = await _unitOfWork.SupplierLogin.GetEntityData<SupplierWithChild>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(GetSupplierLoginDetail)}");
            throw;
        }
    }
    public async Task<bool> ValidateOldPassword(SupplierChangePasswordDTO inputDTO)
    {
        try
        {
            if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.OldPassword))
            {
                inputDTO.OldPassword = CommonHelper.Encrypt(inputDTO.OldPassword);
                string query = "Select * from SupplierLogin where SupplierId=@SupplierId and Password=@Password and IsActive=1";
                var parameters = new { @SupplierId = inputDTO.Id, @Password = inputDTO.OldPassword };
                var res = await _unitOfWork.SupplierLogin.IsExists(query, parameters);
                return res;
            }
            return false;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(ValidateOldPassword)}");
            throw;
        }
    }
    public async Task<bool> ValidateSupplierCode(SupplierForgetPassword inputDTO)
    {
        try
        {
            if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.SupplierCode))
            {


                string query = "Select SupplierCode from Suppliers where SupplierCode=@SupplierCode and IsActive=1";
                var parameters = new { @SupplierCode = inputDTO.SupplierCode };
                var res = await _unitOfWork.Suppliers.IsExists(query, parameters);
                return res;
            }
            return false;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(ValidateOldPassword)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSupplierBySupplierCode(SupplierForgetPassword inputDTO)
    {
        try
        {
            if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.SupplierCode))
            {
                string query = "Select * from Suppliers where SupplierCode=@SupplierCode and IsActive=1";
                var parameters = new { @SupplierCode = inputDTO.SupplierCode };
                var res = await _unitOfWork.Suppliers.GetEntityData<SupplierDTO>(query, parameters);
                return Ok(res);
            }
            return BadRequest("SupplierCode not found");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(ValidateOldPassword)}");
            throw;
        }
    }
    public async Task<bool> ChangePassword(SupplierChangePasswordDTO inputDTO)
    {
        try
        {
            if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.OldPassword) && !String.IsNullOrEmpty(inputDTO.NewPassword))
            {
                //inputDTO.OldPassword = CommonHelper.Encrypt(inputDTO.OldPassword);
                string query = "Select * from SupplierLogin where SupplierId=@SupplierId and Password=@Password and IsActive=1";
                var parameters = new { @SupplierId = inputDTO.Id, @Password = inputDTO.OldPassword };
                var res = await _unitOfWork.SupplierLogin.IsExists(query, parameters);

                if (res == true)
                {
                    SupplierLogin supplierLogin = await _unitOfWork.SupplierLogin.GetEntityData<SupplierLogin>(query, parameters);
                    supplierLogin.Password = CommonHelper.Encrypt(inputDTO.NewPassword);
                    var updated = await _unitOfWork.SupplierLogin.UpdateAsync(supplierLogin);
                    return updated;
                }
            }
            return false;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(ValidateOldPassword)}");
            throw;
        }
    }
    public async Task<bool> ResetPassword(SupplierResetPasswordDTO inputDTO)
    {
        try
        {
            if (inputDTO != null && !String.IsNullOrEmpty(inputDTO.NewPassword))
            {

                string query = "Select * from SupplierLogin where SupplierId=@SupplierId and IsActive=1";
                var parameters = new { @SupplierId = inputDTO.Id };
                var res = await _unitOfWork.SupplierLogin.IsExists(query, parameters);

                if (res == true)
                {
                    SupplierLogin supplierLogin = await _unitOfWork.SupplierLogin.GetEntityData<SupplierLogin>(query, parameters);
                    supplierLogin.Password = CommonHelper.Encrypt(inputDTO.NewPassword);
                    supplierLogin.FirstTimePasswordReset = true;
                    var updated = await _unitOfWork.SupplierLogin.UpdateAsync(supplierLogin);
                    return updated;
                }
            }
            return false;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(ValidateOldPassword)}");
            throw;
        }
    }
    public async Task<IActionResult> WithDrawlProjectFromSupplier(int Id)
    {
        try
        {
            string sQuery = "Select * from ProjectMapping where Id=@Id";
            var sParam = new { @Id = Id };
            var projectMapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(sQuery, sParam);
            projectMapping.IsChecked = 1;
            var updated = await _unitOfWork.ProjectMapping.UpdateAsync(projectMapping);
            if (updated == true)
            {
                return Ok();
            }
            return BadRequest("Unable to withdraw right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(ValidateOldPassword)}");
            throw;
        }
    }
    public async Task<IActionResult> StartProjectFromSupplier(int Id)
    {
        try
        {
            string pQuery = "Select * from ProjectMapping where Id=@Id";
            var pParam = new { @Id = Id };
            var projectMapping = await _unitOfWork.ProjectMapping.GetEntityData<ProjectMapping>(pQuery, pParam);

            string sQuery = "select count(*) from SupplierProjects where Status='Complete' and SId=@SID";
            var sParam = new { @SID = projectMapping.Sid };
            var Completes = await _unitOfWork.Suppliers.GetEntityCount(sQuery, sParam);

            if (projectMapping.Respondants > Completes)
            {
                projectMapping.IsChecked = 0;
                var updated = await _unitOfWork.ProjectMapping.UpdateAsync(projectMapping);
                if (updated == true)
                {
                    return Ok();
                }
            }
            else
            {
                return BadRequest("Project has gone Overquota");
            }
            return BadRequest("Unable to withdraw right now");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Login Detail {nameof(ValidateOldPassword)}");
            throw;
        }
    }
}
