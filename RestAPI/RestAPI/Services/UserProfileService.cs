using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RestAPI.Common.Enums;
using RestAPI.DataAccess;
using RestAPI.Models;

namespace RestAPI.Services;

public class UserProfileService: IUserProfileService
{
    private readonly ILogger<UserProfileService> _logger;
    private readonly DataContext _dataContext;
    public UserProfileService(ILogger<UserProfileService> logger, DataContext dataContext)
    {
        _logger = logger;
        _dataContext = dataContext;
    }
    
    public async Task<UserProfileResponse> CreateUserProfile(UserProfileCreate newUserProfile, Guid userId, EUserType role)
    {
        _logger.LogInformation($"Create request for new user profile with role: {role}");

        UserType userType = await _dataContext.UserTypes.FirstOrDefaultAsync(ut => ut.Type == role);

        Gender gender = await _dataContext.Genders.FirstOrDefaultAsync(g => g.Name == newUserProfile.Gender);

        UserProfile userProfile = new UserProfile(newUserProfile, userType, gender, userId);

        var createdUserProfile = await _dataContext.UserProfiles.AddAsync(userProfile);
        
        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"User profile #{createdUserProfile.Entity.Id} created");

        return MapUserProfileResponse(createdUserProfile.Entity);
    }

    public async Task<IEnumerable<UserProfileResponse>> GetAllUserProfiles(IEnumerable<EUserType> userTypes)
    {
        IEnumerable<UserProfile> userProfiles = await _dataContext.UserProfiles
                                                .Include(up => up.Gender)
                                                .Include(up => up.UserType)
                                                .Where(up => (userTypes.Count() == 0) | userTypes.Contains(up.UserType.Type))
                                                .ToListAsync();

        _logger.LogInformation($"{userProfiles.Count()} user profile records found");

        return userProfiles.Select(up => MapUserProfileResponse(up));
        
    }
    
    public async Task<UserProfileResponse> GetUserProfile(Guid userId)
    {
        _logger.LogInformation($"Request for user profile #{userId}");

        UserProfile? userProfile = await _dataContext.UserProfiles
            .Include(up => up.Gender)
            .Include(up => up.UserType)
            .FirstOrDefaultAsync(up => up.Id == userId);

        if (userProfile is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, $"User profile #{userId} could not be found");
        }

        _logger.LogInformation($"Record for user profile #{userId} found");

        return MapUserProfileResponse(userProfile);
    }

    // Get User Profile Address 
    public async Task<IEnumerable<AddressResponse>> GetUserProfileAddresses(Guid userId)
    {
        _logger.LogInformation($"Request for all addresses for user profile #{userId}");

        UserProfile userProfile = FetchUserProfile(userId);

        _logger.LogInformation($"{userProfile.Addresses} address records for user profile #{userId} found");
        
        return MapAddressesResponse(userProfile.Addresses);
    }
    
    // Get User Profile Address 
    public async Task<AddressResponse> GetUserProfileAddress(Guid userId, Guid addressId)
    {
        _logger.LogInformation($"Request for address #{addressId} for user profile #{userId}");

        Address address = _dataContext.Addresses
            .Include(a => a.UserProfile)
            .FirstOrDefault(a => a.Id == addressId);

        if (address is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, $"Address #{addressId} could not be found");
        }

        if (address.UserProfile.Id != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        _logger.LogInformation($"Record for user profile #{userId} address #{addressId} found");

        return MapAddressResponse(address);
    }

    public async Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdate userProfileUpdate, Guid userId)
    {
        _logger.LogInformation($"Request to update user profile #{userId}");

        UserProfile userProfile = FetchUserProfile(userId);

        userProfile.FirstName = userProfileUpdate.FirstName ?? userProfile.FirstName;
        userProfile.Surname = userProfileUpdate.Surname ?? userProfile.FirstName;
        userProfile.Email = userProfileUpdate.Email ?? userProfile.Email;

        if (userProfileUpdate.Gender.HasValue)
        {
            Gender newGender = _dataContext.Genders.FirstOrDefault(g => g.Name == userProfileUpdate.Gender);
            userProfile.Gender = newGender;
        }

        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"User profile #{userId} updated");
        
        return MapUserProfileResponse(userProfile);
    }

    public async Task DeleteUserProfileAddress(Guid userId, Guid addressId)
    {
        _logger.LogInformation($"Request to remove address #{addressId} for user profile #{userId}");

        Address address = _dataContext.Addresses
            .Include(a => a.UserProfile)
            .FirstOrDefault(a => a.Id == addressId);

        if (address is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, $"Address #{addressId} could not be found");
        }

        if (address.UserProfile.Id != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        _dataContext.Remove(address);

        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"Address #{addressId} for user profile #{userId} deleted");
    }

    public async Task CreateUserProfileAddress(AddressCreate addressCreate, Guid userId)
    {
        _logger.LogInformation($"Request to create new address for user profile #{userId}");

        UserProfile userProfile = FetchUserProfile(userId);
        
        Address address = new Address(addressCreate, userProfile);

        await _dataContext.AddAsync(address);

        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"Address #{address.Id} for user profile #{userId} created");

    }

    public async Task UpdateUserProfileAddress(AddressUpdate addressCreate, Guid userId, Guid addressId)
    {
        _logger.LogInformation($"Request to update address #{addressId} for user profile #{userId}");

        Address address = _dataContext.Addresses
            .Include(a => a.UserProfile)
            .FirstOrDefault(a => a.Id == addressId);

        if (address is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, $"Address #{addressId} could not be found");
        }

        if (address.UserProfile.Id != userId)
        {
            throw new HttpStatusException(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        
        address.Country = addressCreate.Country != null ? addressCreate.Country : address.Country;
        address.Province = addressCreate.Province != null ? addressCreate.Province : address.Province;
        address.City = addressCreate.City != null ? addressCreate.City : address.City;
        address.Suburb = addressCreate.Suburb != null ? addressCreate.Suburb : address.Suburb;
        address.PostalCode = addressCreate.PostalCode ?? address.PostalCode;
        address.StreetNumber = addressCreate.StreetNumber != null ? addressCreate.StreetNumber : address.StreetNumber;
        address.StreetName = addressCreate.StreetName != null ? addressCreate.StreetNumber : address.StreetName;
        address.UnitNumber = addressCreate.UnitNumber != null ? addressCreate.UnitNumber : address.UnitNumber;
        address.ComplexName = addressCreate.ComplexName != null ? addressCreate.ComplexName: address.ComplexName;

        await _dataContext.SaveChangesAsync();
        
        _logger.LogInformation($"Address #{address.Id} for user profile #{userId} updated");
    }
    
    private UserProfile FetchUserProfile(Guid userId)
    {
        UserProfile userProfile = _dataContext.UserProfiles
            .Include(up => up.Addresses)
            .Include(up => up.Gender)
            .Include(up => up.UserType)
            .FirstOrDefault(up => up.Id == userId);

        if (userProfile is null)
        {
            throw new HttpStatusException(HttpStatusCode.NotFound, $"User profile #{userId} could not be found");
        }

        return userProfile;
    }
    private IEnumerable<AddressResponse> MapAddressesResponse(IEnumerable<Address> addresses)
    {
        return addresses.Select(a => MapAddressResponse(a));
    }

    private AddressResponse MapAddressResponse(Address address)
    {
        return new AddressResponse
        {
            Id = address.Id,
            Country = address.Country,
            City = address.City,
            Province = address.Province,
            Suburb = address.Suburb,
            PostalCode = address.PostalCode,
            StreetNumber = address.StreetNumber,
            StreetName = address.StreetName,
            UnitNumber = address.UnitNumber,
            ComplexName = address.ComplexName,
        };
    }
    private UserProfileResponse MapUserProfileResponse(UserProfile userProfile)
    {
        return new UserProfileResponse
        {
            Id = userProfile.Id,
            FirstName = userProfile.FirstName,
            Surname = userProfile.Surname,
            Email = userProfile.Email,
            Gender = userProfile.Gender.Name,
            Role = userProfile.UserType.Type
        };
    }
}