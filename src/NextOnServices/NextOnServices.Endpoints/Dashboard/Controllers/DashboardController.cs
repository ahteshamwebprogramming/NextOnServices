using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NextOnServices.Endpoints.Dashboard;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatutoryComponentAPIController> _logger;
    private readonly IMapper _mapper;
    private readonly SimpliDbContext _simpliDbContext;
}
