using Microsoft.AspNetCore.Mvc;
using NextOnServices.Endpoints.Clients;
using NextOnServices.Endpoints.Masters;
using NextOnServices.Endpoints.Suppliers;
using NextOnServices.Infrastructure.Helper;
using NextOnServices.Infrastructure.Models.Client;
using NextOnServices.Infrastructure.Models.Masters;
using NextOnServices.Infrastructure.Models.Supplier;
using NextOnServices.VT.Infrastructure.ViewModels.Client;
using NextOnServices.VT.Infrastructure.ViewModels.Supplier;

namespace NextOnServices.WebUI.VT.Controllers;
[Area("VT")]
public class ClientController : Controller
{
    private readonly ILogger<ClientController> _logger;
    private readonly SuppliersAPIController _suppliersAPIController;
    private readonly CountryAPIController _countryAPIController;
    private readonly ClientsAPIController _clientsAPIController;
    public ClientController(ILogger<ClientController> logger, SuppliersAPIController suppliersAPIController, CountryAPIController countryAPIController, ClientsAPIController clientsAPIController)
    {
        _logger = logger;
        _suppliersAPIController = suppliersAPIController;
        this._countryAPIController = countryAPIController;
        this._clientsAPIController = clientsAPIController;
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
