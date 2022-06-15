using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;

namespace RestAPI.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }
    
    [Route("error")]
    public ErrorResponse Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context.Error;
        var code = HttpStatusCode.InternalServerError; // Default
        
        if (exception is HttpStatusException httpException)
        {
            code = httpException.StatusCode;
        }
        
        _logger.LogInformation($"{(int)code} ERROR: {exception.Message}");
        _logger.LogDebug(exception.ToString());
        
        Response.StatusCode = (int) code;

        return new ErrorResponse(exception);
    }
}