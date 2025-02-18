using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Core.Entities;
using NextOnServices.Core.Repository;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Projects;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.Infrastructure.ViewModels.Dashboard;
using NextOnServices.Services.DBContext;
using System.Data;

namespace NextOnServices.Endpoints.Clients;

[Route("api/[controller]")]
[ApiController]
public class ClientsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClientsAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly DapperDBSetting _dapperDBSetting;
    public ClientsAPIController(IUnitOfWork unitOfWork, ILogger<ClientsAPIController> logger, IMapper mapper, DapperDBSetting dapperDBSetting)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dapperDBSetting = dapperDBSetting;

    }

    [HttpPost(Name = "GetClients")]
    public async Task<IActionResult> GetClients()
    {
        try
        {
            IList<ClientDTO> outputModel = new List<ClientDTO>();
            //outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetAllPagedAsync(10, 0, "where isactive=1", "order by id desc"));
            outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetFilterAll(x => x.IsActive == 1));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetClients)}");
            throw;
        }
    }
    public async Task<IActionResult> GetClient(int ClientId)
    {
        try
        {
            ClientDTO outputModel = new ClientDTO();
            outputModel = _mapper.Map<ClientDTO>(await _unitOfWork.Client.FindByIdAsync(ClientId));
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetClient)}");
            throw;
        }
    }
    public async Task<IActionResult> GetAllClients()
    {
        try
        {
            IList<ClientDTO> outputModel = new List<ClientDTO>();
            //outputModel = _mapper.Map<IList<ClientDTO>>(await _unitOfWork.Client.GetAllPagedAsync(10, 0, "where isactive=1", "order by id desc"));
            //outputModel = _mapper.Map<IList<SupplierDTO>>(await _unitOfWork.Suppliers.GetFilterAll(null));
            outputModel = await _unitOfWork.Client.GetTableData<ClientDTO>("Select * from Clients");
            return Ok(outputModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(GetAllClients)}");
            throw;
        }
    }
    [HttpPost]
    public async Task<IActionResult> ChangeClientStatus(ClientDTO inputData)
    {
        try
        {
            if (inputData != null)
            {
                Client client = await _unitOfWork.Client.FindByIdAsync(inputData.ClientId);
                client.IsActive = inputData.IsActive;
                var res = await _unitOfWork.Client.UpdateAsync(client);
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
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(ChangeClientStatus)}");
            throw;
        }
    }
    public async Task<IActionResult> AddClient(ClientDTO inputData)
    {
        try
        {
            if (ModelState.IsValid && inputData != null)
            {
                if (inputData.ClientId > 0)
                {
                    string queryCount = "Select count(1) from Clients where ltrim(rtrim(Company))=@Company  and ClientId!=@ClientId";
                    var parameterCount = new { @Company = inputData?.Company?.Trim(), @ClientId = inputData?.ClientId };
                    int ClientCount = await _unitOfWork.Client.GetEntityCount(queryCount, parameterCount);
                    if (ClientCount > 0)
                    {
                        return BadRequest("Duplicate Client");
                    }
                    else
                    {
                        NextOnServices.Core.Entities.Client client = await _unitOfWork.Client.FindByIdAsync(inputData.ClientId);
                        inputData.Cstatus = client.Cstatus;
                        inputData.IsActive = client.IsActive;
                        inputData.CreationDate = client.CreationDate;
                        var res = await _unitOfWork.Client.UpdateAsync(_mapper.Map<Core.Entities.Client>(inputData));
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
                    string queryCount = "Select count(1) from Suppliers where ltrim(rtrim(Company))=@Company";
                    var parameterCount = new { @Company = inputData?.Company?.Trim() };
                    int ClientCount = await _unitOfWork.Client.GetEntityCount(queryCount, parameterCount);
                    if (ClientCount > 0)
                    {
                        return BadRequest("Duplicate Supplier");
                    }
                    else
                    {
                        inputData.ClientId = await _unitOfWork.Client.AddAsync(_mapper.Map<Core.Entities.Client>(inputData));
                        if (inputData.ClientId > 0)
                        {
                            inputData.encClientId = CommonHelper.EncryptURLHTML(inputData.ClientId.ToString());
                            return Ok(inputData);
                        }
                        else
                        {
                            return BadRequest("Error in adding supplier");
                        }
                    }
                }
            }
            return BadRequest("Invalid Data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Academics {nameof(AddClient)}");
            throw;
        }
    }
}
