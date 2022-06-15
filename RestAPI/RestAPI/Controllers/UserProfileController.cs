using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestAPI.Common.Helper;
using RestAPI.Common.Enums;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProfileController(
        IUserProfileService userProfileService,
        IOrderService orderService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userProfileService = userProfileService;
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Create new user profile
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     {
    ///        "firstName": "Sebas",
    ///        "surname": "Tian",
    ///        "email": "fake@bbd.co.za"
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), 201)]
    public async Task<ActionResult<UserProfileResponse>> PostCreateUserProfile(UserProfileCreate userProfile)
    {
        Guid userId = OAuth2.UserGuid(_httpContextAccessor);
        EUserType userType = OAuth2.UserRole(_httpContextAccessor);
        
        UserProfileResponse createdUserProfile = await _userProfileService.CreateUserProfile(userProfile, userId, userType);
        
        return StatusCode(201, createdUserProfile);
    }

    /// <summary>
    /// Get all user profiles
    /// </summary>
    [HttpGet]
    [Authorize(Roles="Administrator")]
    [ProducesResponseType(typeof(IEnumerable<UserProfileResponse>), 200)]

    public async Task<ActionResult<IEnumerable<UserProfileResponse>>> GetUserProfiles([FromQuery] IEnumerable<EUserType> types)
    {
        IEnumerable<UserProfileResponse> userProfiles = await _userProfileService.GetAllUserProfiles(types);

        return StatusCode(200, userProfiles);
    }

    /// <summary>
    /// Get single user profile
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("{userId}")]
    [ProducesResponseType(typeof(UserProfileResponse), 200)]

    public async Task<ActionResult<UserProfileResponse>> GetUserProfile(Guid userId)
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        EUserType userType = OAuth2.UserRole(_httpContextAccessor);
        
        if (userProfileId != userId && userType != EUserType.Administrator)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        UserProfileResponse userProfile = await _userProfileService.GetUserProfile(userId);

        return StatusCode(200, userProfile);
    }
    
    /// <summary>
    /// Get all orders linked to a user's profile
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("{userId}/orders")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), 200)]

    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetUserOrders(Guid userId, 
        [FromQuery] IEnumerable<EOrderStatus> statuses,
        [FromQuery] IEnumerable<EOrderType> types
        )
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        EUserType userType = OAuth2.UserRole(_httpContextAccessor);
        
        if (userProfileId != userId && userType != EUserType.Administrator)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        IEnumerable<OrderResponse> userOrders = await _orderService.GetAllOrders(statuses, types, userId);

        return StatusCode(200, userOrders);
    }
    
    /// <summary>
    /// Get all addresses linked to a user's profile
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("{userId}/addresses")]
    [ProducesResponseType(typeof(IEnumerable<AddressResponse>), 200)]

    public async Task<ActionResult<IEnumerable<AddressResponse>>> GetUserProfileAddress(Guid userId)
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        
        if (userProfileId != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        IEnumerable<AddressResponse> addresses = await _userProfileService.GetUserProfileAddresses(userId);

        return StatusCode(200, addresses);
    }
    
    /// <summary>
    /// Create new address linked to a user's profile
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("{userId}/addresses")]
    public async Task<ActionResult> PostUserProfileAddress(AddressCreate addressCreate,Guid userId)
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        
        if (userProfileId != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        await _userProfileService.CreateUserProfileAddress(addressCreate, userId);

        return StatusCode(201);
    }
    
    /// <summary>
    /// Get a single address linked to a user's profile
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("{userId}/addresses/{addressId}")]
    [ProducesResponseType(typeof(AddressResponse), 200)]

    public async Task<ActionResult<AddressResponse>> GetUserProfileAddress(Guid userId, Guid addressId)
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        
        if (userProfileId != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        AddressResponse address = await _userProfileService.GetUserProfileAddress(userId, addressId);

        return StatusCode(200, address);
    }
    
    /// <summary>
    /// Remove an address linked to a user's profile
    /// </summary>
    [HttpDelete]
    [Authorize]
    [Route("{userId}/addresses/{addressId}")]
    public async Task<ActionResult> DeleteUserProfileAddress(Guid userId, Guid addressId)
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        
        if (userProfileId != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        await _userProfileService.DeleteUserProfileAddress(userId, addressId);

        return StatusCode(201);
    }
    
    /// <summary>
    /// Update an address linked to a user's profile
    /// </summary>
    [HttpPut]
    [Authorize]
    [Route("{userId}/addresses/{addressId}")]
    public async Task<ActionResult> PutUserProfileAddress(AddressUpdate addressUpdate, Guid userId, Guid addressId)
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        
        if (userProfileId != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        await _userProfileService.UpdateUserProfileAddress(addressUpdate, userId, addressId);

        return StatusCode(201);
    }
    
    /// <summary>
    /// Update a user's profile
    /// </summary>
    [HttpPut]
    [Authorize]
    [Route("{userId}")]
    [ProducesResponseType(typeof(UserProfileResponse), 200)]
    public async Task<ActionResult<UserProfileResponse>> PutUpdateUserProfile(UserProfileUpdate userProfileUpdate, Guid userId)
    {
        Guid userProfileId = OAuth2.UserGuid(_httpContextAccessor);
        
        if (userProfileId != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        UserProfileResponse userProfile = await _userProfileService.UpdateUserProfile(userProfileUpdate, userId);

        return StatusCode(200, userProfile);
    }
}