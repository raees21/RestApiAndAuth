using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Common.DevUtils;
using RestAPI.Common.Enums;
using RestAPI.Models;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Generate a JWT token for development use
    /// </summary>
    /// /// <remarks>
    /// Sample request:
    ///
    ///     {
    ///        "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///        "type": "Buyer"
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> PostAuthenticate(FakeAuthRequest fakeAuthRequest)
    {
        AuthResponse auth = AccessTokenGenerator.GenerateJWT(fakeAuthRequest, _configuration["JWT:Secret"]);
        return StatusCode(201, auth);
    }
}