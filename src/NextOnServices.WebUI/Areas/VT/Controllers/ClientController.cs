using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Clients;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Projects;
using NextOnServices.Endpoints.Suppliers;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.VT.Infrastructure.ViewModels.Client;

namespace NextOnServices.WebUI.VT.Controllers;
[Area("VT")]
public class ClientController : Controller
{
    private readonly ILogger<ClientController> _logger;
    private readonly SuppliersAPIController _suppliersAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly ClientsAPIController _clientsAPIController;
    private readonly ProjectsAPIController _projectsAPIController;
    public ClientController(ILogger<ClientController> logger, SuppliersAPIController suppliersAPIController, CountryAPIController countryAPIController, ClientsAPIController clientsAPIController, ProjectsAPIController projectsAPIController)
    {
        _logger = logger;
        _suppliersAPIController = suppliersAPIController;
        this._countryAPIController = countryAPIController;
        this._clientsAPIController = clientsAPIController;
        _projectsAPIController = projectsAPIController;
    }

    [Route("/VT/Client/AddClient/{eClientId=null}")]
    public async Task<IActionResult> AddClient(string? eClientId = null)
    {
        ClientViewModel dto = new ClientViewModel();
        ClientDTO? clientDTO = new ClientDTO();
        try
        {
            var countriesRes = await _countryAPIController.GetCountries();
            if (countriesRes != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)countriesRes).StatusCode == 200)
            {
                dto.Countries = (List<CountryMasterDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)countriesRes).Value;
            }
            if (!String.IsNullOrEmpty(eClientId))
            {
                int clientId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eClientId));
                var resClient = await _clientsAPIController.GetClient(clientId);
                if (resClient != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resClient).StatusCode == 200)
                {
                    clientDTO = (ClientDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)resClient).Value;
                }
            }
            else
            {
                clientDTO.ClientId = 0;
            }
        }
        catch (Exception ex)
        {
            if (clientDTO == null)
                clientDTO = new ClientDTO();
            clientDTO.ClientId = 0;
        }
        dto.Client = clientDTO;
        return View(dto);
    }
    public async Task<IActionResult> ClientList()
    {
        ClientViewModel dto = new ClientViewModel();
        var res = await _clientsAPIController.GetAllClients();
        if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
        {
            dto.Clients = (List<ClientDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            if (dto.Clients != null)
            {
                foreach (var item in dto.Clients)
                {
                    item.encClientId = CommonHelper.EncryptURLHTML(item.ClientId.ToString());
                }
            }
        }
        return View(dto);
    }

    [Route("/VT/Client/Preview/{eClientId}")]
    public async Task<IActionResult> ClientPreview(string? eClientId)
    {
        if (string.IsNullOrWhiteSpace(eClientId))
        {
            return RedirectToAction(nameof(ClientList));
        }

        ClientViewModel dto = new ClientViewModel();
        int clientId;

        try
        {
            string decryptedClientId = CommonHelper.DecryptURLHTML(eClientId);
            if (!int.TryParse(decryptedClientId, out clientId))
            {
                return RedirectToAction(nameof(ClientList));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to decrypt client identifier");
            return RedirectToAction(nameof(ClientList));
        }

        try
        {
            var clientResponse = await _clientsAPIController.GetClient(clientId);
            if (clientResponse is ObjectResult clientResult && clientResult.StatusCode == StatusCodes.Status200OK)
            {
                dto.Client = clientResult.Value as ClientDTO;
                if (dto.Client != null)
                {
                    dto.Client.encClientId = eClientId;
                }
            }

            var projectsResponse = await _projectsAPIController.GetClientProjects(clientId);
            if (projectsResponse is ObjectResult projectsResult && projectsResult.StatusCode == StatusCodes.Status200OK)
            {
                dto.ProjectSummaries = projectsResult.Value as List<ClientProjectSummary>;
            }

            if (dto.ProjectSummaries != null)
            {
                foreach (var project in dto.ProjectSummaries)
                {
                    project.EncProjectId = CommonHelper.EncryptURLHTML(project.ProjectId.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while loading client preview");
        }

        dto.ProjectSummaries ??= new List<ClientProjectSummary>();
        dto.Client ??= new ClientDTO();

        return View(dto);
    }
    public async Task<IActionResult> ManageClient([FromBody] ClientDTO inputDTO)
    {
        try
        {
            if (inputDTO.ClientId > 0)
            {
                var res = await _clientsAPIController.AddClient(inputDTO);
                return res;
            }
            else
            {
                inputDTO.Cstatus = 1;
                inputDTO.IsActive = 1;
                inputDTO.CreationDate = DateTime.Now;
                var res = await _clientsAPIController.AddClient(inputDTO);

                return res;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    public async Task<IActionResult> ChangeClientStatus([FromBody] ClientDTO inputDTO)
    {
        try
        {
            if (inputDTO != null)
            {
                if (inputDTO.ClientId > 0)
                {
                    var res = await _clientsAPIController.ChangeClientStatus(inputDTO);
                    return res;
                }
            }
            return BadRequest("Unable To Save right now");
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}
