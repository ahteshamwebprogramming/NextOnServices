using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.Infrastructure.ViewModels.ProjectMapping;
using NextOnServices.Infrastructure.ViewModels.ProjectsURL;
using NextOnServices.Services.DBContext;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace NextOnServices.Endpoints.Projects;

[Route("api/[controller]")]
[ApiController]
public class ProjectMappingAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectMappingAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    public ProjectMappingAPIController(IUnitOfWork unitOfWork, ILogger<ProjectMappingAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;
    }
    public async Task<IActionResult> GetProjectURLMappedCountries(int projectId)
    {
        try
        {
            string query = "Select cm.CountryId,pu.PID,cm.Country  from ProjectsURL pu join CountryMaster cm on pu.CID=cm.CountryId\r\nwhere \r\npu.PID=@PID\r\n";
            var parameters = new { PID = projectId };
            var res = await _unitOfWork.ProjectsUrl.GetTableData<CountryMasterDTO>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjectURLMappedCountries)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddProjectMapping(ProjectMappingDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {

                string queryExistValidate = "Select count(1) from ProjectMapping where CountryID=@CID and SUpplierID=@SupplierId and ProjectID=@PID";
                var parametersExistValidate = new { @CID = inputData.CountryId, @PID = inputData.ProjectId, @SupplierId = inputData.SupplierId };
                int projectMappingExistingCount = await _unitOfWork.ProjectMapping.GetEntityCount(queryExistValidate, parametersExistValidate);
                if (projectMappingExistingCount > 0)
                {
                    return BadRequest("This Supplier is already mapped with the same country. Please select another supplier or another country");
                }

                Supplier supplier = await _unitOfWork.Suppliers.FindByIdAsync(inputData.SupplierId ?? default(int));
                var projectsUrl1Parameters = new { @CID = inputData.CountryId, @PID = inputData.ProjectId };
                string projectsUrl1Query = "Select * from ProjectsUrl where CID=@CID and PID=@PID";
                ProjectsUrl projectsUrl1 = await _unitOfWork.ProjectsUrl.GetEntityData<ProjectsUrl>(projectsUrl1Query, projectsUrl1Parameters);

                ProjectMapping projectMapping = new ProjectMapping();
                projectMapping.ProjectId = inputData.ProjectId;
                projectMapping.CountryId = inputData.CountryId;
                projectMapping.SupplierId = inputData.SupplierId;
                projectMapping.Olink = projectsUrl1.Url;
                projectMapping.Cpi = inputData.Cpi;
                projectMapping.Mlink = inputData.Mlink;
                projectMapping.Sid = inputData.Sid;
                projectMapping.Code = inputData.Code;

                projectMapping.IsUsed = 0;
                projectMapping.CreationDate = DateTime.Now;
                projectMapping.Respondants = inputData.Respondants;
                projectMapping.IsSent = 0;
                projectMapping.Notes = inputData.Notes;


                projectMapping.Completes = supplier.Completes;
                projectMapping.Terminate = supplier.Terminate;
                projectMapping.Overquota = supplier.Overquota;
                projectMapping.Security = supplier.Security;
                projectMapping.Fraud = supplier.Fraud;


                projectMapping.Success = supplier.Success;
                projectMapping.Default = supplier.Default;
                projectMapping.Failure = supplier.Failure;
                projectMapping.QualityTermination = supplier.QualityTermination;
                projectMapping.OverQuota1 = supplier.OverQuota1;

                projectMapping.Block = 0;
                projectMapping.TrackingType = 0;
                projectMapping.Rc = 0;

                projectMapping.AddHashing = inputData.AddHashing;
                projectMapping.ParameterName = inputData.ParameterName;
                projectMapping.HashingType = inputData.HashingType;


                inputData.Id = await _unitOfWork.ProjectMapping.AddAsync(projectMapping);
                if (inputData.Id > 0)
                {
                    return Ok(inputData);
                }
                else
                {
                    return BadRequest("Error in Mapping");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectMapping)}");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProjectMapping(ProjectMappingDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {

                string queryExistValidate = "Select  count(1) from ProjectMapping where CountryID=@CID and SUpplierID=@SupplierId and ProjectID=@PID  and Id!=@Id";
                var parametersExistValidate = new { @CID = inputData.CountryId, @PID = inputData.ProjectId, @SupplierId = inputData.SupplierId, @Id = inputData.Id };
                int projectMappingExistingCount = await _unitOfWork.ProjectMapping.GetEntityCount(queryExistValidate, parametersExistValidate);
                if (projectMappingExistingCount > 0)
                {
                    return BadRequest("This Supplier is already mapped with the same country. Please select another supplier or another country");
                }

                Supplier supplier = await _unitOfWork.Suppliers.FindByIdAsync(inputData.SupplierId ?? default(int));

                var projectsUrl1Parameters = new { @CID = inputData.CountryId, @PID = inputData.ProjectId };
                string projectsUrl1Query = "Select * from ProjectsUrl where CID=@CID and PID=@PID";
                ProjectsUrl projectsUrl1 = await _unitOfWork.ProjectsUrl.GetEntityData<ProjectsUrl>(projectsUrl1Query, projectsUrl1Parameters);

                ProjectMapping projectMappingInDB = await _unitOfWork.ProjectMapping.FindByIdAsync(inputData.Id);

                projectMappingInDB.CountryId = inputData.CountryId;
                projectMappingInDB.SupplierId = inputData.SupplierId;
                projectMappingInDB.Olink = projectsUrl1.Url;
                projectMappingInDB.Cpi = inputData.Cpi;
                projectMappingInDB.Respondants = inputData.Respondants;
                projectMappingInDB.Notes = inputData.Notes;

                projectMappingInDB.Completes = supplier.Completes;
                projectMappingInDB.Terminate = supplier.Terminate;
                projectMappingInDB.Overquota = supplier.Overquota;
                projectMappingInDB.Security = supplier.Security;
                projectMappingInDB.Fraud = supplier.Fraud;


                projectMappingInDB.Success = supplier.Success;
                projectMappingInDB.Default = supplier.Default;
                projectMappingInDB.Failure = supplier.Failure;
                projectMappingInDB.QualityTermination = supplier.QualityTermination;
                projectMappingInDB.OverQuota1 = supplier.OverQuota1;

                projectMappingInDB.AddHashing = inputData.AddHashing;
                projectMappingInDB.ParameterName = inputData.ParameterName;
                projectMappingInDB.HashingType = inputData.HashingType;

                var res = await _unitOfWork.ProjectMapping.UpdateAsync(projectMappingInDB);
                if (res)
                {
                    // Create notification for project mapping change
                    if (projectMappingInDB.SupplierId.HasValue && projectMappingInDB.ProjectId.HasValue)
                    {
                        await CreateProjectChangeNotificationAsync(projectMappingInDB.Id, projectMappingInDB.ProjectId.Value, projectMappingInDB.SupplierId.Value, "Project mapping details have been updated (CPI, Quota, Country, Notes, etc.).");
                    }
                    
                    return Ok(inputData);
                }
                else
                {
                    return BadRequest("Error in Mapping");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(AddProjectMapping)}");
            throw;
        }
    }

    private async Task CreateProjectChangeNotificationAsync(int projectMappingId, int projectId, int supplierId, string changeDescription)
    {
        try
        {
            var message = $"[PROJECT_UPDATE]{changeDescription}";
            
            var notificationSql = @"
                INSERT INTO SupplierProjectMessages 
                (ProjectMappingId, ProjectId, SupplierId, Message, CreatedBy, CreatedByName, CreatedUtc, FromSupplier, IsRead)
                VALUES 
                (@ProjectMappingId, @ProjectId, @SupplierId, @Message, @CreatedBy, @CreatedByName, @CreatedUtc, @FromSupplier, @IsRead)";

            var senderName = "System";
            int? createdBy = null;
            
            if (HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = HttpContext.User.FindFirst("Id")?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    createdBy = userId;
                }
                
                var userNameClaim = HttpContext.User.FindFirst("Name")?.Value ?? 
                                   HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userNameClaim))
                {
                    senderName = userNameClaim;
                }
            }

            await _unitOfWork.ProjectMapping.ExecuteQueryAsync(notificationSql, new
            {
                ProjectMappingId = projectMappingId,
                ProjectId = projectId,
                SupplierId = supplierId,
                Message = message,
                CreatedBy = createdBy,
                CreatedByName = senderName,
                CreatedUtc = DateTime.UtcNow,
                FromSupplier = false,
                IsRead = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project change notification for ProjectMappingId {ProjectMappingId}", projectMappingId);
            // Don't throw - notification failure shouldn't break the update
        }
    }

    public async Task<IActionResult> GetProjectMappingByProjectId(int projectId)
    {
        try
        {
            string query = "Select pm.ID, p.ProjectId\r\n,p.PID ProjectCode \r\n,p.PName ProjectName\r\n,cm.CountryId\r\n,cm.Country\r\n,s.ID SupplierId\r\n,s.Name Supplier\r\n,pm.Respondants\r\n,pm.OLink\r\n,pm.MLink\r\n,pm.Notes\r\n,isnull(pm.IsChecked,0)IsChecked from ProjectMapping pm\r\njoin Projects p on pm.ProjectID=p.ProjectId\r\njoin CountryMaster cm on pm.CountryID=cm.CountryId\r\njoin Suppliers s on pm.SUpplierID=s.ID\r\nwhere pm.ProjectID=@ProjectId";
            var parameters = new { ProjectId = projectId };
            var res = await _unitOfWork.ProjectsUrl.GetTableData<ProjectMappingWithChild>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjectMappingByProjectId)}");
            throw;
        }
    }
    public async Task<IActionResult> GetProjectMappingById(int Id)
    {
        try
        {
            string query = "Select * from ProjectMapping where Id=@Id";
            var parameters = new { Id = Id };
            var res = await _unitOfWork.ProjectsUrl.GetTableData<ProjectMappingWithChild>(query, parameters);
            if (res != null && res.Count > 0)
            {
                return Ok(res.FirstOrDefault());
            }
            else
            {
                return BadRequest("No record found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(GetProjectMappingById)}");
            throw;
        }
    }
    public async Task<IActionResult> CheckProjectMapping(int Id)
    {
        try
        {
            string query = "Select * from ProjectMapping where Id=@Id";
            var parameters = new { Id = Id };
            var projectMapping = await _unitOfWork.ProjectsUrl.GetEntityData<ProjectMapping>(query, parameters);
            if (projectMapping != null)
            {
                if (projectMapping.IsChecked == 1)
                {
                    projectMapping.IsChecked = 0;
                }
                else
                {
                    projectMapping.IsChecked = 1;
                }
                var res = await _unitOfWork.ProjectMapping.UpdateAsync(projectMapping);
                if (res)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return NotFound("No record found");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(CheckProjectMapping)}");
            throw;
        }
    }

    #region ProjectQuestionsMapping

    [HttpGet]
    public async Task<IActionResult> GetProjectQuestionsMapping(int projectId, int? countryId = null)
    {
        try
        {
            string query = "SELECT * FROM ProjectQuestionsMapping WHERE PID = @ProjectId";
            object parameters;
            
            if (countryId.HasValue && countryId.Value > 0)
            {
                query += " AND CID = @CountryId";
                parameters = new { ProjectId = projectId, CountryId = countryId.Value };
            }
            else
            {
                parameters = new { ProjectId = projectId };
            }
            
            var res = await _unitOfWork.ProjectsUrl.GetTableData<ProjectQuestionsMappingDTO>(query, parameters);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving ProjectQuestionsMapping {nameof(GetProjectQuestionsMapping)}");
            return StatusCode(500, "Error retrieving project questions mapping");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectQuestionsMappingByProjectAndCountry(int projectId, int countryId)
    {
        try
        {
            string query = "SELECT * FROM ProjectQuestionsMapping WHERE PID = @ProjectId AND CID = @CountryId";
            var parameters = new { ProjectId = projectId, CountryId = countryId };
            var res = await _unitOfWork.ProjectsUrl.GetTableData<ProjectQuestionsMappingDTO>(query, parameters);
            
            if (res != null && res.Any())
            {
                return Ok(res.FirstOrDefault());
            }
            return Ok((ProjectQuestionsMappingDTO?)null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving ProjectQuestionsMapping {nameof(GetProjectQuestionsMappingByProjectAndCountry)}");
            return StatusCode(500, "Error retrieving project questions mapping");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveProjectQuestionsMapping(ProjectQuestionsMappingDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {
                // Convert selected question IDs to comma-separated string
                string qidsString = "";
                if (inputData.SelectedQuestionIds != null && inputData.SelectedQuestionIds.Any())
                {
                    qidsString = string.Join(",", inputData.SelectedQuestionIds);
                }

                if (inputData.Id > 0)
                {
                    // Update existing
                    string query = "SELECT * FROM ProjectQuestionsMapping WHERE ID = @Id";
                    var parameters = new { Id = inputData.Id };
                    var existing = await _unitOfWork.ProjectsUrl.GetEntityData<ProjectQuestionsMapping>(query, parameters);
                    
                    if (existing != null)
                    {
                        existing.Cid = inputData.Cid;
                        existing.Pid = inputData.Pid;
                        existing.PreviousButton = inputData.PreviousButton;
                        existing.QuestionQid = inputData.QuestionQid;
                        existing.Logo = inputData.Logo;
                        existing.Qids = qidsString;
                        
                        // Use ProjectsUrl repository's ExecuteQueryAsync for update
                        string updateQuery = @"UPDATE ProjectQuestionsMapping 
                                             SET CID = @Cid, PID = @Pid, PreviousButton = @PreviousButton, 
                                                 QuestionQID = @QuestionQid, Logo = @Logo, QIDs = @Qids
                                             WHERE ID = @Id";
                        await _unitOfWork.ProjectMapping.ExecuteQueryAsync(updateQuery, new
                        {
                            Cid = existing.Cid,
                            Pid = existing.Pid,
                            PreviousButton = existing.PreviousButton,
                            QuestionQid = existing.QuestionQid,
                            Logo = existing.Logo,
                            Qids = existing.Qids,
                            Id = existing.Id
                        });
                        
                        inputData.Id = existing.Id;
                        return Ok(inputData);
                    }
                    else
                    {
                        return NotFound("Mapping not found");
                    }
                }
                else
                {
                    // Insert new
                    string insertQuery = @"INSERT INTO ProjectQuestionsMapping (CID, PID, PreviousButton, QuestionQID, Logo, QIDs, CRDate)
                                         VALUES (@Cid, @Pid, @PreviousButton, @QuestionQid, @Logo, @Qids, GETDATE())";
                    
                    var insertParameters = new
                    {
                        Cid = inputData.Cid,
                        Pid = inputData.Pid,
                        PreviousButton = inputData.PreviousButton ?? 0,
                        QuestionQid = inputData.QuestionQid ?? 0,
                        Logo = inputData.Logo ?? 0,
                        Qids = qidsString
                    };
                    
                    await _unitOfWork.ProjectMapping.ExecuteQueryAsync(insertQuery, insertParameters);
                    
                    // Get the inserted ID by querying the last inserted record
                    string getLastQuery = @"SELECT TOP 1 * FROM ProjectQuestionsMapping 
                                           WHERE CID = @Cid AND PID = @Pid 
                                           ORDER BY ID DESC";
                    var lastRecord = await _unitOfWork.ProjectsUrl.GetEntityData<ProjectQuestionsMapping>(getLastQuery, new { Cid = inputData.Cid, Pid = inputData.Pid });
                    if (lastRecord != null)
                    {
                        inputData.Id = lastRecord.Id;
                        inputData.Cid = lastRecord.Cid;
                        inputData.Pid = lastRecord.Pid;
                        inputData.PreviousButton = lastRecord.PreviousButton;
                        inputData.QuestionQid = lastRecord.QuestionQid;
                        inputData.Logo = lastRecord.Logo;
                        inputData.Qids = lastRecord.Qids;
                        return Ok(inputData);
                    }
                    return BadRequest("Failed to insert mapping");
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in saving ProjectQuestionsMapping {nameof(SaveProjectQuestionsMapping)}");
            return StatusCode(500, "Error saving project questions mapping");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuestions()
    {
        try
        {
            string query = "SELECT Id, QuestionID, QuestionLabel, QuestionType, CreationDate FROM QuestionsMaster ORDER BY QuestionID";
            var res = await _unitOfWork.ProjectsUrl.GetTableData<Infrastructure.Models.Questionnaire.QuestionsMasterDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retrieving questions {nameof(GetAllQuestions)}");
            return StatusCode(500, "Error retrieving questions");
        }
    }

    #endregion
}
