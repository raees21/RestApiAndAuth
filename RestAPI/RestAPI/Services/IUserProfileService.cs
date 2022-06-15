using RestAPI.Common.Enums;
using RestAPI.Models;

namespace RestAPI.Services;

public interface IUserProfileService
{
    public Task<UserProfileResponse> CreateUserProfile(UserProfileCreate newUser, Guid userId, EUserType role);
    public Task<IEnumerable<UserProfileResponse>> GetAllUserProfiles(IEnumerable<EUserType> userTypes);
    public Task<UserProfileResponse> GetUserProfile(Guid userProfileId);
    public Task<IEnumerable<AddressResponse>> GetUserProfileAddresses(Guid userId);
    public Task<AddressResponse> GetUserProfileAddress(Guid userId, Guid addressId);
    public Task<UserProfileResponse> UpdateUserProfile(UserProfileUpdate userProfileUpdate, Guid userProfileId);
    public Task DeleteUserProfileAddress(Guid userId, Guid addressId);
    public Task CreateUserProfileAddress(AddressCreate addressCreate, Guid userId);
    public Task UpdateUserProfileAddress(AddressUpdate addressCreate, Guid userId, Guid addressId);

}