namespace BaseApp.API.Controllers.Bases;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class ApiControllerBase : ApiResultController
{
}
